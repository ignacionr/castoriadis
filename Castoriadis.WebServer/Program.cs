using Castoriadis.Server;
using System;
using System.Threading;
using Castoriadis.Client;

namespace Castoriadis.WebServer
{
	public class MainClass
	{
		public static Func<IHttpServiceContext> HttpdFactory = () => new HttpServiceContext(new Agora());
		public static Func<string,INamespaceBinding> BindingFactory = nsName => NamespaceBinding.Create(nsName);

		public static void Main (string[] args)
		{
			var evtQuit = new ManualResetEvent (false);
			var httpd = HttpdFactory ();
			int port = 80;
			if (args != null && args.Length > 0) {
				port = int.Parse (args [0]);
			}

			using (var ns = BindingFactory("httpd")) {
				var taskCastoriadis = ns
					.HandleTopic<UrlRegistration> ("register", ur => httpd.AddRegistration(ur))
					.HandleTopic<UrlRegistration> ("unregister", ur => httpd.RemoveRegistration(ur))
					.HandleTopic<object>("quit", o => evtQuit.Set())
						.RunService ();
				var taskHttpd = httpd.RunService (port);
				evtQuit.WaitOne ();
				if (taskCastoriadis.Status == System.Threading.Tasks.TaskStatus.Faulted) {
					throw taskCastoriadis.Exception;
				}
				if (taskHttpd.Status == System.Threading.Tasks.TaskStatus.Faulted) {
					throw taskHttpd.Exception;
				}
			}
		}
	}
}
