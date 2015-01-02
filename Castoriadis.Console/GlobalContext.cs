using System;
using System.Linq;
using System.Collections.Generic;
using Castoriadis.Client;

namespace Castoriadis.Console
{
	public class GlobalContext: SemanticContext
	{
		public Agora agora = new Agora ();
		public bool End = false;

		static Dictionary<string,Func<GlobalContext,string,SemanticContext>> cmdLets = 
		new Dictionary<string, Func<GlobalContext,string, SemanticContext>> {
			{"quit", (c,t) => {c.End = true; c.Result = "Ending the console."; return c;} },
			{"refresh", (c,t) => {c.agora.Refresh(); c.Result = "Agora refreshed."; return c;} },
			{"who", (c,t) => {c.Result = c.agora.Who(); return c;}},
			{"help", (c,t) => {c.Result = string.Join(",", cmdLets.Keys); return c;}},
			{"registrations", (c,t) => {c.Result = string.Join("; ", c.agora.GetRegistrations().Select(reg => reg.ToString())); return c;}},
		};

		#region implemented abstract members of SemanticContext

		protected override Func<string, SemanticContext> Resolve (string token)
		{
			Func<GlobalContext,string, SemanticContext> result;
			if (!cmdLets.TryGetValue (token, out result)) {
				// is this a registered namespace?
				var r = this.agora.GetRegistrations ().FirstOrDefault (reg => reg.Namespace == token);
				if (r != null) {
					var targetContext = new NamespaceContext (this, token);
					return t => targetContext;
				}
				return null;
			}
			return t => result(this,t);
		}

		#endregion

		public override string ToString ()
		{
			return "[GlobalContext]";
		}
	}
}
