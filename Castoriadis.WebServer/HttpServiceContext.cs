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
		IAgora agora;

		List<UrlRegistration> managedRegistrations = new List<UrlRegistration>();

		public HttpServiceContext(IAgora agora) {
			this.agora = agora;
		}

		public bool AddRegistration(UrlRegistration registration) {
			managedRegistrations.Add (registration);
			return true;
		}

		public bool RemoveRegistration(UrlRegistration registration) {
			var target = managedRegistrations.FirstOrDefault (r => r.Topic == registration.Topic);
			if (target == null) {
				return false;
			}
			managedRegistrations.Remove (target);
			return true;
		}

		void RunSynch(int port) {
			using (var host = new HttpListener()) {
				host.Prefixes.Add(string.Format("http://+:{0}/", port));
				host.Start();
				//Console.WriteLine(string.Format("Castoriadis.WebServer is on {0}", uri));
				while(true) {
					var ctx = host.GetContext();
					HandleContextAsync(ctx);
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

		Task HandleContextAsync (HttpListenerContext ctx)
		{
			return Task.Run (() => {
				var resp = ctx.Response;
				var registration = UrlRegistration.Match(this.managedRegistrations, ctx.Request.Url);
				var wresp = null == registration ? _no_registration : 
					agora.ResolveSingle<LocalWebResponse>(registration.Module, ctx.Request.Url.AbsoluteUri, registration.Configuration);
				resp.StatusCode = wresp.StatusCode;
				resp.ContentType = wresp.ContentType;
				byte[] responseContents = wresp.IsBinary ? Convert.FromBase64String(wresp.Contents) : Encoding.UTF8.GetBytes(wresp.Contents);
				resp.ContentLength64 = responseContents.LongLength;
				resp.OutputStream.Write(responseContents, 0, responseContents.Length);
			});
		}
	}
}
