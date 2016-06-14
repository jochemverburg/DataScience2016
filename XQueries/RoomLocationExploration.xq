let $doc := doc("activitiesPerStudy.xml")
return <total>{
           for $activity in $doc/total/StudyActivity
           let $room := $activity/Activity/Event/Room
           let $building := substring($room, 0, 3)
           group by $study := $activity/Study, $building
           order by $study, count($activity) descending, $building
           return 
           <room study="{$study}" building="{$building}" timesUsed="{count($activity)}"/>
     }</total>