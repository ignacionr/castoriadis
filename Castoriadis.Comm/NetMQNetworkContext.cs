using System;
using NetMQ;
using Castoriadis.Comm;

namespace Castoriadis.Comm
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
			return new SocketWrapper (netMqContext.CreateResponseSocket ());
		}

		#endregion
	}
}

