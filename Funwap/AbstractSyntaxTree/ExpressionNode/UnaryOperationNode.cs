using System.Text;
using System.Collections;
using Funwap.LexicalAnalysis;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
    /// <summary>
    /// Denote a unary operation node of the Abstract Syntax Tree.
    /// </summary>
    public class UnaryOperationNode : OperationNode
    {
        #region MEMBER VARIABLES

        // Being a unary operation node, it has one child.
		private ExpressionNode node;

        #endregion

        #region CONSTRUCTOR

        /// <summary>Initializes a new instance of the <see cref="UnaryOperationNode"/> class.</summary>
		/// <param name="token">The <see cref="Token"/> containing the type of operation that we want to execute.</param>
        /// <param name="n">The child on which the operation is performed.</param>
		public UnaryOperationNode(Token token, ExpressionNode n) : base(token)
        {
            this.node = n;

            // Add to the $GraphicNode the child
            this.GraphicNode.AddChild(this.node.GraphicNode);
        }

        #endregion

        #region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		/// <exception cref="System.FunwapException">Thrown when the operation is not supported.</exception>
		public override Eval Check(Stack EnvStack)
		{
			Eval value = null;
			switch (this.node.Check(EnvStack).GetEvalType().Item1)
			{
				case EvalType.INT:
					if (this.type == OperationType.MINUS)
						value = new Eval(this.Token, EvalType.INT);
					else
						throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
					break;

				case EvalType.BOOL:
					if (this.type == OperationType.NOT)
						value = new Eval(this.Token, EvalType.BOOL);
					else
						throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
					break;
				default:
					throw new System.FunwapException("ParseTreeException: the operation is not supported.", this.Token);
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
			sb.Append(OperationNode.OperationType_Compile(this.type));
			this.node.Compile(r, sb, tab);
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

            // Takes the value from his child.
            Eval childResult = this.node.GetValue(r);

            // Each type supports different operators with different behaviour.
            switch (childResult.GetEvalType().Item1)
            {
                case EvalType.INT:
                    int iV = childResult.GetIValue();

                    // Depending of the operation type it executes the proper calculation.
                    if (this.type == OperationType.MINUS)
                        value = new Eval(this.Token, -iV);
                    break;

                case EvalType.BOOL:
                    bool bV = childResult.GetBValue();

                    // Depending of the operation type it executes the proper calculation.
                    if (this.type == OperationType.NOT)
						value = new Eval(this.Token, !bV);
                    break;
            }

            return value;
        }
        #endregion

        #endregion
    }
}
