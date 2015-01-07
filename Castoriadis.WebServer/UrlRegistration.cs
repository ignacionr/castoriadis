using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Castoriadis.WebServer
{
	public class UrlRegistration
	{
		public string Topic {get;set;}
		public string Module {get;set;}
		public object Configuration { get; set; }
		public int Priority {get;set;}

		public static UrlRegistration Match (IEnumerable<UrlRegistration> managedRegistrations, Uri url)
		{
			return managedRegistrations
				.OrderBy (r => r.Priority)
				.FirstOrDefault (r => Regex.IsMatch (url.AbsoluteUri, r.Topic));
		}
	}
}
