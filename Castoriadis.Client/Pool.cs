using System;
using System.Collections.Generic;
using System.Linq;

namespace Castoriadis.Client
{
	public class Pool<TR, TK>
	{
		Dictionary<TK, List<TR>> pool = new Dictionary<TK, List<TR>>();
		Func<TK,TR> resourceFactory;

		public Pool(Func<TK,TR> factory) {
			this.resourceFactory = factory;
		}

        public void Remove(TK key)
        {
            List<TR> poolItem;
            if (this.pool.TryGetValue(key, out poolItem))
            {
                poolItem.OfType<IDisposable>().ToList().ForEach(d => d.Dispose());
                this.pool.Remove(key);
            }
        }

		public PoolResource<TR> Get(TK key) {
			List<TR> poolItem;
			if (!this.pool.TryGetValue (key, out poolItem)) {
				poolItem = new List<TR> ();
				pool [key] = poolItem;
			}
			TR resource;
			if (poolItem.Count == 0) {
				resource = this.resourceFactory (key);
			} else {
				resource = poolItem.First ();
				poolItem.Remove (resource);
			}
			return new PoolResource<TR> (resource, r => poolItem.Add (r));
		}
	}
}

