using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Funwap.GraphicTree;

namespace Funwap
{
	/// <summary>
	/// The Window that shows graphically the Abstract Syntax Tree.
	/// </summary>
	public partial class AST : Form
	{
		#region VARIABLES

		// The Graphic root of the Abstract Syntax Tree
		GTree<GNode> root = null;

		// True if the Graphic Tree is been arranged.
		bool arranged = false;

		#endregion

		#region CONSTRUCTOR
		/// <summary>Initializes a new instance of the <see cref="AST" /> class.</summary>
		/// <param name="root">The Graphic root of the Abstract Syntax Tree.</param>
		public AST(GTree<GNode> root)
		{
			InitializeComponent();

			this.root = root;
		}
		#endregion

		#region DrawingBox_Paint
		/// <summary>
		/// Handles the Paint event of the DrawingBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
		private void DrawingBox_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			e.Graphics.TranslateTransform(DrawingBox.AutoScrollPosition.X, DrawingBox.AutoScrollPosition.Y);

			if (root != null)
			{
				if (!arranged)
				{
					// Arrange the Graphic Tree.
					root.Arrange(g);

					// Set the size of the DrawingBox panel in order to contain the graphic tree
					DrawingBox.Size = Size.Round(root.TreeArea.Size + new SizeF(10, 10));

					// Set the AutoScrollMinSize property of the Panel to the size of the Graphic Tree.
					Panel.AutoScrollMinSize = DrawingBox.Size;

					// Anchor the DrawingBox to the center of the Form
					this.ClientSize = DrawingBox.Size + new Size(30, 30);
					DrawingBox.Location = new Point(
						this.ClientSize.Width / 2 - DrawingBox.Size.Width / 2,
						this.ClientSize.Height / 2 - DrawingBox.Size.Height / 2
					);
					DrawingBox.Anchor = AnchorStyles.None;
					this.CenterToScreen();

					// Set arranged to true in a way to not rearrange again.
					arranged = true;
				}

				// Draw the Graphic Tree.
				root.Draw(g);
			}
		}
		#endregion

		#region AST_KeyPress
		private void AST_KeyPress(object sender, KeyPressEventArgs e)
		{
			this.Close();
		}
		#endregion
	}
}
