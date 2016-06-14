let $result := <root>{
           for $activity in doc("tomModules.xml")/Activities/Activity
           let $hours := $activity/Event/End/Hour - $activity/Event/Start/Hour
           let $lectures := <Lecture>{$activity/Event}<Hours>{$hours}</Hours></Lecture>
           group by $studyYear := $activity/StudyYear, $date := $activity/Event/Start/Date, $room := $activity/Event/Room
           return 
           <lecturecategory year="{$studyYear}" room="{$room}" date="{$date}">
             {$lectures}
           </lecturecategory>
       }</root>
return $result