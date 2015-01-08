using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using Funwap.LexicalAnalysis;
using Funwap.AbstractSyntaxTree;
using Funwap.Environment;

namespace Funwap
{
	/// <summary>
	/// The Window that manages the results of the interpretation or of the compilation of the code.
	/// </summary>
	public partial class Result : Form
	{
		#region VARIABLES

		/// <summary>The caller <see cref="IDE"/> of this instance.</summary>
		public IDE Caller = null;

		// The Token of the "Main" function.
		private Token tokenMain = null;

		// The root of the Abstract Syntax Tree.
		private BlockNode root = null;

		/// <summary>The Enviroment Stack.</summary>
		public Stack EnvStack = new Stack();

		// Used to store the output of the 
		private StringBuilder exeOutput;

		// Regular expression used to match Compiled errors
		private Regex errorRegex;

		/// <summary>If it has found a C# compiler of version 4.</summary>
		public bool CanCompile = true;

		// The directory where the C# compiler was found.
		private string compileDir = null;

		/// <summary>The list of references to add in the C# compiler options while compiling the C# file.</summary>
		public List<string> References = new List<string>();

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Result"/> class.
		/// </summary>
		/// <param name="caller">The caller <see cref="IDE"/> of this instance.</param>
		/// <param name="main">The Token of the "Main" function.</param>
		/// <param name="root">The root of the  Abstract Syntax Tree.</param>
		public Result(IDE caller, Token main, BlockNode root)
		{
			InitializeComponent();

			this.Caller = caller;
			this.tokenMain = main;
			this.root = root;

			errorRegex = new Regex(@"error CS\d*: (.*)\n");

			// Search for the C# compiler of version 4 since it is the only one that allowed the "Task" command of C#.
			// Therfore set CanCompile at true only if it finds it.
			string[] dirs = Directory.GetDirectories(@"c:\windows\Microsoft.NET\Framework", "v4*");
			if (dirs.Length > 0){ compileDir = dirs[0]; CanCompile = true; }
			else { compileDir = null; CanCompile = false; }
		}

		/// <summary>
		/// Inteprets the code using the Abstract Syntax Tree.
		/// </summary>
		public void Intepret()
		{
			try {
				// Check the code
				this.root.Check(EnvStack); 

				// Execute the program
				this.root.GetValue(this);

				// Look at the first Activation Record
				Env env = (Env)this.EnvStack.Peek();

				// Search in the environment the main() function
				Eval main = env.Apply(this.tokenMain).Item1;

				// Call the main
				main.GetFValue().Item1.Call(this, env, new List<Eval>());

				Caller.Console.AppendText("Code interpreted with success.\r\n");
			}
			catch (System.FunwapException ex)
			{
				Caller.TokenError(ex.Message, ex.Token);
				this.Close();
			}
		}

		/// <summary>
		/// Compile the code using the Abstract Syntax Tree.
		/// </summary>
		public void Compile(string nameFile)
		{
			try
			{
				StringBuilder sb = new StringBuilder();

				// Check the code
				this.root.Check(EnvStack);

				// Execute the program
				this.root.Compile(this, sb, 0);

				string dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
				using (StreamWriter outfile = new StreamWriter(dir + "\\" + nameFile + ".cs"))
				{
					outfile.Write(sb.ToString());
				}
				Caller.Console.AppendText("Code translated with success into C# file \"" + dir + "\\" + nameFile + ".cs\"\r\n");

				if (this.CanCompile)
					this.LaunchCompileExe(nameFile, false, true);
			}
			catch (System.FunwapException ex)
			{
				Caller.TokenError(ex.Message, ex.Token);
				this.Close();
			}
		}


		/// <summary>
		/// Call the <see cref="Stdin"/> windows and gets its result, that is the User input.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that rappresent the User input.</returns>
		public string GetStdin()
		{
			Stdin stdin = new Stdin();
			string s = "";

			// Show Stdin as a modal dialog and determine if DialogResult == OK.
			if (stdin.ShowDialog(this) == DialogResult.OK)
				// Read the contents of Stdin's TextBox.
				s = stdin.Input.Text;
			stdin.Dispose();

			return s;
		}

		/// <summary>
		/// Handles the KeyPress event of the Stdout control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
		private void Stdout_KeyPress(object sender, KeyPressEventArgs e)
		{
			this.Close();
		}


		/// <summary>
		/// Manages all the output of the executable process launched.
		/// </summary>
		/// <param name="sendingProcess">The executable process launched.</param>
		/// <param name="outLine">The <see cref="DataReceivedEventArgs"/> instance containing the event data.</param>
		private void ExeOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			// Collect the exe command output.
			if (!String.IsNullOrEmpty(outLine.Data))
				// Add the text to the collected output.
				exeOutput.AppendLine(outLine.Data);
		}

		/// <summary>
		/// Launch the C# compiler in order to trasform the C# code into an executable file
		/// or a dll file, depending on the <paramref name="library"/> value.
		/// </summary>
		/// <param name="nameFile">The name of the file containig C# code.</param>
		/// <param name="library">if set to <c>true</c> we want to create a library.</param>
		/// <param name="needRef">If set to <c>true</c> the file needs the references to the libraries.</param>
		public void LaunchCompileExe(string nameFile, bool library, bool needRef)
		{
			string workingDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			// All the information about the process to execute.
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WorkingDirectory = workingDirectory;
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			startInfo.FileName = compileDir + "\\csc.exe";
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;

			if (library)
				startInfo.Arguments = "/t:library " + nameFile + ".cs";
			else
			{
				// If the file needs some reference, build them properly.
				string reference = "";
				if (needRef)
				{
					reference += "/lib:" + workingDirectory + " ";
					foreach (string r in this.References)
						reference += "/r:" + r + ".dll ";
				}
				startInfo.Arguments = reference + " /t:exe /out:" + nameFile + ".exe " + nameFile + ".cs";
			}
			startInfo.RedirectStandardOutput = true;

			try
			{
				// Starts the process with the info we specified.
				Process exeProcess = Process.Start(startInfo);

				// Stores all the output of the process in the exeOutput variable.
				exeOutput = new StringBuilder();
				exeProcess.OutputDataReceived += new DataReceivedEventHandler(ExeOutputHandler);

				// Starts the asynchronous read of the exe output stream.
				exeProcess.BeginOutputReadLine();

				// Waits the end of the process.
				exeProcess.WaitForExit();
				
				// Matches the possible errors genereted by the process.
				MatchCollection m = errorRegex.Matches(exeOutput.ToString());

				// If it finds some errors, print them on the Console.
				if (m.Count > 0)
				{
					Caller.Console.AppendText("LaunchCompileEXE:\r\n");
					foreach (Match match in m)
						Caller.Console.AppendText("\t- " + match.Groups[1].Value + ";\r\n");
					exeProcess.Close();
					this.Close();
				}
				else
				{
					Caller.Console.AppendText("Code compiled with success into executable file \"" + workingDirectory + "\\" + nameFile + ".exe\"\r\n");
					exeProcess.Close();
					this.Close();
				}
			}
			catch (System.Exception ex)
			{
				Caller.Console.AppendText("LaunchCompileEXE: ");
				Caller.Console.AppendText(ex.Message);
				this.Close();
			}
		}
	}
}
