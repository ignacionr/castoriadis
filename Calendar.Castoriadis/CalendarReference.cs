using System;

namespace Calendar.Castoriadis
{
	public class CalendarReference
	{
		public string Url { get; set; }
		public string Name { get; set; }

		public override int GetHashCode ()
		{
			return Name == null ? 0 : Name.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			var o2 = obj as CalendarReference;
			if (o2 == null)
				return false;
			return this.Url == o2.Url || this.Name == o2.Name;
		}
	}
}

