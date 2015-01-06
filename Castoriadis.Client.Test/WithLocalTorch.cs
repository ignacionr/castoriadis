using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Castoriadis.Client.Test
{
    [TestFixture]
	public class WithLocalTorch
	{
		IAgora target;
		NetworkContextMock networkContext;
        private MockSocket torchSocket;

		[SetUp]
		public void Setup() {
			this.networkContext = new NetworkContextMock ();
            this.networkContext.mockSocketCreated = ms =>
            {
                ms.allowBind = true;
                this.torchSocket = ms;
            };
			this.target = new Agora(networkContext);
		}

        [Test]
        public void registration_succeeds_for_correct_message()
        {
            var response = ProcessCommand("get-registrations");
            Assert.IsFalse(response.Contains("test-ns"));
            response = ProcessCommand("register test-ns tcp://test.local:48489");
            Assert.IsTrue(Convert.ToBoolean(response));
            response = ProcessCommand("get-registrations");
            Assert.IsTrue(response.Contains("test-ns"));
        }

        [Test]
        public void RegistrationSucceedsForCorrectMessageWithService()
        {
            var response = ProcessCommand("get-registrations");
            Assert.IsFalse(response.Contains("test2-ns"));
            response = ProcessCommand("register test2-ns tcp://test.local:48489 service.exe");
            Assert.IsTrue(Convert.ToBoolean(response));
            response = ProcessCommand("get-registrations");
            Assert.IsTrue(response.Contains("test2-ns"));
        }

        [Test]
        public void CallerSocketBindsToRegisteredUri()
        {
            int port = 44567;
            var endpoint = string.Format("tcp://localhost:{0}", port);
            var response = ProcessCommand(string.Format("register test3-ns {0} service.exe", endpoint));
            Assert.IsTrue(Convert.ToBoolean(response));
            string actualEndpoint = string.Empty;
            this.networkContext.mockSocketCreated = ms =>
            {
                ms.processConnect = ep => actualEndpoint = ep;
                ms.receivedStrings.Enqueue("OK");
            };
            this.target.ResolveSingle("test3-ns", "test", null);
            Assert.AreEqual(endpoint, actualEndpoint);
        }

        private string ProcessCommand(string cmdText)
        {
            this.torchSocket.receivedStrings.Enqueue(cmdText);
            while (this.torchSocket.sentStrings.Count < 1) { ;}
            var response = this.torchSocket.sentStrings.Dequeue();
            return response;
        }
	}
}

