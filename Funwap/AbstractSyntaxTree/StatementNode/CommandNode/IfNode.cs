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
	/// Denote a IF statement node of the Abstract Syntax Tree.
	/// </summary>
	class IfNode : CommandNode
    {
        #region MEMBER VARIABLES

        // Being a IF node, it has three children:
        //      1. it is the condition subtree;
        //      2. it is the subtree originated by the THEN code;
        //      3. it is optional and is the subtree originated by the ELSE code;
        private ExpressionNode condNode;
        private BlockNode thenNode;
        private BlockNode elseNode;

        #endregion

        #region CONSTRUCTOR

        /// <summary>Initializes a new instance of the <see cref="IfNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the IF statement.</param>
		/// <param name="condN">The subtree representing the condition.</param>
        /// <param name="thenN">The subtree originated by the THEN code.</param>
        /// <param name="elseN">The subtree originated by the ELSE code.</param>
		public IfNode(Token token, ExpressionNode condN, BlockNode thenN, BlockNode elseN = null) : base(token, "If")
        {
            this.condNode = condN;
            this.thenNode = thenN;
            this.elseNode = elseN;

            // Add to the $GraphicNode the three children.
            this.GraphicNode.AddChild(this.condNode.GraphicNode);
            this.GraphicNode.AddChild(this.thenNode.GraphicNode);
            if (elseNode != null)
                this.GraphicNode.AddChild(this.elseNode.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		/// <exception cref="System.FunwapException">Thrown when the IF condition is not a boolean type or when the branches do not return the same type.</exception>
		public override Eval Check(Stack EnvStack)
		{
			// Check the condition node and if its value is boolean type.
			if (this.condNode.Check(EnvStack).GetEvalType().Item1 != EvalType.BOOL)
				throw new System.FunwapException("ParseTreeException: the IF condition must be boolean type.", this.Token);

			// Check the value of the THEN Block.
			Eval val1 = this.thenNode.Check(EnvStack);
			if (this.elseNode != null)
			{
				// Since there is the ELSE Block, it's returned value must be equal to the returned value of the THEN branch.
				Eval val2 = this.elseNode.Check(EnvStack);
				bool cond = ((val1 == null) && (val2 != null));
				cond = cond || ((val1 != null) && (val2 == null));
				cond = cond || ((val1 != null) && (val2 != null) && (!Eval.EqualTypes(val1.GetEvalType(), val2.GetEvalType())));
				if (cond)
					throw new System.FunwapException("ParseTreeException: branches must return the same type.", this.Token);
			}

			return val1;
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			sb.Append(SyntacticNode.Tab(tab) + "if (");
			this.condNode.Compile(r, sb, tab);
			sb.Append(")");
			sb.AppendLine();
			this.thenNode.Compile(r, sb, tab);
			if (this.elseNode != null)
			{
				sb.AppendLine();
				sb.AppendLine(SyntacticNode.Tab(tab) + "else");
				this.elseNode.Compile(r, sb, tab);
			}
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
			// The method takes the value of the condition child and if it is true execute the THEN child;
			// otherwise, the ELSE child if it exists.

            // Take the value from the condition child.
            if (this.condNode.GetValue(r).GetBValue())
                // Execute the THEN child and return its value (different from null if contains a ReturnNode)
                return this.thenNode.GetValue(r);
            else if (this.elseNode != null)
                // Execute the ELSE child and return its value (different from null if contains a ReturnNode)
                return this.elseNode.GetValue(r);
            else
                return null;
        }
        #endregion

        #endregion
    }
}
