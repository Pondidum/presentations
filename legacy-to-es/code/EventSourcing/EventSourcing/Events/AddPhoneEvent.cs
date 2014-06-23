using EventSourcing.Infrastructure;

namespace EventSourcing.Events
{
	public class AddPhoneEvent : DomainEvent
	{
		public string Number { get; private set; }
		public string Extension { get; private set; }

		public AddPhoneEvent(string number, string extension)
		{
			Number = number;
			Extension = extension;
		}
	}
}
