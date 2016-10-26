using System;
using System.Web.UI.WebControls;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Используется для загрузки контролов.
	/// </summary>
	public class ControlLoader : PlaceHolder
	{
		string _path = "";
		public string Path
		{
			get{ return _path; }
			set{ _path = value; }
		}
		protected override void CreateChildControls()
		{
			try
			{
				this.Controls.Add(this.Page.LoadControl(_path));
			}
			catch(Exception ex)
			{
				Config.ReportError(ex);
			}
			base.CreateChildControls ();
		}

	}
}
