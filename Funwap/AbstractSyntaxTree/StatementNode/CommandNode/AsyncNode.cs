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
	/// Denote a asynchronous operation node of the Abstract Syntax Tree.
	/// </summary>
	class AsyncNode : CommandNode
	{
		#region MEMBER VARIABLES

		// The Token of the variable involved into the asynchronous operation.
		private Token ide;
		// The expression to execute asynchronously
		private ExpressionNode expNode;
		// The primitive value of the variable that will take the input result, useful in the compilation phase.
		private EvalType varType;

		#endregion

		#region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="AsyncNode" /> class.</summary>
		/// <param name="token">The <see cref="Token"/> containing the Async statement.</param>
		/// <param name="ide">The <see cref="Token"/> of the variable involved into the asynchronous operation.</param>
		/// <param name="expNode">The expression to execute asynchronously.</param>
		public AsyncNode(Token token, Token ide, ExpressionNode expNode) : base(token, "Async")
		{
			this.ide = ide;
			this.expNode = expNode;

			// Add to the $GraphicNode the child.
			this.GraphicNode.AddChild(expNode.GraphicNode);
		}

		#endregion

		#region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		public override Eval Check(Stack EnvStack)
		{
			Env env = (Env)EnvStack.Peek();
			// Search in the enviroment for the type of the variable that will take the input result.
			this.varType = env.Apply(this.ide).Item1.GetEvalType().Item1;
			return this.expNode.Check(EnvStack);
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		/// <exception cref="System.FunwapException">Thrown when <see cref="AsyncNode"/> does not support the type of the variable.</exception>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			// Initialize the variable to a default value.
			switch (this.varType)
			{
				case EvalType.INT: sb.AppendLine("0;"); break;
				case EvalType.BOOL: sb.AppendLine("false;"); break;
				case EvalType.CHAR: sb.AppendLine("'0';"); break;
				case EvalType.STRING: sb.AppendLine("\"\";"); break;
				case EvalType.URL: sb.AppendLine("\"\";"); break;
				default:
					throw new System.FunwapException("ParseTreeException: AsyncNode does not support a " + Eval.EvalType_ToString(this.varType) + " type variable.", this.Token);
			}

			// Create a new Task that will assign the new value, obtained executing the expression, to the variable. 
			string taskIde = "Task_"+this.ide.Value;
			sb.Append(SyntacticNode.Tab(tab) + "Task " + taskIde + " = new Task(delegate() { " + this.ide.Value + " = ");
			this.expNode.Compile(r, sb, tab);
			sb.AppendLine("; });");

			// Start the new Task.
			sb.Append(SyntacticNode.Tab(tab) + taskIde + ".Start()");
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval" /> value representing the valuation of the code contained in the tree of this node.</returns>
		/// <exception cref="System.FunwapException">Thrown in all the cases, since it is not possible to interpet a code containing the Async statement.</exception>
		public override Eval GetValue(Result r)
		{
			// We have not implemented yet the interpetation of the Async statement.
			throw new System.FunwapException("ParseTreeException: cannot interpret a code containing the Async statement.");
		}
		#endregion

		#endregion
	}
}
