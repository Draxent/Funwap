using System;
using System.Text;
using System.Collections;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
    /// <summary>
    /// Denote a node of the Abstract Syntax Tree that contain an integer value.
    /// </summary>
    public class VarNode : ExpressionNode
    {
		#region MEMBER VARIABLES

		// True if the variable is asynchronous one, useful in the compilation phase.
		private bool async;

		#endregion

        #region CONSTRUCTOR

        /// <summary>Initializes a new instance of the <see cref="VarNode"/> class.</summary>
		/// <param name="token">The <see cref="Token"/> containing the identifier of the variable.</param>
		public VarNode(Token token) : base(token)
        {
            // Assign to the node's label the token value.
			CircleNode n = new CircleNode();
			n.AddText(token.Value);
            this.GraphicNode = new GTree<GNode>(n);

            // Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
            this.GraphicNode.SyntacticNode = this;
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
			// Search in the enviroment for variable.
			Tuple<Eval, bool> p = env.Apply(this.Token);
			Eval value = p.Item1;
			// Save the information about its flag.
			this.async = p.Item2;
			return value;
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			if (this.async)
			{
				// Since it is an asynchronous variable we have to wait the end of the Task performing the requested assignment.
				int previousLine = sb.ToString().LastIndexOf('\n');
				sb.Insert(previousLine, SyntacticNode.Tab(tab) + "Task_"+this.Token.Value+".Wait();\r\n");
			}
			sb.Append(this.Token.Value);
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		/// <exception cref="System.FunwapException">Thrown when the variable is not been initialized.</exception>
		public override Eval GetValue(Result r)
        {
			Env env = (Env)r.EnvStack.Peek();
			// Search in the enviroment for the value of the variable.
			Eval value = env.Apply(this.Token).Item1;

			// If the value is null, means that the varible was not been initialized.
			if (value.GetValue() == null)
				throw new System.FunwapException("ParseTreeException: variable \"" + this.Token.Value + "\" is not been initialized.", this.Token);
			return value;
        }
        #endregion

        #endregion

    }
}
