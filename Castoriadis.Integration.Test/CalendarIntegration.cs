using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Castoriadis.Client;
using System.Threading;

namespace Castoriadis.Integration.Test
{
	[TestFixture()]
	public class CalendarIntegration
	{
		[Test()]
		public void GetAPublicGoogleCalendar ()
		{
			IAgora agora = new Agora ();
			var pattayaILoveRoom1PublicUrl = "https://www.google.com/calendar/ical/sm0qpodqh5iedidbd4g0j9hsp0%40group.calendar.google.com/public/basic.ics";
			var taskRunCalendar = Task.Run (() => Calendar.Castoriadis.MainClass.Main (null));
			// wait until it's registered
			while (!agora.GetRegistrations().Any(reg => reg.Namespace == "calendar")) {
				Thread.Sleep (100);
			}
			var result = agora.ResolveSingle<dynamic> ("calendar", "calendar", new {
				Name="Pattaya I Love Room #1",
				Url = pattayaILoveRoom1PublicUrl
			}, 10000);
			Assert.NotNull (result);
		}
	}
}

