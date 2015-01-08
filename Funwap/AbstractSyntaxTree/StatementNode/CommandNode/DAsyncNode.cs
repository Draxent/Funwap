using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Drawing;
using Funwap.LexicalAnalysis;
using Funwap.GraphicTree;
using Funwap.Environment;

namespace Funwap.AbstractSyntaxTree
{
	/// <summary>
	/// Denote a asynchronous distributed operation node of the Abstract Syntax Tree.
	/// </summary>
	class DAsyncNode : CommandNode
	{
		#region MEMBER VARIABLES

		// The Token of the variable involved into the asynchronous operation.
		private Token ide;
		// The url of the remote server.
		private VarNode urlNode;
		// The remote function to call.
		private CallNode callNode;
		// The primitive value of the variable that will take the input result, useful in the compilation phase.
		private EvalType varType;
		// The value of the url, useful in the compilation phase.
		private Eval url;

		#endregion

		#region CONSTRUCTOR

		/// <summary>Initializes a new instance of the <see cref="DAsyncNode" /> class.</summary>
		/// <param name="token">The <see cref="Token"/> containing the DAsync statement.</param>
		/// <param name="ide">The <see cref="Token"/> of the variable involved into the asynchronous operation.</param>
		/// <param name="urlNode">The <see cref="VarNode"/> containing the URL associated to this distributed operation.</param>
		/// <param name="callNode">The remote function to call.</param>
		public DAsyncNode(Token token, Token ide, VarNode urlNode, CallNode callNode) : base(token, "DAsync")
		{
			this.ide = ide;
			this.urlNode = urlNode;
			this.callNode = callNode;

			// Add to the $GraphicNode the two children.
			this.GraphicNode.AddChild(urlNode.GraphicNode);
			this.GraphicNode.AddChild(callNode.GraphicNode);
		}

		#endregion

		#region PUBLIC METHODS

		#region Check
		/// <summary>It is a method that perform the Type and Enviroment Checking.</summary>
		/// <param name="EnvStack">The Enviroment Stack.</param>
		/// <returns>An <see cref="Eval"/> value that can be used by the parent node.</returns>
		public override Eval Check(Stack EnvStack)
		{
			Env env = (Env)EnvStack.Peek();
			// Search in the enviroment for the type of the variable that will take the input result.
			this.varType = env.Apply(this.ide).Item1.GetEvalType().Item1;

			// Check the VarNode of the url and see if it has URL type.
			this.url = this.urlNode.Check(EnvStack);
			if (this.url.GetEvalType().Item1 != EvalType.URL)
				throw new System.FunwapException("ParseTreeException: the variable \"" + url.Token.Value + "\" should be of url type.", this.Token);

			// Check the CallNode and return its value, in this way the AssignNode can check if the assignment is correct.
			return this.callNode.Check(EnvStack);
		}
		#endregion

		#region Compile
		/// <summary>It is a method to compile the code contained in the tree of this node.</summary>
		/// <param name="r">The form window used only by the <see cref="DAsyncNode"/>.</param>
		/// <param name="sb">The <see cref="StringBuilder"/> used to form the string.</param>
		/// <param name="tab">The number of tabulation we want to add to each row generated with the aim of generating a code indented properly.</param>
		public override void Compile(Result r, StringBuilder sb, int tab)
		{
			// Initialize the variable to a default value.
			switch (this.varType)
			{
				case EvalType.INT: sb.AppendLine("0;"); break;
				case EvalType.BOOL: sb.AppendLine("false;"); break;
				case EvalType.CHAR: sb.AppendLine("'0';"); break;
				case EvalType.STRING: sb.AppendLine("\"\";"); break;
				case EvalType.URL: sb.AppendLine("\"\";"); break;
				default:
					throw new System.FunwapException("ParseTreeException: DAsyncNode does not support a " + Eval.EvalType_ToString(this.varType) + " type variable.", this.Token);
			}

			// Create a new Task that will assign the new value, obtained calling the remote function, to the variable. 
			string taskIde = "Task_" + this.ide.Value;
			string remoteClass = this.callNode.Token.Value.ToUpper();
			string serverName = remoteClass + "_Server";
			string remoteInterface = remoteClass + "_Interface";
			string rtypeRemoteFun = Eval.EvalType_Compile(this.varType);
			sb.AppendLine(SyntacticNode.Tab(tab) + "Task " + taskIde + " = new Task(");
			sb.AppendLine(SyntacticNode.Tab(tab + 1) + "delegate()");
			sb.AppendLine(SyntacticNode.Tab(tab + 1) + "{");
			sb.AppendLine(SyntacticNode.Tab(tab + 2) + "try");
			sb.AppendLine(SyntacticNode.Tab(tab + 2) + "{");
			sb.AppendLine(SyntacticNode.Tab(tab + 3) + "ChannelServices.RegisterChannel(new TcpChannel(), false);");
			sb.AppendLine(SyntacticNode.Tab(tab + 3) + remoteInterface + " remoteObject = (" + remoteInterface + ")Activator.GetObject(typeof(" + remoteInterface + "), " + this.urlNode.Token.Value + ");");
			sb.Append(SyntacticNode.Tab(tab + 3) + this.ide.Value + " = remoteObject.");
			this.callNode.Compile(r, sb, 0);
			sb.AppendLine(";");
			sb.AppendLine(SyntacticNode.Tab(tab + 2) + "}");
			sb.AppendLine(SyntacticNode.Tab(tab + 2) + "catch (RemotingException) { Console.WriteLine(\"The channel has already been registered.\"); }");
			sb.AppendLine(SyntacticNode.Tab(tab + 2) + "catch (System.Net.Sockets.SocketException) { Console.WriteLine(\"No connection could be made because the target machine actively refused it.\"); }");
			sb.AppendLine(SyntacticNode.Tab(tab + 1) + "}");
			sb.AppendLine(SyntacticNode.Tab(tab) + ");");

			// Start the new Task.
			sb.Append(SyntacticNode.Tab(tab) + taskIde + ".Start()");

			// Create the Server file that will serve the clients asking for the remote function.
			Tuple<string, string> url = this.url.GetUValue();
			Tuple<string, string, int> code = ExtractCode(sb, this.callNode.Token.Value);
			StringBuilder sb2 = new StringBuilder();
			sb2.AppendLine("using System;");
			sb2.AppendLine("using System.Threading;");
			sb2.AppendLine("using System.Threading.Tasks;");
			sb2.AppendLine("using System.Runtime.Remoting;");
			sb2.AppendLine("using System.Runtime.Remoting.Channels;");
			sb2.AppendLine("using System.Runtime.Remoting.Channels.Tcp;");
			sb2.AppendLine();
			sb2.AppendLine("public interface " + remoteInterface);
			sb2.AppendLine("{");
			sb2.AppendLine("\t" + rtypeRemoteFun + " " + code.Item1 + ";");
			sb2.AppendLine("}");
			sb2.AppendLine();

			sb2.AppendLine("public class " + remoteClass + " : MarshalByRefObject, " + remoteInterface);
			sb2.AppendLine("{");
			sb2.AppendLine(code.Item2);
			sb2.AppendLine("}");
			sb2.AppendLine();

			sb2.AppendLine("class Program");
			sb2.AppendLine("{");
			sb2.AppendLine("\t" + "static void Main()");
			sb2.AppendLine("\t" + "{");
			sb2.AppendLine("\t\t" + "try");
			sb2.AppendLine("\t\t" + "{");
			sb2.AppendLine("\t\t\t" + "ChannelServices.RegisterChannel(new TcpChannel(" + url.Item1 + "), false);");
			sb2.AppendLine("\t\t\t" + "RemotingConfiguration.RegisterWellKnownServiceType(Type.GetType(\"" + remoteClass + "\"), \"" + url.Item2 + "\", WellKnownObjectMode.SingleCall);");
			sb2.AppendLine("\t\t\t" + "Console.WriteLine(\"" + remoteClass + " Server started...\");");
			sb2.AppendLine("\t\t\t" + "Console.WriteLine(\"Press ENTER to quit.\");");
			sb2.AppendLine("\t\t" + "}");
			sb2.AppendLine("\t\t" + "catch (RemotingException) { Console.WriteLine(\"The channel has already been registered.\"); }");
			sb2.AppendLine("\t\t" + "catch (System.Net.Sockets.SocketException) { Console.WriteLine(\"Socket address is already been used.\"); }");
			sb2.AppendLine("\t\t" + "Console.ReadLine();");
			sb2.AppendLine("\t" + "}");
			sb2.AppendLine("}");

			// Write the code of the Server file in the Desktop folder.
			string dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			using (StreamWriter outfile = new StreamWriter(dir + "\\" + serverName + ".cs"))
			{
				outfile.Write(sb2.ToString());
			}
			r.Caller.Console.AppendText("Code translated with success into C# file \"" + dir + "\\" + serverName + ".cs\"\r\n");

			if (r.CanCompile)
			{
				// Add the reference to this file.
				r.References.Add(serverName);
				// Compile the Server file, genereting the .dll and the executable files.
				r.LaunchCompileExe(serverName, true, false);
				r.LaunchCompileExe(serverName, false, false);
				// Insert the library of the Server.
				sb.Insert(code.Item3, "using " + serverName + ";\r\n");
			}
		}
		#endregion

		#region GetValue
		/// <summary>It is a method to return the value of the node exploring the tree under it.</summary>
		/// <param name="r">The form window used for Stdin and Stdout.</param>
		/// <returns>An <see cref="Eval" /> value representing the valuation of the code contained in the tree of this node.</returns>
		/// <exception cref="System.FunwapException">Thrown in all the cases, since it is not possible to interpet a code containing the DAsync statement.</exception>
		public override Eval GetValue(Result r)
		{
			// We have not implemented yet the interpetation of the DAsync statement.
			throw new System.FunwapException("ParseTreeException: cannot interpret a code containing the DAsync statement.");
		}
		#endregion

		#endregion

		#region ExtractCode
		private static Tuple<string, string, int> ExtractCode(StringBuilder sb, string nameFun)
		{
			Regex r = new Regex(nameFun + @"\(.*\)");
			string code = sb.ToString();
			int start = code.IndexOf("\tstatic");
			int end = code.LastIndexOf("static");
			string seqCode = code.Substring(start, end - start).Replace("static", "public");
			Match m = r.Match(seqCode);
			int posLib = code.IndexOf("\r\nclass Program");
			return new Tuple<string, string, int>(m.Value, seqCode, posLib);
		}
		#endregion
	}
}
