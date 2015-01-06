using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using ServiceStack;
using Castoriadis.Comm;

namespace Castoriadis.Client
{
	public class Torch
	{
		INetworkContext netMqContext;

		ISocket sock;

		Dictionary<string,List<ServiceRegistration>> nsRegistrations = new Dictionary<string, List<ServiceRegistration>>();

		public Torch(INetworkContext context) {
			this.netMqContext = context;
		}

		public ServiceRegistration[] GetRegistrations ()
		{
			return this.nsRegistrations.SelectMany(reg => reg.Value).ToArray ();
		}

		public string Who ()
		{
			if (this.IsLocal) {
				return IdentifyMyself ();
			}
			this.sock.Send ("who");
			return this.sock.ReceiveString (TimeSpan.FromMilliseconds(500));
		}

		public void Start ()
		{
			// try to bind
			try {
				this.sock = this.netMqContext.CreateResponseSocket();
				this.sock.Bind("tcp://*:50111");
				this.SetupLocal();
			}
			catch {
				// cannot bind, try to connect
				if (null != this.sock) {
					this.sock.Dispose ();
				}
				this.sock = this.netMqContext.CreateRequestSocket ();
				this.sock.Bind ("tcp://localhost:50111");
				this.Refresh ();
			}
		}

		public List<ServiceRegistration> GetNamespaceRegistrations (string ns)
		{
			List<ServiceRegistration> regs = default(List<ServiceRegistration>);
			nsRegistrations.TryGetValue (ns, out regs);
			return regs;
		}

		bool Register (string[] data)
		{
			var reg = new ServiceRegistration { 
				Namespace = data[0], 
				Endpoint = data[1],
				Provider = data.Length >= 3 ? data[2].Replace("%20", " ") : null
			};
			var list = default(List<ServiceRegistration>);
			if (!nsRegistrations.TryGetValue (reg.Namespace, out list)) {
				list = new List<ServiceRegistration> ();
				nsRegistrations [reg.Namespace] = list;
			}
			list.Add (reg);
			return true;
		}

		bool Unregister (string[] data)
		{
			var list = default(List<ServiceRegistration>);
			if (nsRegistrations.TryGetValue (data[0], out list)) {
				var reg = list.FirstOrDefault (r => r.Endpoint == data [1]);
				if (reg != null) {
					list.Remove (reg);
					return true;
				}
			}
			return false;
		}

		string IdentifyMyself ()
		{
			return Assembly.GetEntryAssembly ().Location;
		}

		void HandleRequests () {
			var handlers = new Dictionary<string,Func<string[],object>> {
				{"get-registrations", parts => this.nsRegistrations},
				{"register", parts => this.Register(parts.Skip(1).ToArray())},
				{"unregister", parts => this.Unregister(parts.Skip(1).ToArray())},
				{"who", parts => IdentifyMyself() },
			};
			var not_found = new Func<string[],object> (parts => "Not found!");
			for (;;) {
				var text = this.sock.ReceiveString (TimeSpan.FromMilliseconds(500));
				if (!string.IsNullOrEmpty (text)) {
					try {
						var parts = text.Split (' ');
						var handler = default(Func<string[],object>);
						if (!handlers.TryGetValue (parts [0], out handler)) {
							handler = not_found;
						}
						this.sock.Send(DynamicJson.Serialize(handler(parts)));
					}
					catch(Exception ex) {
						this.sock.Send (ex.Message);
					}
				}
			}
		}

		void SetupLocal ()
		{
			Task.Run(new Action(HandleRequests));
			this.IsLocal = true;
		}

		public void Refresh ()
		{
			if (!this.IsLocal) {
				this.sock.Send ("get-registrations");
				var text = this.sock.ReceiveString (TimeSpan.FromMilliseconds(500));
				this.nsRegistrations = text.FromJson<Dictionary<string,List<ServiceRegistration>>>();
			}
		}

		bool IsLocal { get; set; }
	}
}

