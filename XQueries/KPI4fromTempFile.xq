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

let $doc := doc("KPItemporaryresultsfromc.xml")

  for $category in $doc/root/lecturecategory[Lecture/Hours/text()]
  
  let $minhours := min($category/Lecture/Start/Hour)
  let $minminutes := min($category/Lecture[Start/Hour=$minhours]/Start/Minute)
  let $min := $minhours * 60 + $minminutes
  
  let $maxhours := max($category/Lecture/End/Hour)
  let $maxminutes := max($category/Lecture[End/Hour=$maxhours]/End/Minute)
  let $max := $maxhours * 60 + $maxminutes
  
  let $difference := $max - $min
  let $toolong := if($difference > (8*60 + 15)) then 1 else 0
  
  let $date := local:cast_to_date($category/@date)
  let $year := year-from-date($date)
  let $month := month-from-date($date)
  group by $year, $month
  order by $year, $month
  
  let $fail := sum($toolong)
  let $pass := count($category)-$fail
  
  return <kpi4 year="{$year}" month="{$month}" pass="{$pass}" fail="{$fail}" ratio="{$pass div ($pass + $fail)}"></kpi4>