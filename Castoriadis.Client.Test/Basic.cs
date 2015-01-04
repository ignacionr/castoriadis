using System;
using NUnit.Framework;

namespace Castoriadis.Client.Test
{
	public class Basic
	{
		Agora target;
		NetworkContextMock networkContext;

		[Test]
		void when_torch_can_bind_it_is_local() {
			this.networkContext = new NetworkContextMock ();
			this.target = new Agora(networkContext);

		}
	}
}

