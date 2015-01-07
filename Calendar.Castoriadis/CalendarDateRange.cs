using System.Linq;
using Castoriadis.Server;
using System.Net;
using System;

namespace Calendar.Castoriadis
{
	public class CalendarDateRange
	{
		public CalendarReference Calendar { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
	}
}
