using NUnit.Framework;
using System;
using Castoriadis.Client;

namespace Castoriadis.Founder.NuGet.Test
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void TestCase ()
		{
			IAgora agora = new Agora ();
			var target = new NuGetFounder ();
			target.EnsureNamespace (agora, "httpd", "Castoriadis.WebServer");
		}
	}
}

