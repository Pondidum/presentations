using System;
using System.Linq;
using System.Windows.Forms;
using DbDemo.Version1.Entities;

namespace DbDemo.Version1
{
	public partial class Form1 : Form
	{
		private AddressCollection _addresses;

		public Form1()
		{
			InitializeComponent();
			_addresses = new AddressCollection();
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			_addresses.Load();
			dgv.DataSource = _addresses;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			_addresses.First().Save();
		}
	}
}
