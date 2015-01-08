using System;
using System.Collections.Generic;
using Funwap.LexicalAnalysis;

namespace Funwap.Environment
{
	/// <summary>
	/// This class represents the Environment and it is a Dictionary of keys of type string,
	/// and values are pair: <see cref="Eval"/> value, boolean flag associated to the variable (used to distinguish the asyncronous variable from the syncronous ones).
	/// </summary>
    public class Env : Dictionary<string, Tuple<Eval, bool>>
    {
        #region PROPERTIES

		/// <summary>
		/// Gets or sets the parent Environment of this one.
		/// </summary>
        public Env Parent { get; set; }

        #endregion

        #region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="Env"/> class.
		/// </summary>
        public Env()
        {
            this.Parent = null;
        }
        #endregion

        #region PRIVATE METHODS

        #region RecursiveBind
		/// <summary>
		/// Binds the <paramref name="ide"/> with the <paramref name="value"/>.
		/// </summary>
		/// <param name="ide">The identifier.</param>
		/// <param name="value">The value to bind to the identifier.</param>
		/// <param name="declaration">In order to distinguish the type of binding to perform.</param>
		/// <returns><code>true</code> if it performs the binding; otherwise <code>false</code>.</returns>
		/// <exception cref="System.FunwapException">Thrown when the variable was already declared with a different type.</exception>
		private bool RecursiveBind(string ide, Tuple<Eval, bool> value, bool declaration)
        {
			// Search the key in this Environment.
            if (this.ContainsKey(ide))
            {
				// Check if the type of the the value found is the same as the the new value.
				if (!Eval.EqualTypes(this[ide].Item1.GetEvalType(), value.Item1.GetEvalType()))
					throw new System.FunwapException("EnvironmentException: \"" + this[ide].Item1.Token.Value + "\" is already declared with different type " + Eval.TypeToString(this[ide].Item1.GetEvalType()) + ".", value.Item1.Token);

				// If it is a declaration, we don't want to change the value found but we are going to add the bind to the local Environment.
				if (declaration) return false;
				else
				{
					// Since it is an assignment, we update the found value with the new value.
					this[ide] = value;
					return true;
				}
            }
            else
            {
				// We try to recursively search the key asking to the parent Environment.
                if (this.Parent != null)
                    return this.Parent.RecursiveBind(ide, value, declaration);
                else
                    return false;
            }
        }
        #endregion

        #endregion

        #region PUBLIC METHODS

		#region Clone
		/// <summary>Clones this instance.</summary>
		/// <returns>A <see cref="Env"/> copy of this istance.</returns>
		public Env Clone()
		{
			Env ret = new Env();
			// The Environment parent is NOT copied.
			ret.Parent = this.Parent;

			// For each KeyValuePair contained in this dictionary, add a copy of it to the ret Enviroment.
			foreach (KeyValuePair<string, Tuple<Eval, bool>> entry in this)
				ret.Add(entry.Key, new Tuple<Eval, bool>(entry.Value.Item1.Clone(), entry.Value.Item2));

			return ret;
		}
		#endregion

		#region Apply
		/// <summary>
		/// Searches in the Environment for identifier contained in the value of the Token <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The Token containing the identifier.</param>
		/// <returns>The value associated to this key.</returns>
		/// <exception cref="System.FunwapException">Thrown when it could not found the key in the Environment.</exception>
		public Tuple<Eval, bool> Apply(Token t)
        {
			// Search the key in the local Environment.
            if (this.ContainsKey(t.Value))
				return this[t.Value];
			// If it could not found it in the local, try to recursively search it asking to the parent Environment.
            else if (this.Parent != null)
                return this.Parent.Apply(t);
            else
				throw new System.FunwapException("EnvironmentException: cannot found \"" + t.Value + "\" in the Environment.", t);
        }
        #endregion

        #region Bind
		/// <summary>
		/// Binds the identifier with the value formed by the pair: (<paramref name="value"/>, <paramref name="async"/>).
		/// </summary>
		/// <param name="ide">The identifier.</param>
		/// <param name="value">The <see cref="Eval"/> value.</param>
		/// <param name="declaration">In order to distinguish the type of binding to perform.</param>
		/// <param name="async">The asynchronous flag associated to the variable.</param>
		public void Bind(string ide, Eval value, bool declaration, bool async = false)
        {
			// Try to Bind the identifier with the value using the RecursiveBind function
			bool bound = this.RecursiveBind(ide, new Tuple<Eval, bool>(value, async), declaration);

			// If RecursiveBind did not performed the binding, we do it locally.
			if (!bound)
				this.Add(ide, new Tuple<Eval, bool>(value, async));
        }
        #endregion

        #region ToString
		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<string, Tuple<Eval, bool>> entry in this)
                s += entry.Key + " → " + entry.Value.Item1.ToString() + ";\n";
            return s;
        }
        #endregion

		#endregion
	}
}
