using NUnit.Framework;
using System;
using Castoriadis.Server;
using Moq;

namespace Castoriadis.WebServer.Test
{
	[TestFixture()]
	public class Test
	{
		MockedNamespaceBinding binding = new MockedNamespaceBinding();
		MockedHttpServiceContext httpd = new MockedHttpServiceContext ();

		[TestFixtureSetUp]
		public void Init() {
			MainClass.BindingFactory = ns => this.binding;
			MainClass.HttpdFactory = () => this.httpd;
		}

		[Test()]
		public void WebServerRegistersForTopic ([Values("register","unregister","quit")] string topic)
		{
			binding.MockedCalls.Enqueue (Tuple.Create("quit", default(object)));
			MainClass.Main (null);
			Assert.IsTrue (binding.topicRegistrations.ContainsKey (topic));
		}

		[Test]
		public void WebServerRegistersUrl() {
			var moduleName = "sample-module";
			var priority = 500; 
			var topic = "test.*";
			var reg = new UrlRegistration{
				Module = moduleName,
				Priority = priority,
				Topic = topic
			};
			binding.MockedCalls.Enqueue (Tuple.Create("register", (object)reg));
			binding.MockedCalls.Enqueue (Tuple.Create("quit", default(object)));
			MainClass.Main (null);
			Assert.IsTrue (httpd.registrations.Contains (reg));
		}
	}
}

