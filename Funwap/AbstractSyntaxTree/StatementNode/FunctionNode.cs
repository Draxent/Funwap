using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
	/// <summary>
	/// Denote a function node of the Abstract Syntax Tree.
	/// </summary>
    public class FunctionNode : StatementNode
    {
        #region MEMBER VARIABLES

		/// <summary>The return type of the function</summary>
		public Tuple<EvalType, List<EvalType>, Object> Rtype;

		/// <summary>The formal parameters of the function.</summary>
		public List<Tuple<string, EvalType>> FormalParameters;

		/// <summary>The <see cref="BlockNode"/> rappresenting the body of the function.</summary>
		public BlockNode BodyNode;

		/// <summary>The returned value of the function, useful for type checking.</summary>
		public Eval ReturnValue = null; 

        #endregion

        #region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="FunctionNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the name of the function.</param>
		/// <param name="r">The r.</param>
		/// <param name="fp">The fp.</param>
		/// <param name="bodyN">The body n.</param>
		public FunctionNode(Token token, Tuple<EvalType, List<EvalType>, Object> r, List<Tuple<string, EvalType>> fp, BlockNode bodyN) : base(token)
        {
			this.Rtype = r;
            this.FormalParameters = fp;
            this.BodyNode = bodyN;

            // Assign to the node's text the title
			SquareNode n = new SquareNode();
			this.SetTitle(n);
            this.GraphicNode = new GTree<GNode>(n);

            // Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
            this.GraphicNode.SyntacticNode = this;

            // Add to the $GraphicNode its child.
            this.GraphicNode.AddChild(this.BodyNode.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region GetTitle
		/// <summary>
		/// Gets the title of this function.
		/// </summary>
		/// <returns>A <see cref="System.String"/> rappresenting the title of the function.</returns>
		public string GetTitle()
		{
			StringBuilder title = new StringBuilder();
			title.Append(Eval.TypeToString(this.Rtype));
			title.Append(" " + this.Token.Value + "(");

			foreach (Tuple<string, EvalType> p in this.FormalParameters)
				title.Append(Eval.EvalType_ToString(p.Item2) + " " + p.Item1 + ",");
			title.Remove(title.Length - 1, 1);
			title.Append(")");

			return title.ToString();
		}
		#endregion

        #region SetTitle
		/// <summary>Sets the title of this graphical node <paramref name="n"/></summary>
		/// <param name="n">The <see cref="SquareNode"/> used to rappresent this <see cref="FunctionNode"/>.</param>
        public void SetTitle(SquareNode n)
        {
			if (this.Rtype != null)
				n.AddText(Eval.TypeToString(this.Rtype), KnownColor.Indigo, FontStyle.Italic);
				
			if (this.Token.Value != "func")
				n.AddText(this.Token.Value);
			else
				n.AddText("func", KnownColor.Black, FontStyle.Bold);
			 

			n.AddText("(", KnownColor.DarkBlue, FontStyle.Bold);
			int i = 0;
			foreach (Tuple<string, EvalType> p in this.FormalParameters)
			{
				i++;
				n.AddText(Eval.EvalType_ToString(p.Item2), KnownColor.Indigo, FontStyle.Italic);
				
				if (i == this.FormalParameters.Count) // Last
					n.AddText(" " + p.Item1);
				else
					n.AddText(" " + p.Item1 + ",");
			}
			n.AddText(")", KnownColor.DarkBlue, FontStyle.Bold);
        }
        #endregion

		#region GetFunctionType
		/// <summary>Gets the type of the function.</summary>
		/// <returns>The type of the function</returns>
		public Tuple<EvalType, List<EvalType>, Object> GetFunctionType()
		{
			List<EvalType> l = new List<EvalType>();
			foreach (Tuple<string, EvalType> p in this.FormalParameters)
				l.Add(p.Item2);
			return new Tuple<EvalType, List<EvalType>, Object>(EvalType.FUN, l, this.Rtype);
		}
		#endregion

		#region Call
		/// <summary>Perform the call of this function's instance.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <param name="parent">The parent Enviroment.</param>
		/// <param name="args">The arguments passed to the function.</param>
		/// <returns></returns>
		public Eval Call(Result r, Env parent, List<Eval> args)
        {
			// Set up the action that the bodyNode has to do it after it creates its environment.
			this.BodyNode.CallBinding = new Action<Env>(
				delegate(Env env)
				{
					// Set the parent of created enviroment to the enviroment indicated by the function call. 
					env.Parent = parent;
					// Link the formal parameters with the actual parameters.
					for (int i = 0; i < args.Count; i++)
						env.Bind(this.FormalParameters[i].Item1, args[i], true);
				}
			);

            // Execute the body.
            return this.BodyNode.GetValue(r);
        }
        #endregion

		#region CheckRType
		/// <summary>
		/// Check if the returned value respect the specifics of the return type.
		/// </summary>
		/// <param name="t">The token of the function.</param>
		/// <param name="rtype">The expected return type.</param>
		/// <param name="returnValue">The value returned by the function.</param>
		/// <exception cref="System.FunwapException">Thrown when the expected return type and the returned value are different.</exception>
		public static void CheckRType(Token t, Tuple<EvalType, List<EvalType>, Object> rtype, Eval returnValue)
		{
			// If the expected return type is "void", the returnValue must be null.
			if ((returnValue != null) && (rtype == null))
				throw new System.FunwapException("ParseTreeException: the function \"" + t.Value + "\"  return the type " + Eval.EvalType_ToString(returnValue.GetEvalType().Item1) + " that was not expected.", t);
			else if (rtype != null)
			{
				// If the expected return type is different from "void", the returnValue cannot be null.
				if (returnValue == null)
					throw new System.FunwapException("ParseTreeException: the function \"" + t.Value + "\"  do not return the expected type " + Eval.EvalType_ToString(rtype.Item1) + ".", t);
				else
				{
					EvalType vt = returnValue.GetEvalType().Item1;
					// The primitive type of the return value and the expected value must concide.
					if (vt != rtype.Item1)
						throw new System.FunwapException("ParseTreeException: the function \"" + t.Value + "\" returned type " + Eval.EvalType_ToString(rtype.Item1) + " and the returned value type " + Eval.EvalType_ToString(vt) + " do not match.", t);

					if (vt == EvalType.FUN)
					{
						// The primitive type is FUN type, so get the FunctionNode of the returnValue.
						FunctionNode f = returnValue.GetFValue().Item1;

						// If the expected type is expecting parameters but the returned value do not present formal parameters, they are different.
 						// Same case for the opposite situation.
						if (( rtype.Item2 != null && f.FormalParameters == null)||(( rtype.Item2 == null && f.FormalParameters != null)))
							throw new System.FunwapException("ParseTreeException: the function \"" + t.Value + "\" returned type has not the same parameters of the returned function.", t);
						
						if (rtype.Item2 != null && f.FormalParameters != null)
						{
							// Trasform the parameters in array to simplify the comparison.
							Tuple<string, EvalType>[] formalPar = f.FormalParameters.ToArray();
							EvalType[] expectedPar = rtype.Item2.ToArray();

							// If the length of the two array is different, they expects a different number of parameters, so they are different.
							if (expectedPar.Length != formalPar.Length)
								throw new System.FunwapException("ParseTreeException: the function \"" + t.Value + "\" returned type has not the same number of parameters of the returned function.", t);
							
							// Check if all the parameters have equal type.
							for(int i = 0; i < expectedPar.Length; i++)
							{
								if (expectedPar[i] != formalPar[i].Item2)
									throw new System.FunwapException("ParseTreeException: the " + (i + 1) + "° parameter of the returned function has to be of " + expectedPar[i] + " type.", returnValue.Token);
							}
						}

						// If we reach this point means that until now the expected type follows correctly the returned value.
						// It remains only to check if the expected return type follows the actual return value of the function.
						// We do this calling recurively this function.
						FunctionNode.CheckRType(t, (Tuple<EvalType, List<EvalType>, Object>)rtype.Item3, f.ReturnValue);
					}
				}
			}
		}
		#endregion

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		public override Eval Check(Stack EnvStack)
		{
			Env parent = (Env)EnvStack.Peek();

			if (this.Token.Value != "func")
				parent.Bind(this.Token.Value, new Eval(this.Token, this, parent), true);

			// Set up the action that the bodyNode has to do it after it creates its environment
			this.BodyNode.CallBinding = new Action<Env>(
				delegate(Env env)
				{
					env.Parent = parent;
					for (int i = 0; i < FormalParameters.Count; i++)
						env.Bind(this.FormalParameters[i].Item1, new Eval(this.Token, this.FormalParameters[i].Item2), true);
				}
			);

			ReturnValue = this.BodyNode.Check(EnvStack);
			FunctionNode.CheckRType(this.Token, this.Rtype, ReturnValue);

			if (this.Token.Value != "func")
				return null;
			else
				return new Eval(this.Token, this, parent);
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			if (this.Token.Value != "func")
			{
				sb.Append(SyntacticNode.Tab(tab));

				// All the declaration inside the Program Block must be "static".
				// Instead of checking in which block we are, for simplicity, we prefered here to check the number of tabulation.
				if (tab == 1) sb.Append("static ");
				
				// Compile the return type of the function
				sb.Append(Eval.CompileType(this.Rtype));
				sb.Append(" " + this.Token.Value);
			}
			else
				// The anonymous function is managed through the keyword "delegate" in C#.
				sb.Append("delegate");

			sb.Append("(");

			// Compile all the formalParameters
			bool first = true;
			foreach (Tuple<string, EvalType> p in this.FormalParameters)
			{
				if (!first) sb.Append(",");
				sb.Append(Eval.EvalType_Compile(p.Item2) + " " + p.Item1);
				first = false;
			}
			sb.AppendLine(")");

			this.BodyNode.Compile(r, sb, tab);
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
			Env env = (Env)r.EnvStack.Peek();
			if (this.Token.Value != "func")
			{
				// Bind the value with its identifier
				env.Bind(this.Token.Value, new Eval(this.Token, this, env), true);
				return null;
			}
			else
				// Anonymous function. The environment it is cloned.
				// In this way, the anonymous function will have its private parent Environment.
				return new Eval(this.Token, this, env.Clone());
        }
        #endregion

        #endregion
	}
}
