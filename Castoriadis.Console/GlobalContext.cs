using System;
using System.Linq;
using System.Collections.Generic;
using Castoriadis.Client;
using System.Text.RegularExpressions;

namespace Castoriadis.Console
{
	public class GlobalContext: SemanticContext
	{
		public IAgora agora;
		public bool End = false;

		public GlobalContext(IAgora theAgora) {
			this.agora = theAgora;
		}

		static Dictionary<string,Func<GlobalContext,object,SemanticContext>> cmdLets = 
		new Dictionary<string, Func<GlobalContext,object, SemanticContext>> {
			{"quit", (c,t) => {c.End = true; c.Result = "Ending the console."; return c;} },
			{"refresh", (c,t) => {c.agora.Refresh(); c.Result = "Agora refreshed."; return c;} },
			{"who", (c,t) => {c.Result = c.agora.Who(); return c;}},
			{"help", (c,t) => {c.Result = string.Join(",", cmdLets.Keys); return c;}},
			{"registrations", (c,t) => {c.Result = string.Join("; ", c.agora.GetRegistrations().Select(reg => reg.ToString())); return c;}},
            {"resolve", (c,t) => {
                var parsed = Regex.Match(t.ToString(), "(\\S+) (\\S+)( (.*))?");
                c.Result = c.agora.ResolveSingle<dynamic>(parsed.Groups[1].Value, 
                    parsed.Groups[2].Value, null); return c;}},
		};

		#region implemented abstract members of SemanticContext

		protected override Func<object, SemanticContext> Resolve (string token)
		{
			Func<GlobalContext,object, SemanticContext> result;
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
