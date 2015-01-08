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
	/// Denote a FOR statement node of the Abstract Syntax Tree.
	/// </summary>
	class ForNode : CommandNode
	{
		#region MEMBER VARIABLES

		// Being a FOR node, it has four children:
		//		1. it is a statment;
		//      2. it is the condition subtree;
		//		3. it is a statment;
		//      4. it is the subtree originated by the BODY (of the for) code;
		private StatementNode stmNode1;
		private ExpressionNode condNode;
		private StatementNode stmNode2;
		private BlockNode bodyNode;

		#endregion

		#region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="ForNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the FOR statement.</param>
		/// <param name="stm1">The first statement executed only at the begining.</param>
		/// <param name="condN">The subtree representing the condition.</param>
		/// <param name="stm2">The second statement executed at the end of each iteration.</param>
		/// <param name="body">The subtree originated by the BODY code.</param>
		public ForNode(Token token, StatementNode stm1, ExpressionNode condN, StatementNode stm2, BlockNode body) : base(token, "For")
		{
			this.stmNode1 = stm1;
			this.condNode = condN;
			this.stmNode2 = stm2;
			this.bodyNode = body;

			// Add to the $GraphicNode the four children.
			this.GraphicNode.AddChild(this.stmNode1.GraphicNode);
			this.GraphicNode.AddChild(this.condNode.GraphicNode);
			this.GraphicNode.AddChild(this.stmNode2.GraphicNode);
			this.GraphicNode.AddChild(this.bodyNode.GraphicNode);
		}

		#endregion

		#region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		/// <exception cref="System.FunwapException">Thrown when the FOR condition is not a boolean type.</exception>
		public override Eval Check(Stack EnvStack)
		{
			// Check the two statements.
			this.stmNode1.Check(EnvStack);
			this.stmNode2.Check(EnvStack);

			// Check the condition node and if its value is boolean type.
			if (this.condNode.Check(EnvStack).GetEvalType().Item1 != EvalType.BOOL)
				throw new System.FunwapException("ParseTreeException: the FOR condition must be boolean type.", this.Token);

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
			sb.Append(SyntacticNode.Tab(tab) + "for (");
			this.stmNode1.Compile(r, sb, 0);
			sb.Remove(sb.Length - 1, 1);
			sb.Append(" ; ");
			this.condNode.Compile(r, sb, 0);
			sb.Append(" ; ");
			this.stmNode2.Compile(r, sb, 0);
			sb.Remove(sb.Length - 1, 1);
			sb.Append(")");
			sb.AppendLine();
			this.bodyNode.Compile(r, sb, tab);
		}
		#endregion

		#region GetValue
		public override Eval GetValue(Result r)
		{
			// Evaluate the first statement only the first time.
			this.stmNode1.GetValue(r);
			return GetValue2(r);
		}
		#endregion

		#region GetValue2
		/// <summary>Manage and evaluate the cycle part of the FOR statement.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public Eval GetValue2(Result r)
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
				{
					// Evaluate the second statement
					this.stmNode2.GetValue(r);

					// Otherwise, we execute again the BODY code, until the condition not become FALSE
					return this.GetValue2(r);
				}
			}
			else
				return null;
		}
		#endregion

		#endregion
	}
}
