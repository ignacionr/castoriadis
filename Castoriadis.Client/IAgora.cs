using System;

namespace Castoriadis.Client
{
	public interface IAgora
	{
		void Refresh();
		string Who();
		ServiceRegistration[] GetRegistrations ();
		RT ResolveSingle<RT>(string ns, string item, object query, int timeout = 500);
	}
}

