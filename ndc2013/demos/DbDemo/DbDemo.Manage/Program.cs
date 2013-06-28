
using System;
using System.Collections.Generic;
using System.Diagnostics;
using DbDemo.Manage.Entities;

namespace DbDemo.Manage
{
	class Program
	{
		static void Main(string[] args)
		{

			var actions = new Dictionary<Char, Set>();

			actions['1'] = new Set { Name = "Setup Database", Action = DatabaseStructure.SetupDatabase };
			actions['2'] = new Set { Name = "Upgrade: Expand", Action = DatabaseStructure.Expand };
			actions['3'] = new Set { Name = "Upgrade: Collapse", Action = DatabaseStructure.Collapse };
			
            actions['4'] = new Set { Name = "Run version 1", Action = () => ShowView(new DbDemo.Manage.Version1.AddressCollection()) };
            actions['5'] = new Set { Name = "Run version 2", Action = () => ShowView(new DbDemo.Manage.Version2.AddressCollection()) };
            actions['6'] = new Set { Name = "Run version 3", Action = () => ShowView(new DbDemo.Manage.Version3.AddressCollection()) };

			actions['0'] = new Set { Name = "Quit", Action = () => Environment.Exit(0) };



			while (true)
			{
				Console.WriteLine("");

				foreach (var set in actions)
				{
					Console.WriteLine("{0}. {1}", set.Key, set.Value.Name);
				}

				var line = "";

				while (string.IsNullOrWhiteSpace(line))
				{
					line = Console.ReadLine();
				}

				var key = line[0];

				if (actions.ContainsKey(key))
				{
					actions[key].Action();
				}
			}
		}

        private static void ShowView(IEntityCollection collection)
        {
            using (var frm = new Form1(collection))
            {
                frm.ShowDialog();
            }
        }

		private class Set
		{
			public String Name { get; set; }
			public Action Action { get; set; }
		}
	}
}
