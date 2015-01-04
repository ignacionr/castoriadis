using System;
using NUnit.Framework;
using Castoriadis.Client;

namespace Castoriadis.Console.Test
{
	public class AgoraMock: IAgora
	{
		public object lastQuery;
		public string lastItem;

		#region IAgora implementation
		public void Refresh ()
		{
			throw new NotImplementedException ();
		}
		public string Who ()
		{
			throw new NotImplementedException ();
		}
		public ServiceRegistration[] GetRegistrations ()
		{
			return new [] {new ServiceRegistration{
					Endpoint = "tcp://test.com:999",
					Namespace = "dummy-namespace",
					Provider = "dummyprovider.exe"
				}};
		}
		public dynamic ResolveSingle (string ns, string item, object query)
		{
			this.lastItem = item;
			this.lastQuery = query;
			return true;
		}
		#endregion
	}
}

