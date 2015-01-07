using System;
using System.Collections.Generic;
using Castoriadis.Server;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Castoriadis.WebServer.Test
{
	public class MockedNamespaceBinding: INamespaceBinding
	{
		public Dictionary<string,Func<object,object>> topicRegistrations = new Dictionary<string, Func<object, object>>();
		public Queue<Tuple<string,object>> MockedCalls = new Queue<Tuple<string, object>>();

		#region INamespaceBinding implementation

		public System.Threading.Tasks.Task RunService ()
		{
			return Task.Run (() => {
				while(MockedCalls.Count > 0) {
					var call = MockedCalls.Dequeue();
					Func<object,object> handler;
					if (topicRegistrations.TryGetValue(call.Item1, out handler)) {
						handler(call.Item2);
					} else {
						Assert.Fail("No registered topic will handle {0}", call.Item1);
					}
				}
			});
		}

		public INamespaceBinding HandleAll (Func<string, string, object> catcher)
		{
			throw new NotImplementedException ();
		}

		public INamespaceBinding HandleTopic<TQ> (string topic, Func<TQ, object> handler)
		{
			this.topicRegistrations [topic] = o => handler ((TQ)o);
			return this;
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
		}

		#endregion
	}
}

