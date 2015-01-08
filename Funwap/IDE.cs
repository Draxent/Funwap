﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;
using Funwap.AbstractSyntaxTree;
using Funwap.SyntacticAnalysis;

namespace Funwap
{
    /// <summary>
	/// The Window that rappresent the IDE (Integrated development environment).
    /// </summary>
    public partial class IDE : Form
    {
        #region VARIABLES

		// The root of the  Abstract Syntax Tree.
		BlockNode root = null;

		// The Token of the "Main" function.
		Token main = null;

		// Collection of Tokens generated by the Scanner phase, used by the Colorize function.
		IEnumerable<Token> tokens = null;

		// List of Tokens obtained trasforming properly $tokens variable, used in the Parser phase.
		List<Token> tokenlist = null;

		// The Scanner object.
		Scanner scanner = null;

		// The Parser object.
		Parser parser = null;

		// Saves the previous state of the code: editable or uneditable.
		bool editable = true;

		// The full path of the opened or saved file.
		string fullNameFile = null;

		// The name of the opened or saved file.
		string nameFile = null;

		// Saves the position and the length of the last error in order to remove the graphical signaling in red.
		System.Tuple<int, int> lastError = null;

        #endregion

		#region CONSTRUCTOR
		/// <summary>
        /// Initializes a new instance of the <see cref="IDE"/> class.
        /// </summary>
        public IDE()
        {
            InitializeComponent();
			Editor.AcceptsTab = true;
        }
        #endregion

		#region PUBLIC METHODS

		#region TokenError
		/// <summary>Manage the graphical behavior when an error is generated.</summary>
		/// <param name="msg">The errror message.</param>
		/// <param name="t">The <see cref="Token"/> that generate the error.</param>
		public void TokenError(string msg, Token t)
		{
			// Write the error message on the Console.
			Console.AppendText(msg + "\r\n");

			// Underline the token that generated the error if it was specified.
			if (t != null)
			{
				lastError = new System.Tuple<int, int>(t.Index, t.Length);
				Editor.Select(t.Index, t.Length);
				Editor.SelectionBackColor = Color.Red;
				Editor.DeselectAll();
			}
		}
		#endregion

		#endregion

		#region PRIVATE METHODS

		#region EditCode
		/// <summary>Perform all the operation to make the code editable or not.</summary>
		/// <param name="codeEditable">If set to <c>true</c> make the code editable, if set to <c>false</c> make the code uneditable.</param>
		private void EditCode(bool codeEditable)
		{
			// If the previous state of the code did not change we do not need to perform the operations below, unless the Scanner is null.
			if (editable == codeEditable)
			{
				if (codeEditable == true) return;
				else if (scanner != null) return;
			}

			if (codeEditable)
			{
				// We want to edit the code, so we change the ReadOnly property of the Editor and we reset the color of the code.
				Editor.ReadOnly = false;
				Editor.SelectAll();
				Editor.SelectionBackColor = Color.White;
				Editor.SelectionColor = Color.Black;
				using (Font normalFont = new Font("Courier New", 12f))
				{
					Editor.SelectionFont = normalFont;
				}
				Editor.Select(0, 1);
				Editor.DeselectAll();
				Console.AppendText("Now, the Editor is editable!\r\n");
				editable = true;
			}
			else
			{
				// We are satisfied with the edits done to the code.
				// So we Scanner the new code (if it was not already done) and colorize the code.
				Editor.ReadOnly = true;
				Editor.Select(0, 1);
				Editor.DeselectAll();
				if (scanner == null)
					ScannerPhase();
				Colorize(tokens);
				Console.AppendText("The Editor is not editable anymore!\r\n");
				editable = false;
			}
		}
		#endregion

		#region ScannerPhase
		/// <summary>All the action to perform during the Scanners phase.</summary>
		private void ScannerPhase()
		{
			// Remove from the text all '\r' character
			Editor.Text = Editor.Text.Replace("\r", "");

			// Initialze the Scanner
			scanner = new Scanner();
			scanner.Initialize(Editor.Text);

			try
			{
				// Create the collection of Token that can be enumerated.
				tokens = scanner.Tokenize();

				// Trasform the token enumerator in list and remove all the comments
				tokenlist = new List<Token>();
				foreach (var token in tokens)
					if (token.Type != TokenType.COMMENT)
						tokenlist.Add(token);

				Console.AppendText("Scanner Phase compleated!\r\n");
			}
			catch (System.FunwapException ex)
			{
				scanner = null;
				tokens = null;
				tokenlist = null;
				lastError = null;
				TokenError(ex.Message, ex.Token);
			}
		}
		#endregion

		#region RemoveTokenError
		/// <summary>Removes the graphical signaling in red of the last error.</summary>
		private void RemoveTokenError()
		{
			// Removes last token error highlight
			if (lastError != null)
			{
				Editor.Select(lastError.Item1, lastError.Item2);
				Editor.SelectionBackColor = Color.White;
				Editor.SelectionColor = Color.Black;
				Editor.Select(0, 1);
				Editor.DeselectAll();
			}
		}
		#endregion

		#region Colorize
		/// <summary>
		/// Color the code depending on the type of token.
		/// </summary>
		/// <param name="tokens">List of tokens.</param>
		private void Colorize(IEnumerable<Token> tokens)
		{
			if (tokens == null) return;

			using (Font normalFont = new Font("Courier New", 12f),
						boldFont = new Font("Courier New", 12f, FontStyle.Bold),
						italicFont = new Font("Courier New", 12f, FontStyle.Italic),
						urlFont = new Font("Courier New", 12f, FontStyle.Italic | FontStyle.Underline))
			{
				foreach (var token in tokens)
				{
					Editor.Select(token.Index, token.Length);
					Editor.SelectionBackColor = Color.White;
					switch (token.Type)
					{
						case TokenType.TYPEBOOL: case TokenType.TYPECHAR: case TokenType.TYPEFUN:
						case TokenType.TYPEINT: case TokenType.TYPEURL: case TokenType.TYPESTRING:
							Editor.SelectionColor = Color.Indigo;
							Editor.SelectionFont = italicFont;
							break;
						case TokenType.MAIN:
							Editor.SelectionColor = Color.DarkMagenta;
							Editor.SelectionFont = boldFont;
							break;
						case TokenType.DECLVAR: case TokenType.DECLFUNC:
							Editor.SelectionColor = Color.Black;
							Editor.SelectionFont = boldFont;
							break;
						case TokenType.COMMENT:
							Editor.SelectionColor = Color.Green;
							Editor.SelectionFont = normalFont;
							break;
						case TokenType.URL:
							Editor.SelectionColor = Color.Blue;
							Editor.SelectionFont = urlFont;
							break;
						case TokenType.NUMBER: case TokenType.TRUE: case TokenType.FALSE:
							Editor.SelectionColor = Color.Orange;
							Editor.SelectionFont = boldFont;
							break;
						case TokenType.CHAR: case TokenType.STRING:
							Editor.SelectionColor = Color.DarkRed;
							Editor.SelectionFont = normalFont;
							break;
						case TokenType.ASYNC: case TokenType.DASYNC: case TokenType.ELSE: case TokenType.FOR:
						case TokenType.IF: case TokenType.PRINTLN: case TokenType.READLN: case TokenType.RETURN: case TokenType.WHILE:
							Editor.SelectionColor = Color.Blue;
							Editor.SelectionFont = boldFont;
							break;
						default:
							if ((token.Type != TokenType.IDE) && (token.Type != TokenType.EOF))
							{
								Editor.SelectionColor = Color.DarkBlue;
								Editor.SelectionFont = boldFont;
							}
							else
							{
								Editor.SelectionColor = Color.Black;
								Editor.SelectionFont = normalFont;
							}
							break;
					}
					Editor.DeselectAll();
				}
			}

			// Brings the cursor to the top of the Editor area
			Editor.Select(0, 1);
			Editor.DeselectAll();
		}
		#endregion

		#region ParserPhase
		/// <summary>All the action to perform during the Parser phase.</summary>
		private void ParserPhase()
		{
			if ((tokenlist != null) && (parser == null))
			{
				try
				{
					// Initialize the Parser.
					parser = new Parser(tokenlist);

					// Create the Abstract Syntax Tree.
					System.Tuple<Token, BlockNode> pair = parser.ParseTree();
					main = pair.Item1;
					root = pair.Item2;

					Console.AppendText("Parser Phase compleated!\r\n");
				}
				catch (System.FunwapException ex) { TokenError(ex.Message, ex.Token); }
			}
		}
		#endregion

		#region MenuItem_OpenFile_Click
		/// <summary>
        /// Handles the Click event of the MenuItem_OpenFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MenuItem_OpenFile_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            // If the file is selected correctly.
			if ((ofd.ShowDialog() == DialogResult.OK) && (ofd.FileName != fullNameFile))
            {
				// Read the entire file and write its content on the Editor.
				Editor.Text = System.IO.File.ReadAllText(ofd.FileName);
				fullNameFile = ofd.FileName;

				// Get the local file name
				string[] tmp = ofd.FileName.Split(new char[2] { '\\', '.' });
				nameFile = tmp[tmp.Length - 2];

				// Reset Scanner & Parser
				scanner = null;
				parser = null;
				root = null;
				lastError = null;

				Console.AppendText("The file " + ofd.FileName + " has been loaded correctly!\r\n");
				EditCode(false);
            }
        }
        #endregion

		#region MenuItem_SaveFile_Click
		private void MenuItem_SaveFile_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Text File | *.txt";
			sfd.FileName = nameFile + ".txt";

			// If the file is selected correctly.
			if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(sfd.OpenFile());
				fullNameFile = sfd.FileName;
				writer.Write(Editor.Text);
                writer.Dispose();
                writer.Close();

				// Get the local file name
				string[] tmp = sfd.FileName.Split(new char[2] { '\\', '.' });
				nameFile = tmp[tmp.Length - 2];

				Console.AppendText("The code has been saved in the file " + sfd.FileName + "!\r\n");
				EditCode(false);
            }
		}
		#endregion

		#region MenuItem_EditFile_Click
		private void MenuItem_EditFile_Click(object sender, System.EventArgs e)
		{
			EditCode(true);
		}
		#endregion

		#region MenuItem_Exit_Click
		/// <summary>
        /// Handles the Click event of the MenuItem_Exit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MenuItem_Exit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region MenuItem_Help_Click
        /// <summary>
        /// Handles the Click event of the MenuItem_Help control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MenuItem_Help_Click(object sender, System.EventArgs e)
        {
            string text = "";
            text += "In order to have your code file interpetered or compiled, do the following steps:\n";
            text +=	"  1) Click on File -> Open File.\n";
			text += "  2) Select a text file with the same format of the files that\n";
			text += "     you can find in the Examples folder.\n";
			text += "  3) Click on Execute -> Compile, if you want to compile the code\n";
			text +=	"     or click on Execute -> Interpret, if you want to interpret it.\n\n";
			text += "For more information, read the file documentation.pdf!";
            MessageBox.Show(text, "Help");
        }
        #endregion

        #region MenuItem_About_Click
        /// <summary>
        /// Handles the Click event of the MenuItem_About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MenuItem_About_Click(object sender, System.EventArgs e)
        {
            string text = "";
            text += "Program written by Federico Conte.\n";
            text += "Info: fconte90@gmail.com\n";
            text += "Date: 07/01/2015 \tRelease 1.0";
            MessageBox.Show(text, "About");
        }
        #endregion

		#region MenuItem_ShowAST_Click
		/// <summary>
		/// Handles the Click event of the MenuItem_ShowAST control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MenuItem_ShowAST_Click(object sender, System.EventArgs e)
		{
			// We need to perfrom the Scanner and the Parser phase before generating the Abstract Syntax Tree.
			EditCode(false);
			ParserPhase();
			// If the root is null, means that the previous steps did not have success.
			if (root != null)
			{
				AST ast = new AST(root.GraphicNode);
				ast.Show();
			}
			else
			{
				// Write an error on the Console if it could not generate the AST
				Console.AppendText("Impossible generate the AST since there are errors in the code!\r\n");
			}
		}
		#endregion

		#region MenuItem_Compile_Click
		private void MenuItem_Compile_Click(object sender, System.EventArgs e)
		{
			// Remove the last error, since we are compiling the code again. 
			RemoveTokenError();
			// We need to perfrom the Scanner and the Parser phase before generating the Abstract Syntax Tree.
			EditCode(false);
			ParserPhase();
			// If the root is null, means that the previous steps did not have success.
			if (root != null)
			{
				Result res = new Result(this, main, root);
				res.Show();
				res.Compile(nameFile);
			}
			else
			{
				// Write an error on the Console if it could not compile the code
				Console.AppendText("Impossible compile the code since there are errors in it!\r\n");
			}
		}
		#endregion

		#region MenuItem_Interpret_Click
		private void MenuItem_Interpret_Click(object sender, System.EventArgs e)
		{
			// Remove the last error, since we are interpreting the code again. 
			RemoveTokenError();
			// We need to perfrom the Scanner and the Parser phase before generating the Abstract Syntax Tree.
			EditCode(false);
			ParserPhase();
			// If the root is null, means that the previous steps did not have success.
			if (root != null)
			{
				Result res = new Result(this, main, root);
				res.Show();
				res.Intepret();
			}
			else
			{
				// Write an error on the Console if it could not interpret the code
				Console.AppendText("Impossible interpret the code since there are errors in it!\r\n");
			}
		}
		#endregion

		#region Editor_KeyDown
		private void Editor_KeyDown(object sender, KeyEventArgs e)
		{
			// Reset Scanner & Parser, since we are changing the code. 
			scanner = null;
			parser = null;
			root = null;
			lastError = null;
		}
		#endregion

		#endregion

	}
}
