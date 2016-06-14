declare function local:unique_course_times($input as element()*, $output as element()*)
as element()*
{
  if(empty($input)) then
    $output
  else
    let $activity := $input[1]
    return
      let $new-output := if($output[Event/Start/Date = $activity/Event/Start/Date]
          [Course/CourseModCode = $activity/Course/CourseModCode]
          [StudyYear = $activity/StudyYear]
          [Event/End/Hour = $activity/Event/End/Hour]
          [Event/Start/Hour = $activity/descendant::Start/Hour]
      ) then
          $output
      else
          ($activity, $output)
      return
          local:unique_course_times($input[position() > 1], $new-output)
};

let $doc := doc("tomModules.xml")
let $uniqueCourseEvents := local:unique_course_times($doc/Activities/Activity, ())

return (
           for $activity in $uniqueCourseEvents
           let $hours := $activity/Event/End/Hour - $activity/Event/Start/Hour
           let $lectures := <Lecture>{$activity/Event/Start}{$activity/Event/End}<Hours>{$hours}</Hours></Lecture>
           group by $studyYear := $activity/StudyYear, $code := $activity/Course/CourseModCode, $date := $activity/Event/Start/Date, $module := $activity/ModYear
           return 
           <lecturecategory year="{$studyYear}" code="{$code}" date="{$date}" module="{$module}">
             {$lectures}
           </lecturecategory>
         )