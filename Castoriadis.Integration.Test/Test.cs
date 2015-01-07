using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Castoriadis.Client;
using System.Net;

namespace Castoriadis.Integration.Test
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void WebServerStartsAndResponds404 ()
		{
			IAgora agora = new Agora ();
			// start the web server
			var taskWeb = Task.Run(() => WebServer.MainClass.Main (new[]{"8080"}));
			while (!agora.GetRegistrations().Any(reg => reg.Namespace == "httpd")) {
				System.Threading.Thread.Sleep (100);
				agora.Refresh ();
			}
			System.Threading.Thread.Sleep (2000);
			if (taskWeb.Status != TaskStatus.Running) {
				throw taskWeb.Exception;
			}
			using (var wc = new WebClient()) {
				try{
					var text = wc.DownloadString ("http://localhost:8080/");
					Assert.Fail();
				}
				catch(WebException wex) {
					Assert.AreEqual ("The remote server returned an error: (404) Not Found.", wex.Message);
				}
			}
		}
	}
}
