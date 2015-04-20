using System;
using System.Linq;
using Castoriadis.Client;

namespace Castoriadis.Console
{
	public abstract class SemanticContext
	{
		protected bool HandlesText { get; set; }

		protected abstract Func<object,SemanticContext> Resolve(string token);

		public SemanticContext Process (string text) {
			try {
			var idxSpace = text.IndexOf (' ');
			var item = idxSpace >= 0 ? text.Substring (0, idxSpace) : text;
			object query = idxSpace >= 0 ? text.Substring (idxSpace + 1) : default(object);
			var subContext = Resolve (item);
			if (null == subContext) {
				this.Result = "Command not found.";
				return this;
			} else {
				return subContext (query);
			}
			}
			catch(Exception ex) {
				this.Result = ex.Message;
				return this;
			}
		}

		public object Result {
			get;
			set;
		}
	}
}
