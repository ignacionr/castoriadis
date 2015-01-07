using System;

namespace Castoriadis.WebServer
{
	public class LocalWebResponse
	{
		public int StatusCode { get; set;}
		public string ContentType {get;set;}
		public string Contents { get; set;}

		public bool IsBinary {
			get {
				return !this.ContentType.StartsWith ("text/", StringComparison.InvariantCulture);
			}
		}
	}
}

