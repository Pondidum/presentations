using System;

namespace CommandHandler.Entities
{
	public interface IKeyed
	{
		Guid ID { get; set; }
	}
}
