using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
	/// <summary>
	/// Denote a declaration node of the Abstract Syntax Tree.
	/// </summary>
    class DeclarationNode : StatementNode
    {
        #region MEMBER VARIABLES

		// The type of this variable.
		private Tuple<EvalType, List<EvalType>, Object> type;
		// The valueNode containing the value to be assigned to the identifier, stored in Token.Value.
        private ExpressionNode valueNode;

        #endregion

        #region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="DeclarationNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the identifier.</param>
		/// <param name="t">The type of this variable.</param>
		/// <param name="valueN">The subtree representing the value to be assigned to the identifier.</param>
		public DeclarationNode(Token token, Tuple<EvalType, List<EvalType>, Object>  t, ExpressionNode valueN = null) : base(token)
        {
			this.type = t;
            this.valueNode = valueN;

            // Assign to the node's text the title
			SquareNode n = new SquareNode();
			n.AddText(Eval.TypeToString(t), KnownColor.Indigo, FontStyle.Italic);
			n.AddText(token.Value);
            this.GraphicNode = new GTree<GNode>(n);

            // Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
            this.GraphicNode.SyntacticNode = this;

            // Add to the $GraphicNode its child.
            if (this.valueNode != null)
                this.GraphicNode.AddChild(this.valueNode.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		/// <exception cref="System.FunwapException">Thrown when the assignment does not respect the expected type.</exception>
		public override Eval Check(Stack EnvStack)
		{
			Env env = (Env)EnvStack.Peek();
			bool asyn = false;
			Eval value;

			if (this.valueNode != null)
			{
				// Since the valueNode is present, check it.
				value = this.valueNode.Check(EnvStack);
				
				// If the expected type is different from the type of the value obtained above, thrown an exception.
				if (!Eval.EqualTypes(this.type, value.GetEvalType()))
					throw new System.FunwapException("ParseTreeException: the variable \"" + this.Token.Value + "\" should have type " + Eval.TypeToString(this.type) + ", so cannot take a value of type " + Eval.TypeToString(value.GetEvalType()) + ".", this.Token);

				// If the valueNode is an AsyncNode or DAsyncNode node, set the asyn flag to true.
				if ((this.valueNode.GetType().ToString() == "Funwap.AbstractSyntaxTree.AsyncNode") || (this.valueNode.GetType().ToString() == "Funwap.AbstractSyntaxTree.DAsyncNode"))
					asyn = true;
			}
			else
				// If the value is not present, build an Eval with the type we expect and null value.
				value = new Eval(this.Token, this.type);

			// Bind the value with its identifier.
			env.Bind(this.Token.Value, value, true, asyn);
			return null;
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			sb.Append(SyntacticNode.Tab(tab));

			// All the declaration inside the Program Block must be "static".
			// Instead of checking in which block we are, for simplicity, we prefered here to check the number of tabulation.
			if (tab == 1) sb.Append("static ");

			sb.Append(Eval.CompileType(this.type));
			sb.Append(" " + this.Token.Value);
			if (this.valueNode != null)
			{
				sb.Append(" = ");
				this.valueNode.Compile(r, sb, tab);
			}
			sb.Append(";");
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
			Env env = (Env)r.EnvStack.Peek();
            Eval value;

			if (this.valueNode != null)
				// Take the value of the child.
				value = this.valueNode.GetValue(r);
			else
				// If the value is not present, build an Eval with the type we expect and null value.
				value = new Eval(this.Token, this.type);

            // Bind the value with its identifier.
			env.Bind(this.Token.Value, value, true);

            return null;
        }
        #endregion

        #endregion
    }
}
