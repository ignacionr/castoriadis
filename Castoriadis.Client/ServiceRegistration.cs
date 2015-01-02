using System;
using System.Linq;
using NetMQ;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;

namespace Castoriadis.Client
{
	public class ServiceRegistration
	{
		public string Namespace {get;set;}
		public string Endpoint {get;set;}
		public string Provider{get;set;}

		public override string ToString ()
		{
			return string.Format ("[ServiceRegistration: Namespace={0}, Endpoint={1}, Provider={2}]", Namespace, Endpoint, Provider);
		}
	}
}

