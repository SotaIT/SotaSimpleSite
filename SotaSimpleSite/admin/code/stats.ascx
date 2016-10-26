<%@ Control Language="c#" %>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<%@ Import Namespace="Sota.Data.Simple"%>
<%@ Import Namespace="System.Data"%>
<%@ Register TagPrefix="sota" Namespace="Sota.Web.UI.WebControls" Assembly="SotaSimpleSite"%>
<%@ Register TagPrefix="sss" Namespace="Sota.Web.SimpleSite" Assembly="SotaSimpleSite"%>
<script runat="server">
	DataTable logs = null;//Log.GetAll();
	ArrayList arr = null;
	ArrayList arrA = Log.GetTypes();
	void Page_Load(object sender, EventArgs e)
	{
		DateTime now = Config.Main.Now().Date;
		dtBegin.Date = now.AddDays(-1);
		dtBegin.MaxDate = now;
		dtEnd.MaxDate = now;
		if(!IsPostBack)
		{
			ReadData();
			arr = arrA;
		}
	}
	void ReadData()
	{
		 logs = Log.GetByParams(null, dtEnd.Date.AddDays(1).AddMinutes(-1), dtBegin.Date.AddMinutes(-1), null);
	}
	void Show(object sender, EventArgs e)
	{
		 GetDate(dtBegin);
		 GetDate(dtEnd);
		 ReadData();
		 
		 if(!Util.IsBlank(Request.Form["type"]))
		 {
			arr = new ArrayList(Request.Form["type"].Split(','));
		 }
		 else
		 {
			arr = arrA;
		 }
	}
	void GetDate(Sota.Web.UI.WebControls.CBDatePicker cb)
	{
		int year	= -1;
		int month	= -1;
		int day		= -1;
		if (Request.Form[cb.YearControlName] != null)
		{
			year	= Int32.Parse(Request.Form[cb.YearControlName]);
		}

		if (Request.Form[cb.MonthControlName] != null)
		{
			month	= Int32.Parse(Request.Form[cb.MonthControlName]);
		}

		if (Request.Form[cb.DayControlName] != null)
		{
			day		= Int32.Parse(Request.Form[cb.DayControlName]);
		}
		if(year!=-1 && month!=-1 && day!=-1)
		{
			cb.Date = new DateTime(year,month,day);
		}
	}
	int GetL(DateTime date, object type)
	{
		return logs.Select("(type='"+type+"') AND (datetime<'"+date.AddDays(1).ToString("yyyy-MM-dd")+"') AND (datetime>='"+date.ToString("yyyy-MM-dd")+"')").Length;
	}
</script>
<style type="text/css">
table.form td{padding:2px;}
.stats td,.stats th{text-align:center;border:1px solid #ddd;padding:2px;color:#444;}
.stats {margin-bottom:20px;}
</style>
<table class="form">
<tr>
<td style="vertical-align:middle">С </td>
<td><sota:cbdatepicker runat="server" id="dtBegin"/>
</td>
<td style="vertical-align:middle"> по </td>
<td><sota:cbdatepicker runat="server" id="dtEnd"/></td>
</tr>
<tr>
<td colspan="4">
<%for(int i=0;i<arrA.Count;i++){%>
<input type="checkbox" name="type" value="<%=arrA[i]%>" id="type<%=i%><%if(arr.Contains(arrA[i])){%>" checked="checked<%}%>" /><label for="type<%=i%>"><%=arrA[i]%></label>
<%}%>
</td>
</tr>
<tr>
<td colspan="4"><asp:button runat="server" text="Показать" onclick="Show" id="Button1" />
<a href='log_view.aspx?type=Unique&sort=1&date_f=<%=dtBegin.Date.ToString("yyyy-MM-dd")%>&date_t=<%=dtEnd.Date.AddDays(1).ToString("yyyy-MM-dd")%>'>Подробнее &gt;&gt;</a>
</td>
</tr>
</table>
<br />
<table class="stats">
<tr style="font-weight:bold;background-color:#eee;">
<td colspan="2">Дата</td>
<%
ArrayList arr1 = new ArrayList();
for(int i=0;i<arr.Count;i++)
{
	arr1.Add(0);
	%>
	<td><%=arr[i]%></td>
	<%
%>
<%}%>
</tr>
<%for(DateTime date = dtBegin.Date;date<=dtEnd.Date;date=date.AddDays(1))
{
	%>
	<tr>
	<td><%=date.ToLongDateString()%></td>
	<td style="<%if(date.DayOfWeek==DayOfWeek.Sunday || date.DayOfWeek==DayOfWeek.Saturday){%>color:#f77;<%}%>"><%=date.ToString("ddd")%></td>
	
	<%
	for(int i=0;i<arr.Count;i++)
	{
		int c = GetL(date, arr[i]);
		arr1[i] = Convert.ToInt32(arr1[i]) + c;
		%>
		<td><%=c%></td>
		<%
	}
	%>
	</tr>
	<%if(date.DayOfWeek==DayOfWeek.Sunday || date==dtEnd.Date){%>
	<tr style="font-weight:bold;background-color:#eee;">
	<td colspan="2">Итого  за неделю</td>
	<%
			for(int i=0;i<arr.Count;i++)
			{
			%>
			<td><%=arr1[i]%></td>
			<%
				arr1[i] = 0;
			}%>
	</tr>
	<%
	}
}%>

</table>
