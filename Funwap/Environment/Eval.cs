using System;
using System.Text;
using System.Collections.Generic;
using Funwap.LexicalAnalysis;
using Funwap.AbstractSyntaxTree;

namespace Funwap.Environment
{
	#region EvalType
	/// <summary>
	/// It is an enumeration type that reports all possible primitive types supported by the <see cref="Eval"/> class.
	/// </summary>
    public enum EvalType
    {
		/// <summary>If it has no type.</summary>
		VOID,

		/// <summary>Type integer, "int"</summary>
        INT,
		
		/// <summary>Type boolean, "bool"</summary>
        BOOL,

		/// <summary>Type character, "char"</summary>
		CHAR,

		/// <summary>Type string, "string"</summary>
		STRING,

		/// <summary>Type network address, "url"</summary>
		URL,

		/// <summary>Type function, "fun"</summary>
		FUN,

		/// <summary>Special type used for ReadNode in the type checking phase</summary> 
		ALL
    }
	#endregion

	/// <summary>
	/// This class encloses all possible types and values that a variable can get.
	/// </summary>
	public class Eval
    {
		#region MEMBER VARIABLES

		/// <summary>The <see cref="Token"/> of the variable.</summary>
		public Token Token;

		// The type of the variable. Usually only the first item of the Tuple is present, the primitive type, and the other are set to null.
		// If it is a FUN type, instead, the second item contains all the type that the functions expects as parameters
		// and the third item contains the return type that the functions expects.
		private Tuple<EvalType, List<EvalType>, Object> type;

		// The value of this Eval object.
		private object value;

		#endregion

        #region CONSTRUCTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is not specified and the type it is surelly a primitive one. 
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="et">The primitive type, <see cref="EvalType"/>, to associate with this object.</param>
		public Eval(Token t, EvalType et)
		{
			this.Token = t;
			this.value = null;
			this.type = new Tuple<EvalType, List<EvalType>, Object>(et, null, null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is not specified.
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="et">The type associate to this object.</param>
		public Eval(Token t, Tuple<EvalType, List<EvalType>, Object> et)
		{
			this.Token = t;
			this.value = null;
			this.type = et;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is integer.
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="newValue">Integer value.</param>
        public Eval(Token t, int newValue)
        {
			this.Token = t;
            this.value = newValue;
            this.type = new Tuple<EvalType,List<EvalType>,object>(EvalType.INT, null, null);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is boolean.
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="newValue">Boolean value.</param>
		public Eval(Token t, bool newValue)
        {
			this.Token = t;
            this.value = newValue;
			this.type = new Tuple<EvalType, List<EvalType>, object>(EvalType.BOOL, null, null);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is a character.
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="newValue">Character value.</param>
		public Eval(Token t, char newValue)
        {
			this.Token = t;
            this.value = newValue;
			this.type = new Tuple<EvalType, List<EvalType>, object>(EvalType.CHAR, null, null);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is a string.
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="newValue">String value.</param>
		public Eval(Token t, string newValue)
		{
			this.Token = t;
			this.value = newValue;
			this.type = new Tuple<EvalType, List<EvalType>, object>(EvalType.STRING, null, null); 
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is a url.
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="newValue">A <see cref="Tuple"/> containing: the port on which the server channel is listening and the object URI.</param>
		public Eval(Token t, Tuple<string, string> newValue)
		{
			this.Token = t;
			this.value = newValue;
			this.type = new Tuple<EvalType, List<EvalType>, object>(EvalType.URL, null, null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eval"/> class, where the value is a <see cref="FunctionNode"/>.
		/// </summary>
		/// <param name="t">The <see cref="Token"/>.</param>
		/// <param name="newValue">The <see cref="FunctionNode"/>.</param>
		/// <param name="parent">The environment, <see cref="Env"/>, to which the <see cref="FunctionNode"/> refers looking at its parent.</param>
		public Eval(Token t, FunctionNode newValue, Env parent)
        {
			this.Token = t;
			this.value = new Tuple<FunctionNode, Env>(newValue, parent);
			this.type = newValue.GetFunctionType();
        }

        #endregion

        #region STATIC METHODS

		#region EvalType_ToString
		/// <summary>Convert the <see cref="EvalType"/> into a <see cref="System.String" />.</summary>
		/// <param name="t">The <see cref="EvalType"/> to convert into a <see cref="System.String" />.</param>
		/// <returns>A <see cref="System.String" />.</returns>
		/// <exception cref="System.FunwapException">Thrown when the <see cref="EvalType"/> <paramref name="t"/> was not expected.</exception>
		public static string EvalType_ToString(EvalType t)
		{
			switch (t)
			{
				case EvalType.VOID: return "";
				case EvalType.INT: return "int";
				case EvalType.BOOL: return "bool";
				case EvalType.CHAR: return "char";
				case EvalType.STRING: return "string";
				case EvalType.URL: return "url";
				case EvalType.FUN: return "fun";
				default:
					throw new System.FunwapException("ParseTreeException: cannot convert EvalType to string.");
			}
		}
		#endregion

		#region EvalType_Compile
		/// <summary>Convert the <see cref="EvalType"/> into a <see cref="System.String" /> compliant to the C# code.</summary>
		/// <param name="t">The <see cref="EvalType"/> to compile.</param>
		/// <returns>A <see cref="System.String" />.</returns>
		/// <exception cref="System.FunwapException">Thrown when the <see cref="EvalType"/> <paramref name="t"/> was not expected.</exception>
		public static string EvalType_Compile(EvalType t)
		{
			switch (t)
			{
				case EvalType.VOID: return "void";
				case EvalType.INT: return "int";
				case EvalType.BOOL: return "bool";
				case EvalType.CHAR: return "char";
				case EvalType.STRING: return "string";
				case EvalType.URL: return "string";
				default:
					throw new System.FunwapException("ParseTreeException: cannot compile EvalType.");
			}
		}
		#endregion

		#region TypeToString2
		/// <summary>Convert the type into a string.</summary>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="type">The type to convert.</param>
		private static void TypeToString2(StringBuilder sb, Tuple<EvalType, List<EvalType>, Object> type)
		{
			if (type != null)
			{
				// Convert the primitive type into a string.
				sb.Append(Eval.EvalType_ToString(type.Item1));

				// If the Item2 is null means that primitive type is not a function.
				if (type.Item2 != null)
				{
					// The primitive type is a function.
					sb.Append("(");

					bool first = true;
					foreach (EvalType t in type.Item2)
					{
						if (!first)
							sb.Append(", ");
						// Convert the primitive type of all the parameters, they cannot be of function type. 
						sb.Append(Eval.EvalType_ToString(t));
						first = false;
					}
					sb.Append(")");

					// If the return type is present, convert it into a string calling recursively this function.
					if (type.Item3 != null)
						Eval.TypeToString2(sb, (Tuple<EvalType, List<EvalType>, Object>)type.Item3);
				}
			}
		}
		#endregion

		#region ComileType2
		/// <summary>Convert the type of this istance into a string compliant to the C# code.</summary>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="type">The type to compile.</param>
		private static void ComileType2(StringBuilder sb, Tuple<EvalType, List<EvalType>, Object> type)
		{
			if (type != null)
			{
				if (type.Item1 == EvalType.FUN)
				{
					// The primitive type is a function.

					// Append "Func" if the function has a return type, otherwise "Action".
					if (type.Item3 != null) sb.Append("Func");
					else sb.Append("Action");

					// If there are neither parameters neither the return type it jump this piece.
					if ((type.Item2 != null && type.Item2.Count > 0) || (type.Item3 != null))
					{
						sb.Append("<");

						bool first = true;
						foreach (EvalType t in type.Item2)
						{
							if (!first) sb.Append(",");
							// Compile the primitive type of all the parameters, they cannot be of function type. 
							sb.Append(Eval.EvalType_Compile(t));
							first = false;
						}

						// If the return type is present, compile it calling recursively this function.
						if (type.Item3 != null)
						{
							if (!first) sb.Append(",");
							Eval.ComileType2(sb, (Tuple<EvalType, List<EvalType>, Object>)type.Item3);
						}

						sb.Append(">");
					}
				}
				else
					// If it is not a function, it is sufficient to compile the primitive type.
					sb.Append(Eval.EvalType_Compile(type.Item1));
			}
			else
				// A variable with no type in C# has type "void".
				sb.Append("void");
		}
		#endregion

		#region TypeToString
		/// <summary>Convert a type into a <see cref="System.String" />.</summary>
		/// <param name="type">The type to convert into a <see cref="System.String" />.</param>
		/// <returns>A <see cref="System.String" />.</returns>
		public static string TypeToString(Tuple<EvalType, List<EvalType>, Object> type)
		{
			StringBuilder s = new StringBuilder();
			Eval.TypeToString2(s, type);
			return s.ToString();
		}
		#endregion

		#region CompileType
		/// <summary>Convert a type into a <see cref="System.String" /> compliant to the C# code.</summary>
		/// <param name="type">The type to compile.</param>
		/// <returns>A <see cref="System.String" />.</returns>
		public static string CompileType(Tuple<EvalType, List<EvalType>, Object> type)
		{
			StringBuilder s = new StringBuilder();
			Eval.ComileType2(s, type);
			return s.ToString();
		}
		#endregion

        #region ConvertType
		/// <summary>Covert a <see cref="TokenType"/> into an <see cref="EvalType"/>.</summary>
		/// <param name="t">The <see cref="TokenType"/> <paramref name="t"/> to convert.</param>
        /// <returns>The converted type.</returns>
		/// <exception cref="System.FunwapException">Thrown when it cannot do the conversion.</exception>
        public static EvalType ConvertType(TokenType t)
        {
            switch (t)
            {
                case TokenType.TYPEINT: return EvalType.INT;
                case TokenType.TYPEBOOL: return EvalType.BOOL;
                case TokenType.TYPECHAR: return EvalType.CHAR;
				case TokenType.TYPESTRING: return EvalType.STRING;
				case TokenType.TYPEURL: return EvalType.URL;
				case TokenType.TYPEFUN: return EvalType.FUN;
                default:
                    throw new System.FunwapException("ParseTreeException: failed conversion.");
            }
        }
        #endregion

		#region EqualTypes
		/// <summary>Check if the two types: <paramref name="type1"/> e <paramref name="type2"/> are equal.</summary>
		/// <param name="type1">The first type.</param>
		/// <param name="type2">The second type.</param>
		/// <returns><code>true</code> if they are equal, otherwise <code>false</code>.</returns>
		public static bool EqualTypes(Tuple<EvalType, List<EvalType>, Object> type1, Tuple<EvalType, List<EvalType>, Object> type2)
		{
			// If they are both null, they are equal.
			if (type1 == null && type2 == null) return true;

			// If one of them is null while the other is not, they are different. If the primitive type is different, they are different.
			if ((type1 == null && type2 != null)||(type1 != null && type2 == null)||(type1.Item1 != type2.Item1))
				return false;

			if (type1.Item1 == EvalType.FUN)
			{
				// The primitive type is a function.

				// If the Item2, rappresenting the parameters' types, is present only for one of them, they are different. 
				if ((type1.Item2 == null && type2.Item2 != null) || (type1.Item2 != null && type2.Item2 == null))
					return false;
				if (type1.Item2 != null && type2.Item2 != null)
				{
					// Item2 is different from null for both of them.

					// Trasform the parameters in array to simplify the comparison.
					EvalType[] par1 = type1.Item2.ToArray();
					EvalType[] par2 = type2.Item2.ToArray();

					// If the length of the two array is different, they expects a different number of parameters, so they are different.
					if (par1.Length != par2.Length)
						return false;

					// Check if all the parameters have equal type.
					for (int i = 0; i < par1.Length; i++)
						if (par1[i] != par2[i])
							return false;
				}
				// If we reach this point means that until now the two types are equal, it remains only to check if they have the same return type.
				// So the decision about if they are equal or not is entrusted calling recurively this function on the Item3 part of the two types.
				return Eval.EqualTypes((Tuple<EvalType, List<EvalType>, Object>)type1.Item3, (Tuple<EvalType, List<EvalType>, Object>)type2.Item3);
			}

			return true;
		}
		#endregion

		#endregion

		#region PUBLIC METHODS

		#region GetIValue
		/// <summary>If this instance has primitive type int, return an integer value, otherwise it throws an exception.</summary>
		/// <returns>An integer value.</returns>
		/// <exception cref="System.FunwapException">Thrown when it is not possible to return an integer value for this Eval object.</exception>
		public int GetIValue()
        {
            if (this.type.Item1 == EvalType.INT)
                return (int)this.value;
            else
                throw new System.FunwapException("ParseTreeException: GetValue cannot return an integer value for this Eval object.", this.Token);
        }
		#endregion

		#region GetBValue
		/// <summary>If this instance has primitive type bool, return an boolean value, otherwise it throws an exception.</summary>
		/// <returns>A boolean value.</returns>
		/// <exception cref="System.FunwapException">Thrown when it is not possible to return a boolean value for this Eval object.</exception>
        public bool GetBValue()
        {
            if (this.type.Item1 == EvalType.BOOL)
                return (bool)this.value;
            else
				throw new System.FunwapException("ParseTreeException: GetValue cannot return a boolean value for this Eval object.", this.Token);
        }
		#endregion

		#region GetCValue
		/// <summary>If this instance has primitive type char, return a character value, otherwise it throws an exception.</summary>
		/// <returns>A character value.</returns>
		/// <exception cref="System.FunwapException">Thrown when it is not possible to return a character value for this Eval object.</exception>
        public char GetCValue()
        {
            if (this.type.Item1 == EvalType.CHAR)
                return (char)this.value;
            else
				throw new System.FunwapException("ParseTreeException: GetValue cannot return a character value for this Eval object.", this.Token);
        }
		#endregion

		#region GetSValue
		/// <summary>If this instance has primitive type string, return a string value, otherwise it throws an exception.</summary>
		/// <returns>A string value.</returns>
		/// <exception cref="System.FunwapException">Thrown when it is not possible to return a string value for this Eval object.</exception>
		public string GetSValue()
		{
			if (this.type.Item1 == EvalType.STRING)
				return (string)this.value;
			else
				throw new System.FunwapException("ParseTreeException: GetValue cannot return a string value for this Eval object.", this.Token);
		}
		#endregion

		#region GetFValue
		/// <summary>If this instance has primitive type fun, return a functon value, otherwise it throws an exception.</summary>
		/// <returns>The pair (<see cref="FunctionNode"/>,  <see cref="Env"/>).</returns>
		/// <exception cref="System.FunwapException">Thrown when it is not possible to return a function value for this Eval object.</exception>
		public Tuple<FunctionNode, Env> GetFValue()
		{
			if (this.type.Item1 == EvalType.FUN)
				return (Tuple<FunctionNode, Env>)this.value;
			else
				throw new System.FunwapException("ParseTreeException: GetValue cannot return a function value for this Eval object.", this.Token);
		}
		#endregion

		#region GetUValue
		/// <summary>If this instance has primitive type url, return a url value, otherwise it throws an exception.</summary>
		/// <returns>A <see cref="Tuple"/> containing: the port on which the server channel is listening and the object URI.</returns>
		/// <exception cref="System.FunwapException">Thrown when it is not possible to return a url value for this Eval object.</exception>
		public Tuple<string, string> GetUValue()
		{
			if (this.type.Item1 == EvalType.URL)
				return (Tuple<string, string>)this.value;
			else
				throw new System.FunwapException("ParseTreeException: GetValue cannot return a url value for this Eval object.", this.Token);
		}
		#endregion

		#region ToString
		/// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
			switch (this.type.Item1)
			{
				case EvalType.FUN: return ((FunctionNode)this.value).GetTitle();
				case EvalType.URL:
					Tuple<string, string> url = this.GetUValue();
					return "tcp://localhost:" + url.Item1 + "/" + url.Item2;
				default:
					return this.value.ToString();

			}                
        }
		#endregion

		#region Clone
		/// <summary>Clones this instance.</summary>
		/// <returns>A <see cref="Eval"/> copy of this istance.</returns>
		public Eval Clone()
		{
			Eval v = new Eval(this.Token, this.type);
			// the value it is NOT cloned.
			v.value = this.value;
			return v;
		}
		#endregion

		#region GetValue
		/// <summary>Gets the value of this istance.</summary>
		/// <returns>The value of this istance.</returns>
		public object GetValue()
		{
			return this.value;
		}
		#endregion

		#region GetEvalType
		/// <summary>Gets the type of this istance.</summary>
		/// <returns>The type of this istance.</returns>
		public Tuple<EvalType, List<EvalType>, Object> GetEvalType()
        {
            return this.type;
		}
		#endregion

		#endregion
	}
}
