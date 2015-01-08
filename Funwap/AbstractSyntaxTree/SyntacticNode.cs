using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
	#region SyntacticNode
	/// <summary>
    /// Denote the generic node of the Abstract Syntax Tree.
    /// </summary>
    abstract public class SyntacticNode
    {
        #region MEMBER VARIABLES

        /// <summary>The node contains a pointer to its visual representation.</summary>
        public GTree<GNode> GraphicNode;

		/// <summary>The <see cref="Token"/> representing the node.</summary>
		public Token Token;

        #endregion

        #region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="SyntacticNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> associated to this node.</param>
		public SyntacticNode(Token token)
        {
			this.Token = token;
        }

        #endregion

		#region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		abstract public Eval Check(Stack EnvStack);
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		abstract public void Compile(Result r, StringBuilder sb, int tab);
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
        abstract public Eval GetValue(Result r);
        #endregion

        #endregion

		#region Tab
		/// <summary>Generate a string with <paramref name="tab"/> number of tabulation.</summary>
		/// <param name="tab">The number of tabulation we want to generate.</param>
		/// <returns>A <see cref="System.String" />.</returns>
		protected static string Tab(int tab)
		{
			string s = "";
			for (int i = 0; i < tab; i++)
				s += "\t";
			return s;
		}
		#endregion
    }
	#endregion

	#region StatementNode
	/// <summary>
	/// Used to specified the type of <see cref="SyntacticNode"/>, it denotes a statement node of the Abstract Syntax Tree.
	/// </summary>
	abstract public class StatementNode : SyntacticNode
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StatementNode"/> class.
		/// </summary>
		/// <param name="token">The <see cref="Token" /> associated to this node.</param>
		public StatementNode(Token token) : base(token) { }
	}
	#endregion

	#region CommandNode
	/// <summary>
	/// Used to specified the type of <see cref="StatementNode"/>, it denotes a command node of the Abstract Syntax Tree.
	/// </summary>
	abstract public class CommandNode : StatementNode
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandNode"/> class.
		/// </summary>
		/// <param name="token">The <see cref="Token" /> associated to this node.</param>
		/// <param name="title">The title of this command node.</param>
		public CommandNode(Token token, string title) : base(token)
		{
			// Assign to the node's text the title
			SquareNode n = new SquareNode();
			n.AddText(title, KnownColor.Blue, FontStyle.Bold);
			n.Margin = 2;
			n.DeleteBorderPen();
			this.GraphicNode = new GTree<GNode>(n);

			// Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
			this.GraphicNode.SyntacticNode = this;
		}
	}
	#endregion

	#region ExpressionNode
	/// <summary>
	/// Used to specified the type of <see cref="SyntacticNode"/>, it denotes an expression node of the Abstract Syntax Tree.
	/// </summary>
	abstract public class ExpressionNode : StatementNode
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExpressionNode"/> class.
		/// </summary>
		/// <param name="token">The <see cref="Token" /> associated to this node.</param>
		public ExpressionNode(Token token) : base(token) { }
	}
	#endregion
}
