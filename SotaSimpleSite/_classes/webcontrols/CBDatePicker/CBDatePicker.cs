using System;
using System.Globalization;
using System.Web.UI;
using System.ComponentModel;

namespace Sota.Web.UI.WebControls
{
	/// <summary>
	/// Контрол для выбора даты из трех списков(месяц, год, день)
	/// </summary>
	[DefaultProperty("SelectedDate"),
		ToolboxData("<{0}:CBDatePicker runat=server></{0}:CBDatePicker>")]
	public sealed class CBDatePicker : System.Web.UI.WebControls.Table, System.Web.UI.IPostBackDataHandler
	{

		[Category("Appearance"),
			DefaultValue(MonthNameType.Numbers)]
		public MonthNameType MonthType
		{
			get
			{
				return ViewState["MonthNameType"] == null 
					? MonthNameType.Numbers 
					: (MonthNameType)ViewState["MonthNameType"];
			}
			set
			{
				ViewState["MonthNameType"] = value;
			}
		}

		[Browsable(false)]
		public string[] MonthNames
		{
			get 
			{ 
				return  ViewState["MonthNames"]==null 
					? new string[]{
									  "01", "02", "03", "04",
									  "05", "06", "07", "08",
									  "09", "10", "11", "12"
								  }
					: (string[])ViewState["MonthNames"]; 
			}
			set
			{
				ViewState["MonthNameType"]	= MonthNameType.Custom;
				ViewState["MonthNames"]		= value;
			}
		}
		private string[] GetMonthNames()
		{
			if(MonthType==MonthNameType.Custom)
			{
				return MonthNames;
			}
			return GetMonthNames(MonthType, uiCulture);
		}
		public static string[] GetMonthNames(MonthNameType monthNameType)
		{
			return GetMonthNames(monthNameType, CultureInfo.CurrentUICulture);
		}
		public static string[] GetMonthNames(MonthNameType monthNameType, CultureInfo culture)
		{
			switch (monthNameType)
			{
				case MonthNameType.Numbers:
					return new string[]{
										   "01", "02", "03", "04",
										   "05", "06", "07", "08",
										   "09", "10", "11", "12",
											""
									   };
				case MonthNameType.ShortNames:
					return culture.DateTimeFormat.AbbreviatedMonthNames;
				case MonthNameType.FullNames:
					return culture.DateTimeFormat.MonthNames;
				case MonthNameType.GenitiveNames:
					return new string[]
						{
							new DateTime(2006,1,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,2,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,3,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,4,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,5,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,6,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,7,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,8,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,9,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,10,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,11,1).ToString("ddMMMM").Substring(2),
							new DateTime(2006,12,1).ToString("ddMMMM").Substring(2),
							""};
			}
			return null;
		}
		[Bindable(true),
			Category("Behavior")]
		public DateTime Date
		{
			get
			{
				return  ViewState["Date"] == null 
					? DateTime.Now.Date 
					: (DateTime) ViewState["Date"];
			}
			set
			{
				if(value > MaxDate || value < MinDate)
				{
					//ViewState["Date"] = MaxDate;
					throw new ArgumentOutOfRangeException("value",value,string.Format("Date must be between {0} and {1}", MinDate.ToShortDateString(), MaxDate.ToShortDateString()));
				}
				else
				{
					ViewState["Date"] = value;
				}
			}
		}
		[Category("Appearance"),
		DefaultValue(typeof(CultureInfo),"ru-RU")]
		public CultureInfo uiCulture
		{
			get
			{
				return ViewState["uiCulture"] == null 
					? CultureInfo.CurrentUICulture 
					: (CultureInfo) ViewState["uiCulture"];
			}
			set
			{
				ViewState["uiCulture"] = value;
			}
		}

		[Bindable(true),
			Category("Behavior"),
			DefaultValue(false)]
		public bool IsNull
		{
			get
			{
				return ViewState["IsNull"] == null
					? false 
					: Convert.ToBoolean(ViewState["IsNull"]);
			}
			set { ViewState["IsNull"] = value; }
		}

		[Bindable(true),
			Category("Behavior"),
			DefaultValue(false)]
		public bool UseNull
		{
			get
			{
				return ViewState["UseNull"] == null 
					? false 
					: Convert.ToBoolean(ViewState["UseNull"]);
			}
			set { ViewState["UseNull"] = value; }
		}

		[Bindable(true),
			Category("Behavior"),
			DefaultValue("~/adminscript.ashx?script=comboboxdatepicker.js")]
		public string ScriptFile
		{
			get 
			{
				return ViewState["ScriptFile"] == null 
					?	"~/adminscript.ashx?script=comboboxdatepicker.js"
					:	(string)ViewState["ScriptFile"];
			}
			set { ViewState["ScriptFile"] = value; }
		}

		[Bindable(true),
			Category("Behavior"),
		DefaultValue(typeof(DateTime), "9999-12-31")]
		public DateTime MaxDate
		{
			get
			{
				return ViewState["MaxDate"] == null
					? DateTime.MaxValue.Date 
					: (DateTime) ViewState["MaxDate"];
			}
			set
			{
				if(value < MinDate)
				{
					value = MinDate;
				}
				if(value < Date)
				{
					ViewState["Date"] = value;
				}
				ViewState["MaxDate"] = value;
			}
		}

		[Bindable(true),
			Category("Behavior"),
			DefaultValue("")]
		public DateTime MinDate
		{
			get 
			{
				return ViewState["MinDate"] == null
					? DateTime.MinValue.Date 
					: (DateTime) ViewState["MinDate"];
			}
			set
			{
				if(value > Date)
				{
					ViewState["Date"] = value;
				}
				ViewState["MinDate"] = value;
			}
		}

		[Browsable(false)]
		public string DayControlName
		{
			get { return this.UniqueID + ":Day"; }
		}

		[Browsable(false)]
		public string DayControlID
		{
			get { return this.ClientID + "_Day"; }
		}

		[Browsable(false)]
		public string MonthControlName
		{
			get { return this.UniqueID + ":Month"; }
		}

		[Browsable(false)]
		public string MonthControlID
		{
			get { return this.ClientID + "_Month"; }
		}

		[Browsable(false)]
		public string YearControlName
		{
			get { return this.UniqueID + ":Year"; }
		}

		[Browsable(false)]
		public string YearControlID
		{
			get { return this.ClientID + "_Year"; }
		}

		[Browsable(false)]
		public string MaxHiddenControlName
		{
			get { return this.UniqueID + ":MaxHidden"; }
		}

		[Browsable(false)]
		public string MaxHiddenControlID
		{
			get { return this.ClientID + "_MaxHidden"; }
		}

		[Browsable(false)]
		public string MinHiddenControlName
		{
			get { return this.UniqueID + ":MinHidden"; }
		}

		[Browsable(false)]
		public string MinHiddenControlID
		{
			get { return this.ClientID + "_MinHidden"; }
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.Page != null)
			{
				if(this.AutoPostBack)
				{
					Page.RegisterRequiresPostBack(this);
				}
				else if (!this.Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "CBDatePickerScript"))
				{
					this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "CBDatePickerScript",
						"<script src=\"" + this.Page.ResolveUrl(this.ScriptFile) + "\" type=\"text/javascript\"></script>");
				}
			}
		}

		/// <summary>
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			base.RenderBeginTag(output);
			output.RenderBeginTag(HtmlTextWriterTag.Tr);
			output.RenderBeginTag(HtmlTextWriterTag.Td);
			RenderDay(output);
			output.RenderEndTag(); //td
			output.RenderBeginTag(HtmlTextWriterTag.Td);
			RenderMonth(output);
			output.RenderEndTag(); //td
			output.RenderBeginTag(HtmlTextWriterTag.Td);
			RenderYear(output);
			RenderMaxMin(output);
			output.RenderEndTag(); //td
			output.RenderEndTag(); //tr
			output.RenderEndTag(); //table
		}

		private void RenderDay(HtmlTextWriter output)
		{
			int nDays = DateTime.DaysInMonth(this.Date.Year, this.Date.Month) + 1;
			output.AddAttribute(HtmlTextWriterAttribute.Id, this.DayControlID);
			output.AddAttribute(HtmlTextWriterAttribute.Name, this.DayControlName);
			if (this.AutoPostBack)
				output.AddAttribute(HtmlTextWriterAttribute.Onchange, this.Page.ClientScript.GetPostBackEventReference(this, ""));
			else
				output.AddAttribute(HtmlTextWriterAttribute.Onchange, "CBDatePicker_DayChanged('" + this.ClientID + "');");
			output.RenderBeginTag(HtmlTextWriterTag.Select);
			if(this.UseNull)
			{
				output.WriteBeginTag("option");
				output.WriteAttribute("value", "-1");
				if (this.IsNull)
					output.Write(" selected=\"selected\"");
				output.Write(">");
				output.Write("");
				output.WriteEndTag("option");
				output.WriteLine("");
			}
			for (int i = 1; i < nDays; i++)
			{
				output.WriteBeginTag("option");
				output.WriteAttribute("value", i.ToString());
				if (this.Date.Day == i && !this.IsNull)
					output.Write(" selected=\"selected\"");
				output.Write(">");
				if (i < 10)
					output.Write("0");
				output.Write(i.ToString());
				output.WriteEndTag("option");
				output.WriteLine("");
			}
			output.RenderEndTag(); //select
		}

		private void RenderMonth(HtmlTextWriter output)
		{
			output.AddAttribute(HtmlTextWriterAttribute.Id, this.MonthControlID);
			output.AddAttribute(HtmlTextWriterAttribute.Name, this.MonthControlName);
			if (this.AutoPostBack)
				output.AddAttribute(HtmlTextWriterAttribute.Onchange, this.Page.ClientScript.GetPostBackEventReference(this, ""));
			else
				output.AddAttribute(HtmlTextWriterAttribute.Onchange, "CBDatePicker_MonthChanged('" + this.ClientID + "');");
			output.RenderBeginTag(HtmlTextWriterTag.Select);
			if(this.UseNull)
			{
				output.WriteBeginTag("option");
				output.WriteAttribute("value", "-1");
				if (this.IsNull)
					output.Write(" selected=\"selected\"");
				output.Write(">");
				output.Write("");
				output.WriteEndTag("option");
				output.WriteLine("");
			}
			string[] monthNames = GetMonthNames();
			for (int i = 0; i < 12; i++)
			{
				string sMonth = monthNames.Length > i ? monthNames[i] : (i + 1).ToString();
				output.WriteBeginTag("option");
				output.WriteAttribute("value", (i + 1).ToString());
				if (this.Date.Month == i + 1 && !this.IsNull)
					output.Write(" selected=\"selected\"");
				output.Write(">");
				output.Write(sMonth);
				output.WriteEndTag("option");
				output.WriteLine("");
			}
			output.RenderEndTag(); //select
		}

		private void RenderYear(HtmlTextWriter output)
		{
			int minYear = Math.Max(this.Date.Year - 5, MinDate.Year);
			int maxYear = Math.Min(this.Date.Year + 5, MaxDate.Year)+1;
			output.AddAttribute(HtmlTextWriterAttribute.Id, this.YearControlID);
			output.AddAttribute(HtmlTextWriterAttribute.Name, this.YearControlName);
			if (this.AutoPostBack)
				output.AddAttribute(HtmlTextWriterAttribute.Onchange, this.Page.ClientScript.GetPostBackEventReference(this, ""));
			else
				output.AddAttribute(HtmlTextWriterAttribute.Onchange, "CBDatePicker_YearChanged('" + this.ClientID + "');");
			output.RenderBeginTag(HtmlTextWriterTag.Select);
			if(this.UseNull)
			{
				output.WriteBeginTag("option");
				output.WriteAttribute("value", "-1");
				if (this.IsNull)
					output.Write(" selected=\"selected\"");
				output.Write(">");
				output.Write("");
				output.WriteEndTag("option");
				output.WriteLine("");
			}
			for (int i = minYear; i < maxYear; i++)
			{
				output.WriteBeginTag("option");
				output.WriteAttribute("value", i.ToString());
				if (this.Date.Year == i && !this.IsNull)
					output.Write(" selected=\"selected\"");
				output.Write(">");
				output.Write(i.ToString());
				output.WriteEndTag("option");
				output.WriteLine("");
			}
			output.RenderEndTag(); //select
		}

		private void RenderMaxMin(HtmlTextWriter output)
		{
			RenderHidden(output, this.MaxHiddenControlID, this.MaxHiddenControlName, this.MaxDate.Day.ToString() + "." + this.MaxDate.Month.ToString() + "." + this.MaxDate.Year.ToString());
			RenderHidden(output, this.MinHiddenControlID, this.MinHiddenControlName, this.MinDate.Day.ToString() + "." + this.MinDate.Month.ToString() + "." + this.MinDate.Year.ToString());
		}

		private void RenderHidden(HtmlTextWriter output, string id, string name, string value)
		{
			output.WriteBeginTag("input");
			output.WriteAttribute("id", id);
			output.WriteAttribute("name", name);
			output.WriteAttribute("type", "hidden");
			output.WriteAttribute("value", value);
			output.Write(" />");
		}

		[Bindable(true),
			Category("Behavior"),
			DefaultValue(false)]
		public bool AutoPostBack
		{
			get
			{
				return this.ViewState["AutoPostBack"]==null ? false : Convert.ToBoolean(this.ViewState["AutoPostBack"]);
			}
			set { this.ViewState["AutoPostBack"] = value; }
		}

		public event EventHandler DateChanged;

		private void OnDateChanged(EventArgs e)
		{
			if (DateChanged != null)
			{
				DateChanged(this, e);
			}
		}

		public void RaisePostDataChangedEvent()
		{
			OnDateChanged(EventArgs.Empty);
		}

		public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			bool retVal = false;
			int year	= -1;
			int month	= -1;
			int day		= -1;
			if (postCollection[this.YearControlName] != null)
			{
				year	= Int32.Parse(postCollection[this.YearControlName]);
			}

			if (postCollection[this.MonthControlName] != null)
			{
				month	= Int32.Parse(postCollection[this.MonthControlName]);
			}

			if (postCollection[this.DayControlName] != null)
			{
				day		= Int32.Parse(postCollection[this.DayControlName]);
			}
			if(year==-1 || month==-1 || day==-1)
			{
				if (!IsNull)
				{
					IsNull = true;
					retVal = true;
				}
			}
			else
			{
				DateTime presentValue = Date;
				DateTime postedValue = new DateTime(year,month,day);
				if (!presentValue.Equals(postedValue))
				{
					Date = postedValue;
					retVal = true;
				}
			}
			if (UseNull && IsNull && year>-1 && month>-1 && day>-1)
			{
				IsNull = false;
				retVal = true;
			}

			return retVal;
		}

		public override string ToString()
		{
			return this.Date.ToString("yyyy-MM-dd");
		}

		public enum MonthNameType
		{
			Custom,
			Numbers,
			ShortNames,
			FullNames,
			GenitiveNames
		}
	}
}