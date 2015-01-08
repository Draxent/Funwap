using System;
using System.Text;
using System.Collections;
using System.Drawing;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
	/// <summary>
	/// Denote a return node of the Abstract Syntax Tree.
	/// </summary>
    class ReturnNode : CommandNode
    {
        #region MEMBER VARIABLES

		// The valueNode containing the value to be returned.
        private SyntacticNode valueNode;

        #endregion

        #region CONSTRUCTOR

        /// <summary>Initializes a new instance of the <see cref="ReturnNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the RETURN statement.</param>
		/// <param name="valueN">The subtree representing the returned value.</param>
		public ReturnNode(Token token, SyntacticNode valueN) : base(token, "Return")
        {
            this.valueNode = valueN;

            // Add to the $GraphicNode the child.
            this.GraphicNode.AddChild(this.valueNode.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		public override Eval Check(Stack EnvStack)
		{
			// Check the valueNode and return its value.
			return this.valueNode.Check(EnvStack);
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			sb.Append(SyntacticNode.Tab(tab) + "return ");
			this.valueNode.Compile(r, sb, tab);
			sb.Append(";");
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
			// Return the value from the child.
			return this.valueNode.GetValue(r);
        }
        #endregion

        #endregion
    }
}
