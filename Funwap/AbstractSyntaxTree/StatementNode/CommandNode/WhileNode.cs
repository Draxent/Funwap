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
	/// Denote a WHILE statement node of the Abstract Syntax Tree.
	/// </summary>
	class WhileNode : CommandNode
    {
        #region MEMBER VARIABLES

        // Being a WHILE node, it has two children:
        //      1. it is the condition subtree;
        //      2. it is the subtree originated by the BODY (of the while) code;
        private ExpressionNode condNode;
        private BlockNode bodyNode;

        #endregion

        #region CONSTRUCTOR

        /// <summary>Initializes a new instance of the <see cref="WhileNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the WHILE statement.</param>
		/// <param name="condN">The subtree representing the condition.</param>
		/// <param name="bodyN">The subtree originated by the BODY code.</param>
		public WhileNode(Token token, ExpressionNode condN, BlockNode bodyN) : base(token, "While")
        {
            this.condNode = condN;
            this.bodyNode = bodyN;

            // Add to the $GraphicNode the two children.
            this.GraphicNode.AddChild(this.condNode.GraphicNode);
            this.GraphicNode.AddChild(this.bodyNode.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		/// <exception cref="System.FunwapException">Thrown when the WHILE condition is not a boolean type.</exception>
		public override Eval Check(Stack EnvStack)
		{
			// Check the condition node and if its value is boolean type.
			if (this.condNode.Check(EnvStack).GetEvalType().Item1 != EvalType.BOOL)
				throw new System.FunwapException("ParseTreeException: the WHILE condition must be boolean type.", this.Token);

			// Check the body and return its value.
			return this.bodyNode.Check(EnvStack);
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			sb.Append(SyntacticNode.Tab(tab) + "while (");
			this.condNode.Compile(r, sb, tab);
			sb.Append(")");
			sb.AppendLine();
			this.bodyNode.Compile(r, sb, tab);
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
            // Take the value from the condition child.
            if (this.condNode.GetValue(r).GetBValue())
            {
                // Execute the BODY child
                Eval value = this.bodyNode.GetValue(r);

                if (value != null)
                    // If $value is different from null, it is surely a ReturnNode, so we return its value.
                    return value;
                else
                    // Otherwise, we execute again the BODY code, until the condition not become FALSE
                    return this.GetValue(r);
            }
            else
                return null;
        }
        #endregion

        #endregion
    }
}
