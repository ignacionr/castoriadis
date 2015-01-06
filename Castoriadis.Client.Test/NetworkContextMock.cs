using System;
using NUnit.Framework;
using Castoriadis.Comm;

namespace Castoriadis.Client.Test
{
	class NetworkContextMock: INetworkContext
	{
        public Action<MockSocket> mockSocketCreated;

		#region INetworkContext implementation
		public ISocket CreateRequestSocket ()
		{
            return CreateMockSocket(false);
		}

        private ISocket CreateMockSocket(bool beResponse)
        {
            var res = new MockSocket(beResponse);
            if (mockSocketCreated != null)
            {
                mockSocketCreated(res);
            }
            return res;
        }
		public ISocket CreateResponseSocket ()
		{
            return CreateMockSocket(true);
        }
		#endregion
	}
}

