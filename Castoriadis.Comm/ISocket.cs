using System;

namespace Castoriadis.Comm
{
	public interface ISocket: IDisposable
	{
		void Bind(string endpoint);
		void Connect (string endpoint);
		void Send(string contents);
		string ReceiveString (TimeSpan timeout);
	}
}
