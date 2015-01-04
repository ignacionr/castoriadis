using System;
using Castoriadis.Server;
using System.Net;

namespace Calendar.Castoriadis
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var man = new CalendarManager ();
			using (var ns = NamespaceBinding.Create("calendar")) {
				ns
					.HandleTopic<CalendarReference>("calendar", q => man.GetCalendar(q))
						.RunService ()
						.Wait();
			}
		}
	}
}
