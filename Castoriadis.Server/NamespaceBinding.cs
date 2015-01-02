using System;
using System.Reflection;
using NetMQ;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Castoriadis.Server
{
	public class NamespaceBinding: IDisposable
	{
		string nsName;

		int bindingPort;

		NetMQ.Sockets.ResponseSocket sock;

		static NetMQContext _mqContext = NetMQ.NetMQContext.Create();

		public static NamespaceBinding Create(string namespaceName, int port = 0) {
			var result = new NamespaceBinding (namespaceName, port);
			if (port == 0) {
				var rnd = new Random ();
				for (bool bound = false; !bound;) {
					result.bindingPort = rnd.Next (10000, 65000);
					bound = result.TryBind ();
				}
			} else {
				result.Bind ();
			}
			result.Announce ();
			return result;
		}

		static string TellTorch (string message)
		{
			// connect to the local torch
			using (var torchSocket = _mqContext.CreateRequestSocket ()) {
				torchSocket.Connect ("tcp://localhost:50111");
				// let it know we're implementing this service
				torchSocket.Send (message);
				var text = torchSocket.ReceiveString ();
				return text;
			}
		}

		void Announce ()
		{
			var message = string.Format ("register {0} tcp://{1}:{2} {3}", 
			                             this.nsName, 
			                             System.Net.Dns.GetHostName(),
			                             this.bindingPort,
			                             Assembly.GetEntryAssembly ().Location.Replace (" ", "%20"));
			TellTorch (message);
		}

		private NamespaceBinding (string namespaceName, int port)
		{
			this.nsName = namespaceName;
			this.bindingPort = port;
			this.sock = _mqContext.CreateResponseSocket ();
		}

		void Bind ()
		{
			this.sock.Bind (string.Format ("tcp://*:{0}", this.bindingPort));
		}

		bool TryBind() {
			var ret = false;
			try {
				Bind ();
				ret = true;
			}
			catch{
			}
			return ret;
		}

		void Renounce ()
		{
			var message = string.Format ("unregister {0} tcp://{1}:{2} {3}", 
			                             this.nsName, 
			                             System.Net.Dns.GetHostName(),
			                             this.bindingPort,
			                             Assembly.GetEntryAssembly ().Location.Replace (" ", "%20"));
			TellTorch (message);
		}

		Func<string,string,object> catchAll;

		public Task RunService() {
			for (;;) {
				var cmd = this.sock.ReceiveString (TimeSpan.FromMilliseconds (500));
				if (!string.IsNullOrEmpty (cmd)) {
					// evaluate which handler will process it, and thus the shape of the query object
					var idxSpace = cmd.IndexOf (' ');
					var item = (idxSpace >= 0) ? cmd.Substring (0, idxSpace) : cmd;
				var query = (idxSpace >= 0) ? cmd.Substring (idxSpace + 1) : string.Empty;
					object result = null;
					bool handled = false;
					if (catchAll != null) {
						result = catchAll (item, query);
						handled = true;
					}
					if (!handled) {
						result = "Command not found!";
					}
					this.sock.Send (JsonConvert.SerializeObject (result));
				}
			}
		}

		public NamespaceBinding HandleAll(Func<string,string,object> catcher) {
			this.catchAll = catcher;
			return this;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			this.Renounce ();
		}

		#endregion
	}
}

