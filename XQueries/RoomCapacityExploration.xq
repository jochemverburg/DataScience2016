let $doc := doc("activities.xml")
return <total>{
           for $activity in $doc/Activities/Activity
           let $capacity := $activity/Event/Capacity
           group by $room := $activity/Event/Room
           return 
           <room name="{$room}" timesUsed="{count($activity)}" capacity="{max($capacity)}"/>
     }</total>