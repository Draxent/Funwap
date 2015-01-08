namespace Funwap.LexicalAnalysis
{
    #region TokenType
    /// <summary>
    /// It is an enumeration type that reports all possible tokens supported by the <see cref="Token"/> class.
    /// </summary>
    public enum TokenType
	{
		#region Declarations
		/// <summary>Declaration of variable, "var"</summary>
		DECLVAR,

		/// <summary>Declaration of function, "func"</summary>
		DECLFUNC,
		#endregion

		#region Types
		/// <summary>Type integer, "int"</summary>
        TYPEINT,

        /// <summary>Type boolean, "bool"</summary>
        TYPEBOOL,

        /// <summary>Type character, "char"</summary>
        TYPECHAR,

		/// <summary>Type string, "string"</summary>
		TYPESTRING,

        /// <summary>Type network address, "url"</summary>
        TYPEURL,

        /// <summary>Type function, "fun"</summary>
        TYPEFUN,
        #endregion

        #region Symbols
        /// <summary>Symbol "{"</summary>
        CURLYBR_OPEN,

        /// <summary>Symbol "}"</summary>
        CURLYBR_CLOSE,

        /// <summary>Symbol "("</summary>
        ROUNDBR_OPEN,

        /// <summary>Symbol ")"</summary>
        ROUNDBR_CLOSE,

        /// <summary>Symbol ";"</summary>
        SEMICOLONS,

        /// <summary>Symbol ","</summary>
        COMMA,
        #endregion

        #region Stmts
        /// <summary>Incremental statement, "++"</summary>
        INCR,

        /// <summary>Decremental statement, "--"</summary>
        DECR,

        /// <summary>Assignment operator, "="</summary>
        ASSIGN,

        /// <summary>Assignment operator, "+="</summary>
        ASSIGN_PLUS,

        /// <summary>Assignment operator, "-="</summary>
        ASSIGN_MINUS,

        /// <summary>Conditional statement, "if"</summary>
        IF,

        /// <summary>Conditional statement, "else"</summary>
        ELSE,

        /// <summary>Iteration statement, "while"</summary>
        WHILE,

        /// <summary>Iteration statement, "for"</summary>
        FOR,

        /// <summary>Asynchronous statement, "async"</summary>
        ASYNC,

        /// <summary>Distributed statement, "dasync"</summary>
        DASYNC,

        /// <summary>Return statement, "return"</summary>
        RETURN,

        /// <summary>Print statement, "println"</summary>
        PRINTLN,

        /// <summary>Read statement, "readln"</summary>
        READLN,
        #endregion

        #region Operators
		/// <summary>Logical OR, "<![CDATA[||]]>"</summary>
        OR,

		/// <summary>Logical AND, "<![CDATA[&&]]>"</summary>
        AND,

		/// <summary>Logical NOT, "<![CDATA[!]]>"</summary>
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
        #endregion

        #region Constants
		/// <summary>Any integer number, ex: <example>1</example></summary>
        NUMBER,

		/// <summary>Any character delimited by single quotes, ex: <example>'a'</example></summary>
        CHAR,

		/// <summary>Any url, identified by the following regular expression: "<![CDATA[tcp://localhost:(\d+)/(\w+)]]>"</summary>
		URL,

		/// <summary>Any sequence of characters delimited by double quotes, ex: <example>"string"</example></summary>
        STRING,

        /// <summary>Any identifier, identified by the following regular expression: "<![CDATA[^[_a-zA-Z][_a-zA-Z0-9]*]]>"</summary>
        IDE,

        /// <summary>Logical value, "<![CDATA[true]]>"</summary>
        TRUE,

        /// <summary>Logical value, "<![CDATA[false]]>"</summary>
        FALSE,
        #endregion

		/// <summary>Identifier "main" is treated separately to ensure that the program always present the main function.</summary>
		MAIN,

		/// <summary>Line or multiline comments</summary>
		COMMENT,

        /// <summary>Token not recognized</summary>
        UNKNOWN,

        /// <summary>End of file</summary>
        EOF
    };
    #endregion

    /// <summary>
    /// The class constructs a Token, that is a single atomic unit of the language.
    /// </summary>
    public class Token
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets the type of token that we want to create.
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// Gets or sets the text value of this token.
        /// </summary>
        public string Value { get; set; }

		/// <summary>
		/// Gets or sets the position inside the source text where this token was found.
		/// </summary>
		public int Index { get; set; }

        /// <summary>
        /// Gets or sets the line position inside the source text where this token was found.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column position inside the source text where this token was found.
        /// </summary>
        public int Column { get; set; }

		/// <summary>
		/// Gets or sets the length of the found token.
		/// </summary>
		public int Length { get; set; }

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token()
        {
            this.Type = TokenType.UNKNOWN;
            this.Value = "";
			this.Index = 0;
            this.Row = 1;
            this.Column = 0;
			this.Length = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="type">The type of the Token.</param>
        /// <param name="value">The text value.</param>
		/// <param name="index">The position where it was found.</param>
		/// <param name="row">The row position where it was found.</param>
		/// <param name="column">The column position where it was found.</param>
		/// <param name="length">Its length.</param>
		public Token(TokenType type, string value, int index, int row, int column, int length)
        {
            this.Type = type;
            this.Value = value;
			this.Index = index;
            this.Row = row;
            this.Column = column;
			this.Length = length;
        }

        #endregion

        #region PUBLIC METHODS

        #region ToString
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
			return ("[" + this.Type + ", " + this.Value + ", " + this.Index + ", (" + this.Row + ", " + this.Column + "), " + this.Length + "]");
        }
        #endregion

        #region ItIsType
		/// <summary>
		/// Check if the Token is a type Token, like "int", "bool", "char", ...
		/// </summary>
		/// <returns><code>true</code> if the Token is a type Toke, otherwise <code>false</code>.</returns>
        public bool ItIsType()
        {
            return ((this.Type == TokenType.TYPEINT)
                ||  (this.Type == TokenType.TYPEBOOL)
                ||  (this.Type == TokenType.TYPECHAR)
				||  (this.Type == TokenType.TYPESTRING)
                ||  (this.Type == TokenType.TYPEURL)
                ||  (this.Type == TokenType.TYPEFUN)
           );
        }
        #endregion

        #region ItIsStmt
		/// <summary>
		/// Check if the Token is a statement Token, like "if", "while", "for", ...
		/// </summary>
		/// <returns><code>true</code> if the Token is a statement Toke, otherwise <code>false</code>.</returns>
        public bool ItIsStmt()
        {
            return ((this.Type == TokenType.IDE)
				||	(this.Type == TokenType.ASYNC)
				||	(this.Type == TokenType.DASYNC)
                ||  (this.Type == TokenType.IF)
                ||  (this.Type == TokenType.WHILE)
                ||  (this.Type == TokenType.FOR)
                ||  (this.Type == TokenType.RETURN)
				||	(this.Type == TokenType.PRINTLN)
				||  (this.Type == TokenType.READLN)
            );
        }
        #endregion

        #endregion
    }
}
