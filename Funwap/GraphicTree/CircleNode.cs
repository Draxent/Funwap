using System;
using System.Drawing;

namespace Funwap.GraphicTree
{
    /// <summary>
    /// A class to draw a circular node that follows the interface <see cref="IDrawable"/>.
    /// </summary>
    public class CircleNode : GNode
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="CircleNode"/> class.
        /// </summary>
        public CircleNode() : base()
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
                g.FillEllipse(this.BgBrush, rect);

			if (pen != null)
			{
				using (Pen p = new Pen(Color.FromKnownColor(pen.Item1), pen.Item2))
				{
					p.DashStyle = pen.Item3;
					g.DrawEllipse(p, rect);
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

            // Translate so we can assume the ellipse is centered at the origin
            target.X -= center.X;
            target.Y -= center.Y;

            // Determine whether the $target point is inside the ellipse
            float w = size.Width / 2;
            float h = size.Height / 2;

            // The equation for the ellipse, that is the node's border, is [x^2/w^2 + y^2/h^2 = 1]
            return ((Math.Pow(target.X, 2) / Math.Pow(w, 2) + Math.Pow(target.Y, 2) / Math.Pow(h, 2)) <= 1);
        }
        #endregion

        #endregion
    }
}
