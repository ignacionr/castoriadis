using System;
using Castoriadis.Client;

namespace Castoriadis.Console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var context = new GlobalContext();
			SemanticContext currentContext = context;
			while (!context.End) {
				System.Console.Write ("{0}> ", currentContext);
				var text = System.Console.ReadLine ();
				currentContext = currentContext.Process (text);
				System.Console.WriteLine (currentContext.Result);
			}
		}
	}
}
