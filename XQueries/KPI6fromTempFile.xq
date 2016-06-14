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

declare function local:day-of-week
  ( $date as xs:anyAtomicType? )  as xs:integer? {

  if (empty($date))
  then ()
  else xs:integer((xs:date($date) - xs:date('1901-01-06'))
          div xs:dayTimeDuration('P1D')) mod 7
 } ;

let $doc := doc("KPItemporaryresultsfromc.xml")

  for $category in $doc/root/lecturecategory[Lecture/Hours/text()][@date!=""][local:day-of-week(local:cast_to_date(@date))=5]
  let $kpi6 := if ($category/Lecture/End/Hour>=19) then 1 else 0
  group by $year :=  $category/@year
  order by $year
  
  let $fail := sum($kpi6)
  let $pass := count($category)-$fail
  return <kpi6 year="{$year}" pass="{$pass}" fail="{$fail}" ratio="{$pass div ($pass + $fail)}"></kpi6>