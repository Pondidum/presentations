using System.Collections.Generic;
using DbDemo.Manage.Entities;

namespace DbDemo.Manage.Version2
{
    public class AddressCollection : List<Address>, IEntityCollection
	{
		private const string Sql = "select * from addresses";

		public void Load()
		{
			Clear();

            Database.ExecuteReader(Sql, reader =>
            {
                while (reader.Read())
                {
                    Add(new Address(reader));
                }
            });
		}

        public void Save()
        {
            foreach (var address in this)
            {
                address.Save();
            }
        }
	}
}