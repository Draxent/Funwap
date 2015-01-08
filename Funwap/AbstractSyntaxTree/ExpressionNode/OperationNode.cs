using System;
using System.Text;
using System.Collections;
using System.Drawing;
using Funwap.LexicalAnalysis;
using Funwap.Environment;
using Funwap.GraphicTree;

namespace Funwap.AbstractSyntaxTree
{
    #region OperationType
    /// <summary>
    /// It is an enumeration type that reports all possible operations supported by the <see cref="OperationNode"/> class.
    /// </summary>
    public enum OperationType
    {
        /// <summary>Logical OR, "<![CDATA[or]]>"</summary>
        OR,

        /// <summary>Logical AND, "<![CDATA[and]]>"</summary>
        AND,

        /// <summary>Logical NOT, "<![CDATA[not]]>"</summary>
        NOT,

        /// <summary>Equality operator, "<![CDATA[==]]>"</summary>
        EQUAL,

        /// <summary>Inequality operator, "<![CDATA[!=]]>"</summary>
        INEQUAL,

        /// <summary>Greater than operator, "<![CDATA[>]]>"</summary>
        GREATER,

        /// <summary>Greater than or equal to operator, "<![CDATA[>=]]>"</summary>
        GREATEREQ,

        /// <summary>Less than operator, "<![CDATA[<]]>"</summary>
        LESS,

        /// <summary>Less than or equal to operator, "<![CDATA[<=]]>"</summary>
        LESSEQ,

        /// <summary>Addition operator, "<![CDATA[+]]>"</summary>
        PLUS,

        /// <summary>Subtraction operator, "<![CDATA[-]]>"</summary>
        MINUS,

        /// <summary>Multiplication operator, "<![CDATA[*]]>"</summary>
        MUL,

        /// <summary>Division operator, "<![CDATA[/]]>"</summary>
        DIV,
    };
    #endregion

    /// <summary>
    /// Denote an node operational of the Abstract Syntax Tree.
    /// </summary>
    abstract public class OperationNode : ExpressionNode
    {
        #region MEMBER VARIABLES

        /// <summary>Represent the type of operation that we want to execute.</summary>
        protected OperationType type;

        #endregion

        #region CONSTRUCTOR

        /// <summary>Initializes a new instance of the <see cref="OperationNode"/> class.</summary>
		/// <param name="token">The <see cref="Token"/> containing the type of operation that we want to execute.</param>
        public OperationNode(Token token) : base (token)
        {
            this.type = ConvertType(token.Type);

            // Assign to the node's text the symbol of the chosen operation.
			CircleNode n = new CircleNode();
			n.AddText(OperationType_ToString(this.type), KnownColor.DarkBlue, FontStyle.Bold, 14);
            this.GraphicNode = new GTree<GNode>(n);

            // Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
            this.GraphicNode.SyntacticNode = this;
        }

        #endregion

        #region PUBLIC STATIC METHODS

        #region ConvertType
        /// <summary>
		/// Covert a <see cref="TokenType"/> into an <see cref="OperationType"/>.
        /// </summary>
		/// <param name="t">The <see cref="TokenType"/> <paramref name="t"/> to convert.</param>
        /// <returns>The converted value.</returns>
		/// <exception cref="System.FunwapException">Thrown when it cannot do the conversion.</exception>
        public static OperationType ConvertType(TokenType t)
        {
            switch (t)
            {
                case TokenType.OR:          return OperationType.OR;
                case TokenType.AND:         return OperationType.AND;
                case TokenType.NOT:         return OperationType.NOT;
                case TokenType.EQUAL:       return OperationType.EQUAL;
                case TokenType.INEQUAL:     return OperationType.INEQUAL;
                case TokenType.GREATER:     return OperationType.GREATER;
                case TokenType.GREATEREQ:   return OperationType.GREATEREQ;
                case TokenType.LESS:        return OperationType.LESS;
                case TokenType.LESSEQ:      return OperationType.LESSEQ;
                case TokenType.PLUS:        return OperationType.PLUS;
                case TokenType.MINUS:       return OperationType.MINUS;
                case TokenType.MUL:         return OperationType.MUL;
                case TokenType.DIV:         return OperationType.DIV;
                default:
                    throw new System.FunwapException("ParseTreeException: failed conversion.");
            }
        }
        #endregion

        #region OperationType_ToString
        /// <summary>
		/// Represent the <see cref="OperationType"/> <paramref name="t"/> in text format.
        /// </summary>
		/// <param name="t">The <see cref="OperationType"/> to trasform into text.</param>
        /// <returns>A <see cref="System.String"/></returns>
        public static string OperationType_ToString(OperationType t)
        {
            switch (t)
            {
                case OperationType.OR:          return "OR";
                case OperationType.AND:         return "AND";
                case OperationType.NOT:         return "NOT";
                case OperationType.EQUAL:       return "=";
                case OperationType.INEQUAL:     return "≠";
                case OperationType.GREATER:     return ">";
                case OperationType.GREATEREQ:   return "≥";
                case OperationType.LESS:        return "<";
                case OperationType.LESSEQ:      return "≤";
                case OperationType.PLUS:        return "+";
                case OperationType.MINUS:       return "-";
                case OperationType.MUL:         return "×";
                case OperationType.DIV:         return "÷";
                default: return "";
            }
        }
        #endregion

		#region OperationType_Compile
		/// <summary>
		/// Convert the <see cref="OperationType"/> <paramref name="t"/> into a string compliant to the C# code.
		/// </summary>
		/// <param name="t">The <see cref="OperationType"/> to compile.</param>
		/// <returns>A <see cref="System.String"/>.</returns>
		public static string OperationType_Compile(OperationType t)
		{
			switch (t)
			{
				case OperationType.OR: return "||";
				case OperationType.AND: return "&&";
				case OperationType.NOT: return "|";
				case OperationType.EQUAL: return "==";
				case OperationType.INEQUAL: return "!=";
				case OperationType.GREATER: return ">";
				case OperationType.GREATEREQ: return ">=";
				case OperationType.LESS: return "<";
				case OperationType.LESSEQ: return "<=";
				case OperationType.PLUS: return "+";
				case OperationType.MINUS: return "-";
				case OperationType.MUL: return "*";
				case OperationType.DIV: return "/";
				default:
					throw new System.FunwapException("ParseTreeException: cannot compile the operation type.");
			}
		}
		#endregion

        #endregion
    }
}
