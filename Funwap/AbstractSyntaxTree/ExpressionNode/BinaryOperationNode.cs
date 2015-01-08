using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Funwap.LexicalAnalysis;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
    /// <summary>
    /// Denote a binary operation node of the Abstract Syntax Tree.
    /// </summary>
    public class BinaryOperationNode : OperationNode
    {
        #region MEMBER VARIABLES

        // Being a binary operation node, it has two children: one on left and one on right.
        private ExpressionNode leftNode;
		private ExpressionNode rightNode;

        #endregion

        #region CONSTRUCTOR

        /// <summary>Initializes a new instance of the <see cref="BinaryOperationNode"/> class.</summary>
		/// <param name="token">The <see cref="Token"/> containing the type of operation that we want to execute.</param>
		/// <param name="l">The left child on which the operation is performed.</param>
		/// <param name="r">The right child on which the operation is performed.</param>
		public BinaryOperationNode(Token token, ExpressionNode l, ExpressionNode r) : base(token)
        {
            this.leftNode = l;
            this.rightNode = r;

			// Add to the $GraphicNode the two children.
            this.GraphicNode.AddChild(this.leftNode.GraphicNode);
            this.GraphicNode.AddChild(this.rightNode.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		/// <exception cref="System.FunwapException">Thrown when the operation is not supported or the type of the two operators are different.</exception>
		public override Eval Check(Stack EnvStack)
		{
			EvalType leftType = this.leftNode.Check(EnvStack).GetEvalType().Item1;
			EvalType rightType = this.rightNode.Check(EnvStack).GetEvalType().Item1;

			// Check if the primitive type of the two children is the same.
			if (leftType != rightType)
				throw new System.FunwapException("ParseTreeException: operation between different type is not supported.", this.Token);

			// Check all the supported operation, and for each of them generate the proper
			// Eval result, where only the primitive type is specified.
			Eval value = null;
			switch (leftType)
			{
				case EvalType.INT:
					switch (this.type)
					{
						case OperationType.EQUAL: case OperationType.INEQUAL: case OperationType.GREATER:
						case OperationType.GREATEREQ: case OperationType.LESS: case OperationType.LESSEQ:
							value = new Eval(this.Token, EvalType.BOOL);
							break;
						case OperationType.PLUS: case OperationType.MINUS: case OperationType.MUL: case OperationType.DIV:
							value = new Eval(this.Token, EvalType.INT);
							break;
						default: throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
					}
					break;

				case EvalType.BOOL:
					switch (this.type)
					{
						case OperationType.OR: case OperationType.AND: case OperationType.EQUAL: case OperationType.INEQUAL:
							value = new Eval(this.Token, EvalType.BOOL);
							break;
						default: throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
					}
					break;

				case EvalType.CHAR:
					switch (this.type)
					{
						case OperationType.EQUAL: case OperationType.INEQUAL: case OperationType.GREATER:
						case OperationType.GREATEREQ: case OperationType.LESS: case OperationType.LESSEQ:
							value = new Eval(this.Token, EvalType.BOOL);
							break;
						default: throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
					}
					break;

				case EvalType.STRING:
					if (this.type == OperationType.PLUS) value = new Eval(this.Token, EvalType.STRING);
					else throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
					break;

				default: throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
			}
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
			sb.Append("(");
			this.leftNode.Compile(r, sb, tab);
			sb.Append(" " + OperationNode.OperationType_Compile(this.type) + " ");
			this.rightNode.Compile(r, sb, tab);
			sb.Append(")");
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
            Eval value = null;

            // Takes the value from the left child.
            Eval leftResult = this.leftNode.GetValue(r);

            // Takes the value from the right child.
            Eval rightResult = this.rightNode.GetValue(r);

            // Each type supports different operators with different behaviour.
            switch (leftResult.GetEvalType().Item1)
            {
                case EvalType.INT:
                    int iLV = leftResult.GetIValue();
                    int iRV = rightResult.GetIValue();

                    // Depending of the operation type it executes the proper calculation.
                    switch (this.type)
                    {
                        case OperationType.EQUAL:     value = new Eval(this.Token, iLV == iRV); break;
                        case OperationType.INEQUAL:   value = new Eval(this.Token, iLV != iRV); break;
                        case OperationType.GREATER:   value = new Eval(this.Token, iLV > iRV);  break;
                        case OperationType.GREATEREQ: value = new Eval(this.Token, iLV >= iRV); break;
                        case OperationType.LESS:      value = new Eval(this.Token, iLV < iRV);  break;
                        case OperationType.LESSEQ:    value = new Eval(this.Token, iLV <= iRV); break;
                        case OperationType.PLUS:      value = new Eval(this.Token, iLV + iRV);  break;
                        case OperationType.MINUS:     value = new Eval(this.Token, iLV - iRV);  break;
                        case OperationType.MUL:       value = new Eval(this.Token, iLV * iRV);  break;
                        case OperationType.DIV:
                            try { value = new Eval(this.Token, iLV / iRV); }
                            catch (System.DivideByZeroException) { throw new System.FunwapException("ParseTreeException: you're trying to divide by zero, you cannot do that.", this.Token); }
                            break;
                    }

                    break;

                case EvalType.BOOL:
                    bool bLV = leftResult.GetBValue();
                    bool bRV = rightResult.GetBValue();

                    // Depending of the operation type it executes the proper calculation.
                    switch (this.type)
                    {
                        case OperationType.OR: value = new Eval(this.Token, bLV || bRV); break;
                        case OperationType.AND: value = new Eval(this.Token, bLV && bRV); break;
						case OperationType.EQUAL: value = new Eval(this.Token, bLV == bRV); break;
						case OperationType.INEQUAL: value = new Eval(this.Token, bLV != bRV); break;
                    }

                    break;

                case EvalType.CHAR:
                    char cLV = leftResult.GetCValue();
                    char cRV = rightResult.GetCValue();

                    // Depending of the operation type it executes the proper calculation.
                    switch (this.type)
                    {
						case OperationType.EQUAL: value = new Eval(this.Token, cLV == cRV); break;
						case OperationType.INEQUAL: value = new Eval(this.Token, cLV != cRV); break;
						case OperationType.GREATER: value = new Eval(this.Token, cLV > cRV); break;
						case OperationType.GREATEREQ: value = new Eval(this.Token, cLV >= cRV); break;
						case OperationType.LESS: value = new Eval(this.Token, cLV < cRV); break;
						case OperationType.LESSEQ: value = new Eval(this.Token, cLV <= cRV); break;
                    }

                    break;
				
				case EvalType.STRING:
					string sLV = leftResult.GetSValue();
					string sRV = rightResult.GetSValue();

					// Depending of the operation type it executes the proper calculation.
					if (this.type == OperationType.PLUS)
						value = new Eval(this.Token, String.Concat(sLV, sRV));
					break;
            }

            return value;
        }
        #endregion

        #endregion
	}
}
