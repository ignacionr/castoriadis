using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using Castoriadis.Comm;

namespace Castoriadis.Client
{
	public class Agora: IAgora
	{
		Pool<ISocket,List<string>> socketPool;
		INetworkContext context;
		Torch torch;

		ISocket Connect (List<string> arg)
		{
			var sock = context.CreateRequestSocket();
			arg.ForEach(sock.Connect);
			return sock;
		}

		public Agora(INetworkContext networkContext = null) {
			if (networkContext == null) {
				networkContext = new NetMQNetworkContext ();
			}
			socketPool = new Pool<ISocket, List<string>> (Connect);
			// try to connect to a local torch, or setup one
            this.context = networkContext;
			torch = new Torch (this.context);
			torch.Start();
		}

		public void Refresh() {
			this.torch.Refresh ();
		}

		public string Who() {
			return this.torch.Who ();
		}

		public ServiceRegistration[] GetRegistrations() {
			return this.torch.GetRegistrations ();
		}

		public dynamic ResolveSingle(string ns, string item, object query) {
			// find the registration
			var regs = this.torch.GetNamespaceRegistrations(ns).Select(reg => reg.Endpoint).ToList();
			// obtain a connected socket
			using (var sock = socketPool.Get (regs)) {
				// issue the query
				sock.R.Send (query == null ? item : string.Join (" ", item, DynamicJson.Serialize(query)));
				// unpack the results
				var text = sock.R.ReceiveString(TimeSpan.FromMilliseconds(500));
				if (text == null) {
					throw new TimeoutException ();
				}
				return DynamicJson.Deserialize(text);
			}
		}
	}
}

