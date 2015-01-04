using System;
using Castoriadis.Client;

namespace Castoriadis.Console
{
	public class MainClass
	{
		public static Func<IAgora> agoraFactory = () => new Agora();
		public static Func<string> _readCommand = System.Console.ReadLine;
		public static Action<object> _writeResult = System.Console.WriteLine;
		public static Action<SemanticContext> _prompt = ctx => System.Console.Write ("{0}> ", ctx);

		public static void Main (string[] args)
		{
			var context = new GlobalContext(agoraFactory());
			SemanticContext currentContext = context;
			while (!context.End) {
				_prompt(currentContext);
				var text = _readCommand ();
				currentContext = currentContext.Process (text);
				_writeResult (currentContext.Result);
			}
		}
	}
}
