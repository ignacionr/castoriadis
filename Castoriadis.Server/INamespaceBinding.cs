using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ServiceStack.Text;
using ServiceStack;

namespace Castoriadis.Server
{
	public interface INamespaceBinding: IDisposable
	{
		Task RunService ();
		INamespaceBinding HandleAll(Func<string,string,object> catcher);
		INamespaceBinding HandleTopic<TQ> (string topic, Func<TQ,object> handler);
	}
}

