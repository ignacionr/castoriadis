using System;
using NUnit.Framework;

namespace Castoriadis.Client.Test
{
	class NetworkContextMock: INetworkContext
	{
		#region INetworkContext implementation
		public ISocket CreateRequestSocket ()
		{
			return new MockSocket (false);
		}
		public ISocket CreateResponseSocket ()
		{
			return new MockSocket (true);
		}
		#endregion
	}
}

