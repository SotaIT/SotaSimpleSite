using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Sota.Web.UI.WebControls
{
	/// <summary>
	///  ‡ÎÂÌ‰‡¸.
	/// </summary>
	public class Calendar : Control
	{
		public Calendar()
		{
			table = new HtmlTable();
			table.Attributes["class"] = "calendar";
		}

		private enum DayMonth
		{
			Next,
			Prev,
			Current
		}

		private HtmlTable table;

		[Category("Appearance"), DefaultValue("calendar"), Description("CSS ClassName"), Bindable(true)]
		public string CssClass
		{
			get { return table.Attributes["class"]; }
			set { table.Attributes["class"] = value; }
		}


		[DefaultValue(-1), Category("Appearance"), Bindable(true), Description("Table CellPadding")]
		public int CellPadding
		{
			get { return table.CellPadding; }
			set { table.CellPadding = value; }
		}

		[Category("Appearance"), Bindable(true), Description("Table CellSpacing"), DefaultValue(-1)]
		public int CellSpacing
		{
			get { return table.CellSpacing; }
			set { table.CellSpacing = value; }
		}

		[Browsable(false)]
		public override bool EnableViewState
		{
			get { return false; }
			set { base.EnableViewState = false; }
		}

		private string _selectedCss = "selected";

		[Category("Appearance"), DefaultValue("selected"), Description("Selected day cell CSS ClassName"), Bindable(true)]
		public string SelectedCssClass
		{
			get { return _selectedCss; }
			set { _selectedCss = value; }
		}

		private string _todayCss = "today";

		[Category("Appearance"), DefaultValue("today"), Description("Today cell CSS ClassName"), Bindable(true)]
		public string TodayCssClass
		{
			get { return _todayCss; }
			set { _todayCss = value; }
		}

		private string _weekEndCss = "weekend";

		[Category("Appearance"), DefaultValue("weekend"), Description("WeekEnd cell CSS ClassName"), Bindable(true)]
		public string WeekEndCssClass
		{
			get { return _weekEndCss; }
			set { _weekEndCss = value; }
		}

		private string _prevCss = "prev";

		[Category("Appearance"), DefaultValue("prev"), Description("Previous month cell CSS ClassName"), Bindable(true)]
		public string PrevCssClass
		{
			get { return _prevCss; }
			set { _prevCss = value; }
		}

		private string _nextCss = "next";

		[Category("Appearance"), DefaultValue("next"), Description("Next month cell CSS ClassName"), Bindable(true)]
		public string NextCssClass
		{
			get { return _nextCss; }
			set { _nextCss = value; }
		}

		private ShowOtherMonth _other = ShowOtherMonth.None;

		[Category("Appearance"), DefaultValue(ShowOtherMonth.None), Description("How to show other month's days"), Bindable(true)]
		public ShowOtherMonth ShowOtherMonth
		{
			get { return _other; }
			set { _other = value; }
		}

		private RepeatDirection _direction = RepeatDirection.Horizontal;

		[Category("Behavior"), DefaultValue(RepeatDirection.Horizontal), Description("Direction of the week"), Bindable(true), NotifyParentProperty(true)]
		public RepeatDirection WeekDirection
		{
			get { return _direction; }
			set { _direction = value; }
		}

		private DateTime _date = DateTime.Today;

		[Category("Behavior"), Bindable(true), Description("Selected date"), NotifyParentProperty(true)]
		public DateTime Date
		{
			get { return _date; }
			set { _date = value; }
		}

		private string _urlFormat = "?date={y}-{m}-{d}";

		[Category("Behavior"), Bindable(true), Description("Url format"), DefaultValue("?date={y}-{m}-{d}")]
		public string UrlFormat
		{
			get { return _urlFormat; }
			set { _urlFormat = value; }
		}

		private string _emptyCellHtml = "&nbsp;";

		[Category("Behavior"), Bindable(true), Description("HTML code of the empty cell"), DefaultValue("&nbsp;")]
		public string EmptyCellHtml
		{
			get { return _emptyCellHtml; }
			set { _emptyCellHtml = value; }
		}

		private string[] _urlDays = new string[0];

		[Category("Behavior"), Bindable(true), Description("Url days"), DefaultValue(null)]
		public string UrlDays
		{
			get { return String.Join(",", _urlDays); }
			set
			{
				string val = value.Trim(',');
				if (Regex.Match(val, "^[0-9,]+$").Value == val)
					_urlDays = val.Split(',');
			}
		}

		private string[] _nextDays = new string[0];

		[Category("Behavior"), Bindable(true), Description("Next month url days"), DefaultValue(null)]
		public string NextUrlDays
		{
			get { return String.Join(",", _nextDays); }
			set
			{
				string val = value.Trim(',');
				if (Regex.Match(val, "^[0-9,]+$").Value == val)
					_nextDays = val.Split(',');
			}
		}

		private string[] _prevDays = new string[0];

		[Category("Behavior"), Bindable(true), Description("Previous month url days"), DefaultValue(null)]
		public string PrevUrlDays
		{
			get { return String.Join(",", _prevDays); }
			set
			{
				string val = value.Trim(',');
				if (Regex.Match(val, "^[0-9,]+$").Value == val)
					_prevDays = val.Split(',');
			}
		}

		private Display _showWeekDays = Display.Before;

		[Category("Behavior"), Bindable(true), Description("How to show week days"), DefaultValue(Display.Before)]
		public Display DisplayWeekDays
		{
			get { return _showWeekDays; }
			set { _showWeekDays = value; }
		}

		private string[] _daysOfweek = {"œÌ", "¬Ú", "—", "◊Ú", "œÚ", "—·", "¬Ò"};

		[Category("Behavior"), Bindable(true), Description("Names of the week days"), DefaultValue("œÌ,¬Ú,—,◊Ú,œÚ,—·,¬Ò")]
		public string DaysOfWeek
		{
			get { return String.Join(",", _daysOfweek); }
			set
			{
				string[] ar = value.Split(',');
				if (ar.Length == 7)
					_daysOfweek = ar;
			}
		}

		private void appendStringArray(ref string[] arr, string value)
		{
			string[] temp = new string[arr.Length + 1];
			arr.CopyTo(temp, 0);
			temp[arr.Length] = value;
			arr = temp;
		}

		public void AddDate(DateTime date)
		{
			if (date.Year != _date.Year)
				return;
			int m = date.Month - _date.Month;
			switch (m)
			{
				case -1:
					appendStringArray(ref this._prevDays, date.Day.ToString());
					break;
				case 0:
					appendStringArray(ref this._urlDays, date.Day.ToString());
					break;
				case 1:
					appendStringArray(ref this._nextDays, date.Day.ToString());
					break;
			}
		}

		private HtmlTableCell CreateTd(int day, DayMonth month, bool weekend)
		{
			HtmlTableCell cell = new HtmlTableCell();
			string cssClass = string.Empty;
			if (this._selectedCss.Length > 0 && month == DayMonth.Current && _date.Day == day)
			{
				cssClass = this._selectedCss;
			}
			if (this._todayCss.Length > 0 
				&& DateTime.Today.Day == day 
				&& ((month == DayMonth.Current && _date.Month == DateTime.Today.Month)
					|| (month==DayMonth.Prev && _date.Month - 1 == DateTime.Today.Month)
					|| (month==DayMonth.Next && _date.Month + 1 == DateTime.Today.Month)))
			{
				cssClass = cssClass.Length == 0 ? this._todayCss : cssClass + " " + this._todayCss;
			}
			if ((_other == ShowOtherMonth.Next || _other == ShowOtherMonth.Both) && month == DayMonth.Next && this._nextCss.Length > 0)
			{
				cssClass = cssClass.Length == 0 ? this._nextCss : cssClass + " " + this._nextCss;
			}
			if ((_other == ShowOtherMonth.Prev || _other == ShowOtherMonth.Both) && month == DayMonth.Prev && this._prevCss.Length > 0)
			{
				cssClass = cssClass.Length == 0 ? this._prevCss : cssClass + " " + this._prevCss;
			}
			if (weekend && this._weekEndCss.Length > 0)
			{
				cssClass = cssClass.Length == 0 ? this._weekEndCss : cssClass + " " + this._weekEndCss;
			}

			if (cssClass.Length != 0)
			{
				cell.Attributes.Add("class", cssClass);
			}
			if (month == DayMonth.Prev)
			{
				if (_other == ShowOtherMonth.Prev || _other == ShowOtherMonth.Both)
				{
					if (_prevDays.Length == 0
						|| ((IList) _prevDays).Contains(day.ToString()))
					{
						cell.InnerHtml = "<a href=\"" +
							_urlFormat.Replace("{d}", day.ToString())
								.Replace("{m}", _date.AddMonths(-1).Month.ToString())
								.Replace("{y}", _date.AddMonths(-1).Year.ToString()) +
							"\">" + day.ToString() + "</a>";
					}
					else
					{
						cell.InnerText = day.ToString();
					}
				}
				else
				{
					cell.InnerHtml = this._emptyCellHtml;
				}
			}
			else if (month == DayMonth.Next)
			{
				if (_other == ShowOtherMonth.Next || _other == ShowOtherMonth.Both)
				{
					if (_nextDays.Length == 0
						|| ((IList) _nextDays).Contains(day.ToString()))
					{
						cell.InnerHtml = "<a href=\"" +
							_urlFormat.Replace("{d}", day.ToString())
								.Replace("{m}", _date.AddMonths(1).Month.ToString())
								.Replace("{y}", _date.AddMonths(1).Year.ToString()) +
							"\">" + day.ToString() + "</a>";
					}
					else
					{
						cell.InnerText = day.ToString();
					}
				}
				else
				{
					cell.InnerHtml = this._emptyCellHtml;
				}
			}
			else
			{
				if (_urlDays.Length == 0
					|| ((IList) _urlDays).Contains(day.ToString()))
					cell.InnerHtml = "<a href=\"" +
						_urlFormat.Replace("{d}", day.ToString())
							.Replace("{m}", _date.Month.ToString())
							.Replace("{y}", _date.Year.ToString()) +
						"\">" + day.ToString() + "</a>";
				else
				{
					cell.InnerText = day.ToString();
				}
			}
			return cell;
		}


		private HtmlTableCell CreateTh(int dayOfweek)
		{
			HtmlTableCell cell = new HtmlTableCell("th");
			cell.InnerText = _daysOfweek[dayOfweek];
			return cell;
		}

		private void CreateTable()
		{
			this.table.ID = this.ID;
			this.table.Rows.Clear();
			int dayStart = (int) (new DateTime(_date.Year, _date.Month, 1).DayOfWeek);
			if (dayStart == 0) dayStart = 7;
			int daysinmonth = DateTime.DaysInMonth(_date.Year, _date.Month);
			int n1 = daysinmonth + dayStart - 1;
			int n2;
			Math.DivRem(n1, 7, out n2);
			int tdcount = n1 + 7 - n2 + 1;
			if (_direction == RepeatDirection.Horizontal)
			{
				if (this._showWeekDays == Display.Before)
				{
					this.table.Rows.Add(new HtmlTableRow());
					for (int i = 0; i < 7; i++)
						this.table.Rows[0].Cells.Add(this.CreateTh(i));
				}
				HtmlTableRow row = new HtmlTableRow();
				//this.table.Rows.Add(row);
				for (int i = 1; i < tdcount; i++)
				{
					int day = i - dayStart + 1;
					Math.DivRem(i - 1, 7, out n2);
					if (n2 == 0)
					{
						this.table.Rows.Add(row = new HtmlTableRow());
					}
					if (i < dayStart)
					{
						row.Cells.Add(this.CreateTd(_date.AddDays(i - dayStart - _date.Day + 1).Day, DayMonth.Prev, n2 > 4));
					}
					else if (i > daysinmonth + dayStart - 1)
					{
						row.Cells.Add(this.CreateTd(day - daysinmonth, DayMonth.Next, n2 > 4));
					}
					else
					{
						row.Cells.Add(this.CreateTd(day, DayMonth.Current, n2 > 4));
					}
				}
				if (this._showWeekDays == Display.After)
				{
					this.table.Rows.Add(row = new HtmlTableRow());
					for (int i = 0; i < 7; i++)
						row.Cells.Add(this.CreateTh(i));
				}
			}
			else
			{
				int c = tdcount/7;
				for (int i = 0; i < 7; i++)
				{
					this.table.Rows.Add(new HtmlTableRow());
					if (this._showWeekDays == Display.Before)
					{
						this.table.Rows[i].Cells.Add(this.CreateTh(i));
					}
					for (int j = 0; j < c; j++)
					{
						int k = i + j*7 + 1;
						int day = k - dayStart + 1;
						if (k < dayStart)
							this.table.Rows[i].Cells.Add(this.CreateTd(_date.AddDays(k - dayStart - _date.Day + 1).Day, DayMonth.Prev, i > 4));
						else if (k > daysinmonth + dayStart - 1)
							this.table.Rows[i].Cells.Add(this.CreateTd(day - daysinmonth, DayMonth.Next, i > 4));
						else
							this.table.Rows[i].Cells.Add(this.CreateTd(day, DayMonth.Current, i > 4));

					}
					if (this._showWeekDays == Display.After)
					{
						this.table.Rows[i].Cells.Add(this.CreateTh(i));
					}
				}
			}
			this.Controls.Add(table);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			CreateTable();
			base.Render(writer);
		}

	}

	public enum ShowOtherMonth
	{
		None,
		Prev,
		Next,
		Both
	}
}