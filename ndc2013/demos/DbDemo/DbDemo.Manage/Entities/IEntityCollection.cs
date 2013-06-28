using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDemo.Manage.Entities
{
    public interface IEntityCollection
    {
        void Load();
        void Save();
    }
}
