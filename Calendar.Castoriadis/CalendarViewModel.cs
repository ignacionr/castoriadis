using System.Linq;
using System.Collections.Generic;
using Castoriadis.Server;
using System.Net;

namespace Calendar.Castoriadis
{
	public class CalendarViewModel
	{
		public List<FreeBusyViewModel> FreeBusy {get; private set;}

		public string Name {
			get;
			set;
		}

		public CalendarViewModel (DDay.iCal.IICalendar ic)
		{
			this.Name = ic.Name;
			this.FreeBusy = ic.FreeBusy
				.Select(fb=>new FreeBusyViewModel {
					Start = fb.Start.Value,
					End = fb.End.Value,
				}).ToList();
		}
	}
}
