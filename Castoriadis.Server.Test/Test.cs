using NUnit.Framework;
using System;
using System.Collections.Generic;
using Castoriadis.Comm;
using Moq;
using System.Threading;

namespace Castoriadis.Server.Test
{
	[TestFixture()]
	public class Test
	{
		Mock<ISocket> mockSock = new Mock<ISocket> ();
		Queue<string> commands = new Queue<string>();

		[TestFixtureSetUp]
		public void Setup ()
		{
			mockSock.Setup (x => x.ReceiveString (It.IsAny<TimeSpan> ())).Returns (() => {
				if (this.commands.Count > 0) {
					return commands.Dequeue();
				}
				Thread.Sleep(50);
				return null;
			});
			var contextMoq = new Mock<INetworkContext> ();
			NamespaceBinding._mqContext = contextMoq.Object;
			contextMoq.Setup (x => x.CreateRequestSocket ()).Returns(mockSock.Object);
			contextMoq.Setup (x => x.CreateResponseSocket ()).Returns (mockSock.Object);
		}

		[Test]
		public void ServerWillBind() {
			var actualEndpoint = default(string);
			using (var ns = NamespaceBinding.Create("test1")) {
				;
			}
			mockSock.Verify (x => x.Bind (It.IsNotNull<string> ()), Times.Once);
		}
		
		[Test]
		public void ServerWillHandleCatchAll() {
			var result = false;
			var ev = new AutoResetEvent (false);
			this.commands.Enqueue ("true"); // the result from torch
			this.commands.Enqueue ("test"); // a new command from a client
			using (var ns = NamespaceBinding.Create("test2")) {
				ns.HandleAll ((c,s) => {result = true; ev.Set();return true;})
					.RunService();
				ev.WaitOne (10000);
			}
			Assert.IsTrue (result);
		}
		
		[Test]
		public void ServerWillHandleTopicWithInt() {
			this.Setup ();
			var result = 0;
			var ev = new AutoResetEvent (false);
			this.commands.Clear ();
			this.commands.Enqueue ("true"); // the result from torch
			this.commands.Enqueue ("test 42"); // a new command from a client
			using (var ns = NamespaceBinding.Create("test2")) {
				ns.HandleTopic<int>("test", 
				                    t => {result = t; ev.Set(); return true;})
					.RunService();
				ev.WaitOne (10000);
			}
			Assert.AreEqual (42, result);
		}

		class Point{
			public int x; 
			public int y;
		}

		[Test]
		public void ServerWillHandleTopicWithPoint() {
			var result = default(Point);
			var ev = new AutoResetEvent (false);
			this.commands.Clear ();
			this.commands.Enqueue ("true"); // the result from torch
			this.commands.Enqueue ("test2 {x:12,y:55}"); // a new command from a client
			using (var ns = NamespaceBinding.Create("test2")) {
				ns.HandleTopic<Point>("test2", 
				                      t => { result = t; ev.Set(); return t;})
					.RunService();
				ev.WaitOne(10000);
			}
			Assert.AreEqual (result.x, 12);
			Assert.AreEqual (result.y, 55);
		}
	}
}

