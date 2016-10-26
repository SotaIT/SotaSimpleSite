<%@ Control Language="c#" AutoEventWireup="false" Codebehind="execute.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.execute" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>



<script type="text/javascript">
function InsertTemplate(id)
{
	document.getElementById('<%=txtCmd.ClientID%>').innerText = document.getElementById(id).innerText;
}
</script>

<table width="100%">
	<tr>
		<td><asp:label id="lblError" runat="server" forecolor="red"></asp:label></td></tr>
	<tr><td><asp:DropDownList Runat="server" ID="cmbConnector"></asp:DropDownList></td></tr>
	<tr>
		<td width="100%"><asp:textbox id="txtCmd" width="100%" height="200px" runat="server" textmode="MultiLine">
			</asp:textbox></td>
		<td valign="top" nowrap><a href="#" onclick="InsertTemplate('create_sql');">Создать БД
				(SQL Server)</a><br /><br>
				<a href="#" onclick="InsertTemplate('check_sql');">Проверить базу
				(SQL Server)</a><br /><br>
				<a href="#" onclick="InsertTemplate('clear_log_sql');">Очистить логи
				(SQL Server, MySQL)</a><br /><br>
				<a href="#" onclick="InsertTemplate('clear_ex_sql');">Очистить логи
				ошибок (SQL Server, MySQL)</a><br /><br>
				<a href="#" onclick="InsertTemplate('clear_list_sql');">Очистить список
				(SQL Server)</a><br /><br>
				</td></tr>
	<tr>
		<td>
			<asp:button id="cmdOk" runat="server" text="Execute" /></td>
		<td>&nbsp;</td>
	</tr></table>
<asp:label id="lblAffected" runat="server"></asp:label>
<asp:placeholder id="phGrid" runat="server">
</asp:placeholder>
<div style="DISPLAY:none">
	<div id="create_sql">
		CREATE TABLE [List] (
		[iItemId] [int] IDENTITY (1, 1) NOT NULL ,
		[iParentId] [int] NULL CONSTRAINT [DF_List_iParentId] DEFAULT ((-1)),
		[sListName] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		[sGuid] [nvarchar] (255) COLLATE Cyrillic_General_CI_AS NULL ,
		[iDeleted] [int] NULL CONSTRAINT [DF_List_bDeleted] DEFAULT (0),
		[iLevel] [int] NULL ,
		CONSTRAINT [PK_List] PRIMARY KEY CLUSTERED
		(
		[iItemId]
		) ON [PRIMARY]
		) ON [PRIMARY]
		;<br><br>
		CREATE TABLE [ListItemField] (
		[iItemId] [int] NOT NULL ,
		[sFieldName] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NOT NULL ,
		[sValue] [ntext] COLLATE Cyrillic_General_CI_AS NULL ,
		CONSTRAINT [PK_ListItemField] PRIMARY KEY CLUSTERED
		(
		[iItemId],
		[sFieldName]
		) ON [PRIMARY] ,
		CONSTRAINT [FK_ListItemField_List] FOREIGN KEY
		(
		[iItemId]
		) REFERENCES [List] (
		[iItemId]
		)
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
		;<br><br>
		CREATE TABLE [Logs] (
		[iLogId] [int] IDENTITY (1, 1) NOT NULL,
		[dtDateTime] [datetime] NULL ,
		[sType] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		[sParams] [ntext] COLLATE Cyrillic_General_CI_AS NULL ,
		[sSite] [nvarchar] (20) COLLATE Cyrillic_General_CI_AS NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
		;<br><br>
		CREATE TABLE [UserInGroup] (
		[iUserId] [int] NOT NULL ,
		[iGroupId] [int] NOT NULL ,
		CONSTRAINT [PK_UserInGroup] PRIMARY KEY CLUSTERED
		(
		[iUserId],
		[iGroupId]
		) ON [PRIMARY]
		) ON [PRIMARY]
		;<br><br>
		CREATE TABLE [UserGroup] (
		[iGroupId] [int] IDENTITY (1, 1) NOT NULL ,
		[sGroupName] [nvarchar] (255) COLLATE Cyrillic_General_CI_AS NULL ,
		CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED
		(
		[iGroupId]
		) ON [PRIMARY]
		) ON [PRIMARY]
		;<br><br>
		CREATE TABLE [User] (
		[iUserId] [int] IDENTITY (1, 1) NOT NULL ,
		[sLogin] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		[sEmail] [nvarchar] (128) COLLATE Cyrillic_General_CI_AS NULL ,
		[sPassword] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		[iEnabled] [int] NULL ,
		[sActivationCode] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED
		(
		[iUserId]
		) ON [PRIMARY]
		) ON [PRIMARY]
		;<br><br>
		CREATE TABLE [SecurityPart] (
		[iPartId] [int] IDENTITY (1, 1) NOT NULL ,
		[sPartName] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		CONSTRAINT [PK_SecurityPart] PRIMARY KEY CLUSTERED
		(
		[iPartId]
		) ON [PRIMARY]
		) ON [PRIMARY]
		;<br><br>
		CREATE TABLE [Action] (
		[iActionId] [int] IDENTITY (1, 1) NOT NULL ,
		[sActionName] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		[sActionId] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NULL ,
		CONSTRAINT [PK_Action] PRIMARY KEY CLUSTERED
		(
		[iActionId]
		) ON [PRIMARY]
		) ON [PRIMARY]
		;<br><br>
		CREATE TABLE [AccessRule] (
		[iGroupId] [int] NOT NULL ,
		[iPartId] [int] NOT NULL ,
		[iActionId] [int] NOT NULL ,
		CONSTRAINT [PK_GroupCan] PRIMARY KEY CLUSTERED
		(
		[iGroupId],
		[iPartId],
		[iActionId]
		) ON [PRIMARY]
		) ON [PRIMARY]
		;<br><br>
		CREATE TABLE [UserField] (
		[iUserId] [int] NOT NULL ,
		[sFieldName] [nvarchar] (50) COLLATE Cyrillic_General_CI_AS NOT NULL ,
		[sValue] [ntext] COLLATE Cyrillic_General_CI_AS NULL ,
		CONSTRAINT [PK_UserField] PRIMARY KEY CLUSTERED
		(
		[iUserId],
		[sFieldName]
		) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
		;
	</div>
	<div id="check_sql">
	SELECT TOP 1 * FROM Logs;<br>
	SELECT TOP 1 * FROM List;<br>
	SELECT TOP 1 * FROM ListItemField;<br>
	SELECT TOP 1 * FROM AccessRule;<br>
	SELECT TOP 1 * FROM Action;<br>
	SELECT TOP 1 * FROM SecurityPart;<br>
	SELECT TOP 1 * FROM [User];<br>
	SELECT TOP 1 * FROM UserField;<br>
	SELECT TOP 1 * FROM UserGroup;<br>
	SELECT TOP 1 * FROM UserInGroup;
	</div>
	<div id="clear_log_sql">
	DELETE FROM Logs WHERE sType = 'Hit' OR sType = 'SearchEngine' 
	</div>
	<div id="clear_ex_sql">
	DELETE FROM Logs WHERE sType LIKE '%Exception'
	</div>
	<div id="clear_list_sql">
	DELETE FROM [ListItemField]
      WHERE iItemId in (SELECT iItemId FROM List WHERE sListName='ListName');<br>
	DELETE FROM [List]
      WHERE sListName='ListName';
	</div>
</div>
