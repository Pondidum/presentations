using System;

namespace EntitiesCqrsCommandHandler.Entities
{
	public interface IKeyed
	{
		Guid ID { get; set; }
	}
}
