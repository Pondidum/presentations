using System;

namespace EntitiesCqrs
{
	public class Entity
	{
		public Guid ID { get; set; }
		public Guid ParentID { get; set; }

		public virtual void Save()
		{
		}
	}
}
