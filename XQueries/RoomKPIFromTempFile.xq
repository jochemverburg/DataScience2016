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

declare function local:unique_room_times($input as element()*, $output as element()*)
as element()*
{
  if(empty($input)) then
    $output
  else
    let $lecture := $input[1]
    return
      let $new-output := if($output[Event/Start/Hour = $lecture/Event/Start/Hour]
          [Event/End/Hour = $lecture/Event/End/Hour]
      ) then
          $output
      else
          ($lecture, $output)
      return
          local:unique_room_times($input[position() > 1], $new-output)
};

let $doc := doc("RoomKPItemporaryresults.xml")

  for $category in $doc/root/lecturecategory[@date!=""]
  let $startDay := 8
  let $endDay := 17
  let $totalhours := $endDay - $startDay
  let $hours := sum(local:unique_room_times($category/Lecture, ())[Event/Start/Hour>=$startDay][Event/End/Hour<=$endDay]/Hours)
  let $occupation := $hours div $totalhours
  
  let $date := local:cast_to_date($category/@date)
  let $year := year-from-date($date)
  let $month := month-from-date($date)
  group by $year, $month
  order by $year, $month
  return <roomkpi year="{$year}" month="{$month}" occupation="{avg($occupation)}"></roomkpi>
  