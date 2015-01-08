using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Funwap.LexicalAnalysis
{
	/// <summary>
	/// It is the class performing the lexical analysis,
	/// that is the process of converting a sequence of characters into a sequence of tokens.
	/// </summary>
	public class Scanner
	{
		#region MEMBER VARIABLES

		// The string to convert.
		private string source = "";

		// The index of the character that we are analysing.
		private int index = 0;

		// The line where we are in the $source
		private int row = 1;

		// The column where we are in the $source
		private int column = 1;

		// List of rule (regular expression) used to match the tokens, ordered by priority. 
		private Regex[] rules = new Regex[49];

		// Regular expression used to match line comments
		private Regex lineCommentRule;

		// Regular expression used to match multiple line comments
		private Regex multilineCommentRule;

		// Results of the list of rules ordered in the same way.
		private TokenType[] results = new TokenType[49];

		#endregion

		#region CONSTRUCTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="Scanner"/> class.
		/// </summary>
		public Scanner()
		{
			rules[0] = new Regex(@"^{");							results[0] = TokenType.CURLYBR_OPEN;
			rules[1] = new Regex(@"^}");							results[1] = TokenType.CURLYBR_CLOSE;
			rules[2] = new Regex(@"^\(");							results[2] = TokenType.ROUNDBR_OPEN;
			rules[3] = new Regex(@"^\)");							results[3] = TokenType.ROUNDBR_CLOSE;
			rules[4] = new Regex(@"^;");							results[4] = TokenType.SEMICOLONS;
			rules[5] = new Regex(@"^,");							results[5] = TokenType.COMMA;
			rules[6] = new Regex(@"^\*");							results[6] = TokenType.MUL;
			rules[7] = new Regex(@"^\/");							results[7] = TokenType.DIV;
			rules[8] = new Regex(@"^\+\+");							results[8] = TokenType.INCR;
			rules[9] = new Regex(@"^--");							results[9] = TokenType.DECR;
			rules[10] = new Regex(@"^\+=");							results[10] = TokenType.ASSIGN_PLUS;
			rules[11] = new Regex(@"^-=");							results[11] = TokenType.ASSIGN_MINUS;
			rules[12] = new Regex(@"^\+");							results[12] = TokenType.PLUS;
			rules[13] = new Regex(@"^-");							results[13] = TokenType.MINUS;
			rules[14] = new Regex(@"^&&");							results[14] = TokenType.AND;
			rules[15] = new Regex(@"^\|\|");						results[15] = TokenType.OR;
			rules[16] = new Regex(@"^>=");							results[16] = TokenType.GREATEREQ;
			rules[17] = new Regex(@"^<=");							results[17] = TokenType.LESSEQ;
			rules[18] = new Regex(@"^==");							results[18] = TokenType.EQUAL;
			rules[19] = new Regex(@"^!=");							results[19] = TokenType.INEQUAL;
			rules[20] = new Regex(@"^>");							results[20] = TokenType.GREATER;
			rules[21] = new Regex(@"^<");							results[21] = TokenType.LESS;
			rules[22] = new Regex(@"^=");							results[22] = TokenType.ASSIGN;
			rules[23] = new Regex(@"^\!");							results[23] = TokenType.NOT;
			rules[24] = new Regex(@"^\'.\'");						results[24] = TokenType.CHAR;
			rules[25] = new Regex(@"^tcp://localhost:(\d+)/(\w+)");	results[25] = TokenType.URL;
			rules[26] = new Regex("^\"[^\"]*\"");					results[26] = TokenType.STRING;
			rules[27] = new Regex(@"^\d+");							results[27] = TokenType.NUMBER;
			rules[28] = new Regex(@"^var");							results[28] = TokenType.DECLVAR;
			rules[29] = new Regex(@"^func");						results[29] = TokenType.DECLFUNC;
			rules[30] = new Regex(@"^int");							results[30] = TokenType.TYPEINT;
			rules[31] = new Regex(@"^bool");						results[31] = TokenType.TYPEBOOL;
			rules[32] = new Regex(@"^char");						results[32] = TokenType.TYPECHAR;
			rules[33] = new Regex(@"^string");						results[33] = TokenType.TYPESTRING;
			rules[34] = new Regex(@"^url");							results[34] = TokenType.TYPEURL;
			rules[35] = new Regex(@"^fun");							results[35] = TokenType.TYPEFUN;
			rules[36] = new Regex(@"^if");							results[36] = TokenType.IF;
			rules[37] = new Regex(@"^else");						results[37] = TokenType.ELSE;
			rules[38] = new Regex(@"^while");						results[38] = TokenType.WHILE;
			rules[39] = new Regex(@"^for");							results[39] = TokenType.FOR;
			rules[40] = new Regex(@"^return");						results[40] = TokenType.RETURN;
			rules[41] = new Regex(@"^println");						results[41] = TokenType.PRINTLN;
			rules[42] = new Regex(@"^readln");						results[42] = TokenType.READLN;
			rules[43] = new Regex(@"^async");						results[43] = TokenType.ASYNC;
			rules[44] = new Regex(@"^dasync");						results[44] = TokenType.DASYNC;
			rules[45] = new Regex(@"^true");						results[45] = TokenType.TRUE;
			rules[46] = new Regex(@"^false");						results[46] = TokenType.FALSE;
			rules[47] = new Regex(@"^Main");						results[47] = TokenType.MAIN;
			rules[48] = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]*");		results[48] = TokenType.IDE;
			lineCommentRule = new Regex(@".+");
			multilineCommentRule = new Regex(@"\/\*+([^\*])+\*+\/");
		}

		#endregion

		#region PRIVATE METHODS

		#region Advance
		/// <summary>
		/// Advances properly the source text.
		/// </summary>
		/// <param name="newRow">if set to <c>true</c> increase the row counter and set to zero the column counter.</param>
		/// <exception cref="System.FunwapException">Thrown when the source text ends before expected.</exception>
		private void Advance(bool newRow = false)
		{
			if (this.index < this.source.Length)
			{
				this.index++;
				if (newRow)
				{
					this.row++;
					this.column = 1;
				}
				else
					this.column++;
			}
			else
				throw new System.FunwapException("LexicalException: unexpected EOF.");
		}
		#endregion

		#region LookAhead
		/// <summary>
		/// Look to the character in position <paramref name="p" /> respect to the current one in the source text.
		/// If the source is not ended, check if the character is equal to <paramref name="c" />.
		/// </summary>
		/// <param name="p">How much look ahead.</param>
		/// <param name="c">The character to compare.</param>
		/// <returns>
		///   <c>true</c> if the character is equal; oterwise, <c>false</c>.
		/// </returns>
		private bool LookAhead(int p, char c)
		{
			if (((this.index + p) < this.source.Length) && (this.source[this.index + p] == c))
				return true;
			else
				return false;
		}
		#endregion

		#region Skip_WhiteSpace
		/// <summary>
		/// Skips all whitespace characters and to get to the meaningful text.
		/// </summary>
		private void Skip_WhiteSpace()
		{
			// While the current character is a whitespace, skip it.
			while (Char.IsWhiteSpace(this.source[this.index]))
			{
				// If is a newline update the $line counter, otherwise advance the $index.
				if (this.source[this.index] == '\n')
				{
					if ((this.source[this.index] == '\r') && this.LookAhead(1, '\n'))
						this.Advance();
					this.Advance(true);
				}
				else
					this.Advance();
			}
		}
		#endregion

		#region Skip_Comments
		/// <summary>
		/// Skips all comments to get to the meaningful text.
		/// Actually, a Comment token is created in order to implement the colorize functionality of the source code.
		/// The Comment token will be skipped in the parser implementation.
		/// </summary>
		private Token Skip_Comments()
		{
			if (this.source[this.index] == '/')
			{
				Token t = new Token();
				t.Index = this.index;
				t.Row = this.row;
				t.Column = this.column;
				t.Type = TokenType.COMMENT;

				// Line comment
				if (this.LookAhead(1, '/'))
				{
					// Ignore everythings on the line
					Match m = lineCommentRule.Match(this.source, this.index, this.source.Length - this.index);
					this.index += m.Length;

					t.Value = m.Value;					
					t.Length = m.Length;
					return t;
				}
				// Multiple-line comment
				else if (this.LookAhead(1, '*'))
				{
					// Match the multiline comments
					Match m = multilineCommentRule.Match(this.source, this.index, this.source.Length - this.index);
					this.index += m.Length;
					string[] srows = m.Value.Split('\n');
					this.column = srows[srows.Length - 1].Length + 1;
					this.row += srows.Length - 1;

					t.Value = m.Value;
					t.Length = m.Length;
					return t;
				}
			}
			return null;
		}
		#endregion

		#region ScanToken
		/// <summary>
		/// We try to recognize the token applaying the list of rules
		/// </summary>
		/// <returns>The recognized token.</returns>
		/// <exception cref="System.FunwapException">Thrown when the token cannot be recognized.</exception>
		private Token ScanToken()
		{
			bool found = false;
			// Creates a new empty token and set its index to the current index.
			Token t = new Token(TokenType.UNKNOWN, "", this.index, this.row, this.column, 1);

			for (int i = 0; (i < rules.Length) && !found; i++)
			{
				Match m = rules[i].Match(this.source, this.index, this.source.Length - this.index);
				if (m.Success)
				{
					if (i == 24) // char
						t.Value = m.Value.Substring(1, 1);
					else if (i == 26) // string
						t.Value = m.Value.Substring(1, m.Length - 2);
					else
						t.Value = m.Value;
					t.Type = results[i];
					t.Length = m.Length;
					this.index += m.Length;
					this.column += m.Length;
					found = true;
				}
			}

			if (found)
				return t;
			else
				throw new System.FunwapException("LexicalException: undefined token " + t, t);
		}
		#endregion

		#endregion

		#region PUBLIC METHODS

		#region Initialize
		/// <summary>
		/// Initializes the Scanner.
		/// </summary>
		/// <param name="text">The source text to scan.</param>
		public void Initialize(string text)
		{
			this.source = text;
		}
		#endregion

		#region Tokenize
		/// <summary>
		/// Tokenize the source text.
		/// </summary>
		/// <returns>A collection of Token that can be enumerated.</returns>
		public IEnumerable<Token> Tokenize()
		{
			// Start to the first character of the source string.
			this.index = 0;
			this.row = 1;
			this.column = 1;

			// While the string is not ended.
			while (this.index < this.source.Length)
			{
				int oldindex;
				// Repeat until the index does not change.
				do
				{
					oldindex = this.index;

					// Skips all whitespace characters.
					this.Skip_WhiteSpace();

					// Find comments.
					Token t = this.Skip_Comments();
					// If a comment token was found, return it
					if (t != null)
						yield return t;
				} while (this.index > oldindex);

				// Returns each recognized token one at a time.
				yield return ScanToken();
			}

			// At the end returns the EOF Token.
			yield return new Token(TokenType.EOF, "EOF", this.index, this.row, this.column, 0);
		}
		#endregion

		#endregion
	}
}
