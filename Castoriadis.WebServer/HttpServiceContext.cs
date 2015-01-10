using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using Castoriadis.Client;
using System;

namespace Castoriadis.WebServer
{
	public class HttpServiceContext: IHttpServiceContext
	{
		static Action<string> logger = Console.WriteLine;
		IAgora agora;

		List<UrlRegistration> managedRegistrations = new List<UrlRegistration>();

		public HttpServiceContext(IAgora agora) {
			this.agora = agora;
		}

		public bool AddRegistration(UrlRegistration registration) {
			managedRegistrations.Add (registration);
			logger (string.Format ("Registered: {0}", registration));
			// make sure we are up to date with Castoriadis registrations
			this.agora.Refresh ();
			return true;
		}

		public bool RemoveRegistration(UrlRegistration registration) {
			var target = managedRegistrations.FirstOrDefault (r => r.Topic == registration.Topic);
			if (target == null) {
				return false;
			}
			managedRegistrations.Remove (target);
			logger (string.Format ("Unregistered: {0}", target));
			return true;
		}

		void RunSynch(int port) {
			using (var host = new HttpListener()) {
				var pfx = string.Format ("http://+:{0}/", port);
				host.Prefixes.Add(pfx);
				host.Start();
				logger (pfx);
				while(true) {
					var ctx = host.GetContext();
					HandleContextAsync(ctx).ContinueWith(t => {
						if (t.IsFaulted) {
							var msg = string.Join("\n", t.Exception.InnerExceptions.Select(ex => 
								string.Join(":", ex.Message, ex.StackTrace)));
							logger(msg);
							SendResponse(new LocalWebResponse{
								StatusCode = 500,
								ContentType = "text/plain",
								Contents = msg}, ctx.Response);
						}
					});
				}
			}
		}

		public Task RunService(int port) {
			return Task.Run (() => RunSynch(port));
		}

		static readonly LocalWebResponse _no_registration = new LocalWebResponse {
			StatusCode = 404,
			Contents = "Not found!",
			ContentType = "text/plain"
		};

		static void SendResponse (LocalWebResponse wresp, HttpListenerResponse resp)
		{
			resp.StatusCode = wresp.StatusCode;
			resp.ContentType = wresp.ContentType;
			byte[] responseContents = wresp.IsBinary ? Convert.FromBase64String (wresp.Contents) : Encoding.UTF8.GetBytes (wresp.Contents);
			resp.ContentLength64 = responseContents.LongLength;
			resp.OutputStream.Write (responseContents, 0, responseContents.Length);
		}

		Task HandleContextAsync (HttpListenerContext ctx)
		{
			logger (ctx.Request.RawUrl);
			return Task.Run (() => {
				var registration = UrlRegistration.Match(this.managedRegistrations, ctx.Request.Url);
				logger(string.Format("Registration found: {0}", registration));
				var wresp = null == registration ? _no_registration : 
					agora.ResolveSingle<LocalWebResponse>(registration.Module, ctx.Request.Url.AbsoluteUri, registration.Configuration);
				logger(string.Format("Response: {0}", wresp));
				var resp = ctx.Response;
				SendResponse (wresp, resp);
			});
		}
	}
}
