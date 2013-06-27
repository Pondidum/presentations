using System;
using System.Configuration;
using System.Reflection;

namespace DeployTestApp
{
	class Program
	{
		static void Main(string[] args)
		{

			var mailserver = ConfigurationManager.AppSettings["mailserver"];

			Console.WriteLine("Version is: {0}", Assembly.GetExecutingAssembly().GetName().Version);
			Console.WriteLine("Mailserver is: {0}", mailserver);

			Console.ReadKey();
		}
	}
}
