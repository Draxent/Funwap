using System.Collections.Generic;
using System.Drawing;
using Funwap.AbstractSyntaxTree;

namespace Funwap.GraphicTree
{
    /// <summary>
    /// A class to draw a generic tree made of children of the same arbitrary type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The element type of the children of the tree. It has to be IDrawable.</typeparam>
    public class GTree<T> where T : IDrawable
    {
        #region CONSTANTS

        // Space to skip horizontally and vertically between siblings and between generations
        private const float HOFFSET = 5;
        private const float VOFFSET = 10;

        #endregion

        #region MEMBER VARIABLES

        /// <summary>The node of the tree of arbitrary type T.</summary>
        public T Node;

        /// <summary>Private member for <see cref="Children"/>.</summary>
        private List<GTree<T>> children = new List<GTree<T>>();

        /// <summary>Private member for <see cref="TreeArea"/>.</summary>
        private RectangleF treeArea = new RectangleF(0, 0, 0, 0);

        /// <summary>Private member for <see cref="HiddenSubTree"/>.</summary>
        private bool hidden = false;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets a space where to keep save the node's label.
        /// </summary>
        public string OldTextNode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SyntacticNode"/> object which has genereted it
        /// </summary>
        public SyntacticNode SyntacticNode { get; set; }

        /// <summary>
        /// Gets the The list of children rooted in this node of arbitrary type T.
        /// We have a list of children since it is a generic tree.
        /// </summary>
        public List<GTree<T>> Children
        {
            get
            {
                if (hidden) return new List<GTree<T>>();
                else return children;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide or not the children of the node.
        /// </summary>
        /// <value>
        /// The HiddenSubTree property gets/sets the value of the bool field, hidden.
        /// </value>
        public bool HiddenSubTree {
            get { return hidden; }
            set { hidden = value; }
        }

        /// <summary>
        /// Gets or sets the pen used to draw the arcs of the tree.
        /// </summary>
        public Pen ArcPen { get; set; }

        /// <summary>
        /// Gets or sets the rectangle that encloses the tree.
        /// </summary>
        public RectangleF TreeArea
        {
            get { return treeArea; }
            set { treeArea = value; }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the rectangle that encloses the tree.
        /// </summary>
        public float X
        {
            get { return treeArea.X; }
            set { treeArea.X = value; }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the rectangle that encloses the tree.
        /// </summary>
        public float Y
        {
            get { return treeArea.Y; }
            set { treeArea.Y = value; }
        }

        /// <summary>
        /// Gets or sets the Width property of the rectangle that encloses the tree.
        /// </summary>
        public float Width
        {
            get { return treeArea.Width; }
            set { treeArea.Width = value; }
        }

        /// <summary>
        /// Gets or sets the Height property of the rectangle that encloses the tree.
        /// </summary>
        public float Height
        {
            get { return treeArea.Height; }
            set { treeArea.Height = value; }
        }

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="GTree{T}"/> class.
        /// </summary>
        /// <param name="newNode">The node of the tree</param>
        public GTree(T newNode)
        {
            Node = newNode;

            // Assign default values to the ArcPen
            this.ArcPen = Pens.Black;
        }

        #endregion

        #region PRIVATE METHODS

        #region GetNodeCenter
        /// <summary>
        /// Calculate the node's center from the infromation of the TreeArea
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <returns>The point to the center of the node.</returns>
        private PointF GetNodeCenter(Graphics g)
        {
            float x = this.TreeArea.X + this.TreeArea.Width / 2;
            float y = this.TreeArea.Y + Node.GetSize(g).Height / 2;
            return new PointF(x, y);
        }
        #endregion

        #region DrawArcs
        /// <summary>
        ///  Draw the arcs for the subtree rooted in this node.
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        private void DrawArcs(Graphics g)
        {
            foreach (GTree<T> child in this.Children)
            {
                // Draw the arc between this node and this $child
                g.DrawLine(this.ArcPen, this.GetNodeCenter(g), child.GetNodeCenter(g));

                // Recursively make the $child draw its subtree nodes
                child.DrawArcs(g);
            }
        }
        #endregion

        #region DrawNodes
        /// <summary>
        /// Draw the nodes for the subtree rooted in this node
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        private void DrawNodes(Graphics g)
        {
            // Draw this node
            PointF center = this.GetNodeCenter(g);
            Node.Draw(g, center.X, center.Y);

            // Recursively make the child draw its subtree nodes
            foreach (GTree<T> child in this.Children)
                child.DrawNodes(g);
        }
        #endregion

		#region MoveTree
		/// <summary>
		/// Move the entire Tree of (<paramref name="x"/>, <paramref name="y"/>) respect to the previous location.
		/// </summary>
		/// <param name="x">The x-coordinate.</param>
		/// <param name="y">The y-coordinate.</param>
		private void MoveTree(float x, float y)
		{
			// Increase the coordinate of the $TreeArea.
			this.X += x;
			this.Y += y;

			// Recursively do the same thing for all the children.
			foreach (GTree<T> child in this.Children)
				child.MoveTree(x, y);
		}
		#endregion

        #endregion

        #region PUBLIC METHODS

        #region AddChild
        /// <summary>
        /// Add a child to the tree.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(GTree<T> child)
        {
            this.Children.Add(child);
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw the subtree rooted in this node.
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        public void Draw(Graphics g)
        {
            DrawArcs(g);
            DrawNodes(g);
        }
        #endregion

        #region Arrange
        /// <summary>
        /// Arrange the node and its children.
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        public void Arrange(Graphics g)
        {
            // Set TreeArea to the node's size.
            SizeF size = Node.GetSize(g);

            // Recursively arrange our children, allowing room for this node.
            float posX = this.X;
            foreach (GTree<T> child in this.Children)
            {
                // Arrange this child's subtree.
                child.X = posX;
                child.Y = this.Y + Node.GetSize(g).Height + VOFFSET;
                child.Arrange(g);

                // See if this increases the height of the tree.
                if (this.TreeArea.Bottom < (child.TreeArea.Bottom))
                    this.Height = child.TreeArea.Bottom - this.Y;

                // Allow room before the next sibling.
                posX += child.Width + HOFFSET;
            }
            if (this.Children.Count > 0)
            {
                // Remove the spacing after the last child.
                posX -= HOFFSET;
                this.Width = posX - this.X;
            }
            else
            {
                // It has no children so the $TreeArea is equal to the node's size.
                this.Width = size.Width;
                this.Height = size.Height;
            }

            // See if this node is wider than the subtree under it.
            if (size.Width > this.Width)
            {
                // Center the subtrees under this node moving all its children of $translX.
                float translX = (size.Width - this.Width) / 2;
                foreach (GTree<T> child in this.Children)
                    child.MoveTree(translX, 0);

                // The Width property of the $TreeArea is equal to the node's width since it dominates over its subtree.
                this.Width = size.Width;
            }
        }
        #endregion

        #region NodeAtPoint
        /// <summary>
        /// Return the GTree at the <paramref name="target"/> point (or <c>null</c> if there isn't one there).
        /// </summary>
        /// <param name="g">The Graphics Context.</param>
        /// <param name="target">The target point.</param>
        /// <returns>Node of the Tree pointed.</returns>
        public GTree<T> NodeAtPoint(Graphics g, PointF target)
        {
            // See if the point is inside this node.
            if (this.Node.IsPointInside(g, this.GetNodeCenter(g), target)) return this;

            // Recursively see if the point is inside one of its children.
            foreach (GTree<T> child in this.Children)
            {
                GTree<T> hitNode = child.NodeAtPoint(g, target);
                if (hitNode != null) return hitNode;
            }

            return null;
        }
        #endregion

        #endregion
    }
}
