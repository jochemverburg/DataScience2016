using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMLDB
{
    /// <summary>
    /// This class was used to transform XML results back into a TSV-file, so it could be imported in Excel for analysis.
    /// </summary>
    public class XMLToTSV
    {
        public const string TAB = "\t";
        
        /// <summary>
        /// This automatically reads every element from an input-file and puts the attributes in one row separated by tabs.
        /// The results can't be nested and all information is expected to be in the attributes.
        /// </summary>
        /// <param name="readFile">The location of the input-file with the XML-results.</param>
        /// <param name="outputFile">The location of the output, tsv.</param>
        /// <param name="attributes">The attributes that have to be found and written to the output.</param>
        public static void ConvertAttributesToTSV(string readFile, string outputFile, List<string> attributes)
        {
            string input = File.ReadAllText(readFile);

            XDocument doc;

            try
            {
                doc = XDocument.Parse(input);
            }
            catch (XmlException e)
            {
                input = "<root>" + input + "</root>";
                doc = XDocument.Parse(input);
            }

            using (StreamWriter output = new StreamWriter(outputFile))
            {
                IEnumerable<XElement> elements = from element in doc.Root.Descendants() select element;

                if (attributes.Count > 0)
                {
                    string line = attributes.ElementAt(0);

                    for (int i = 1; i < attributes.Count; i++)
                    {
                        line += TAB + attributes.ElementAt(i);
                    }

                    output.WriteLine(line);
                }

                foreach (XElement e in elements)
                {
                    if (attributes.Count > 0)
                    {
                        string line = e.Attribute(attributes.ElementAt(0)).Value;

                        for (int i = 1; i < attributes.Count; i++)
                        {
                            line += TAB + e.Attribute(attributes.ElementAt(i)).Value;
                        }

                        output.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Converts all results for the trend analysis of study and a location.
        /// </summary>
        public static void ConvertKPIs()
        {
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpi12-monthlyresults.xml", "KPI12MonthResults.txt", new List<string>() { "year", "month", "kpi1pass", "kpi1fail", "kpi1ratio", "kpi2pass", "kpi2fail", "kpi2ratio", "kpi12pass", "kpi12fail", "kpi12ratio" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpi3-monthlyresults.xml", "KPI3MonthResults.txt", new List<string>() { "year", "month", "pass", "fail", "ratio" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpi3-results.xml", "KPI3Results.txt", new List<string>() { "year", "pass", "fail", "ratio" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpi4-monthlyresults.xml", "KPI4MonthResults.txt", new List<string>() { "year", "month", "pass", "fail", "ratio" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpi4-results.xml", "KPI4Results.txt", new List<string>() { "year", "pass", "fail", "ratio" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpi5-results.xml", "KPI5Results.txt", new List<string>() { "year", "pass", "fail", "ratio" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpi6-results.xml", "KPI6Results.txt", new List<string>() { "year", "pass", "fail", "ratio" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/roomkpi-monthlyresults.xml", "KPIRoomMonthResults.txt", new List<string>() { "year", "month", "occupation" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/roomkpi-results.xml", "KPIRoomResults.txt", new List<string>() { "year", "occupation" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/kpis-monthlylectures.xml", "KPIMonthNumbers.txt", new List<string>() { "year", "month", "total" });
        }

        /// <summary>
        /// Converts all results for the exploration of study and a location.
        /// </summary>
        public static void ConvertExplorationStudyLocation()
        {
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/building-study correlation room results.xml", "StudyRoomExploration.txt", new List<string>() { "study", "name", "timesUsed" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/building-study correlation floor results.xml", "StudyFloorExploration.txt", new List<string>() { "study", "floor", "timesUsed" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/building-study correlation building results.xml", "StudyBuildingExploration.txt", new List<string>() { "study", "building", "timesUsed" });
        }
        
        /// <summary>
        /// Converts all results for the trend analysis of study and a location.
        /// </summary>
        public static void ConvertTrendStudyLocation()
        {
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/building-study trend analysis room month results.xml", "StudyRoomTrend.txt", new List<string>() { "studyyear", "study", "year", "month", "room", "timesUsed" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/building-study trend analysis floor month results.xml", "StudyFloorTrend.txt", new List<string>() { "studyyear", "study", "year", "month", "floor", "timesUsed" });
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/building-study trend analysis building month results.xml", "StudyBuildingTrend.txt", new List<string>() { "studyyear", "study", "year", "month", "building", "timesUsed" });
        }

        /// <summary>
        /// Converts all results for the trend analysis of capacity.
        /// </summary>
        public static void ConvertExplorationCapacity()
        {
            string input = File.ReadAllText("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/Room-capacity result new.xml");
            XDocument doc = XDocument.Parse(input);

            using (StreamWriter output = new StreamWriter("ExplorationCapacityNew.txt"))
            {
                IEnumerable<XElement> elements = from element in doc.Root.Descendants() select element;
                foreach (XElement e in elements)
                {
                    string line = e.Attribute("name").Value + TAB + e.Attribute("timesUsed").Value + TAB + e.Attribute("capacity").Value;
                    output.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Converts all results for the trend analysis of capacity.
        /// </summary>
        public static void ConvertTrendAnalysisCapacity()
        {
            ConvertAttributesToTSV("D:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/trend analysis room-capacity result new.xml", "TrendAnalysisCapacityNew.txt", new List<string>() { "year", "date", "room", "timesUsed", "capacity" });
        }
    }
}
