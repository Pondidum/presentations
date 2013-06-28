using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using DbDemo.Manage.Entities;

namespace DbDemo.Manage
{
	public partial class Form1 : Form
	{
		private IEntityCollection _collection;

        public Form1(IEntityCollection collection)
		{
			InitializeComponent();
			_collection = collection;
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			_collection.Load();
			dgv.DataSource = _collection;
		}

        private void btnSave_Click(object sender, EventArgs e)
        {
            _collection.Save();
        }

	}
}
