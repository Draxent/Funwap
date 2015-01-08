using System;
using System.Drawing;

namespace Funwap.GraphicTree
{
    /// <summary>
    /// A class to draw a rectangular node that follows the interface <see cref="IDrawable"/>.
    /// </summary>
    public class SquareNode : GNode
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="SquareNode"/> class.
        /// </summary>
        public SquareNode() : base()
        {

        }
        #endregion

        #region PROTECTED METHODS

        #region DrawNode
        /// <summary>
        /// Draw the node centered at (<paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <param name="size">The size of the node.</param>
        /// <param name="x">The x-coordinate of the center.</param>
        /// <param name="y">The y-coordinate of the center.</param>
        protected override void DrawNode(Graphics g, SizeF size, float x, float y)
        {
            RectangleF rect = new RectangleF(x - size.Width / 2, y - size.Height / 2, size.Width, size.Height);

            if (this.BgBrush != null)
                g.FillRectangle(this.BgBrush, rect);

			if (pen != null)
			{
				using (Pen p = new Pen(Color.FromKnownColor(pen.Item1), pen.Item2))
				{
					p.DashStyle = pen.Item3;
					g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
				}
			}
        }
        #endregion

        #endregion

        #region PUBLIC METHODS

        #region IsPointInside
        /// <summary>
        /// Return true if the <paramref name="target"/> point is inside this node
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <param name="center">The center point.</param>
        /// <param name="target">The target point.</param>
        /// <returns><c>true</c> if the target point is inside this node; otherwise, <c>false</c>.</returns>
        public override bool IsPointInside(Graphics g, PointF center, PointF target)
        {
            // Get the size of the node
            SizeF size = this.GetSize(g);

            // Calculate the distance between the $center and $target points
            SizeF distance = new SizeF(Math.Abs(target.X - center.X), Math.Abs(target.Y - center.Y));

            // Let us consider the size of the sides halved
            float w = size.Width / 2;
            float h = size.Height / 2;

            // The $target point is inside only when the $distance does not exceed the sides halved
            return ((distance.Width <= w) && (distance.Height <= h));
        }
        #endregion

        #endregion
    }
}
