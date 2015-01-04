using System;
using NetMQ;

namespace Castoriadis.Client
{
	public class NetMQNetworkContext: INetworkContext
	{
		NetMQContext netMqContext;

		public NetMQNetworkContext ()
		{
			this.netMqContext = NetMQContext.Create ();
		}

		#region INetworkContext implementation

		public ISocket CreateRequestSocket ()
		{
			return new SocketWrapper (netMqContext.CreateRequestSocket ());
		}

		public ISocket CreateResponseSocket ()
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

