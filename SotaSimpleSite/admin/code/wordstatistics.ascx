<%@ Control Language="c#" CodeBehind="wordstatistics.ascx.cs" AutoEventWireup="false" Inherits="Sota.Web.SimpleSite.Code.Admin.WordStatistics" targetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<style type="text/css">
<!--
.divWords
{
	OVERFLOW:auto;
	HEIGHT:100%;
	padding-right: 10px;
}
.divWords a
{
	color: #000000;
	text-decoration:none;
}
.divWords img 
{
	border: 0px;
	vertical-align: middle;
}
-->
</style>
<script type="text/javascript">
<!--
function InsertWord(w)
{
	if(opener)
		opener.InsertWord(w);
	else if(window.dialogArguments)
		window.dialogArguments.InsertWord(w);
}
//-->
</script>
<table width="100%" height="100%">
	<tr height="100%">
	<td>
	<div class="divWords">
			<table border="0" cellpadding="2" cellspacing="0" width="100%">
				<asp:repeater id="rptWords" runat="server">
					<itemtemplate>		
					<%string Img = ResolveUrl("~/admin.ashx?img=");%>
						<tr>
						<td><%=++iRowNumber%></td>
							<td width="100%" nowrap>
								<a href='javascript:InsertWord(<%# "\""+DataBinder.Eval(Container.DataItem, "word").ToString()+"\""%>);' title="Добавить">
									<img src="<%=Img%>key.gif"> <%#DataBinder.Eval(Container.DataItem, "word")%>
								</a>
							</td>
							<td align="right" nowrap><%#DataBinder.Eval(Container.DataItem, "count")%></td>
						</tr>
					</itemtemplate>
				</asp:repeater>
			</table>
			</div>
		</td>
		</tr>
	<tr>
		<td align="center"><input type="button" value="Закрыть" onclick="window.close();"></td>
	</tr>
</table>
