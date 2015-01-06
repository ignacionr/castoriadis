using System;
using System.Threading.Tasks;

namespace Castoriadis.Server
{
	public interface INamespaceBinding: IDisposable
	{
		Task RunService ();
		INamespaceBinding HandleAll(Func<string,string,object> catcher);
		INamespaceBinding HandleTopic<TQ> (string topic, Func<TQ,object> handler);
	}
}

