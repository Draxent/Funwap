using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Funwap.GraphicTree
{
    /// <summary>
    /// A class to draw a graphical node that follows the interface <see cref="IDrawable"/>.
    /// </summary>
    public abstract class GNode : IDrawable
    {
		#region CONSTANTS

		// Default graphical properties
		private const int dMargin = 10;
		private const string dFontFamily = "Courier New";
		private const float dSizeText = 12;
		private const KnownColor dTextColor = KnownColor.Black;
		private const float dPenWidth = 1;
		private const DashStyle dPenStyle = DashStyle.Solid;
		private const KnownColor dPenColor = KnownColor.Black;
		private const KnownColor dBgColor = KnownColor.White;

		#endregion

		#region VARIABLES

		/// <summary>It contains the list of texts that make up the title of the node, each one with their graphical properties.</summary>
		protected List<Tuple<string, KnownColor, FontStyle, float, string>> textList = new List<Tuple<string, KnownColor, FontStyle, float, string>>();


		/// <summary>The properties of the pen used to draw the node's border.</summary>
		protected Tuple<KnownColor, float, DashStyle> pen = new Tuple<KnownColor, float, DashStyle>(dPenColor, dPenWidth, dPenStyle);

		#endregion

		#region PROPERTIES

		/// <summary>
        /// Gets or sets the margin space to leave on the left and right for 
        /// the node label when measuring the ellipse size, that is the node's border.
        /// </summary>
        public int Margin { get; set; }

        /// <summary>
        /// Gets or sets the brush used for the node's background.
        /// </summary>
        public Brush BgBrush { get; set; }

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="GNode"/> class.
        /// </summary>
        public GNode()
        {
			this.Margin = dMargin;
			this.BgBrush = new SolidBrush(Color.FromKnownColor(dBgColor));
        }

        #endregion

        #region PROTECTED METHODS

        #region DrawNode
        /// <summary>
        /// Draw the node, without label, centered at (<paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <param name="size">The size of the node obtained calling <see cref="GetSize"/> method.</param>
        /// <param name="x">The x-coordinate of the center.</param>
        /// <param name="y">The y-coordinate of the center.</param>
        abstract protected void DrawNode(Graphics g, SizeF size, float x, float y);
        #endregion

        #endregion

        #region PUBLIC METHODS

		#region AddText
		/// <summary>
		/// Allows to add a text, with private formatting, into the graphic node.
		/// </summary>
		/// <param name="s">The text string.</param>
		/// <param name="txtColor">Color of the text.</param>
		/// <param name="fs">The font style of the text.</param>
		/// <param name="sizeText">The size of the text.</param>
		/// <param name="family">The font family of the text.</param>
		public void AddText(string s, KnownColor txtColor = dTextColor, FontStyle fs = FontStyle.Regular, float sizeText = dSizeText, string family = dFontFamily)
		{
			textList.Add(new Tuple<string, KnownColor, FontStyle, float, string>(s, txtColor, fs, sizeText, family));
		}
		#endregion

		#region SetBorderPen
		/// <summary>
		/// Set the properties of the border pen.
		/// </summary>
		/// <param name="penColor">Color of the pen.</param>
		/// <param name="width">The line's width.</param>
		/// <param name="st">The pen's style.</param>
		public void SetBorderPen(KnownColor penColor, float width = dPenWidth, DashStyle st = dPenStyle)
		{
			pen = new Tuple<KnownColor, float, DashStyle>(penColor, width, st);
		}
		#endregion

		#region DeleteBorderPen
		/// <summary>
		/// Set to <c>null</c> the properties of the border pen, so the border will not be drawn.
		/// </summary>
		public void DeleteBorderPen()
		{
			pen = null;
		}
		#endregion

        #region GetSize
        /// <summary>
        /// Return the size of the string plus a margin.
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <returns>The calulated size.</returns>
        public SizeF GetSize(Graphics g)
        {
			// Take the size of the margin.
			SizeF margin = new SizeF(this.Margin, this.Margin);
			SizeF r = new SizeF(0, 0);

			// Sum all the strings measure of the node's text.
			foreach(Tuple<string, KnownColor, FontStyle, float, string> t in textList)
			{
				using (Font f = new Font(t.Item5, t.Item4, t.Item3))
				{
					SizeF s = g.MeasureString(t.Item1, f);
					r.Width += s.Width;
					r.Height = Math.Max(r.Height, s.Height);
				}
			}

            return margin + r;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw the node centered at (<paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <param name="x">The x-coordinate of the center.</param>
        /// <param name="y">The y-coordinate of the center.</param>
        public void Draw(Graphics g, float x, float y)
        {
            SizeF size = this.GetSize(g);

			// Calculate the position of the string
			float posx = x - (size.Width - this.Margin) / 2;
			float posy = y - (size.Height - this.Margin) / 2;

            this.DrawNode(g, size, x, y);

            // Draw the text
			// Draw all the text, with their formatting.
			foreach (Tuple<string, KnownColor, FontStyle, float, string> t in textList)
			{
				using (Font f = new Font(t.Item5, t.Item4, t.Item3))
				{
					// Draw the string
					g.DrawString(t.Item1, f, new SolidBrush(Color.FromKnownColor(t.Item2)), posx, posy);

					// Update the position of the end of the printed string
					posx += g.MeasureString(t.Item1, f).Width;
				}
			}
        }
        #endregion

        #region IsPointInside
        /// <summary>
        /// Return true if the <paramref name="target"/> point is inside this node
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <param name="center">The center point.</param>
        /// <param name="target">The target point.</param>
        /// <returns><c>true</c> if the target point is inside this node; otherwise, <c>false</c>.</returns>
        abstract public bool IsPointInside(Graphics g, PointF center, PointF target);
        #endregion

        #endregion
    }
}
