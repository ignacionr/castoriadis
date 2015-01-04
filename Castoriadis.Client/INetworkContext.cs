using System;

namespace Castoriadis.Client
{
	public interface INetworkContext
	{
		ISocket CreateRequestSocket ();
		ISocket CreateResponseSocket();
	}
}

