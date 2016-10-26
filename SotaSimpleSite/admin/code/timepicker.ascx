<%@ Control Language="c#" AutoEventWireup="true" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script runat="server">
DateTime initDate = DateTime.Now;
string sOpenerField = string.Empty;
void Page_Load(object sender, System.EventArgs e)
{
	initDate = Sota.Web.SimpleSite.Config.Main.Now();
	if(Request.QueryString["field"]!=null)
		sOpenerField = Request.QueryString["field"];
	if(Request.QueryString["time"]!=null)
	{
		try
		{
			initDate = DateTime.Parse(Request.QueryString["time"]);
		}
		catch{}
	}

}
string NumberToString(int n)
{
	if(n<10)
		return "0"+n.ToString();
	return n.ToString();
}
</script>
<script type="text/javascript">
function Ok_Click()
{
	if(opener)
	{
		if(<%=sOpenerField.Length%>)
		{
			opener.document.getElementById('<%=sOpenerField%>').value = frmTime.cmbHour.value+":"+frmTime.cmbMinute.value+":"+frmTime.cmbSecond.value;
		}
	}
	else
	{
		window.returnValue = frmTime.cmbHour.value+":"+frmTime.cmbMinute.value+":"+frmTime.cmbSecond.value;
	}
	window.close();
}
function Cancel_Click()
{
	window.close();
}
</script>
<form name="frmTime">
<table border="0" cellpadding="2" width="100%" height="100%">
<tr><td nowrap  width="100%" height="100%" valign="top">
<select name="cmbHour">
<%
for(int i=0;i<24;i++)
{
	Response.Write("<option value=\"");
	Response.Write(NumberToString(i));
	Response.Write("\"");
	if(i==initDate.Hour)
		Response.Write(" selected");
	Response.Write(">");
	Response.Write(NumberToString(i));
	Response.Write("</option>\n");
}
%>
</select>
:
<select name="cmbMinute">
<%
for(int i=0;i<60;i++)
{
	Response.Write("<option value=\"");
	Response.Write(NumberToString(i));
	Response.Write("\"");
	if(i==initDate.Minute)
		Response.Write(" selected");
	Response.Write(">");
	Response.Write(NumberToString(i));
	Response.Write("</option>\n");
}
%>
</select>
:
<select name="cmbSecond">
<%
for(int i=0;i<60;i++)
{
	Response.Write("<option value=\"");
	Response.Write(NumberToString(i));
	Response.Write("\"");
	if(i==initDate.Second)
		Response.Write(" selected");
	Response.Write(">");
	Response.Write(NumberToString(i));
	Response.Write("</option>\n");
}
%>
</select>
</form>
</td>
</tr>
<tr>
<td align="center"  valign="top">
<input type="button" value="ОК" onclick="Ok_Click();">
<input type="button" value="Отмена" onclick="Cancel_Click();">
</td></tr>
</table>
