//using System;
//
//namespace Sota.Web.SimpleSite
//{
//	/// <summary>
//	/// Используется для филтрации запроса списка.
//	/// </summary>
//	public class ListFilter
//	{
//		private string _field;
//
//		public string Field
//		{
//			get { return _field; }
//			set { _field = value; }
//		}
//
//		private object _value;
//
//		public object Value
//		{
//			get { return _value; }
//			set { _value = value; }
//		}
//
//		private ListFilterType _type;
//
//		public ListFilterType FilterType
//		{
//			get { return _type; }
//			set { _type = value; }
//		}
//
//		private string _custom;
//
//		public string Custom
//		{
//			get { return _custom; }
//			set
//			{
//				_type	= ListFilterType.Custom;
//				_custom	= value;
//			}
//		}
//		
//		private Type _dataType;
//
//		public Type DataType
//		{
//			get { return _dataType; }
//			set { _dataType = value; }
//		}
//
//		private ListFilter()
//		{
//		}
//
//		private static ListFilter Create(string field, ListFilterType filterType, object value, Type dataType, string custom)
//		{
//			ListFilter l = new ListFilter();
//			l._field = field;
//			if(custom == null)
//			{
//				l._type = filterType;
//			}
//			else
//			{
//				l._custom = custom;
//			}
//			l._value = value;
//			l._dataType = dataType;
//			return l;
//		}
//		public static ListFilter Create(string field, ListFilterType filterType, object value, Type dataType)
//		{
//			return Create(field, filterType, value, dataType, null);
//		}
//		public static ListFilter Create(string field, ListFilterType filterType, object value)
//		{
//			return Create(field, filterType, value, typeof(string), null);
//		}
//		public static ListFilter Create(string field, object value)
//		{
//			return Create(field, ListFilterType.Equal, value, typeof(string), null);
//		}
//		public static ListFilter Create(string field, string custom, object value, Type dataType)
//		{
//			return Create(field, ListFilterType.Custom, value, dataType, custom);
//		}
//		public static ListFilter Create(string field, string custom, object value)
//		{
//			return Create(field, ListFilterType.Custom, value, typeof(string), custom);
//		}
//		public static ListFilter Create(string field, int filterType, object value, Type dataType)
//		{
//			return Create(field, (ListFilterType)filterType, value, dataType, null);
//		}
//		public static ListFilter Create(string field, int filterType, object value)
//		{
//			return Create(field, (ListFilterType)filterType, value, typeof(string), null);
//		}
//	}
//
//	public enum ListFilterType
//	{
//		Custom,
//		Equal,
//		Contain,
//		Start,
//		End,
//		Less,
//		More
//	}
//}