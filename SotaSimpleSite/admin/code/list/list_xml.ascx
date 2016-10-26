<%@ Control Language="c#" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<%@ Import Namespace="System.Data"%>
<script runat="server">
protected override void OnLoad(EventArgs e)
{
    Response.Clear();
	Response.ContentEncoding = Encoding.UTF8;
	Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
	Response.ContentType = "text/xml";
	Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
	try
	{ 
		string qs_list = Request.QueryString["list"];
		string qs_value = Request.QueryString["value"]== null ? "_id" : Request.QueryString["value"];
		string qs_text = Request.QueryString["text"]== null ? "name" : Request.QueryString["text"];
		if(qs_list==null)
		{
			Response.Write("<list><option value=\"INVALID REQUEST\" /></list>");
		}
		else
		{
			string sort = Request.QueryString["sort"]==null ? "" : Request.QueryString["sort"];
			string filter = Request.QueryString["filter"]==null ? "" : Request.QueryString["filter"];
			Response.Write("<list>");
			DataRow[] rows = null;
			if(qs_list=="users")
			{
				rows = Sota.Web.SimpleSite.Security.SecurityManager.GetUsers().Select(filter,sort);
			}
			else if (qs_list == "s_groups")
			{
				rows = Sota.Web.SimpleSite.Security.SecurityManager.GetGroups().Select(filter, sort);
			}
			else if (qs_list == "s_parts")
			{
				rows = Sota.Web.SimpleSite.Security.SecurityManager.GetParts().Select(filter, sort);
			}
			else if (qs_list == "s_actions")
			{
				rows = Sota.Web.SimpleSite.Security.SecurityManager.GetActions().Select(filter, sort);
			}
			else
			{
				List l = List.Create(qs_list);
				switch(Request.QueryString["type"])
				{
					case "root":
						rows = l.ReadRootItems().Select(filter, sort);
						break;
					case "parent":
						rows = l.ReadRootParentItems().Select(filter, sort);
						break;
                    case "optgroup":
					case "group":
						int parentLevel = 0;
						if(Request.QueryString["level"]!=null)
						{
							parentLevel = int.Parse(Request.QueryString["level"]);
						}
                        string ptext = Request.QueryString["ptext"] == null ? qs_text : Request.QueryString["ptext"];
                        string pvalue = Request.QueryString["pvalue"] == null ? qs_value : Request.QueryString["pvalue"];
                        l.ReadAllFull();
						DataRow[] r1 = l.Data.Select("("+List.FIELD_DELETED+"=0) AND ("+List.FIELD_LEVEL+"="+parentLevel+")", sort);
						for(int i=0;i<r1.Length;i++)
						{
							
							DataRow[] r2 = l.Data.Select("("+List.FIELD_DELETED+"=0) AND ("+List.FIELD_PARENT_ID+"="+r1[i][List.FIELD_ID]+")", sort);
							if(r2.Length>0)
							{
                                if (Request.QueryString["type"] == "optgroup")
                                {
                                    Response.Write(string.Format("<option text=\"{0}\" />",  
										EncodeParam(r1[i][ptext])));
                                }
                                else
                                { 
                                    Response.Write(string.Format("<option value=\"{0}\" text=\"{1}\" />",
										 EncodeParam(r1[i][pvalue]), 
										 EncodeParam(r1[i][ptext]))); 
                                }
                            }
							for(int j=0;j<r2.Length;j++)
							{
								Response.Write(string.Format("<option value=\"{0}\" text=\"{1}{2}\" />",
                                     EncodeParam(r2[j][qs_value]),
                                    Request.QueryString["type"] == "optgroup" ? "" : "-     ",
                                     EncodeParam(r2[j][qs_text])));
							}
						}
						break;
					default:
						rows = l.ReadAllFull().Select(filter, sort);
						break;
				}
			}
			if(rows!=null)
			{
				foreach(DataRow r in rows)
				{
					Response.Write(string.Format("<option value=\"{0}\" text=\"{1}\"/>", 
						 EncodeParam(r[qs_value]),
						 EncodeParam(r[qs_text])));
				}
			}
			Response.Write("</list>");
		}
	}
	catch(Exception ex)
	{
		Response.Clear();
		Response.Write(string.Format("<list><option value=\"{0}\" /></list>", Server.HtmlEncode(ex.ToString())));
	}
    Response.End();
}
string EncodeParam(object value)
{
	return value.ToString().Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");; 
}
</script>