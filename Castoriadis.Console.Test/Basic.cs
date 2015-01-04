using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack;

namespace Castoriadis.Console.Test
{
	[TestFixture]
	public class Basic
	{
		Queue<string> commands = new Queue<string>();
		List<object> responses = new List<object>();
		AgoraMock agoraMock = new AgoraMock();

		[SetUp]
		public void SetitUp() {
			MainClass.agoraFactory = () => agoraMock;
			MainClass._readCommand = () => { var resp = commands.Dequeue(); System.Console.WriteLine(resp); return resp;};
			MainClass._writeResult = responses.Add;
		}

		[Test]
		public void help_includes_help_and_quit() {
			new [] { "help", "quit" }.ToList ().ForEach (commands.Enqueue);
			responses.Clear ();
			MainClass.Main (null);
			Assert.IsTrue (responses [0].ToString ().Contains ("help"));
			Assert.IsTrue (responses [0].ToString ().Contains ("quit"));
		}

		[Test]
		public void can_move_forth_and_back_on_registered_namespaces() {
			new [] { "refresh","registrations","dummy-namespace","try-test {prop:42}","back", "quit" }.ToList ().ForEach (commands.Enqueue);
			responses.Clear ();
			MainClass.Main (null);
			Assert.IsTrue (responses [1].ToString ().Contains ("dummy-namespace"), "registration must be found");
			Assert.IsTrue (responses [3].ToString ().Contains ("True"), "dummy return value must be True");
			Assert.AreEqual(agoraMock.lastItem, "try-test", "the item requested must be as passed");
			Assert.IsNotNull (agoraMock.lastQuery, "must have received a query");
			Assert.AreEqual((string)agoraMock.lastQuery, "{prop:42}");
		}
	}
}

