<%@ Control language="c#" %>
<%@Import Namespace="Sota.Web.SimpleSite"%>
<%@Import Namespace="Sota.Web.SimpleSite.Security"%>
				</td>
			</tr>
			<tr>
				<td><br>
					<table cellpadding="4" cellspacing="0" border="0" width="100%" class="bottomt">
						<tr>
							<td nowrap>SotaSimpleSite v<%=Config.GetVersion()%><%=HttpContext.Current.IsDebuggingEnabled ? " debug" : ""%> (.net <%=Config.GetRuntimeVersion()%>)</td>
							<td width="100%" align="center">&copy; Copyright 2005-<%=DateTime.Now.Year %> SOTA IT .NET</td>
							<%if(UserInfo.Current.IsAuthorized){%><td><a href='?logout='>Выход</a></td><%}%>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</body>
</html>