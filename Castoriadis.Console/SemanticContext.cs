using System;
using System.Linq;
using Castoriadis.Client;

namespace Castoriadis.Console
{
	public abstract class SemanticContext
	{
		protected bool HandlesText { get; set; }

		protected abstract Func<string,SemanticContext> Resolve(string token);

		public SemanticContext Process (string text) {
			var tokens = text.Split (' ');
			var subContext = Resolve (tokens.First ());
			if (null == subContext) {
				this.Result = "Command not found.";
				return this;
			} else {
				return subContext (string.Join (" ", tokens.Skip (1)));
			}
		}

		public object Result {
			get;
			set;
		}
	}
}
