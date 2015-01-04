using System;
using NUnit.Framework;

namespace Castoriadis.Client.Test
{
	public class MockSocket: ISocket
	{
		bool isResponseSocket;

		public MockSocket (bool isResponse)
		{
			this.isResponseSocket = isResponse;
		}

		#region ISocket implementation
		public void Bind (string endpoint)
		{
			Assert.IsTrue (isResponseSocket);
		}
		public void Connect (string endpoint)
		{
			Assert.IsFalse (isResponseSocket);
		}
		public void Send (string contents)
		{
			throw new NotImplementedException ();
		}
		public string ReceiveString (TimeSpan timeout)
		{
			throw new NotImplementedException ();
		}
		#endregion
		#region IDisposable implementation
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

