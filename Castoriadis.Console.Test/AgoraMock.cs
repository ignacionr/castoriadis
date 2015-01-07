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
		public RT ResolveSingle<RT>(string ns, string item, object query, int timeout = 500)
		{
			this.lastItem = item;
			this.lastQuery = query;
			return (RT)Convert.ChangeType (true, typeof(RT));
		}
		#endregion
	}
}

