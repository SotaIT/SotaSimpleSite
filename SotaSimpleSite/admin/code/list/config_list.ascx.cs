using System.Data;
using System.Text;

namespace Sota.Web.SimpleSite.Code.Admin.list
{
	using System;
	using System.Web.UI.WebControls;

	/// <summary>
	///		Summary description for config_list.
	/// </summary>
	public class config_list : System.Web.UI.UserControl
	{
        protected System.Web.UI.HtmlControls.HtmlInputText txtConstr;
        protected System.Web.UI.HtmlControls.HtmlInputText txtTableName;
        protected System.Web.UI.HtmlControls.HtmlInputText txtLevels;
		protected System.Web.UI.HtmlControls.HtmlInputText txtSort;
		protected System.Web.UI.HtmlControls.HtmlInputText txtRootIcon;
		protected System.Web.UI.HtmlControls.HtmlInputText txtOpenAllIcon;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCloseAllIcon;
		protected System.Web.UI.WebControls.Button cmdSaveCommon;
		protected System.Web.UI.WebControls.Button cmdSaveEditor;
		protected System.Web.UI.WebControls.Button cmdVerify;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbProvider;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbSecurityPart;
		public string sList = string.Empty;
		public string sCaption = string.Empty;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox chkNoIcon;
		protected System.Web.UI.WebControls.DataGrid dgColumns;
		public string file = string.Empty;
		protected System.Web.UI.HtmlControls.HtmlInputText txtListCaption;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			string pagex = PageInfo.Current.Pagex;
			sList = PageInfo.Current.Path;
			if (pagex.Length > 0)
			{
				sList = sList.Substring(pagex.Length - 1);
				DataTable tb = Config.GetConfigTable("lists.config", "list");
				DataRow[] r = tb.Select("name='" + sList + "'");
				if (r.Length > 0)
				{
					file = Request.MapPath(r[0]["file"].ToString());
					sCaption = r[0]["caption"].ToString();


				}
				else
				{
					Response.End();
				}
			}
			else
			{
				Response.End();
			}
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			Read();
			this.txtListCaption.Value = this.sCaption;
			if (Request.RequestType.ToLower() == "post")
			{
				if (Request.Form[this.cmdSaveCommon.UniqueID] == null &&
					Request.Form[this.cmdSaveEditor.UniqueID] == null &&
					Request.Form[this.cmdVerify.UniqueID] == null )
				{
					if (Request.Form["hOldFieldName"] != null)
					{
						dgColumns_UpdateCommand();
					}
				}
			}
			else
			{
				if (Request.QueryString["editcolumn"] != null)
				{
					dgColumns_EditCommand(int.Parse(Request.QueryString["editcolumn"]));
				}
				else if (Request.QueryString["deletecolumn"] != null)
				{
					dgColumns_DeleteCommand(Request.QueryString["deletecolumn"]);
				}
				else if (Request.QueryString["newcolumn"] != null)
				{
					dgColumns_AddCommand();
				}
				else if (Request.QueryString["upcolumn"] != null)
				{
					dgColumns_UpCommand(int.Parse(Request.QueryString["upcolumn"]));
				}
				else if (Request.QueryString["downcolumn"] != null)
				{
					dgColumns_DownCommand(int.Parse(Request.QueryString["downcolumn"]));
				}
			}
		}


		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load+=new EventHandler(Page_Load);
			this.cmdSaveCommon.Click+=new EventHandler(cmdSaveCommon_Click);
			this.cmdSaveEditor.Click+=new EventHandler(cmdSaveEditor_Click);
			this.cmdVerify.Click+=new EventHandler(cmdVerify_Click);
		}

		#endregion
/*
 			this.Load+=new EventHandler(Page_Load);
			this.cmdSaveCommon.Click+=new EventHandler(cmdSaveCommon_Click);
			this.cmdSaveEditor.Click+=new EventHandler(cmdSaveEditor_Click);
 
 */
		#region shared

		private void Read()
		{
			DataTable tb = GetListTable();
			DataRow r = null;
			if (tb.Rows.Count > 0)
			{
				r = tb.Rows[0];
				this.txtConstr.Value = r["connectionstring"].ToString();
                this.txtTableName.Value = Util.IsBlank(r["table"]) ? "sss_List_" + sList : r["table"].ToString();

				for (int i = 0; i < this.cmbProvider.Items.Count; i++)
				{
					if (this.cmbProvider.Items[i].Value == r["providertype"].ToString())
					{
						this.cmbProvider.SelectedIndex = i;
						break;
					}
				}
			}

			tb = GetEditorTable();
			if (tb.Rows.Count > 0)
			{
				r = tb.Rows[0];
				this.txtRootIcon.Value = r["root_icon"].ToString();
				this.txtCloseAllIcon.Value = r["closeall_icon"].ToString();
				this.txtOpenAllIcon.Value = r["openall_icon"].ToString();
				this.txtLevels.Value = (Convert.ToInt32(r["levels"]) + 1).ToString();
				this.txtSort.Value = r["sort_expression"].ToString();
				this.chkNoIcon.Checked = r["no_icon"].ToString() == "1";
				
				DataTable dtParts = Sota.Web.SimpleSite.Security.SecurityManager.GetParts();
				string partID = r["part"].ToString();
				if (Request.Form[cmbSecurityPart.UniqueID] != null)
				{
					partID = Request.Form[cmbSecurityPart.UniqueID];
				}
				cmbSecurityPart.SelectedIndex = -1;
				cmbSecurityPart.Items.Clear();
				foreach (DataRow rp in dtParts.Rows)
				{
					ListItem li = new ListItem(rp["partname"].ToString(), rp["partid"].ToString());
					if (partID == li.Value)
					{
						li.Selected = true;
					}
					cmbSecurityPart.Items.Add(li);
				}
			}
			this.dgColumns.DataSource = GetColumnsTable();
			this.dgColumns.DataBind();

		}

		private DataTable GetTable(string name)
		{
			DataSet ds = new DataSet();
			ds.ReadXml(file);
			DataTable tb = ds.Tables[name];
			if (tb == null)
				tb = new DataTable(name);
			return tb;
		}

		private void SaveTable(DataTable tb)
		{
			DataSet ds = new DataSet("root");
			ds.ReadXml(file);
			if (ds.Tables.Contains(tb.TableName))
			{
				ds.Tables.Remove(tb.TableName);
			}
			ds.Tables.Add(tb.Copy());
			ds.WriteXml(file);
		}

		private void AppendOption(StringBuilder sb, string text, string value, bool selected)
		{
			sb.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", value, text, selected ? " selected" : "");
		}

		private void Redirect()
		{
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);
		}

		#endregion

		#region list & editor

		private DataTable GetListTable()
		{
			DataTable tb = GetTable("list");
			if (tb.Columns.Count == 0)
			{
				tb.Columns.Add("providertype");
				tb.Columns.Add("connectionstring");
				tb.Columns.Add("searchin");
                tb.Columns.Add("part");
            }
            if (!tb.Columns.Contains("table"))
            {
                tb.Columns.Add("table");
            }
			return tb;
		}

		private DataTable GetEditorTable()
		{
			DataTable tb = GetTable("editor");
			if (tb.Columns.Count == 0)
			{
				tb.Columns.Add("root_icon");
				tb.Columns.Add("closeall_icon");
				tb.Columns.Add("openall_icon");
				tb.Columns.Add("levels");
				tb.Columns.Add("sort_expression");
				tb.Columns.Add("no_icon");
			}
			return tb;
		}

		private void cmdSaveCommon_Click(object sender, System.EventArgs e)
		{
			DataTable tb = GetListTable();
			DataRow r = null;
			if (tb.Rows.Count > 0)
			{
				r = tb.Rows[0];
			}
			else
			{
				r = tb.NewRow();
				tb.Rows.Add(r);
			}
			r["providertype"]		= this.cmbProvider.Value;
            r["connectionstring"] = this.txtConstr.Value.Trim();
            r["table"] = this.txtTableName.Value.Trim();
            SaveTable(tb);
			Read();
		}

		private void cmdSaveEditor_Click(object sender, System.EventArgs e)
		{
			DataTable tb = GetEditorTable();
			DataRow r = null;
			if (tb.Rows.Count > 0)
			{
				r = tb.Rows[0];
			}
			else
			{
				r = tb.NewRow();
				tb.Rows.Add(r);
			}
			r["root_icon"] = this.txtRootIcon.Value;
			r["closeall_icon"] = this.txtCloseAllIcon.Value;
			r["openall_icon"] = this.txtOpenAllIcon.Value;
			r["levels"] = int.Parse(this.txtLevels.Value) - 1;
			r["sort_expression"] = this.txtSort.Value.Trim();
			r["no_icon"] = (this.chkNoIcon.Checked ? 1 : 0);
			r["part"] = Request.Form[this.cmbSecurityPart.UniqueID];
			SaveTable(tb);
			if (this.txtListCaption.Value != this.sCaption)
			{
				tb = Config.GetConfigTable("lists.config", "list").Copy();
				tb.Select("name='" + sList + "'")[0]["caption"] = this.txtListCaption.Value;
				Config.WriteConfigTable("lists.config", tb, "root");
			}
			Read();
		}

		#endregion

		#region columns

		private DataTable GetColumnsTable()
		{
			DataTable tb = GetTable("column");
			if (!tb.Columns.Contains("fieldname"))
				tb.Columns.Add("fieldname");
			if (!tb.Columns.Contains("caption"))
				tb.Columns.Add("caption");
			if (!tb.Columns.Contains("datatype"))
				tb.Columns.Add("datatype");
			if (!tb.Columns.Contains("inputtype"))
				tb.Columns.Add("inputtype");
			if (!tb.Columns.Contains("defaultvalue"))
				tb.Columns.Add("defaultvalue");
			if (!tb.Columns.Contains("allownull"))
				tb.Columns.Add("allownull");
			if (!tb.Columns.Contains("listurl"))
				tb.Columns.Add("listurl");
			if (!tb.Columns.Contains("expression"))
				tb.Columns.Add("expression");
			if (!tb.Columns.Contains("uploads"))
				tb.Columns.Add("uploads");
			if (!tb.Columns.Contains("filter"))
				tb.Columns.Add("filter");
			if (!tb.Columns.Contains("levels"))
				tb.Columns.Add("levels");
			if (!tb.Columns.Contains("regex"))
				tb.Columns.Add("regex");
			if (!tb.Columns.Contains("extended"))
				tb.Columns.Add("extended");
			return tb;
		}

		private string[] types = {"String", "Int32", "Boolean", "Double", "DateTime"};

		protected string GetDataTypes(string type)
		{
			int n = types.Length;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < n; i++)
			{
				AppendOption(sb, types[i], types[i], types[i] == type);
			}
			return sb.ToString();
		}

		private string[] intypes = {
			"none",
			"text",
			"password",
			"hidden",
			"regex",
			"textarea",
			"checkbox",
			"combobox",
			"file",
			"datetime",
			"date",
			"time",
			"html",
			"multi"
		};

		protected string GetInputTypes(string type)
		{
			int n = intypes.Length;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < n; i++)
			{
				AppendOption(sb, intypes[i], intypes[i], intypes[i] == type);
			}
			return sb.ToString();
		}

		private int GetLevels()
		{
			DataTable tb = GetEditorTable();
			if (tb.Rows.Count == 0)
				return 0;
			return Convert.ToInt32(tb.Rows[0]["levels"]) + 1;
		}

		protected string GetLevelsChecks(string levels)
		{
			if (levels == null)
				levels = string.Empty;
			int l = GetLevels();
			if (l == 0)
				return string.Empty;
			string ls = "," + levels + ",";
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < l; i++)
			{
				sb.AppendFormat("<input type=\"checkbox\" name=\"chkLevel{0}\"{1}>", i, ls.IndexOf("," + i.ToString() + ",") == -1 ? "" : " checked");
			}
			return sb.ToString();
		}

		private void dgColumns_AddCommand()
		{
			DataTable tb = GetColumnsTable();
			tb.Rows.InsertAt(tb.NewRow(), 0);
			this.dgColumns.EditItemIndex = 0;
			this.dgColumns.DataSource = tb;
			this.dgColumns.DataBind();
		}

		private void dgColumns_EditCommand(int index)
		{
			this.dgColumns.EditItemIndex = index;
			this.dgColumns.DataSource = GetColumnsTable();
			this.dgColumns.DataBind();
		}

		private void dgColumns_UpdateCommand()
		{
			if (Request.Form["txtFieldName"].Trim().Length == 0)
				Redirect();
			DataTable tb = GetColumnsTable();
			DataRow r = null;
			DataRow[] rs = tb.Select("fieldname='" + Request.Form["hOldFieldName"] + "'");
			if (rs.Length > 0)
			{
				r = rs[0];
			}
			else if (tb.Select("fieldname='" + Request.Form["txtFieldName"] + "'").Length == 0)
			{
				r = tb.NewRow();
				tb.Rows.Add(r);
			}
			else
			{
				Redirect();
			}
			if (r["fieldname"].ToString() != Request.Form["txtFieldName"].ToLower())
			{
				if (tb.Select("fieldname='" + Request.Form["txtFieldName"] + "'").Length > 0)
					return;
			}
			r["fieldname"] = Request.Form["txtFieldName"].ToLower();
			r["caption"] = Request.Form["txtFieldCaption"];
			r["datatype"] = Request.Form["cmbFieldDataType"];
			r["inputtype"] = Request.Form["cmbFieldInputType"];
			r["defaultvalue"] = Request.Form["txtFieldDefaultValue"];
			r["allownull"] = (Request.Form["chkFieldAllowNull"] != null ? "1" : "0");

			r["expression"] = DBNull.Value;
			r["regex"] = DBNull.Value;
			r["listurl"] = DBNull.Value;
			r["uploads"] = DBNull.Value;
			r["filter"] = DBNull.Value;
			switch (Request.Form["cmbFieldInputType"])
			{
				case "none":
					r["expression"] = Request.Form["txtFieldExpression"];
					break;
				case "regex":
					r["regex"] = Request.Form["txtFieldRegex"];
					break;
				case "multi":
				case "combobox":
					r["listurl"] = Request.Form["txtFieldListUrl"].StartsWith(Keys.UrlParamPageDelimiter) ? Keys.ServerRoot+"/admin/listxml"+Config.Main.Extension+Request.Form["txtFieldListUrl"] : Request.Form["txtFieldListUrl"];
					break;
				case "file":
					r["uploads"] = Request.Form["txtFieldUploads"];
					string uploadsFolder = Request.MapPath(Request.Form["txtFieldUploads"]);
					if (!System.IO.Directory.Exists(uploadsFolder))
					{
						System.IO.Directory.CreateDirectory(uploadsFolder);
					}

					r["filter"] = Request.Form["txtFieldFilter"];
					break;
			}
			string levels = string.Empty;
			int l = GetLevels();
			for (int i = 0; i < l; i++)
			{
				levels += Request.Form["chkLevel" + i.ToString()] != null ? "," + i.ToString() : "";
			}
			r["levels"] = levels.Length > 0 ? levels.Substring(1) : "";
			this.SaveTable(tb);
			this.dgColumns.EditItemIndex = -1;
			Redirect();
		}

		private void dgColumns_DeleteCommand(string field)
		{
			DataTable tb = GetColumnsTable();
			DataRow[] r = tb.Select("fieldname='" + field + "'");
			if (r.Length > 0)
			{
				tb.Rows.Remove(r[0]);
			}
			this.SaveTable(tb);
			Redirect();
		}

		private void dgColumns_DownCommand(int i)
		{
			DataTable tb = GetColumnsTable();
			if(i<tb.Rows.Count-1)
			{
				DataRow row = tb.NewRow();
				foreach(DataColumn col in tb.Columns)
				{
					row[col] = tb.Rows[i][col];
				}
				tb.Rows.RemoveAt(i);
				tb.Rows.InsertAt(row, i+1);
				this.SaveTable(tb);
			}
			Redirect();
		}

		private void dgColumns_UpCommand(int i)
		{
			if(i>0)
			{
				DataTable tb = GetColumnsTable();
				DataRow row = tb.NewRow();
				foreach(DataColumn col in tb.Columns)
				{
					row[col] = tb.Rows[i][col];
				}
				tb.Rows.RemoveAt(i);
				tb.Rows.InsertAt(row, i-1);
				this.SaveTable(tb);
			}
			Redirect();
		}
		#endregion

		private void cmdVerify_Click(object sender, EventArgs e)
		{
			List.Create(sList).VerifyDataStructure();
		}

		protected string GetSecurityParts()
		{
			return "";
		}
	}
}