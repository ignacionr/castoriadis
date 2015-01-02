using System;
using System.Collections.Generic;
using System.Linq;
using NetMQ;
using Newtonsoft.Json;

namespace Castoriadis.Client
{
	public class Agora
	{
		Pool<NetMQSocket,List<string>> socketPool;
		NetMQContext context = NetMQContext.Create();
		Torch torch;

		NetMQSocket Connect (List<string> arg)
		{
			var sock = context.CreateRequestSocket();
			arg.ForEach(endpoint => sock.Connect (endpoint));
			return sock;
		}

		public Agora() {
			socketPool = new Pool<NetMQSocket, List<string>> (Connect);
			// try to connect to a local torch, or setup one
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
				sock.R.Send (string.Join (" ", item, JsonConvert.SerializeObject (query)));
				// unpack the results
				var text = sock.R.ReceiveString(TimeSpan.FromMilliseconds(500));
				return JsonConvert.DeserializeObject<dynamic> (text);
			}
		}
	}
}

