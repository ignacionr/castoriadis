using System;
using Castoriadis.Server;
using System.Net;
using Newtonsoft.Json;

namespace Metwit.Castoriadis
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (var ns = NamespaceBinding.Create("metwit")) {
				ns
					.HandleTopic<LatLong>("weather", q => {
						using(var wc = new WebClient()) {
							var text = wc.DownloadString(
								string.Format("https://api.metwit.com/v2/weather/?location_lat={0}&location_lng={1}", q.Latitude, q.Longitude));
							return JsonConvert.DeserializeObject(text);
						}})
					.RunService ()
					.Wait();
			}
		}
	}
}
