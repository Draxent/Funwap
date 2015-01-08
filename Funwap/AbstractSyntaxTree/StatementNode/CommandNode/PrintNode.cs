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
	/// Denote a print node of the Abstract Syntax Tree.
	/// </summary>
	class PrintNode : CommandNode
	{
		#region MEMBER VARIABLES

		// Array of ExpressionNode to print.
		private ExpressionNode[] exps;

		#endregion

		#region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="PrintNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the PRINT statement.</param>
		/// <param name="exps">The exps.</param>
		public PrintNode(Token token, ExpressionNode[] exps) : base(token, "Print")
		{
			this.exps = exps;

			// Add to the $GraphicNode all the expressions
			for (int i = 0; i < exps.Length; i++)
				this.GraphicNode.AddChild(this.exps[i].GraphicNode);
		}

		#endregion

		#region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		public override Eval Check(Stack EnvStack)
		{
			// Check if all the expressions nodes are correct.
			for (int i = 0; i < exps.Length; i++)
				this.exps[i].Check(EnvStack);

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
			// Write separately all the expression, since it can have problem with parenthesis.
			for (int i = 0; i < exps.Length; i++)
			{
				sb.Append(SyntacticNode.Tab(tab) + "Console.Write(");
				this.exps[i].Compile(r, sb, tab);
				sb.AppendLine(");");
			}
			sb.Append(SyntacticNode.Tab(tab) + "Console.WriteLine();");
		}
		#endregion

		#region GetValue
		public override Eval GetValue(Result r)
		{
			for (int i = 0; i < exps.Length; i++)
			{
				// Take the value of the child.
				Eval value = this.exps[i].GetValue(r);

				// Print the value on the Stdout
				r.Stdout.AppendText(value.ToString());
			}
			// Print the new line on the Stdout
			r.Stdout.AppendText("\r\n");

			return null;
		}
		#endregion

		#endregion
	}
}
