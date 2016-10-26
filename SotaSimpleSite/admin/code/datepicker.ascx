<%@ Control Language="c#" AutoEventWireup="true" targetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script runat="server">
string sOpenerField = string.Empty;
DateTime initDate = DateTime.Now;
void Page_Load(object sender, System.EventArgs e)
{
	initDate = Sota.Web.SimpleSite.Config.Main.Now();
	if(Request.QueryString["field"]!=null)
		sOpenerField = Request.QueryString["field"];
	if(Request.QueryString["date"]!=null)
	{
		try
		{
			initDate = DateTime.Parse(Request.QueryString["date"]);
		}
		catch{}
	}
}
string dn(int number)
{
	if(number<10)
		return "0"+number.ToString();
	return number.ToString();
}
</script>
<script type="text/javascript">
function Ok_Click()
{
	if(opener)
	{
		if(<%=sOpenerField.Length%>)
		{
			opener.document.getElementById('<%=sOpenerField%>').value = document.getElementById("txtChoice").value;
		}
	}
	else
	{
		window.returnValue = document.getElementById("txtChoice").value;
	}
	window.close();
}
function Cancel_Click()
{
	window.close();
}
</script>
<script type="text/javascript">
<!--
var dDate = new Date();
//var dCurMonth = dDate.getMonth();
//var dCurDayOfMonth = dDate.getDate();
//var dCurYear = dDate.getFullYear();
var objPrevElement = new Object();

function fToggleColor(myElement) {
var toggleColor = "#ff0000";
if (myElement.id == "calDateText") {
if (myElement.color == toggleColor) {
myElement.color = "";
} else {
myElement.color = toggleColor;
   }
} else if (myElement.id == "calCell") {
for (var i in myElement.children) {
if (myElement.children[i].id == "calDateText") {
if (myElement.children[i].color == toggleColor) {
myElement.children[i].color = "";
} else {
myElement.children[i].color = toggleColor;
            }
         }
      }
   }
}
function fSetSelectedDay(myElement){
if (myElement.id == "calCell") {
if (!isNaN(parseInt(myElement.children["calDateText"].innerText))) {
myElement.bgColor = "#c0c0c0";
objPrevElement.bgColor = "";
document.all.calSelectedDate.value = parseInt(myElement.children["calDateText"].innerText);
objPrevElement = myElement;
      }
   }
}
function fGetDaysInMonth(iMonth, iYear) {
var dPrevDate = new Date(iYear, iMonth, 0);
return dPrevDate.getDate();
}
function fBuildCal(iYear, iMonth, iDayStyle) {
var aMonth = new Array();
aMonth[0] = new Array(7);
aMonth[1] = new Array(7);
aMonth[2] = new Array(7);
aMonth[3] = new Array(7);
aMonth[4] = new Array(7);
aMonth[5] = new Array(7);
aMonth[6] = new Array(7);
var dCalDate = new Date(iYear, iMonth-1, 1);
var iDayOfFirst = dCalDate.getDay();
var iDaysInMonth = fGetDaysInMonth(iMonth, iYear);
var iVarDate = 1;
var i, d, w;
if (iDayStyle == 2) {
aMonth[0][0] = "Воскресенье";
aMonth[0][1] = "Понедельник";
aMonth[0][2] = "Вторник";
aMonth[0][3] = "Среда";
aMonth[0][4] = "Четверг";
aMonth[0][5] = "Пятница";
aMonth[0][6] = "Суббота";
} else if (iDayStyle == 1) {
aMonth[0][0] = "Вск";
aMonth[0][1] = "Пон";
aMonth[0][2] = "Вт";
aMonth[0][3] = "Ср";
aMonth[0][4] = "Чт";
aMonth[0][5] = "Пт";
aMonth[0][6] = "Сб";
} else {
aMonth[0][0] = "Вс";
aMonth[0][1] = "Пн";
aMonth[0][2] = "Вт";
aMonth[0][3] = "Ср";
aMonth[0][4] = "Чт";
aMonth[0][5] = "Пт";
aMonth[0][6] = "Сб";
}
for (d = iDayOfFirst; d < 7; d++) {
aMonth[1][d] = iVarDate;
iVarDate++;
}
for (w = 2; w < 7; w++) {
for (d = 0; d < 7; d++) {
if (iVarDate <= iDaysInMonth) {
aMonth[w][d] = iVarDate;
iVarDate++;
      }
   }
}
return aMonth;
}
function fDrawCal(iYear, iMonth, iCellWidth, iCellHeight, sDateTextSize, sDateTextWeight, iDayStyle) {
var myMonth;
myMonth = fBuildCal(iYear, iMonth, iDayStyle);
document.write("<table border='1'>")
document.write("<tr>");
document.write("<th>" + myMonth[0][0] + "</th>");
document.write("<th>" + myMonth[0][1] + "</th>");
document.write("<th>" + myMonth[0][2] + "</th>");
document.write("<th>" + myMonth[0][3] + "</th>");
document.write("<th>" + myMonth[0][4] + "</th>");
document.write("<th>" + myMonth[0][5] + "</th>");
document.write("<th>" + myMonth[0][6] + "</th>");
document.write("</tr>");
for (w = 1; w < 7; w++) {
document.write("<tr>")
for (d = 0; d < 7; d++) {
document.write("<td align='left' valign='top' width='" + iCellWidth + "' height='" + iCellHeight + "' id=calCell style='CURSOR:Hand' onMouseOver='fToggleColor(this)' onMouseOut='fToggleColor(this)' onclick=fSetSelectedDay(this)>");
if (!isNaN(myMonth[w][d])) {
document.write("<font id=calDateText onMouseOver='fToggleColor(this)' class='tdDay' onMouseOut='fToggleColor(this)' onclick=fSetSelectedDay(this)>" + myMonth[w][d] + "</font>");
} else {
document.write("<font id=calDateText onMouseOver='fToggleColor(this)' class='tdDay' onMouseOut='fToggleColor(this)' onclick=fSetSelectedDay(this)>&nbsp;</font>");
}
document.write("</td>")
}
document.write("</tr>");
}
document.write("</table>")
}
function fUpdateCal(iYear, iMonth) {
myMonth = fBuildCal(iYear, iMonth);
objPrevElement.bgColor = "";
document.all.calSelectedDate.value = "";
for (w = 1; w < 7; w++) {
for (d = 0; d < 7; d++) {
if (!isNaN(myMonth[w][d])) {
calDateText[((7*w)+d)-7].innerText = myMonth[w][d];
} else {
calDateText[((7*w)+d)-7].innerText = " ";
         }
      }
   }
}
function NextMonth()
{
	ChangeMonth(1);
}
function PrevMonth()
{
	ChangeMonth(-1);
}
function ChangeMonth(n)
{
	var m = frmCalendarSample.tbSelMonth.selectedIndex+n+1;
	while(m<1)
	{
		m=m+12;
		PrevYear();
	}
	while(m>12)
	{
		m=m-12;
		NextYear();
	}
	fUpdateCal(frmCalendarSample.tbSelYear.value, m);
	frmCalendarSample.tbSelMonth.selectedIndex = m-1;
}
function RePaintYear()
{
	var cmbYear = frmCalendarSample.tbSelYear;
	var oldYear	= cmbYear.value;
	var d = oldYear - (Math.round((nMaxYear-nMinYear)/2)+nMinYear);
	nMinYear = nMinYear+d;
	nMaxYear = nMaxYear+d;
	cmbYear.options.length = 0;
	for(i=nMinYear;i<nMaxYear+1;i++)
	{
		oOption = document.createElement("OPTION");				
		cmbYear.options.add(oOption);				
		oOption.value = i;			
		oOption.innerText = i;
		if(i == oldYear)
			oOption.selected = true;
	} 
}
function NextYear()
{	
	ChangeYear(parseInt(frmCalendarSample.tbSelYear.value)+1);
}
function PrevYear()
{
	ChangeYear(parseInt(frmCalendarSample.tbSelYear.value)-1);
}
var nMinYear = <%=initDate.Year-3%>;
var nMaxYear = <%=initDate.Year+3%>;
function ChangeYear(y)
{
	var d = nMaxYear-nMinYear+1;
	while(y<nMinYear)
	{
		y=y+d;
	}
	while(y>nMaxYear)
	{
		y=y-d;
	}
	fUpdateCal(y, frmCalendarSample.tbSelMonth.value);
	for (i = 0; i < frmCalendarSample.tbSelYear.length; i++)
	{
		if(frmCalendarSample.tbSelYear.options[i].value == y)
		{
			frmCalendarSample.tbSelYear.options[i].selected = true;
			break;
		}
	}	
	RePaintYear();
}
function OnDayChange(d)
{
	if(d)
	{
		try{
			document.getElementById("txtChoice").value = frmCalendarSample.tbSelYear.value+"-"+NumberString(frmCalendarSample.tbSelMonth.value)+"-"+NumberString(d);
		}catch(ex){}
	}
}
function NumberString(s)
{
	var n = parseInt(s);
	if(n<10)
		return "0"+n.toString();
	else
		return n.toString();
}
//-->
</script>
<script type="text/javascript" for=window event=onload>
<!-- 
var dCurDate = new Date();
frmCalendarSample.tbSelMonth.options[dCurDate.getMonth()].selected = true;
RePaintYear();
for (i = 0; i < frmCalendarSample.tbSelYear.length; i++)
{
	if(frmCalendarSample.tbSelYear.options[i].value == dCurDate.getFullYear())
	{
		frmCalendarSample.tbSelYear.options[i].selected = true;
		break;
	}
}	
//-->
</script>
<form name="frmCalendarSample" method="post" action="">
<input type="hidden" name="calSelectedDate" value="" onpropertychange="OnDayChange(this.value);">

<table border="1" width="260px">
<tr>
<td><input type="button" value="<<" onclick="PrevYear();"></td>
<td><input type="button" value="<" onclick="PrevMonth();"></td>
<td width="100%" align="center" nowrap>
<select name="tbSelMonth" onchange='fUpdateCal(frmCalendarSample.tbSelYear.value, frmCalendarSample.tbSelMonth.value)'>
<option value="1">Январь</option>
<option value="2">Февраль</option>
<option value="3">Март</option>
<option value="4">Апрель</option>
<option value="5">Май</option>
<option value="6">Июнь</option>
<option value="7">Июль</option>
<option value="8">Август</option>
<option value="9">Сентябрь</option>
<option value="10">Октябрь</option>
<option value="11">Ноябрь</option>
<option value="12">Декабрь</option>
</select>
  
<select name="tbSelYear" onchange='ChangeYear(frmCalendarSample.tbSelYear.value);'>
<option value="<%=initDate.Year%>"><%=initDate.Year%></option>
</select>
</td>
<td><input type="button" value=">" onclick="NextMonth();"></td>
<td><input type="button" value=">>" onclick="NextYear();"></td>
</tr>
<tr>
<td colspan="5">
<script type="text/javascript">
var dCurDate = new Date();
fDrawCal(dCurDate.getFullYear(), dCurDate.getMonth()+1, 30, 30, "12px", "bold", 1);
</script>
</td>
</tr>
<tr><td colspan="5">
<input type="text" id="txtChoice" value="<%=initDate.Year+"-"+dn(initDate.Month)+"-"+dn(initDate.Day)%>">
<input type="button" value="ОК" onclick="Ok_Click();">
<input type="button" value="Отмена" onclick="Cancel_Click();">
</td></tr>
</table>
</form>