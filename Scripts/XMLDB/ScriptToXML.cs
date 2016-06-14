using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLDB
{
    /// <summary>
    /// This class was used to load the data into XML from the Excel-files given (this was done by first transforming the Excel-files to TSV-files).
    /// </summary>
    public class ScriptToXML
    {
        /// <summary>
        /// The main method which transforms all the used tsv files into C# objects and then parses these object to XML.
        /// </summary>
        public static void Main(string[] args)
        {
            ScriptToXML instance = new ScriptToXML();
            instance.ConvertRooms();
            instance.ConvertYear("ActivitiesUT_2013-2014_v2.txt", "20132014");
            Console.WriteLine("20132014 finished");
            instance.ConvertYear("ActivitiesUT_2014-2015_v2.txt", "20142015");
            Console.WriteLine("20142015 finished");
            instance.ConvertYear("ActivitiesUT_2015-2016_v2.txt", "20152016");
            Console.WriteLine("20152016 finished");
            instance.ParseToXML();
        }

        /// <summary>
        /// All the activities from the college year files (every line is one activity)
        /// </summary>
        public List<ActivitiesUT> activities;
        /// <summary>
        /// List of tuples of the room with the capacity of that room
        /// </summary>
        public List<Tuple<string, int>> capacity;

        /// <summary>
        /// Constructor that initializes that list of activities and the list of rooms
        /// </summary>
        public ScriptToXML()
        {
            activities = new List<ActivitiesUT>();
            capacity = new List<Tuple<string, int>>();
        }

        /// <summary>
        /// Method that puts the rooms of the TSV file into the list of the rooms and their capacity
        /// </summary>
        public void ConvertRooms()
        {
            //TSV file of the room is read into the script
            StreamReader rooms = new StreamReader("E:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project files/Timetable files/TSVs/UT_Overview classrooms and facilities 20150609.txt");

            string room;

            //Reads the next line of the TSV file as long as there are new lines
            while ((room = rooms.ReadLine()) != null)
            {
                //The line from the TSV file is split with the delimeter tab
                string[] contentRooms = room.Split('\t');
                //The room can be added of both the name of the room and the capacity of the room are entered in the TSV file
                if (!string.IsNullOrWhiteSpace(contentRooms[0]) && !string.IsNullOrWhiteSpace(contentRooms[6]))
                {
                    ParseRoom(contentRooms[0], contentRooms[6]);
                }
            }

            //Stream that reads the TSV file is closed
            rooms.Close();
        }

        /// <summary>
        /// Method that convert the TSV files from the college years to C#-object
        /// </summary>
        /// <param name="filename">the name of the file, represents the college year</param>
        /// <param name="year">The college year of the file</param>
        public void ConvertYear(string filename, string year )
        {
            //TSV file of the college year is read into the script with a reader
            StreamReader file = new StreamReader("E:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project files/Timetable files/TSVs/" + filename);

            //Writer is used to write each activity to a txt-file
            //This is used for testing purposes
            using (StreamWriter output = new StreamWriter("parsename" + year + ".txt"))
            {
                string line;

                //Read the next line of the TSV files as long as there are new lines
                //Each line represents an activity
                while ((line = file.ReadLine()) != null)
                {
                    //New C# object is created for every activity
                    ActivitiesUT activity = new ActivitiesUT();
                    //year attribute is set to the parameter of the method
                    activity.Year = year;

                    //Some activities from the TSV file are divided over several lines. These splits are marked with a " at the beginning and the end. This code combines these lines again.
                    if (line.Contains("\""))
                    {
                        while (line.Count(f => f == '"') % 2 == 1)
                        {
                            string line2 = file.ReadLine();
                            line += line2;
                        }
                        line = line.Replace("\"", string.Empty);
                    }

                    // The line from the TSV file is split with the delimeter tab
                    string[] content = line.Split('\t');
                    //The name of the activity is parsed in a seperate method
                    ParseNameActivity(content[0], activity);
                    //Description of the activity is set to the second field of the TSV file
                    activity.Description = content[1];
                    //The course or module code of the activity is set. When the activity is part of TOM the third field is used, otherwise the fourth field from the TSV file is used
                    //When both possible fields are not a valid code the CourseModCode is set to null
                    if ((content[2].Contains("#") || content[2].Contains("-")) && (content[3].Contains("#") || content[3].Contains("-")))
                    {
                        activity.CourseModCode = null;
                    }
                    //Edge case where the TOM attribute is not valid
                    else if (content[0].Contains("RT/LW"))
                    {
                        activity.CourseModCode = null;
                    }
                    //If activity is not part of TOM module
                    else if (!activity.TOM)
                    {
                        //Edge case where the TOM attribute is not valid
                        if(content[2].Contains("#") && activity.Name.ToLower().Contains("uitreiking"))
                        {
                            activity.CourseModCode = null;
                        }
                        //CourseModCode is set to the third field of the TSV file
                        else
                        {
                            activity.CourseModCode = content[2];
                        }
                        
                    }
                    //If activity is part of TOM module
                    else
                    {
                        //Edge case where the TOM attribute is not valid
                        if (content[3].Contains("#") && activity.Name.ToLower().Contains("uitreiking"))
                        {
                            activity.CourseModCode = null;
                        }
                        //CourseModCode is set to the fourth field of the TSV file
                        else
                        {
                            activity.CourseModCode = content[3];
                        }
                    }
                    //The attribute ActivityType is set to the fifth field of the TSV file
                    activity.ActivityType = content[4];
                    //Seperate method is used to determine the start and the end of the activity
                    ParseDateTime(content[5], content[6], content[7], activity);
                    //The size of the activity is set to the ninth field of the TSV file
                    activity.Size = Int32.Parse(content[8]);
                    //Seperate method is used to determine the room and the according capacity of that room
                    CheckRoom(content[9], activity);

                    //Activity is add to the list of activities
                    activities.Add(activity);

                    //Activity is add to the output file of this method
                    string activityString = activity.ToString();
                    output.WriteLine(activityString);

                }
            }

            //Stream that reads the TSV file is closed
            file.Close();
		
	    }

        /// <summary>
        /// Method that adds the room and its capacity to the list of rooms
        /// </summary>
        /// <param name="description">The name of the room</param>
        /// <param name="capacityLecture">The capacity of the room</param>
        private void ParseRoom(string description, string capacityLecture)
        {
            //Name of the room is split by the space delimeter 
            string[] parts = description.Split(' ');
            string room;
            //When first the number and then the abbrevation of the building is mentioned
            if (char.IsDigit(description[0]))
            {
                room = parts[1] + " " + parts[0];
            }
            //When first the abbrevation and then the number is mentioned
            else
            {
                room = parts[0] + " " + parts[1];
            }
            //The room and capacity is added to the list of rooms
            capacity.Add(new Tuple<string, int>(room, Int32.Parse(capacityLecture)));
        }
	
        /// <summary>
        /// Method used to parse the name of the activity into different attributes
        /// </summary>
        /// <param name="nameActivity">The name as mentioned in the TSV file</param>
        /// <param name="activity">The current activity</param>
        private void ParseNameActivity(string nameActivity, ActivitiesUT activity)
        {
            //First the study is mentioned, which is parsed with a regex
            string study = Regex.Match(nameActivity, "^[a-zA-Z-]+").Value;
            //The study is removed from the name
            string newName = Regex.Replace(nameActivity, "^" + study, string.Empty);
            //Edge cases where the study name consists of two parts seperated by the space character
            if (study.Equals("MPS"))
            {
                //Extra part is parsed with a regex
                string nextPart = Regex.Match(newName, "^\\s[a-zA-Z]+").Value;
                //If not edge case where the second part should not be included
                if (!nextPart.Equals("M"))
                {
                    //Extra part is added to the study string
                    study += " " + nextPart;
                    //Extra part is removed from the original name
                    newName = Regex.Replace(newName, nextPart, string.Empty);
                }
            }
            //The study is added to studies attribute of the activity
            activity.StudyAbb.Add(study);
            //When the activity includes more than one study
            while(newName[0].Equals("/"))
            {
                //All the extra studies are also matched with a regex, added to the studies attribute, and removed from the name
                study = Regex.Match(newName, "^/[a-zA-Z]+").Value;
                activity.StudyAbb.Add(study);
                newName = Regex.Replace(newName, "^" + study, string.Empty);
            }
            //Space at the beginning is removed from the name
            newName = Regex.Replace(newName, "^\\s", string.Empty);
            //Next value is matched with a regex
            string next = Regex.Match(newName, "[^\\s]+").Value;
            //if next case shows that activity is part of the bachelor before TOM, a master, a minor or a pre-master
            if (next[0].Equals('B') || next.Equals("M") || next.Equals("MI") || next.Equals("PM"))
            {
                //The attribute ModYear is set to the value of next
                activity.ModYear = next;
                //Activity is not part of TOM
                activity.TOM = false;
                //Next with a space at the end is removed from the name
                newName = Regex.Replace(newName, "^" + next + "[\\s]+", string.Empty);
                //If not TOM, the quartiles are entered next in the name and are also parsed with a regex
                string posQuartile = Regex.Match(newName, "[^\\s]+").Value;
                //Quartiles with extra space at the end are removed from the name
                newName = Regex.Replace(newName, posQuartile + "\\s", string.Empty);
                //When the last parsed Regex is indeed a quartile
                if (posQuartile[0].Equals('1') || posQuartile[0].Equals('2'))
                {
                    //Case when only one quartile is entered
                    if (!posQuartile.Contains("+"))
                    {
                        //Quartile is added to the quartiles attribute of the activity
                        activity.Quartile.Add(posQuartile);
                    }
                    else
                    {
                        //More than one quartile are seperated by the '+'-symbol
                        string[] quartiles = posQuartile.Split('+');
                        //All quartiles are added to the quartiles attribute of the activity
                        foreach (string q in quartiles)
                        {
                            activity.Quartile.Add(q);
                        }
                    }

                    
                }
            }
            //When quartiles are immediately entered and the ModYear value is not entered
            else if (next[0].Equals('1') || next[0].Equals('2'))
            {
                //Activity is not part of TOM when quartiles are entered
                activity.TOM = false;
                activity.ModYear = null;
                //Quartile is added to the quartiles attribute of the activity
                if (!next.Contains("+"))
                {
                    activity.Quartile.Add(next);
                }
                else
                    {
                    // More than one quartile are seperated by the '+' - symbol
                    string[] quartiles = next.Split('+');
                    //All quartiles are added to the quartiles attribute of the activity
                    foreach (string q in quartiles)
                    {
                        activity.Quartile.Add(q);
                    }
                }
                //Quartiles with extra space at the end are removed from the name
                newName = Regex.Replace(newName, next + "[\\s]+", string.Empty);
            }
            else
            {
                activity.ModYear = next;
                //Activity is part of TOM
                activity.TOM = true;
                //ModYear with extra space at the end is removed from the name
                newName = Regex.Replace(newName, "^" + next + "[\\s]+", string.Empty);
            }
            //Edge cases where activity is not part of TOM whereas previous code it determined to be part of TOM
            if (activity.StudyAbb.Contains("IEM") || activity.StudyAbb.Contains("NT") || activity.StudyAbb.Contains("PSTS") || activity.StudyAbb.Contains("TM"))
            {
                activity.TOM = false;
            }
            //Edge cases where activity is part of TOM whereas previous code determined it not to be part of TOM
            if (activity.StudyAbb.Contains("TG") && activity.Quartile.Contains("1A") && activity.Quartile.Contains("1B"))
            {
                activity.TOM = true;
            }
            //Edge cases where activity is part of TOM whereas previous code determined it not to be part of TOM
            if (activity.StudyAbb.Contains("TG") && !string.IsNullOrWhiteSpace(activity.ModYear) && activity.ModYear.Equals("B4"))
            {
                activity.TOM = true;
            }
            //Next value is parsed with a regex
            string name = Regex.Match(newName, "^[^/]+").Value;
            //Edge cases where an extra study is entered on a seperate place
            if (name.Contains("("))
            {
                string extraStudy = Regex.Match(name, "\\([a-zA-Z]+\\)").Value;
                name = Regex.Replace(name, "[\\s]*\\([a-zA-Z]+\\)", string.Empty);
                extraStudy = extraStudy.Replace("(", string.Empty);
                extraStudy = extraStudy.Replace(")", string.Empty);
                activity.StudyAbb.Add(extraStudy);
            }
            //Next value is name of the course and name attribute of the activity is set to this value
            activity.Name = name;
            //Name is removed from the name
            newName = Regex.Replace(newName, "^[^/]+", string.Empty);
            //When the rest of the string is empty, no sessiontype is included
            if (newName.Equals(string.Empty))
            {
                activity.SessionType = null;
            }
            //Rest of the string is the sessiontype
            else
            {
                activity.SessionType = Regex.Replace(newName, "^/", string.Empty);
            }
            
        }

        /// <summary>
        /// Method that parses the start and the end of the activity to DateTime objects
        /// </summary>
        /// <param name="date">The date of the activity</param>
        /// <param name="startTime">The start time of the activity</param>
        /// <param name="endTime">The end time of the activity</param>
        /// <param name="activity">The current activity</param>
        private void ParseDateTime(string date, string startTime, string endTime, ActivitiesUT activity)
        {
            //When no date is entered for the activity
            if (date.Equals(String.Empty))
            {
                activity.StartDateTime = null;
                activity.EndDateTime = null;
            }
            //When a date is entered for the activity
            else
            {
                //Different parts of the date are split by the delimeter '-'
                string[] dateParts = date.Split('-');
                //When no start time is entered for the activity
                if (startTime.Equals(String.Empty))
                {
                    activity.StartDateTime = null;
                }
                //When a start time is entered for the activity
                else
                {
                    //Different parts of the time are split by the delimiter ':'
                    string[] startTimeParts = startTime.Split(':');
                    //The different parts from the date and that start time are entered in the constructor of the DateTime object start
                    DateTime start = new DateTime(Int32.Parse(dateParts[2]), Int32.Parse(dateParts[1]), Int32.Parse(dateParts[0]), Int32.Parse(startTimeParts[0]), Int32.Parse(startTimeParts[1]), Int32.Parse(startTimeParts[2]));
                    //DateTime object start is added to the start attribute of the activity
                    activity.StartDateTime = start;
                }
                //When no end time is entered for the activity
                if (endTime.Equals(String.Empty))
                {
                    activity.EndDateTime = null;
                }
                //When a end time is entered for the activity
                else
                {
                    //Different parts of the time are split by the delimiter ':'
                    string[] endTimeParts = endTime.Split(':');
                    //The different parts from the date and the end time are entered in the constructor of the DateTime object end
                    DateTime end = new DateTime(Int32.Parse(dateParts[2]), Int32.Parse(dateParts[1]), Int32.Parse(dateParts[0]), Int32.Parse(endTimeParts[0]), Int32.Parse(endTimeParts[1]), Int32.Parse(endTimeParts[2]));
                    //DateTime object end is added to the end attribute of the activity
                    activity.EndDateTime = end;
                }
            }
        }

        /// <summary>
        /// Method that renames duplicate names of certain rooms with the goal that the same room is named the same in all the data
        /// </summary>
        /// <param name="room">The name of the room</param>
        /// <param name="activity">The current activity</param>
        private void CheckRoom(string room, ActivitiesUT activity)
        {
            //The name of the room is split with the space delimiter
            string[] parts = room.Split(' ');
            //When the room is in the building the Vrijhof
            if(parts[0].Equals("VR"))
            {
                //Second part of the name determines whether or not the room should be renamed with a switch statement
                string caseSwitch = parts[1];
                switch (caseSwitch)
                {
                    case "2.02":
                        activity.Room = "VR 275B";
                        break;
                    case "2.03":
                        activity.Room = "VR 275C";
                        break;
                    case "2.04":
                        activity.Room = "VR 275E";
                        break;
                    case "2.05":
                        activity.Room = "VR 275F";
                        break;
                    case "2.06":
                        activity.Room = "VR 275G";
                        break;
                    case "2.07":
                        activity.Room = "VR 275H";
                        break;
                    case "2.08":
                        activity.Room = "VR 275J";
                        break;
                    case "2.09":
                        activity.Room = "VR 275K";
                        break;
                    case "2.10":
                        activity.Room = "VR 275L";
                        break;
                    case "2.11":
                        activity.Room = "VR 275M";
                        break;
                    case "1.01":
                        activity.Room = "VR 170";
                        break;
                    case "2.13":
                        activity.Room = "VR 275P";
                        break;
                    case "2.14":
                        activity.Room = "VR 275Q";
                        break;
                    case "2.15":
                        activity.Room = "VR 275R";
                        break;
                    case "2.12":
                        activity.Room = "VR 275O";
                        break;
                    case "2.A":
                        activity.Room = "VR 247";
                        break;
                    case "2.B":
                        activity.Room = "VR 248";
                        break;
                    case "2.C":
                        activity.Room = "VR 256";
                        break;
                    case "2.D":
                        activity.Room = "VR 257";
                        break;
                    case "2.E":
                        activity.Room = "VR 258";
                        break;
                    case "1.12":
                        activity.Room = "VR 193A";
                        break;
                    case "1.14":
                        activity.Room = "VR 193D";
                        break;
                    case "1.21":
                        activity.Room = "VR 193L";
                        break;
                    case "1.22":
                        activity.Room = "VR 193N";
                        break;
                    case "2.16":
                        activity.Room = "VR 275T";
                        break;
                    case "275O":
                        activity.Room = "VR 275O";
                        break;
                    case "275R":
                        activity.Room = "VR 275R";
                        break;
                    case "275T":
                        activity.Room = "VR 275T";
                        break;
                    case "247":
                        activity.Room = "VR 247";
                        break;
                    case "248":
                        activity.Room = "VR 248";
                        break;
                    case "256":
                        activity.Room = "VR 256";
                        break;
                    case "257":
                        activity.Room = "VR 257";
                        break;
                    case "258":
                        activity.Room = "VR 258";
                        break;
                    case "275M":
                        activity.Room = "VR 275M";
                        break;
                    case "275L":
                        activity.Room = "VR 275L";
                        break;
                    case "275K":
                        activity.Room = "VR 275K";
                        break;
                    case "275J":
                        activity.Room = "VR 275J";
                        break;
                    case "275H":
                        activity.Room = "VR 275H";
                        break;
                    case "275F":
                        activity.Room = "VR 275F";
                        break;
                    case "275E":
                        activity.Room = "VR 275E";
                        break;
                    case "275C":
                        activity.Room = "VR 275C";
                        break;
                    case "275B":
                        activity.Room = "VR 275B";
                        break;
                    case "193D":
                        activity.Room = "VR 193D";
                        break;
                    case "193N":
                        activity.Room = "VR 193N";
                        break;
                    case "193L":
                        activity.Room = "VR 193L";
                        break;
                    case "191B":
                        activity.Room = "VR 191B";
                        break;
                    case "170":
                        activity.Room = "VR 170";
                        break;
                    case "275Q":
                        activity.Room = "VR 275Q";
                        break;
                    case "193K":
                        activity.Room = "VR 193K";
                        break;
                    case "275G":
                        activity.Room = "VR 275G";
                        break;
                    case "275P":
                        activity.Room = "VR 275P";
                        break;
                    case "193A":
                        activity.Room = "VR 193A";
                        break;

                }
            }
            //When the room is in the building the Gallery
            else if (parts[0].Equals("GY"))
            {
                string number = parts[1];
                switch (number)
                {
                    case "1.86-1.88":
                        activity.Room = "GY 1.86-1.88";
                        break;
                    case "1.86-1":
                        activity.Room = "GY 1.86-1.88";
                        break;
                    case "1.78":
                        activity.Room = "GY 1.78";
                        break;
                    case "1.79":
                        activity.Room = "GY 1.79";
                        break;
                    case "1.81a":
                        activity.Room = "GY 1.81a";
                        break;
                }
            }
            //Edge case where a certain room need to saved in a different way
            else if (parts[0].Equals("ZZ"))
            {
                if(parts.Length > 2)
                {
                    string number = Regex.Match(parts[2], "^[A-Za-z0-9]+").Value;
                    activity.Room = parts[0] + " " + number;
                }
                else
                {
                    activity.Room = Regex.Match(parts[1], "^[A-Za-z0-9]+").Value;
                }
                
            }
            //When the room is entered correctly and no renaming is necessary
            else
            {
                activity.Room = room;
            }
        }

        /// <summary>
        /// Method which parses that activity objects to XML
        /// </summary>
        private void ParseToXML()
        {
            XDocument doc = new XDocument();
            //Root element is set to Activities
            doc.Add(new XElement("Activities"));
            //All activities from the list are parsed to a XML element
            foreach(ActivitiesUT a in activities)
            {
                //Root element of activity is set to Activity
                XElement activity = new XElement("Activity");
                //Attribute Year of the activity is saved in the element StudyYear
                activity.Add(new XElement("StudyYear", a.Year));
                //Attribute TOM of the activity is saved in the element TOM
                activity.Add(new XElement("TOM", a.TOM));
                //When at least one study is entered
                if(a.StudyAbb.Count != 0)
                {
                    //All studies of the activity are added in the element Studies
                    XElement studies = new XElement("Studies");
                    //Every study is parsed to a XML element
                    foreach (string study in a.StudyAbb)
                    {
                        if (!string.IsNullOrWhiteSpace(study))
                        {
                            //Study is saved in the element Study
                            studies.Add(new XElement("Study", study));
                        }
                    }
                    //When at least one study is entered
                    if (studies.HasElements)
                    {
                        //The element studies is added to the XML
                        activity.Add(studies);
                    }    
                }
                //When the attribute ModYear is entered
                if(!string.IsNullOrWhiteSpace(a.ModYear))
                {
                    //Attribute ModYear of the activity is saved in the element ModYear
                    activity.Add(new XElement("ModYear", a.ModYear));
                }
                
                //If at least one quartile is entered
                if(a.Quartile.Count != 0)
                {
                    //All quartiles are saved in the element Quartiles
                    XElement quartiles = new XElement("Quartiles");
                    //Each quartile is saved in a seperate Quartile element
                    foreach (string quartile in a.Quartile)
                    {
                        if (!string.IsNullOrWhiteSpace(quartile))
                        {
                            quartiles.Add(new XElement("Quartile", quartile));
                        }
                    }
                    //When at least one quartile is entered the Quartiles element is added to the XML
                    if (quartiles.HasElements)
                    {
                        activity.Add(quartiles);
                    }
                }
                
                //All information about the activity related to the course is added in the element Course
                XElement course = new XElement("Course");
                //When the name of the course is added for the activity, this is saved in the element Name
                if(!string.IsNullOrWhiteSpace(a.Name))
                {
                    course.Add(new XElement("Name", a.Name));
                }
                //When the course or module code is added for the activity, this is saved in the element CourseModCode
                if (!string.IsNullOrWhiteSpace(a.CourseModCode))
                {
                    course.Add(new XElement("CourseModCode", a.CourseModCode));
                }
                //When the activity type of the course is added for the activity, this is saved in the element ActivityType
                if (!string.IsNullOrWhiteSpace(a.ActivityType))
                {
                    course.Add(new XElement("ActivityType", a.ActivityType));
                }
                //When the sessiom type of the course is added for the activity, this is saved in the element SessionType
                if (!string.IsNullOrWhiteSpace(a.SessionType))
                {
                    course.Add(new XElement("Session", a.SessionType));
                }
                //When the description of the course is added for the activity, this is saved in the element Description
                if (!string.IsNullOrWhiteSpace(a.Description))
                {
                    course.Add(new XElement("Description", a.Description));
                }
                //When at least one of the fields of the course are used, the element course is added to the XML
                if (course.HasElements)
                {
                    activity.Add(course);
                }
                
                //All information of the activity related to where and when the activity took place are saved in the element Event
                XElement whereWhen = new XElement("Event");
                
                //When the object start of the activity is used
                if (a.StartDateTime.HasValue)
                {
                    //All information of the start are added to the element Start
                    XElement start = new XElement("Start");
                    //The date of the start is saved in the attribute Date
                    string date = a.StartDateTime.Value.Year + "-" + a.StartDateTime.Value.Month + "-" + a.StartDateTime.Value.Day;
                    start.Add(new XElement("Date", date));

                    //start.Add(new XElement("Year", a.StartDateTime.Value.Year));
                    //start.Add(new XElement("Month", a.StartDateTime.Value.Month));
                    //start.Add(new XElement("Day", a.StartDateTime.Value.Day));

                    //The hour of the start is saved in the attribute Hour
                    start.Add(new XElement("Hour", a.StartDateTime.Value.Hour));
                    //The minute of the start is saved in the attribute Minute
                    start.Add(new XElement("Minute", a.StartDateTime.Value.Minute));
                    //The second of the start is saved in the attribute Second
                    start.Add(new XElement("Second", a.StartDateTime.Value.Second));
                    //The start of the activity is added to the XML
                    whereWhen.Add(start);
                }
                //When the object end of the activity is used  
                if (a.EndDateTime.HasValue)
                {
                    //All information of the end are added to the element End
                    XElement end = new XElement("End");
                    //The date of the end is saved in the attribute Date
                    string date = a.EndDateTime.Value.Year + "-" + a.EndDateTime.Value.Month + "-" + a.EndDateTime.Value.Day;
                    end.Add(new XElement("Date", date));

                    //end.Add(new XElement("Year", a.EndDateTime.Value.Year));
                    //end.Add(new XElement("Month", a.EndDateTime.Value.Month));
                    //end.Add(new XElement("Day", a.EndDateTime.Value.Day));

                    //The hour of the end is saved in the attribute Hour
                    end.Add(new XElement("Hour", a.EndDateTime.Value.Hour));
                    //The minute of the end is saved in the attribute Minute
                    end.Add(new XElement("Minute", a.EndDateTime.Value.Minute));
                    //The second of the end is saved in the attribute Second
                    end.Add(new XElement("Second", a.EndDateTime.Value.Second));
                    //The end of the activity is added to the XML
                    whereWhen.Add(end);
                }
                //When the size of the event is added for the activity, this is saved in the element RequestedSize
                if (a.Size != null)
                {
                    whereWhen.Add(new XElement("RequestedSize", a.Size));
                }
                //When the room of the event is added for the activity, this is saved in the element Room
                if (!string.IsNullOrWhiteSpace(a.Room))
                {
                    whereWhen.Add(new XElement("Room", a.Room));
                    //All the tuples of the list of rooms are searched for the room of the event
                    foreach (Tuple<string,int> t in capacity)
                    {
                        //When the room is found, the capacity of this room is saved in the element Capacity
                        if (t.Item1.Equals(a.Room))
                        {
                            whereWhen.Add(new XElement("Capacity", t.Item2));
                        }
                    }
                }
                //If at least on of the fields of the event are used, the element Event is added to the XML
                if (whereWhen.HasElements)
                {
                    activity.Add(whereWhen);
                }
                //The Activity element is added to the XML
                doc.Element("Activities").Add(activity);
            }
            //The XML with all the activities is saved with the name activities.xml
            doc.Save("activities.xml");
        }

        /// <summary>
        /// Class which contains the format for the acitivities of the college year files
        /// </summary>
        public class ActivitiesUT
        {
            /// <summary>
            /// Constructur that creates a new list for the studies and the quartiles of the activity
            /// </summary>
            public ActivitiesUT()
            {
                StudyAbb = new List<string>();
                Quartile = new List<string>();
            }

            //The year the activity took place
            public string Year { get; set; }
            //The list of abbrevations of the study this activity took place
            public List<string> StudyAbb { get; set; }
            //The module or study phase this activity took place for
            public string ModYear { get; set; }
            //The list of quartiles in which this activity took place
            public List<string> Quartile { get; set; }
            //The name of the activity
            public string Name { get; set; }
            //The session type of the activity
            public string SessionType { get; set; }
            //The description of the activity
            public string Description { get; set; }
            //Whether this study was part of a TOM module
            public bool TOM { get; set; }
            //The course or module code where this activity was part of
            public string CourseModCode { get; set; }
            //The activity type of the activity
            public string ActivityType { get; set; }
            //The start of the activity
            public DateTime? StartDateTime { get; set; }
            //The end of the activity
            public DateTime? EndDateTime { get; set; }
            //The requested size for the activity
            public int? Size { get; set; }
            //The room where the activity took place
            public string Room { get; set; }
            //The capacity of the room
            public int? CapacityRoom { get; set; }

            //Method to create a string representation of the activity
            public override string ToString()
            {
                return "Year: " + Year + StudyToString() + ", ModYear: " + ModYear + QuartileToString() + ", Name: " + Name + ", SessionType: " + SessionType + ", Description: " + Description + ", TOM: " + TOM + ", CourseModCode: " + CourseModCode + ", ActivityType: " + ActivityType + ", StartDateTime: " + StartDateTime + ", EndDateTime: " + EndDateTime + ", size: " + Size + ", Room: " + Room + ", CapacityRoom: " + CapacityRoom;
            }

            //Method to create a string representation of the list of studies of the activity
            public string StudyToString()
            {
                string s = ", StudyAbbs: ";
                for(int i = 0; i<StudyAbb.Count; i++)
                {
                    if (!(i == StudyAbb.Count - 1))
                    {
                        s += StudyAbb.ElementAt(i) + ", ";
                    } else
                    {
                        s += StudyAbb.ElementAt(i);
                    }
                }
                return s;
            }

            //Method to create a string representation of the list of quartiles of the activity
            public string QuartileToString()
            {
                string s = ", Quartile: ";
                for (int i = 0; i < Quartile.Count; i++)
                {
                    if (!(i == Quartile.Count - 1))
                    {
                        s += Quartile.ElementAt(i) + ", ";
                    }
                    else
                    {
                        s += Quartile.ElementAt(i);
                    }
                }
                return s;
            }

        }

        

    }
}
