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
	/// Denote a read node of the Abstract Syntax Tree.
	/// </summary>
	class ReadNode : CommandNode
	{
		#region MEMBER VARIABLES

		// The Token of the variable involved into the reading operation.
		private Token ide;
		// The primitive value of the variable that will take the input result.
		private EvalType varType;

		#endregion

		#region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="ReadNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the READ statement.</param>
		/// <param name="ide">The IDE.</param>
		public ReadNode(Token token, Token ide) : base(token, "Read")
		{
			this.ide = ide;
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

			// The real type will be checked at runtime.
			return new Eval(this.Token, EvalType.ALL);
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		/// <exception cref="System.FunwapException">Thrown when the <see cref="ReadNode"/> does not support the variable type.</exception>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			string conversion = "string_readline";
			// Initialize the variable to a default value.
			switch (this.varType)
			{
				case EvalType.INT: sb.AppendLine("0;"); conversion = "Convert.ToInt32(string_readline)"; break;
				case EvalType.BOOL: sb.AppendLine("false;"); conversion = "Convert.ToBoolean(string_readline)"; break;
				case EvalType.CHAR: sb.AppendLine("'0';"); conversion = "Convert.ToChar(string_readline)"; break;
				case EvalType.STRING: sb.AppendLine("\"\";"); break;
				case EvalType.URL: sb.AppendLine("\"\";"); break;
				default:
					throw new System.FunwapException("ParseTreeException: ReadNode does not support a " + Eval.EvalType_ToString(this.varType) + " type variable.", this.Token);
			}
			// Try to do the conversion of the string read into the type of the variable.
			sb.AppendLine(SyntacticNode.Tab(tab) + "string string_readline = Console.ReadLine();");
			sb.AppendLine(SyntacticNode.Tab(tab) + "try { " + this.ide.Value + " = " + conversion + "; } ");
			sb.Append(SyntacticNode.Tab(tab) + "catch (FormatException) { Console.WriteLine(\"ParseTreeException: impossible convert the text \\\"\" + string_readline + \"\\\" into a int type\"); }");
		}
		#endregion

		#region GetValue
		/// <summary>
		/// </summary>
		/// <returns><c>null</c></returns>
		public override Eval GetValue(Result r)
		{
			Eval result = null;

			// Read the external input
			string s = r.GetStdin();

			// Write the input on the Stdout
			r.Stdout.AppendText(s + "\r\n");

			// Convert the input in the type of the variable it will be assigned, if it can
			try
			{
				switch (this.varType)
				{
					case EvalType.INT: result = new Eval(ide, System.Convert.ToInt32(s)); break;
					case EvalType.BOOL: result = new Eval(ide, System.Convert.ToBoolean(s)); break;
					case EvalType.CHAR: result = new Eval(ide, System.Convert.ToChar(s)); break;
					case EvalType.STRING: result = new Eval(ide, s); break;
					case EvalType.URL:
						Tuple<string, string> url = ConstantNode.GetInfoUrl(s);
						if (url == null)
							throw new System.FunwapException("ParseTreeException: impossible convert the text \"" + s + "\" into a url type.", ide);
						result = new Eval(ide, url);
						break;
				}
			}
			catch (System.FormatException)
			{
				throw new System.FunwapException("ParseTreeException: impossible convert the text \"" + s + "\" into a " + Eval.EvalType_ToString(this.varType) + " type.", ide);
			}

			return result;
		}
		#endregion

		#endregion
	}
}
