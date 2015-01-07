using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Castoriadis.WebServer.Test
{
	public class MockedHttpServiceContext: IHttpServiceContext
	{
		public List<UrlRegistration> registrations= new List<UrlRegistration>();

		#region IHttpServiceContext implementation

		public bool AddRegistration (UrlRegistration registration)
		{
			this.registrations.Add (registration);
			return true;
		}

		public bool RemoveRegistration (UrlRegistration registration)
		{
			var target = registrations.FirstOrDefault (r => r.Topic == registration.Topic);
			this.registrations.Remove (target);
			return true;
		}

		public Task RunService (int port)
		{
			return Task.Run (() => {});
		}

		#endregion
	}
}

