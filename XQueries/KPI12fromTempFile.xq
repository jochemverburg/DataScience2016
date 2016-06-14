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
  let $totalhours := sum($category/Lecture/Hours/text())
  let $kpi1 := $totalhours >= 4
  let $kpi2 := $totalhours <= 6
  let $kpi12 := $kpi1 and $kpi2
  let $date := local:cast_to_date($category/@date)
  let $year := year-from-date($date)
  let $month := month-from-date($date)
  group by $year, $month
  order by $year, $month
  let $kpi1pass := count($kpi1[.=true()])
  let $kpi1fail := count($kpi1[.=false()])
  let $kpi2pass := count($kpi2[.=true()])
  let $kpi2fail := count($kpi2[.=false()])
  let $kpi12pass := count($kpi12[.=true()])
  let $kpi12fail := count($kpi12[.=false()])
  return <kpis year="{$year}" month="{$month}" total="{count($category)}" kpi1pass="{$kpi1pass}" kpi1fail="{$kpi1fail}" kpi1ratio="{$kpi1pass div ($kpi1pass+$kpi1fail)}" kpi2pass="{$kpi2pass}" kpi2fail="{$kpi2fail}" kpi2ratio="{$kpi2pass div ($kpi2pass+$kpi2fail)}" kpi12pass="{$kpi12pass}" kpi12fail="{$kpi12fail}" kpi12ratio="{$kpi12pass div ($kpi12pass+$kpi12fail)}"></kpis>