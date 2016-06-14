let $doc := doc("activities.xml")

return (
           for $activity in $doc/Activities/Activity
           let $capacity := $activity/Event/Capacity
           group by $studyYear := $activity/StudyYear, $date := $activity/Event/Start/Date, $room := $activity/Event/Room
           order by $studyYear, $date, $room
           return 
           <roomcapacity year="{$studyYear}" date="{$date}" room="{$room}" timesUsed="{count($activity)}" capacity="{max($capacity)}"/>
         )
