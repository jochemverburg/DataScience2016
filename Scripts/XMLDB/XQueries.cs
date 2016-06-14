using net.sf.saxon.om;
using Saxon.Api;
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
    /// This class was used to be able to run XQuery-statements in C# instead of BaseX, especially useful in case there was not enough Main Memory in BaseX.
    /// </summary>
    public class XQueries
    {

        /// <summary>
        /// Runs an XQuery, is not reusable. The strings have to be adjusted to get the right input and output-location and get the right query. Refers to the loaded document using ".". 
        /// </summary>
        public static void SaxonQuery()
        {
            Processor processor = new Processor();
            DocumentBuilder documentBuilder = processor.NewDocumentBuilder();
            documentBuilder.IsLineNumbering = true;
            documentBuilder.WhitespacePolicy = WhitespacePolicy.PreserveAll;
            XQueryCompiler compiler = processor.NewXQueryCompiler();

            //34.36.22
            //string query = "let $years := distinct-values(./Activities/Activity/StudyYear)return <total>{  for $year in $years  return <AcademicYear year=\"{$year}\">  {    let $codes := distinct-values(./Activities/Activity/Course/CourseModCode[preceding::StudyYear=$year])    for $code in $codes    return <Mod code=\"{$code}\"></Mod>  }  </AcademicYear>  }</total>";
            //string query = "element total { (for $year_2 in distinct-values(./*:Activities/*:Activity/*:StudyYear) return element AcademicYear { (attribute year { ($year_2) }, for $code_4 in distinct-values(./parent::*:StudyYear/following::*:CourseModCode) return element Mod { (attribute code { ($code_4) }) }) }) }";
            //string kpi12 = "declare function local:unique_course_times($input as element()*, $output as element()*)as element()*{  if(empty($input)) then    $output  else    let $activity := $input[1]    return      let $new-output := if($output[descendant::Date = $activity/descendant::Date]          [descendant::CourseModCode = $activity/descendant::CourseModCode]          [descendant::StudyYear = $activity/descendant::StudyYear]          [descendant::End/Hour = $activity/descendant::End/Hour]          [descendant::Start/Hour = $activity/descendant::Start/Hour]      ) then          $output      else          ($activity, $output)      return          local:unique_course_times($input[position() > 1], $new-output)};let $doc := .let $uniqueCourseEvents := local:unique_course_times($doc//Activity, ())for $year in distinct-values($uniqueCourseEvents/StudyYear)return  <AcademicYear year=\"{$year}\">{  for $code in distinct-values($uniqueCourseEvents/Course/CourseModCode[preceding::StudyYear=$year])  return <Mod code=\"{$code}\">  {    for $date in distinct-values($uniqueCourseEvents/Event/Start/Date[preceding::Course/CourseModCode=$code][preceding::StudyYear=$year])          let $hours := sum (       for $activity in $uniqueCourseEvents[descendant::Date = $date]          [descendant::CourseModCode = $code]          [descendant::StudyYear = $year]       let $hours := $activity/Event/End/Hour - $activity/Event/Start/Hour       return $hours      )    return <Date date=\"{$date}\">{      $hours}    </Date>  }  </Mod>   }</AcademicYear>";
            //string newfile = "declare function local:unique_course_times($input as element()*, $output as element()*)as element()*{  if(empty($input)) then    $output  else    let $activity := $input[1]    return      let $new-output := if($output[Event/Start/Date = $activity/Event/Start/Date]          [Course/CourseModCode = $activity/Course/CourseModCode]          [StudyYear = $activity/StudyYear]          [Event/End/Hour = $activity/Event/End/Hour]          [Event/Start/Hour = $activity/descendant::Start/Hour]      ) then          $output      else          ($activity, $output)      return          local:unique_course_times($input[position() > 1], $new-output)};let $doc := .let $uniqueCourseEvents := local:unique_course_times($doc/Activities/Activity, ())return (           for $activity in $uniqueCourseEvents           let $lectures := <Lecture>{$activity/StudyYear}{$activity/Course/CourseModCode}{$activity/Event/Start}{$activity/Event/End}</Lecture>           group by $studyYear := $activity/StudyYear           return            <StudyYear year=\"{$studyYear}\">             {for $lecture in $lectures              group by $code := $lecture/CourseModCode              return              <Module code=\"{$code}\">               {for $modlecture in $lecture               group by $date := $modlecture/Start/Date               return <Date date=\"{$date}\">{$modlecture}</Date>               }             </Module>}           </StudyYear>)";
            //To kpi temporary file
            //string newfile = "declare function local:unique_course_times($input as element()*, $output as element()*)as element()*{  if(empty($input)) then    $output  else    let $activity := $input[1]    return      let $new-output := if($output[Event/Start/Date = $activity/Event/Start/Date]          [Course/CourseModCode = $activity/Course/CourseModCode]          [StudyYear = $activity/StudyYear]          [Event/End/Hour = $activity/Event/End/Hour]          [Event/Start/Hour = $activity/descendant::Start/Hour]      ) then          $output      else          ($activity, $output)      return          local:unique_course_times($input[position() > 1], $new-output)};let $doc := .let $uniqueCourseEvents := local:unique_course_times($doc/Activities/Activity, ())return (           for $activity in $uniqueCourseEvents           let $hours := $activity/Event/End/Hour - $activity/Event/Start/Hour           let $lectures := <Lecture>{$activity/Event/Start}{$activity/Event/End}<Hours>{$hours}</Hours></Lecture>           group by $studyYear := $activity/StudyYear, $code := $activity/Course/CourseModCode, $date := $activity/Event/Start/Date, $module := $activity/ModYear           return            <lecturecategory year=\"{$studyYear}\" code=\"{$code}\" date=\"{$date}\" module=\"{$module}\">             {$lectures}           </lecturecategory>         )";
            //To room kpi
            //string newfile = "let $doc := .return <root>{           for $activity in $doc/Activities/Activity           let $hours := $activity/Event/End/Hour - $activity/Event/Start/Hour           let $lectures := <Lecture>{$activity/Event}<Hours>{$hours}</Hours></Lecture>           group by $studyYear := $activity/StudyYear, $date := $activity/Event/Start/Date, $room := $activity/Event/Room           return            <lecturecategory year=\"{$studyYear}\" room=\"{$room}\" date=\"{$date}\">             {$lectures}           </lecturecategory>       }</root>";
            //Attempt 2:
            //string newfile = "let $result := <root>{           for $activity in ./Activities/Activity           let $hours := $activity/Event/End/Hour - $activity/Event/Start/Hour           let $lectures := <Lecture>{$activity/Event}<Hours>{$hours}</Hours></Lecture>           group by $studyYear := $activity/StudyYear, $date := $activity/Event/Start/Date, $room := $activity/Event/Room           return            <lecturecategory year=\"{$studyYear}\" room=\"{$room}\" date=\"{$date}\">             {$lectures}           </lecturecategory>       }</root>return $result";

            string perstudy = "let $doc := .let $result :=             for $activity in $doc/Activities/Activity            return               (for $study in $activity/Studies/Study              return <StudyActivity>{$study}{$activity}</StudyActivity>)return <total>{$result}</total>";

            Console.WriteLine("This is the query");

            if (!String.IsNullOrEmpty(perstudy))
            {

                Console.WriteLine("I'm now going to compile");
                XQueryExecutable executable = compiler.Compile(perstudy);
                Console.WriteLine("I'm now going to load");
                XQueryEvaluator evaluator = executable.Load();
                Console.WriteLine("I'm done loading");

                using (XmlReader myReader = XmlReader.Create(@"D:\OneDrive\Jochem&Marlene\Data Science\1. XMLDB\Project\activities.xml"))
                {
                    Console.WriteLine("I'm setting the context");
                    evaluator.ContextItem = documentBuilder.Build(myReader);
                }

                Console.WriteLine("I'm evaluating");
                XdmValue evaluations = evaluator.Evaluate();
                Console.WriteLine("I'm finished");

                using (StreamWriter output = new StreamWriter(@"D:\OneDrive\Jochem&Marlene\Data Science\1. XMLDB\Project\activitiesPerStudy.xml"))
                {
                    XdmNode node = (XdmNode)evaluations;
                    output.Write(node.OuterXml);
                    
                    output.Flush();
                }
            }
        }

        /*
        public static void RunXQuery()
        {
            string input = File.ReadAllText("E:/OneDrive/Jochem&Marlene/Data Science/1. XMLDB/Project/Room-capacity result.xml");
            XDocument doc = XDocument.Parse(input);
            XmlReader reader = doc.CreateReader();
            XmlWriter writer = doc.CreateWriter();
            
            
            using (StreamWriter output = new StreamWriter("ExplorationCapacity.txt"))
            {
                IEnumerable<XElement> elements = from element in doc.Root.Descendants() select element;
                foreach (XElement e in elements)
                {
                    string line = e.Attribute("name").Value + "\t" + e.Attribute("timesUsed").Value + "\t" + e.Attribute("capacity").Value;
                    output.WriteLine(line);
                }
            }
        }*/
    }
}
