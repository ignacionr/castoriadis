using System;

namespace Castoriadis.Comm
{
	public interface INetworkContext
	{
		ISocket CreateRequestSocket ();
		ISocket CreateResponseSocket();
	}
}

