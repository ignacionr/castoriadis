using System;
using System.Linq;
using System.Collections.Generic;
using Castoriadis.Server;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Castoriadis.WebServer
{
	public interface IHttpServiceContext
	{
		bool AddRegistration (UrlRegistration registration);
		bool RemoveRegistration(UrlRegistration registration);
		Task RunService(int port);
	}
}
