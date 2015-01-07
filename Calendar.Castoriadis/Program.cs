using System;
using System.Linq;
using DDay.iCal;
using Castoriadis.Server;
using System.Collections.Generic;

namespace Calendar.Castoriadis
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			var man = new CalendarManager ();
			using (var ns = NamespaceBinding.Create("calendar")) {
				ns
					.HandleTopic<CalendarReference> ("calendar", q => 
					                                 new CalendarViewModel (man.GetCalendar (q)))
						.HandleTopic<CalendarDateRange> ("available-dates", q => GetAvailableDates(man,q))
						.RunService ()
						.Wait();
			}
		}

		static object GetAvailableDates (CalendarManager man, CalendarDateRange q)
		{
			var cal = man.GetCalendar (q.Calendar);
			var result = new List<DateTime> ();
			var oneDay = TimeSpan.FromDays (1);
			for (var current = q.From; current <= q.To; current += oneDay) {
				if (!cal.FreeBusy.Any (fb => fb.GetFreeBusyStatus (
					new Period (new iCalDateTime(current), oneDay)) != FreeBusyStatus.Free)) {
					result.Add (current);
				}
			}
			return result;
		}
	}
}
