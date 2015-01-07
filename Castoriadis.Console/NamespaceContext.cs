using System;
using System.Linq;
using System.Collections.Generic;
using Castoriadis.Client;

namespace Castoriadis.Console
{
	class NamespaceContext: SemanticContext
	{
		string nsName;

		GlobalContext parentContext;

		public NamespaceContext(GlobalContext parentContext, string nsName) {
			this.nsName = nsName;
			this.parentContext = parentContext;
		}

		public override string ToString ()
		{
			return nsName;
		}

		#region implemented abstract members of SemanticContext

		protected override Func<object, SemanticContext> Resolve (string token)
		{
			if (token == "back")
				return t => parentContext;
			return t => {
				this.Result = parentContext.agora.ResolveSingle<dynamic>(nsName, token, t);
				return this;
			};
		}

		#endregion
	}
}
