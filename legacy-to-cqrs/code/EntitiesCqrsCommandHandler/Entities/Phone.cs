﻿using System;

namespace EntitiesCqrsCommandHandler.Entities
{
	public class Phone
	{
		public Guid ID { get; set; }
		public Guid ParentID { get; set; }

		public string Number { get; set; }
		public string Extension { get; set; }
	}
}
