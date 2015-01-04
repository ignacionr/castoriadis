using System;

namespace Castoriadis.Client
{
	public interface IAgora
	{
		void Refresh();
		string Who();
		ServiceRegistration[] GetRegistrations ();
		dynamic ResolveSingle(string ns, string item, object query);
	}
}

