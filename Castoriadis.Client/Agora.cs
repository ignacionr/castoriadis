using System;
using System.Collections.Generic;
using System.Linq;
using Castoriadis.Comm;
using Newtonsoft.Json;

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

		public Agora(INetworkContext networkContext = null) 
		{
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

		public RT ResolveSingle<RT>(string ns, string item, object query, int timeout = 500) 
		{
			// find the registration
			var r = this.torch.GetNamespaceRegistrations (ns);
			if (null == r) {
                if (this.torch.IsLocal) {
                    this.torch.TryKnownProviders(ns);
                    r = this.torch.GetNamespaceRegistrations(ns);
                }
                else
                {
                    this.Refresh();
                    r = this.torch.GetNamespaceRegistrations(ns);
                }
				if (null == r) {
					throw new Exception (string.Format ("Service {0} is not registered.", ns));
				}
			}
			var regs = r.Select(reg => reg.Endpoint).ToList();
			// obtain a connected socket
			using (var sock = socketPool.Get (regs)) {
				// issue the query
				sock.R.Send (query == null ? item : string.Join (" ", item, 
					query is string ? (string) query : JsonConvert.SerializeObject(query)));
				// unpack the results
				var text = sock.R.ReceiveString(TimeSpan.FromMilliseconds(timeout));
				if (text == null) {
					throw new TimeoutException ();
				}
				return JsonConvert.DeserializeObject<RT>(text);
			}
		}
	}
}

