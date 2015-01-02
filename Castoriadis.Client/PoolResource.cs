using System;

namespace Castoriadis.Client
{
	public class PoolResource<T>: IDisposable
	{
		T managedResource;
		Action<T> returnToPoolAction;

		public PoolResource(T resource, Action<T> returnToPool) {
			this.managedResource = resource;
			this.returnToPoolAction = returnToPool;
		}

		public T R { 
			get {
				return this.managedResource;
			}
		}

		#region IDisposable implementation

		public void Dispose () {
			this.returnToPoolAction (this.managedResource);
		}

		#endregion
	}
}

