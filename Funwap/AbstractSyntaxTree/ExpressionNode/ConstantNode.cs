using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
    /// <summary>
    /// Denote a node of the Abstract Syntax Tree that contain a constant value.
    /// </summary>
    public class ConstantNode : ExpressionNode
    {
        #region MEMBER VARIABLES

        // The value contained in the node.
        private Eval value;

        #endregion

        #region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="ConstantNode" /> class.</summary>
		/// <param name="token">The <see cref="Token"/> containing the constant value.</param>
		/// <exception cref="System.FunwapException">Thrown when the Token type is not an expected type or the url value has not the expected format.</exception>
		public ConstantNode(Token token) : base(token)
        {
			// Assign to the node's label the token value.
			CircleNode n = new CircleNode();

			try
			{
				switch (token.Type)
				{
					case TokenType.NUMBER:
						// Convert the text in integer and assign it to the node
						this.value = new Eval(this.Token, System.Convert.ToInt32(token.Value));
						n.AddText(token.Value, KnownColor.Orange, FontStyle.Bold);
						break;
					case TokenType.TRUE:
						this.value = new Eval(this.Token, true);
						n.AddText(token.Value, KnownColor.Orange, FontStyle.Bold);
						break;
					case TokenType.FALSE:
						this.value = new Eval(this.Token, false);
						n.AddText(token.Value, KnownColor.Orange, FontStyle.Bold);
						break;
					case TokenType.CHAR:
						// Convert the text in character and assign it to the node
						this.value = new Eval(this.Token, System.Convert.ToChar(token.Value));
						n.AddText("'"+token.Value+"'", KnownColor.DarkRed);
						break;
					case TokenType.STRING:
						this.value = new Eval(this.Token, token.Value);
						n.AddText("\""+token.Value+"\"", KnownColor.DarkRed, FontStyle.Regular, 8);
						break;
					case TokenType.URL:
						Tuple<string, string> url = ConstantNode.GetInfoUrl(this.Token.Value);
						if (url == null)
							throw new System.FunwapException("ParseTreeException: \"" + this.Token.Value + "\" is not a correct url.", this.Token);
						this.value = new Eval(this.Token, url);
						n.Margin = 15;
						n.AddText(token.Value, KnownColor.Blue, FontStyle.Italic | FontStyle.Underline, 8);
						break;
				}
			}
			catch (System.FormatException)
			{
				throw new System.FunwapException("ParseTreeException: impossible convert the text \"" + token.Value + "\" into a " + token.Type + " type.", token);
			}
			
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
			return this.value;
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		/// <exception cref="System.FunwapException">Thrown when it is impossible to compile the constant value.</exception>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			switch (this.Token.Type)
			{
				case TokenType.NUMBER: sb.Append(this.value.GetIValue()); break;
				case TokenType.TRUE: sb.Append("true"); break;
				case TokenType.FALSE: sb.Append("false"); break;
				case TokenType.CHAR: sb.Append("'" + this.value.GetCValue() + "'"); break;
				case TokenType.STRING: sb.Append("\"" + this.value.GetSValue() + "\""); break;
				case TokenType.URL: sb.Append("\"" + this.value + "\""); break;
				default:
					throw new System.FunwapException("ParseTreeException: impossible convert the text \"" + this.Token.Value + "\" into a " + this.Token.Type + " type.", this.Token);
			}
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval"/> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
			// Return the value of the node.
            return this.value;
        }
        #endregion

		#region GetInfoUrl
		/// <summary>
		/// Parse URL string in order to extract the useful information from it.
		/// </summary>
		/// <param name="url">The URL string to parse.</param>
		/// <returns>
		///	A <see cref="Tuple"/> containing: the port on which the server channel is listening and the object URI.
		/// </returns>
		public static Tuple<string, string> GetInfoUrl(string url)
		{
			Regex r = new Regex(@"^tcp://localhost:(\d+)/(\w+)");
			Match m = r.Match(url);
			if (m.Success)
				return new Tuple<string, string>(m.Groups[1].Value, m.Groups[2].Value);
			else
				return null;
		}
		#endregion

		#endregion

	}
}
