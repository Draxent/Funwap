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
	#region BlockType
	/// <summary>
	/// It is an enumeration type that allows to distinguish the various Block.
	/// </summary>
	public enum BlockType
	{
		/// <summary>Used for the block of the entire program.</summary>
		PROGRAM_BLOCK,

		/// <summary>Used for the block of the main function.</summary>
		MAIN_BLOCK,

		/// <summary>Used in all the other cases.</summary>
		NORMAL
	};
	#endregion

	/// <summary>
	/// Denote a block node of the Abstract Syntax Tree.
	/// </summary>
    public class BlockNode : SyntacticNode
    {
        #region MEMBER VARIABLES

		/// <summary> Action to perform if it is a function block, in order to bind the formal parameters into its environment.</summary>
		public Action<Env> CallBinding = null;

        // A BlockNode has one child for each code instruction.
        private List<StatementNode> children = new List<StatementNode>();

		// Distinguishes the block type:
		// - program block -> in order to not destory the first Activation Record so that it is possible to retrieve the main function;
		// - main block -> in order to insert the statment readline at the end of the compilation
		private BlockType type;

        #endregion

        #region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="BlockNode" /> class.</summary>
		/// <param name="token">The <see cref="Token" /> containing the symbol "{".</param>
		/// <param name="title">The title of the Block.</param>
		/// <param name="t">The type <see cref="BlockType"/> of this Block.</param>
		public BlockNode(Token token, string title = "Block", BlockType t = BlockType.NORMAL) : base(token)
        {
			this.type = t;

            // Assign to the node's text the title
			SquareNode n = new SquareNode();
			n.AddText(title, KnownColor.Black, FontStyle.Bold);
			n.Margin = 2;
			n.DeleteBorderPen();
            this.GraphicNode = new GTree<GNode>(n);

            // Give a pointer to itself to the $GraphicNode object, in this way they can refer each others.
            this.GraphicNode.SyntacticNode = this;
        }

        #endregion

        #region PUBLIC METHODS

		#region AddChild
		/// <summary>Adds the child <paramref name="n"/> to this instance.</summary>
		/// <param name="n">The <see cref="StatementNode"/> to add to this instance.</param>
		public void AddChild(StatementNode n)
        {
			if (n != null)
			{
				this.children.Add(n);
				this.GraphicNode.AddChild(n.GraphicNode);
			}
        }
        #endregion

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		public override Eval Check(Stack EnvStack)
		{
			Eval value = null;

			// Create the new Activation Record at the begin of the block.
			this.NewActivationRecord(EnvStack);

			foreach (StatementNode child in this.children)
			{
				// Check the value of this child.
				value = child.Check(EnvStack);
				if ((value != null))
					break;
			}

			// Destroy the Activation Record at the end of the block.
			EnvStack.Pop();

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
			if (this.type == BlockType.PROGRAM_BLOCK)
			{
				sb.AppendLine("using System;");
				sb.AppendLine("using System.Threading;");
				sb.AppendLine("using System.Threading.Tasks;");
				sb.AppendLine("using System.Runtime.Remoting;");
				sb.AppendLine("using System.Runtime.Remoting.Channels;");
				sb.AppendLine("using System.Runtime.Remoting.Channels.Tcp;");
				sb.AppendLine();
				sb.AppendLine("class Program");
			}
			sb.AppendLine(SyntacticNode.Tab(tab) + "{");

			foreach (StatementNode child in this.children)
			{
				child.Compile(r, sb, tab + 1);
				sb.AppendLine();
			}

			if (this.type == BlockType.MAIN_BLOCK)
				sb.AppendLine(SyntacticNode.Tab(tab + 1) + "Console.ReadLine();");

			sb.Append(SyntacticNode.Tab(tab) + "}");
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval" /> value representing the valuation of the code contained in the tree of this node.</returns>
		public override Eval GetValue(Result r)
        {
			Eval value = null;

			// Create the new Activation Record at the begin of the block.
			this.NewActivationRecord(r.EnvStack);

			// Simply, the method calls GetValue for each of its children.
            foreach (StatementNode child in this.children)
            {
                // It takes the value of this child, but ignore it since we don't need it,
                // unless it is a ReturnNode, in this case we stop and return its value.
                value = child.GetValue(r);

                // If $value is different from null, it is surely a ReturnNode, so we return its value.
				if (value != null)
					break;
            }

			// Destroy the Activation Record at the end of the block, except for the program block.
			if (this.type != BlockType.PROGRAM_BLOCK)
				r.EnvStack.Pop();

			return value;
		}
		#endregion

		#endregion

		#region PRIVATE METHODS

		#region NewActivationRecord
		/// <summary>Create a new Activation Record.</summary>
		/// <param name="EnvStack">The Environment Stack.</param>
		private void NewActivationRecord(Stack EnvStack)
		{
			// Create a new environment, that is updated for each code instruction executed, and it is initially empty
			Env env = new Env();

			// Link the environment of this node with its parent
			try { env.Parent = (Env)EnvStack.Peek(); }
			catch (InvalidOperationException) { env.Parent = null; }

			// If the Action CallBinding was been defined, means that it is a block of a function and has to bind into its enviroment the formal parameters
			if (this.CallBinding != null)
				this.CallBinding(env);

			// Create a new Activation Record
			EnvStack.Push(env);
		}
		#endregion

		#endregion
	}
}
