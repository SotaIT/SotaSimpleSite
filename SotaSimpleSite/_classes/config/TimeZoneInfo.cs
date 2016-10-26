using System;
using System.Data;
using System.Web;
using System.Web.Caching;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// TimeZoneInfo.
	/// </summary>
	public class TimeZoneInfo
	{
		public TimeZoneInfo(int id)
		{
			DataRow[] rows = Config.GetConfigTable("timezone.config","timezone").Select("id=" + id);
			if(rows.Length==0)
			{
				rows = Config.GetConfigTable("timezone.config","timezone").Select("id=1");
			}
			this._id = id;
			this._isDaylightSaving = (rows[0]["dls"].ToString()=="1");
			this._name = rows[0]["name"].ToString();
			string[] offset = rows[0]["offset"].ToString().Split(':');
			_offset = _offset.Add(new TimeSpan(int.Parse(offset[0]), offset.Length > 1 ? int.Parse(offset[1]) : 0, 0));
			
		}

		private string _name = "NoName";
		private int _id = -1;
		private bool _isDaylightSaving = false;
		private TimeSpan _offset = TimeSpan.Zero;
		bool _isOffsetCalculated = false;
		private TimeSpan _newOffset = TimeSpan.Zero;

		public string Name
		{
			get { return _name; }
		}

		public int ID
		{
			get { return _id; }
		}
		
		public TimeSpan Offset
		{
			get
			{
				if(!_isOffsetCalculated)
				{
					_isOffsetCalculated = true;
					_newOffset = _offset;
					if(_isDaylightSaving)
					{
						TimeZone curTimeZone = TimeZone.CurrentTimeZone;
						if(curTimeZone.IsDaylightSavingTime(DateTime.Now))
						{
							_newOffset = _newOffset.Add(new TimeSpan(1,0,0));
						}
					}
				}
				return _newOffset;
			}
		}

		public bool IsDaylightSaving
		{
			get { return _isDaylightSaving; }
		}
		
		public DateTime FromUtc(DateTime utcTime)
		{
			return utcTime.Add(this.Offset);
		}

		public DateTime Now()
		{
			return FromUtc(DateTime.UtcNow);
		}

		public static TimeZoneInfo Current
		{
			get
			{
				TimeZoneInfo tzi = (TimeZoneInfo)HttpContext.Current.Cache[Keys.KeyTimeZoneInfo];
				if (tzi == null)
				{
					tzi = new TimeZoneInfo(Config.Main.TimeZone);
					HttpContext.Current.Cache.Insert(Keys.KeyTimeZoneInfo, tzi, new CacheDependency(HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + Keys.ConfigTimeZone)));
				}
				return tzi;
			}
		}
	}
}
