<%@ Control Language="c#" %>

<script runat="server">
void Page_Load(object sender, EventArgs e)
    {
		Sota.Web.SimpleSite.Map.MapItem m = Sota.Web.SimpleSite.SiteMap.Current.GetCurrentItem();
		ArrayList arr = new ArrayList();
		for (int i = 0; i < m.Items.Count; i++)
		{
			if (!m.Items[i].Hidden)
			{
				arr.Add(m.Items[i]);
			}
		}
		rpt.DataSource = arr;
		rpt.DataBind();
    }
</script>
<%=Sota.Web.SimpleSite.PageInfo.Current.Body%>
<ul class="list">
<asp:Repeater runat="server" ID="rpt">
<ItemTemplate>
	<li><a href="<%# Eval("Url") %>"><%# Eval("Text") %></a></li>
</ItemTemplate>
</asp:Repeater>
</ul>