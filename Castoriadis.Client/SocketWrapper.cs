using System;
using NetMQ;

namespace Castoriadis.Client
{
	public class SocketWrapper: ISocket
	{
		NetMQSocket socket;
		public SocketWrapper (NetMQ.Sockets.RequestSocket requestSocket)
		{
			this.socket = requestSocket;
		}

		#region ISocket implementation

		public void Bind (string endpoint)
		{
			this.socket.Bind (endpoint);
		}

		public void Connect (string endpoint)
		{
			this.socket.Connect (endpoint);
		}

		public void Send (string contents)
		{
			this.socket.Send (contents);
		}

		public string ReceiveString (TimeSpan timeout)
		{
			return this.socket.ReceiveString (timeout);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			this.socket.Dispose ();
		}

		#endregion
	}
}

