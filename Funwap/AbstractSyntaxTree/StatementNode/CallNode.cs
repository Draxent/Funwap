using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
	/// <summary>
	/// Denote a function call node of the Abstract Syntax Tree.
	/// </summary>
    public class CallNode : ExpressionNode
    {
        #region MEMBER VARIABLES

		// List of expressions that will form the actual parameters passed to the function.
        private List<ExpressionNode> actualParameters;
		// To distinguish if the call is a statement or it is contained inside an expression.
		private bool itIsStm = false;

        #endregion

        #region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="CallNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the name of the function to call.</param>
		/// <param name="ap">List of expressions that will form the actual parameters passed to the function.</param>
		/// <param name="itIsStm"><code>true</code> if it is a statement, otherwise <code>false</code>.</param>
		public CallNode(Token token, List<ExpressionNode> ap, bool itIsStm = false) : base(token)
        {
			this.itIsStm = itIsStm;
            this.actualParameters = ap;

            // Assign to the node's label the token value.
			CircleNode n = new CircleNode();
			n.AddText(token.Value);
            this.GraphicNode = new GTree<GNode>(n);

            // Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
            this.GraphicNode.SyntacticNode = this;

			// Add to the $GraphicNode all the paramenters.
			foreach (ExpressionNode p in ap)
				this.GraphicNode.AddChild(p.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		/// <exception cref="System.FunwapException">Thrown when calling a not function or calling this erroneously.</exception>
		public override Eval Check(Stack EnvStack)
		{
			Env env = (Env)EnvStack.Peek();
			List<EvalType> args = new List<EvalType>();

			// Take the type of each actualParameters
			foreach (ExpressionNode e in this.actualParameters)
				args.Add(e.Check(EnvStack).GetEvalType().Item1);

			// Take the value of the function
			Eval funVal = env.Apply(this.Token).Item1;
			if (funVal.GetEvalType().Item1 != EvalType.FUN)
				throw new System.FunwapException("ParseTreeException: calling a not function.", this.Token);

			FunctionNode fun = funVal.GetFValue().Item1;
			Env parent = funVal.GetFValue().Item2;

			// The number of arguments must be equal to the number of function parameters
			if (args.Count != fun.FormalParameters.Count)
				throw new System.FunwapException("ParseTreeException: call \"" + this.Token.Value + "\" has invalid number of parameters.", this.Token);

			// Check if its arguments of the caller has the right type
			for (int i = 0; i < args.Count; i++)
				if (args[i] != fun.FormalParameters[i].Item2)
					throw new System.FunwapException("ParseTreeException: the " + (i + 1) + "° argument of call \"" + this.Token.Value + "\" has to be of " + fun.FormalParameters[i].Item2 + " type.", this.Token);

			
			if (this.itIsStm)
				// Since it is a statement, we do not consider the type return by the function call.
				return null;
			else
			{
				// Return the return type of the function that has already been checked in the FunctionNode.
				if (fun.Rtype == null)
					return null;
				else if(fun.Rtype.Item1 == EvalType.FUN)
					return fun.ReturnValue;
				else
					return new Eval(this.Token, fun.Rtype);
			}
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			bool first = true;
			if (itIsStm)
				// Since it is a statement it has to respect the indentation of the code.
				sb.Append(SyntacticNode.Tab(tab));
			
			sb.Append(this.Token.Value + "(");

			// Compile all the expressions rappresenting the actual parameters.
			foreach (ExpressionNode e in this.actualParameters)
			{
				if (!first)
					sb.Append(",");
				e.Compile(r, sb, tab);
				first = false;
			}
			
			sb.Append(")");
			if (itIsStm)
				// Since it is a statement we have to append the ";" character at the end of the instraction. 
				sb.AppendLine(";");
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval" /> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
			Env env = (Env)r.EnvStack.Peek();
            List<Eval> actualValues = new List<Eval>();

            // Evaluate each actualParameters
            foreach (ExpressionNode e in this.actualParameters)
                actualValues.Add(e.GetValue(r));

            // Take the value of the function
			Eval funVal = env.Apply(this.Token).Item1;

            FunctionNode fun = funVal.GetFValue().Item1;
			Env parent = funVal.GetFValue().Item2;

			// Perform the function call.
			Eval res = fun.Call(r, parent, actualValues);

			if (this.itIsStm)
				// Since it is a statement, we do not consider the type return by the function call.
				return null;
			else
				// Return the result of the function call.
				return res;
        }
        #endregion

        #endregion

    }
}