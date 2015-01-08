using Funwap.LexicalAnalysis;

namespace System
{
    /// <summary>
    /// Exception class for Funwap.
    /// </summary>
    public class FunwapException : Exception
    {
		/// <summary>
		/// Gets or sets the <see cref="Token"/> that generate the errorr.
		/// </summary>
		public Token Token { get; private set; }

        /// <summary>
		/// Initializes a new instance of the <see cref="FunwapException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
		public FunwapException(string message) : base(message) { this.Token = null;  }

		/// <summary>
		/// Initializes a new instance of the <see cref="FunwapException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="token">The token that generated the error.</param>
		public FunwapException(string message, Token token) : base(message) { this.Token = token; }
    }
}