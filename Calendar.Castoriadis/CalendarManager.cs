using System;
using System.Linq;
using System.Collections.Generic;
using DDay.iCal;
using System.Net;

namespace Calendar.Castoriadis
{
	public class CalendarManager
	{
		Dictionary<CalendarReference,IICalendar> managedCalendars = new Dictionary<CalendarReference,IICalendar> ();

		public IICalendar GetCalendar (CalendarReference q)
		{
			var result = default(IICalendar);
			if (!managedCalendars.TryGetValue (q, out result)) {
				result = iCalendar.LoadFromUri (new Uri(q.Url)).FirstOrDefault();
				managedCalendars [q] = result;
			}
			return result;
		}
	}
}

