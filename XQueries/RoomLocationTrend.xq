declare function local:cast_to_date($input as xs:string)
as xs:date
{
  let $date := 
    if(fn:matches($input, "[0-9]{4}-[0-9]{2}-[0-9]{2}")) 
    then xs:date($input) 
    else 
      let $year := substring-before($input, "-")
      let $secondpart := substring-after($input, "-")
      let $month := substring-before($secondpart, "-")
      let $day := substring-after($secondpart, "-")
      return xs:date(concat($year,"-",(if (string-length($month)=1) then concat("0",$month) else $month),"-",(if (string-length($day)=1) then concat("0",$day) else $day)))
  return $date
};

let $doc := doc("activitiesPerStudy.xml")
return <total>{
           for $activity in $doc/total/StudyActivity[Activity/Event/Start/Date!=""]
           let $room := $activity/Activity/Event/Room
           let $floor := substring($room, 0, 5)
           let $date := local:cast_to_date($activity/Activity/Event/Start/Date)
           let $year := year-from-date($date)
           let $month := month-from-date($date)
           group by $studyYear := $activity/Activity/StudyYear, $year, $month, $study := $activity/Study, $floor
           order by $study, $year, $month, count($activity) descending, $floor
           return 
           <room studyyear="{$studyYear}" study="{$study}" year="{$year}" month="{$month}" floor="{$floor}" timesUsed="{count($activity)}"/>
     }</total>