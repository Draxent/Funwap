using System;
using System.Text;
using System.Collections;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
	/// <summary>
	/// Denote an  assignment node of the Abstract Syntax Tree.
	/// </summary>
    class AssignNode : StatementNode
    {
        #region MEMBER VARIABLES

		// The valueNode containing the value to be assigned to the identifier, stored in Token.Value.
        private SyntacticNode valueNode;

        #endregion

        #region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="AssignNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the identifier.</param>
		/// <param name="valueN">The subtree representing the value to be assigned to the identifier.</param>
		public AssignNode(Token token, SyntacticNode valueN) : base(token)
        {
            this.valueNode = valueN;

            // Assign to the node's text the text $s
			SquareNode n = new SquareNode();
			n.AddText(token.Value);
            this.GraphicNode = new GTree<GNode>(n);

			// Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
			this.GraphicNode.SyntacticNode = this;

            // Add to the $GraphicNode its child.
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

			Eval value = this.valueNode.Check(EnvStack);
			EvalType tvalue = value.GetEvalType().Item1;
			if (tvalue != EvalType.ALL)
			{
				// Search in the enviroment for the type of variable that will take the input result
				Eval var = env.Apply(this.Token).Item1;
				if (!Eval.EqualTypes(var.GetEvalType(), value.GetEvalType()))
					throw new System.FunwapException("ParseTreeException: the variable \"" + this.Token.Value + "\" should have type " + Eval.TypeToString(var.GetEvalType()) + ", so cannot take a value of type " + Eval.TypeToString(value.GetEvalType()) + ".", this.Token);

				bool asyn = false;
				if ((this.valueNode.GetType().ToString() == "Funwap.AbstractSyntaxTree.AsyncNode") || (this.valueNode.GetType().ToString() == "Funwap.AbstractSyntaxTree.DAsyncNode"))
					asyn = true;

				env.Bind(this.Token.Value, value, false, asyn);
			}

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
			sb.Append(SyntacticNode.Tab(tab) + this.Token.Value + " = ");
			this.valueNode.Compile(r, sb, tab);
			if (this.valueNode.GetType().ToString() != "Funwap.AbstractSyntaxTree.ReadNode")
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

            // Take the value of the child.
            Eval value = this.valueNode.GetValue(r);

			// Bind (Update) the value with its identifier
			env.Bind(this.Token.Value, value, false);

            return null;
        }
        #endregion

        #endregion
    }
}
