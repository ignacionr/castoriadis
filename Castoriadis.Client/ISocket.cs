using System;

namespace Castoriadis.Client
{
	public interface ISocket: IDisposable
	{
		void Bind(string endpoint);
		void Connect (string endpoint);
		void Send(string contents);
		string ReceiveString (TimeSpan timeout);
	}
}
