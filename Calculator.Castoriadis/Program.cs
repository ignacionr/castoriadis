using System;
using DynamicExpresso;
using Castoriadis.Server;

namespace Calculator.Castoriadis
{
	class MainClass
	{
		public static void Main (string[] args)	{
			var interpreter = new Interpreter();
			using (var ns = NamespaceBinding.Create("calc")) {
				ns
					.HandleAll ((i,q) => interpreter.Eval (i))
					.RunService ()
					.Wait();
			}
		}
	}
}