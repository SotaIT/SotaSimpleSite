using System;
using System.Collections.Generic;
using System.Web;

namespace Sota.Web.SimpleSite
{
	public class BaseTemplatedPage: BasePage
	{
		public override System.Web.UI.Control FindPlaceHolder(string id)
		{
			return Master.FindControl(id);
		}
	}
}