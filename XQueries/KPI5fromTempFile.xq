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

declare function local:next_day($input as xs:date)
as xs:date
{
  let $date := $input + xs:dayTimeDuration('P1D')
  return $date
};

let $doc := doc("KPItemporaryresultsfromc.xml")

  for $category in $doc/root/lecturecategory[Lecture/Hours/text()][@date!=""]
  let $count := count($category/Lecture[End/Hour>=19])
  let $kpi5 := if ($count=0) then 0 else if(count(
     $doc/root/lecturecategory
     [@year=$category/@year]
     [@code=$category/@code]
     [@date!=""]
     [local:cast_to_date(@date)=local:next_day(local:cast_to_date($category/@date))]
     [@module=$category/@module][Lecture/Start/Hour<10]
    )=0) then 0 else 1
  let $tokenized := tokenize($category/@date, '-')
  group by $year :=  $category/@year
  order by $year
  
  let $fail := sum($kpi5)
  let $pass := count($category)-$fail
  
  return <kpi5 year="{$year}" pass="{$pass}" fail="{$fail}" ratio="{$pass div ($pass + $fail)}"></kpi5>