using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using Castoriadis.Comm;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Threading;

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
				this.sock.Connect ("tcp://localhost:50111");
				this.Refresh ();
			}
		}

		public List<ServiceRegistration> GetNamespaceRegistrations (string ns)
		{
			List<ServiceRegistration> regs = default(List<ServiceRegistration>);
			nsRegistrations.TryGetValue (ns, out regs);
			return regs;
		}

        const string KNOWN_PROVIDERS_FILENAME = "known-providers.json";

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
            Task.Run(() =>
            {
                var knownProviders = new Dictionary<string, string>();
                try
                {
                    knownProviders = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(KNOWN_PROVIDERS_FILENAME));
                }
                catch(Exception) {}
                knownProviders[reg.Namespace] = reg.Provider;
                File.WriteAllText(KNOWN_PROVIDERS_FILENAME, JsonConvert.SerializeObject(knownProviders));
            });
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
						this.sock.Send(JsonConvert.SerializeObject(handler(parts)));
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
                if (text == null)
                {
                    // watch out! the torch was killed, now get the torch
                    this.Start();
                }
                else
                {
                    this.nsRegistrations = JsonConvert.DeserializeObject<Dictionary<string, List<ServiceRegistration>>>(text);
                }
			}
		}

		public bool IsLocal { get; set; }

        internal void TryKnownProviders(string ns)
        {
            var location = FindKnownProviderLocation(ns);
            if (null != location)
            {
                Process.Start(location);
                Thread.Sleep(500);
            }
        }

        private string FindKnownProviderLocation(string ns)
        {
            try
            {
                var knownProviders = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(KNOWN_PROVIDERS_FILENAME));
                return knownProviders[ns];
            }
            catch (Exception)
            {
                // can't do, just ignore
            }
            return null;
        }

        internal void MarkFailed(string ns)
        {
            // this registration timed out, remove it
            this.nsRegistrations.Remove(ns);
        }
    }
}

