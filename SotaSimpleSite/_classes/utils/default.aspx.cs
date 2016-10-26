//using System;
//using System.Data;
//
//namespace Sota.Web.SimpleSite.Utils
//{
//	/// <summary>
//	/// Добавляет домен в список доменов, если его там не было
//	/// </summary>
//	public class DefineDefaultPage : System.Web.UI.Page
//	{
//		private void AddDomain(string protocol, string domain, string main)
//		{
//			DataTable tb = Config.GetDomains();
//			DataRow rn = tb.NewRow();
//			rn["name"] = domain;
//			rn["addpath"] = "";
//			rn["default"] = "default";
//			rn["protocol"] = protocol.Replace("://", "");
//			rn["main"] = main;
//			tb.Rows.Add(rn);
//			Config.WriteConfigTable("domain.config", tb, "domains");
//			Response.Redirect(rn["protocol"].ToString() + Keys.UrlPathProtocolDelimiter + rn["name"].ToString() + Keys.UrlPathDelimiter + rn["default"].ToString() + Config.Main.Extension);
//		}
//
//		protected override void OnLoad(EventArgs e)
//		{
//			string protocol = Util.GetProtocol(Request.IsSecureConnection);
//			string domain = Request.Url.AbsoluteUri.ToLower().Remove(0, protocol.Length).Replace("/default.aspx", "");
//
//			DataTable tb = Config.GetDomains();
//			if (tb.Rows.Count == 0)
//			{
//				AddDomain(protocol, domain, "1");
//			}
//			else
//			{
//				DataRow[] rs = tb.Select("name='" + domain + "'");
//				if (rs.Length == 0)
//				{
//					AddDomain(protocol, domain, "0");
//				}
//			}
//		}
//	}
//}