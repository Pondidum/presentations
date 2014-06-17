using CommandHandler.Entities;

namespace CommandHandler.Commands
{
	public class SaveCandidateCommand : ICommand
	{
		public Candidate Candidate { get; private set; }

		public SaveCandidateCommand(Candidate candidate)
		{
			Candidate = candidate;
		}
	}
}
