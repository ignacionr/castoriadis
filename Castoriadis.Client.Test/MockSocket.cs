using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace Castoriadis.Client.Test
{
	public class MockSocket: ISocket
	{
		public bool isResponseSocket;
        public bool allowBind;
        public Queue<string> receivedStrings = new Queue<string>();
        public Queue<string> sentStrings = new Queue<string>();
        public Action<string> processConnect = ep => { };

		public MockSocket (bool isResponse)
		{
			this.isResponseSocket = isResponse;
		}

		#region ISocket implementation
		public void Bind (string endpoint)
		{
			Assert.IsTrue (isResponseSocket);
            if (!allowBind) throw new OperationCanceledException();
		}
		public void Connect (string endpoint)
		{
			Assert.IsFalse (isResponseSocket);
            processConnect(endpoint);
		}
		public void Send (string contents)
		{
            this.sentStrings.Enqueue(contents);
		}
		public string ReceiveString (TimeSpan timeout)
		{
            if (receivedStrings.Count > 0)
                return receivedStrings.Dequeue();
            Thread.Sleep(timeout);
            return null;
		}
		#endregion
		#region IDisposable implementation
		public void Dispose ()
		{
			;
		}
		#endregion
	}
}