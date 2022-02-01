using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReClassNET.AddressParser;
using ReClassNET.Controls;
using ReClassNET.Extensions;
using ReClassNET.Memory;
using ReClassNET.MemoryScanner;
using ReClassNET.MemoryScanner.Comparer;
using ReClassNET.Nodes;
using ReClassNET.UI;

namespace ReClassNET.Forms
{
	public partial class RecursiveSearchForm : IconForm
	{
		private enum SearchType
		{
		Hex32Node,
		Hex16Node,
		Int32Node,
		Int16Node,
		BoolNode,
		FloatNode,
		DoubleNode,
		Utf8TextPtrNode
		}

		public RecursiveSearchForm()
		{
			InitializeComponent();
			typeComboBox.DataSource = Enum.GetValues(typeof(SearchType));
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			GlobalWindowManager.AddWindow(this);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			GlobalWindowManager.RemoveWindow(this);
		}

		private void searchButton_Click(object sender, EventArgs e)
		{
			this.typeComboBox.Enabled = false;
			this.levelNumericUpDown.Enabled = false;
			this.offsetNumericUpDown.Enabled = false;
			this.searchButton.Enabled = false;
			this.valueTextBox.Enabled = false;
			try
			{
				Search();
				//Program.MainForm.GetMemoryViewControl().SetSelectedNodes(nodesToSelect);
			}
			catch (Exception exception)
			{
				MessageBox.Show("ERROR: " + exception.Message);
			}
			this.typeComboBox.Enabled = true;
			this.levelNumericUpDown.Enabled = true;
			this.offsetNumericUpDown.Enabled = true;
			this.searchButton.Enabled = true;
			this.valueTextBox.Enabled = true;
		}
	}
}
