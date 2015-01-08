using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Funwap
{
	/// <summary>
	/// The Window used as a ShowDialog in order to accept input form the User.
	/// </summary>
	public partial class Stdin : Form
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Stdin"/> class.
		/// </summary>
		public Stdin()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles the KeyPress event of the OKButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
		private void OKButton_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				OKButton.PerformClick();
			}
		}
	}
}
