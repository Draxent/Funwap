using System;
using System.Collections.Generic;
using Funwap.Environment;
using Funwap.LexicalAnalysis;
using Funwap.AbstractSyntaxTree;

namespace Funwap.SyntacticAnalysis
{
    /// <summary>
    /// It is the class performing the syntactic analysis,
    /// that is the process of analysing a string according to the rules of a formal grammar.
    /// </summary>
    public class Parser
    {
        #region MEMBER VARIABLES

        // List of tokens
		private List<Token> tokens;
		private int index = 0;

		// Current token
		private Token current;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
		/// <param name="tokenlist">The list of tokens to be parsed.</param>
		public Parser(List<Token> tokenlist)
        {
			this.tokens = tokenlist;

			// Set current on the first token.
			this.current = this.tokens[this.index];
        }

        #endregion

        #region PRIVATE METHODS

		#region MoveNext
		/// <summary>
		/// Set the variable current to the next Token to analyze.
		/// </summary>
		private void MoveNext()
		{
			this.index++;
			this.current = this.tokens[this.index];
		}
		#endregion

        #region MatchToken
        /// <summary>
		/// Try to see if the token found has the expected type <paramref name="t" /> we wanted. In the positive case, it move to the next Token.
        /// </summary>
        /// <param name="t">TokenType we expect at this point.</param>
        /// <returns>The token if it is matched; otherwise thrown the exception</returns>
		/// <exception cref="System.FunwapException">Thrown when the token found has not expected type we wanted.</exception>
        private Token MatchToken(TokenType t)
        {
            if (this.current.Type == t)
            {
				Token tmp = current;
                MoveNext();
                return tmp;
            }
            else
                throw new System.FunwapException("ParserException: " + this.current + " found has not expected type " + t + ".", this.current);
        }
        #endregion

		#region LookAhead
		/// <summary>
		/// Look ahead of <paramref name="p" /> position in the tokenlist in order to see if the token 
		/// has the expected type <paramref name="t" /> we wanted.
		/// </summary>
		/// <param name="p">How much look ahead.</param>
		/// <param name="t">TokenType we expect at this point.</param>
		/// <returns><c>true</c> if the token is matched; otherwise, <c>false</c>.</returns>
		private bool LookAhead(int p, TokenType t)
		{
			if (((this.index + p) < this.tokens.Count) && (this.tokens[this.index + p].Type == t))
				return true;
			else
				return false;
		}
		#endregion

		#region GetMatchType
		/// <summary>
		/// Try to see if the token found is a type Token, as  "int", "bool", "char"...;  in the positive case, it move to the next Token.
		/// </summary>
		/// <returns>The EvalType found if it is matched; otherwise thrown the exception.</returns>
		/// <exception cref="System.FunwapException">Thrown when the token found has not expected type we wanted.</exception>
		private EvalType GetMatchType()
		{
			// Match the type token, as "int", "bool", "char" ...
			if (!current.ItIsType())
				throw new System.FunwapException("ParserException: " + this.current + " it is not a type token.", this.current);

			// Save the type of the identifier
			EvalType type = Eval.ConvertType(current.Type);
			MoveNext();
			return type;
		}
		#endregion

		#region Main
		/// <summary>
		/// Parse the Main → func Main ( ) Block
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <returns>The "Main" Token found.</returns>
		private Token Main(BlockNode n)
		{
			MatchToken(TokenType.DECLFUNC);
			Token t = MatchToken(TokenType.MAIN);
			MatchToken(TokenType.ROUNDBR_OPEN);
			MatchToken(TokenType.ROUNDBR_CLOSE);

			// Construct the expected tree derived from Block
			BlockNode treeBlock = Block(n, "Block", BlockType.MAIN_BLOCK);

			// Create a FunctionNode
			FunctionNode fNode = new FunctionNode(t, null, new List<Tuple<string, EvalType>>(), treeBlock);

			// Add the node to the BlockNode
			n.AddChild(fNode);

			return t;
		}
		#endregion

		#region DeclList
		/// <summary>
		/// Parse the DeclList → Decl DeclList | ε
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		private void DeclList(BlockNode n)
        {
            // Construct the tree dervide from a declaration
            Decl(n);

            // If true, there is another declaration so call recursivelly DeclList 
			if ((current.Type == TokenType.DECLVAR) || (current.Type == TokenType.DECLFUNC && (LookAhead(1, TokenType.IDE))))
                DeclList(n);
        }
        #endregion

        #region Decl
		/// <summary>
		/// Parse the Decl → var VarDeclList Type ; | FunDecl
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <exception cref="System.FunwapException">Thrown when the variable declared has not a type Token.</exception>
        private void Decl(BlockNode n)
        {
			// If the current Token is "var"
            if (current.Type == TokenType.DECLVAR)
			{
				MoveNext();
				List<Tuple<Token, ExpressionNode>> vars = new List<Tuple<Token, ExpressionNode>>();
				
				// Add to the list of vars all the variables found
				VarDeclList(n, vars);

				// Read the type of the variable
				Tuple<EvalType, List<EvalType>, Object> type = Type();
				if (type == null)
					throw new System.FunwapException("ParserException: " + this.current + " it is not a type token.", this.current);
 
				MatchToken(TokenType.SEMICOLONS);

				// Add all the variables found to the BlockNode
				for (int i = 0; i < vars.Count; i++)
				{
					// Create a DeclarationNode
                    DeclarationNode dNode = new DeclarationNode(vars[i].Item1, type, vars[i].Item2);

                    // Add the node to the BlockNode
                    n.AddChild(dNode);
				}
            }
			else
				// Since the current Token it is not "var", can be "func"
				n.AddChild(FunDecl(n, false));
        }
        #endregion

		#region FunDecl
		/// <summary>
		/// Parse the FunDecl → func IDE ( Params ) RType Block
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <param name="anonymous">if set to <c>true</c> do no parse the IDE.</param>
		/// <returns>The <see cref="FunctionNode"/> created if a function declaration is found.</returns>
		private FunctionNode FunDecl(BlockNode n, bool anonymous)
		{
			bool cond = (current.Type == TokenType.DECLFUNC);

			// If the function is anonymous, has to have a name.
			if (!anonymous)
				cond = cond && (LookAhead(1, TokenType.IDE));

			// If "func" it is not found means that there is no declaration
			if (cond)
			{
				// The name of anonymous function is "func"
				Token ide = current;
				MoveNext();

				// If it is not anonymous, take the name of the function
				if(!anonymous)
					ide = MatchToken(TokenType.IDE);

				MatchToken(TokenType.ROUNDBR_OPEN);
				// Read the formal parameters of the function
				List<Tuple<string, EvalType>> formalParameters = Params();
				MatchToken(TokenType.ROUNDBR_CLOSE);

				// Read the return type of the function
				Tuple<EvalType, List<EvalType>, Object> rtype = Type();

				// Construct the expected tree derived from Block
				BlockNode treeBlock = Block(n);

				// Create a FunctionNode
				return new FunctionNode(ide, rtype, formalParameters, treeBlock);
			}

			return null;
		}
		#endregion

		#region Type
		/// <summary>
		/// Parse the RType → Type | ε, where Type → int | bool | char | url | string | fun ( TypeList ) RType
		/// </summary>
		/// <returns>
		///		A <see cref="Tuple"/> containing 3 elements, if the EvalType is not a FUN type only the first element is different from <code>null</code>:
		///		-	The EvalType found;
		///		-	The list of EvalType containing in the parameters of the FUN type;
		///		-	The returned type of the FUN type;
		///	</returns>
		/// <exception cref="System.FunwapException">Thrown when a parameter of type FUN is not a valid type.</exception>
		private Tuple<EvalType, List<EvalType>, Object> Type()
		{
			// The return type is set initially to "void".
			Tuple<EvalType, List<EvalType>, Object> type = null;
			// In case of function type, it will contain the return type of the function.
			Object deepRType = null;

			// If the current Token is a type.
			if (current.ItIsType())
			{
				// $t will get a EvalType like INT, BOOL, CHAR, ...
				EvalType t = Eval.ConvertType(current.Type);
				// If it is not a FUN type, it has no parameters
				List<EvalType> pl = null;
				MoveNext();

				// If it is a FUN type.
				if (t == EvalType.FUN)
				{
					pl = new List<EvalType>();
					MatchToken(TokenType.ROUNDBR_OPEN);

					// Could be a function without parameters.
					if (current.ItIsType())
					{
						// First input type of "fun".
						pl.Add(Eval.ConvertType(current.Type));
						MoveNext();

						// Other input type of "fun".
						while (current.Type == TokenType.COMMA)
						{
							MoveNext();

							if ((current.ItIsType()) && (current.Type != TokenType.TYPEFUN))
								// Add the EvalType to the parameters list, it cannot be a FUN type
								pl.Add(Eval.ConvertType(current.Type));
							else
								throw new System.FunwapException("ParserException: " + this.current + " is not a valid type.", this.current);
						}
					}

					MatchToken(TokenType.ROUNDBR_CLOSE);

					// The last type is the output type of "fun", so recursively recall the Type() parser. 
					deepRType = Type();
				}

				type = new Tuple<EvalType, List<EvalType>, Object>(t, pl, deepRType);
			}

			return type;
		}
		#endregion

		#region VarDeclList
		/// <summary>
		/// Parse the VarDeclList → VarDecl, VarDeclList | VarDecl
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <param name="vars">Each variable found it is add to this list.</param>
		private void VarDeclList(BlockNode n, List<Tuple<Token, ExpressionNode>> vars)
		{
			// Add to $vars the first declared variable.
			vars.Add(VarDecl(n));

			// There is another declaration of variable so call recursivelly VarDeclList 
			if (current.Type == TokenType.COMMA)
			{
				MoveNext();
				VarDeclList(n, vars);
			}
		}
		#endregion

		#region VarDecl
		/// <summary>
		/// Parse the VarDecl → IDE | IDE = Exp
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <returns>
		///		A <see cref="Tuple"/> containing 2 elements: the Token rappresenting variable name and the possible <see cref="ExpressionNode"/> assingned to it.
		///	</returns>
		private Tuple<Token, ExpressionNode> VarDecl(BlockNode n)
		{
			ExpressionNode treeExp = null;
			// Take the name of the variable
			Token ide = MatchToken(TokenType.IDE);

			// If there is a "=" symbol means that there will be an expression after this symbol.
			if (current.Type == TokenType.ASSIGN)
			{
				MoveNext();
				// Construct the expected tree derived from Exp
				treeExp = Exp();
			}

			return new Tuple<Token, ExpressionNode>(ide, treeExp);
		}
		#endregion

        #region Params
		/// <summary>
		/// Parse the Params → ParamList | ε, where ParamList → IDE Type , ParamList | IDE Type
		/// </summary>
		/// <returns>The list of formal parameters found.</returns>
        private List<Tuple<string, EvalType>> Params()
        {
            List<Tuple<string, EvalType>> fp = new List<Tuple<string, EvalType>>();

			// If there is a identifier means that there is at least a parameter.
			if (current.Type == TokenType.IDE)
			{
				// Add this identifier to the formal parameters.
				fp.Add(new Tuple<string, EvalType>(MatchToken(TokenType.IDE).Value, GetMatchType()));

				// Until we find a "," characters we expected another parameters.
				while (current.Type == TokenType.COMMA)
				{
					MoveNext();
					fp.Add(new Tuple<string, EvalType>(MatchToken(TokenType.IDE).Value, GetMatchType()));
				}
			}

            return fp;
        }
        #endregion

        #region Block
		/// <summary>
		/// Parse the Block → { DeclList StmtList }
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <param name="title">The title of the <see cref="BlockNode"/> created.</param>
		/// <param name="type">A <see cref="BlockNode"/> can be Program block, a Main block o just a normal block.</param>
		/// <returns>The <see cref="BlockNode"/> created.</returns>
        private BlockNode Block(BlockNode n, string title = "Block", BlockType type = BlockType.NORMAL)
        {
            Token t = MatchToken(TokenType.CURLYBR_OPEN);
			BlockNode bNode = new BlockNode(t, title, type);

            // Add to the BlockNode, the list of declarations
            DeclList(bNode);

            // Add to the BlockNode, the list of statements
            StmtList(bNode);

            MatchToken(TokenType.CURLYBR_CLOSE);
            return bNode;
        }
        #endregion

        #region StmtList
		/// <summary>
		/// Parse the StmtList → Stmt StmtList | ε
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
        private void StmtList(BlockNode n)
        {
            // Construct the tree derived from a statement
            Stmt(n);

            // If true, there is another statements so call recursivelly StmtList 
            if (current.ItIsStmt())
                StmtList(n);
        }
        #endregion

		#region Stmt
		/// <summary>
		/// Parse the Stmt → IDE StmtIDE | IDE = readln(); | Call; | IDE = async(Exp); | IDE = dasync(IDE,Call);
		///					 | if(Exp) Block ElseStmt | while(Exp) Block | for(IDE = Exp; Exp; IDE StmtIDE) Block
		///					 | return Exp; | return FunDecl; | println (PrintList);
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		private void Stmt(BlockNode n)
        {
			Token t = null;
            StatementNode sNode = null;
			ExpressionNode treeExp = null;
			BlockNode bodyNode = null;

            switch (current.Type)
            {
                case TokenType.IDE:
					// Save in variable t the identifier
					t = MatchToken(TokenType.IDE);
					if (current.Type == TokenType.ROUNDBR_OPEN)
					{
						// Parse the Stmt → Call;
						sNode = Call(t, true);
						MatchToken(TokenType.SEMICOLONS);
					}
					else if ((current.Type == TokenType.ASSIGN) && (LookAhead(1, TokenType.READLN)))
					{
						// Parse the Stmt → IDE = readln();
						MoveNext();
						ReadNode readN = new ReadNode(current, t);
						MoveNext();
						MatchToken(TokenType.ROUNDBR_OPEN);
						MatchToken(TokenType.ROUNDBR_CLOSE);
						sNode = Assign(n, t, readN);
					}
					else if ((current.Type == TokenType.ASSIGN) && (LookAhead(1, TokenType.ASYNC)))
					{
						// Parse the Stmt → IDE = async(Exp);
						MoveNext();
						Token tokenAsync = current;
						MoveNext();
						MatchToken(TokenType.ROUNDBR_OPEN);
						ExpressionNode exp = Exp();
						MatchToken(TokenType.ROUNDBR_CLOSE);
						AsyncNode asyN = new AsyncNode(tokenAsync, t, exp);
						sNode = Assign(n, t, asyN);
					}
					else if ((current.Type == TokenType.ASSIGN) && (LookAhead(1, TokenType.DASYNC)))
					{
						// Parse the Stmt → IDE = dasync(IDE,Call);
						MoveNext();
						Token tokenAsync = current;
						MoveNext();
						MatchToken(TokenType.ROUNDBR_OPEN);
						VarNode url = new VarNode(MatchToken(TokenType.IDE));
						MatchToken(TokenType.COMMA);
						CallNode call = Call(MatchToken(TokenType.IDE));
						MatchToken(TokenType.ROUNDBR_CLOSE);
						DAsyncNode dasyN = new DAsyncNode(tokenAsync, t, url, call);
						sNode = Assign(n, t, dasyN);
					}
					else
						// Parse the Stmt → IDE StmtIDE
						sNode = StmtIDE(n, t);
                    break;

				case TokenType.IF:
					// Parse the Stmt → if(Exp) Block ElseStmt, where ElseStmt → else Block | ε
					t = current;
					MoveNext();

					MatchToken(TokenType.ROUNDBR_OPEN);
					treeExp = Exp();
					MatchToken(TokenType.ROUNDBR_CLOSE);

					BlockNode thenNode = Block(n, "Then");

					BlockNode elseNode = null;
					// If it finds the Token "else", it add the "Else" Block.
					if (current.Type == TokenType.ELSE)
					{
						MoveNext();
						elseNode = Block(n, "Else");
					}
					sNode = new IfNode(t, treeExp, thenNode, elseNode);
					break;

				case TokenType.WHILE:
					// Parse the Stmt → while(Exp) Block
					t = current;
					MoveNext();

					MatchToken(TokenType.ROUNDBR_OPEN);
					treeExp = Exp();
					MatchToken(TokenType.ROUNDBR_CLOSE);

					bodyNode = Block(n);
					sNode = new WhileNode(t, treeExp, bodyNode);
					break;

				case TokenType.FOR:
					// Parse the Stmt → for(IDE = Exp; Exp; IDE StmtIDE) Block
					t = current;
					MoveNext();

					// Parse: (IDE = Exp;
					MatchToken(TokenType.ROUNDBR_OPEN);
					Token ide1 = MatchToken(TokenType.IDE);
					MatchToken(TokenType.ASSIGN);
					StatementNode s1 = Assign(n, ide1, Exp());

					// Parse: Exp;
					treeExp = Exp();
					MatchToken(TokenType.SEMICOLONS);

					// Parse: IDE StmtIDE)
					Token ide2 = MatchToken(TokenType.IDE);
					StatementNode s2 = StmtIDE(n, ide2, false);
					MatchToken(TokenType.ROUNDBR_CLOSE);

					bodyNode = Block(n);
					sNode = new ForNode(t, s1, treeExp, s2, bodyNode);
					break;

				case TokenType.RETURN:
					// Parse the Stmt → return Exp; | return FunDecl;
					SyntacticNode rightN;
					t = current;
					MoveNext();

					// If the Token "func" is found, it has to be a anonymous function, otherwise an expression.
					if (current.Type == TokenType.DECLFUNC)
						rightN = FunDecl(n, true);
					else
						rightN = Exp();

					sNode = new ReturnNode(t, rightN);
					MatchToken(TokenType.SEMICOLONS);
					break;

				case TokenType.PRINTLN:
					// Parse the Stmt → println (PrintList); , where PrintList → Exp, PrintList | Exp
					t = current;
					MoveNext();
					MatchToken(TokenType.ROUNDBR_OPEN);

					List<ExpressionNode> expList = new List<ExpressionNode>();
					// Add the first expression.
					expList.Add(Exp());
					// If the Token found is "," means there are other expression to add.
					while (current.Type == TokenType.COMMA)
					{
						// Add the other expressions.
						MoveNext();
						expList.Add(Exp());
					}

					sNode = new PrintNode(t, expList.ToArray());

					MatchToken(TokenType.ROUNDBR_CLOSE);
					MatchToken(TokenType.SEMICOLONS);
					break;
            }

            if (sNode != null)
				// Add the created node to the BlockNode.
                n.AddChild(sNode);
        }
        #endregion

		#region StmtIDE
		/// <summary>
		/// Parse the StmtIDE → = Exp ; | += Exp ; | -= Exp ; | ++ ; | -- ;
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <param name="ide">The identifier.</param>
		/// <param name="semicolons">If set to <c>true</c> match the Token ";".</param>
		/// <returns>The <see cref="AssignNode"/> created.</returns>
		/// <exception cref="System.FunwapException">Thrown when the parsed statement is invalid.</exception>
		private AssignNode StmtIDE(BlockNode n, Token ide, bool semicolons = true)
		{
			Token t = null;
			AssignNode sNode = null;
			ExpressionNode treeExp = null;

			switch (current.Type)
			{
				case TokenType.ASSIGN:
					// Parse the StmtIDE → = Exp ;
					MoveNext();

					treeExp = Exp();
					sNode = Assign(n, ide, treeExp, null, semicolons);
					break;
				
				case TokenType.ASSIGN_MINUS: case TokenType.ASSIGN_PLUS:
					// Parse the StmtIDE → += Exp ; | -= Exp ;
					t = current;
					MoveNext();

					// Change the type of the Token.
					if (current.Type == TokenType.ASSIGN_MINUS) t.Type = TokenType.MINUS;
					else t.Type = TokenType.PLUS;

					treeExp = Exp();
					sNode = Assign(n, ide, treeExp, t, semicolons);
					break;

				case TokenType.INCR: case TokenType.DECR:
					// Parse the StmtIDE → ++ ; | -- ;
					t = current;
					MoveNext();

					// Change the type of the Token.
					if (current.Type == TokenType.DECR) t.Type = TokenType.MINUS;
					else t.Type = TokenType.PLUS;

					// Create a new node with the goal of trasforming: x++ into x=x+1 and x-- into x=x-1
					Token oneToken = new Token(TokenType.NUMBER, "1", t.Index, t.Row, t.Column, t.Length);
					ConstantNode one = new ConstantNode(oneToken);
					
					sNode = Assign(n, ide, one, t, semicolons);
					break;

				default:
					throw new System.FunwapException("ParserException: invalid statement, token: " + current + ".", current);
			}
			return sNode;
		}
		#endregion

		#region Assign
		/// <summary>
		/// Manage the creation of the <see cref="AssignNode"/> taking into consideration all the possible cases shown in StmtIDE.
		/// </summary>
		/// <param name="n">The <see cref="BlockNode"/> to which add the created node.</param>
		/// <param name="ide">The identifier.</param>
		/// <param name="rightNode">The right node.</param>
		/// <param name="op">The operation to perform between the identifier and the right node.</param>
		/// <param name="semicolons">If set to <c>true</c> match the Token ";".</param>
		/// <returns>The <see cref="AssignNode"/> created.</returns>
		private AssignNode Assign(BlockNode n, Token ide, SyntacticNode rightNode, Token op = null, bool semicolons = true)
		{
			VarNode copyide = null;
			BinaryOperationNode opNode = null;
			SyntacticNode tree = null;

			// If the operation Token is present
			if (op != null)
			{
				// Create a VarNode of the identifier, in order to preform the operation between the identifier and the right node.
				copyide = new VarNode(ide);
				// Create a BinaryOperationNode that contains on the left the copied identifier and on the right the rightNode.
				opNode = new BinaryOperationNode(op, copyide, (ExpressionNode)rightNode);
				tree = opNode;
			}
			else
				tree = rightNode;
			AssignNode sNode = new AssignNode(ide, tree);

			if (semicolons)
				MatchToken(TokenType.SEMICOLONS);
			return sNode;
		}
		#endregion

		#region Call
		/// <summary>
		/// Parse the Call → IDE ( Args )
		/// </summary>
		/// <param name="ide">The identifier.</param>
		/// <param name="itIsStm">In order to distinguish the case of statement Call and the Call inside expression.</param>
		/// <returns>The <see cref="CallNode"/> created.</returns>
		private CallNode Call(Token ide, bool itIsStm = false)
		{
			List<ExpressionNode> actualParameters = new List<ExpressionNode>();

			this.MatchToken(TokenType.ROUNDBR_OPEN);
			// If the Token is different from ")", means that there are parameter.
			if (current.Type != TokenType.ROUNDBR_CLOSE)
			{
				// Add the first expression to the actual parameters list.
				actualParameters.Add(Exp());

				// Until it finds the Token "," means there are other expression to be add to the actual parameters list.
				while (current.Type == TokenType.COMMA)
				{
					// Add the other expressions.
					MoveNext();
					actualParameters.Add(Exp());
				}
			}
			this.MatchToken(TokenType.ROUNDBR_CLOSE);

			return new CallNode(ide, actualParameters, itIsStm);
		}
		#endregion

		#region ProductionCase1
		// example: SumExp -> Term MoreTerms
		private ExpressionNode ProductionCase1(Func<ExpressionNode> Element, Func<ExpressionNode, ExpressionNode> MoreElements)
        {
            // Construct the expected tree derived from Element
            ExpressionNode treeElement = Element();

            // See if the expression contains more Elements (Element op Element op Element ...)
            ExpressionNode treeMoreElements = MoreElements(treeElement);

            return treeMoreElements;
        }
        #endregion
        #region ProductionCase2
        // example: MoreTerms -> + Term | - Term | ε
		private ExpressionNode ProductionCase2(List<TokenType> lType, ExpressionNode n, Func<ExpressionNode> Element, Func<ExpressionNode, ExpressionNode> MoreElements)
        {
            bool cond = false;

            foreach (TokenType t in lType)
                if (current.Type == t)
                    cond = true;

            // If we find the token type we was expecting, this means that we found another Element
            if (cond)
            {
				Token op = current;

                // Read the next token
				MoveNext();

                // Construct the expected tree derived from Element
                ExpressionNode treeElement = Element();

                // Create a BinaryOperationNode with $n, the tree passed from to the function, as left child and $treeElement as right child
                BinaryOperationNode opNode = new BinaryOperationNode(op, n, treeElement);

                // Recursive recall the function MoreTerms, to see if contains more Terms (Term +/- Term +/- Term ...)
                ExpressionNode treeMoreElements = MoreElements(opNode);

                return treeMoreElements;
            }
            else return n;
        }
        #endregion
        #region ProductionCase3
        // UnaryExp -> - UnaryExp | Factor
		private ExpressionNode ProductionCase3(TokenType t, Func<ExpressionNode> Element1, Func<ExpressionNode> Element2)
        {
            if (current.Type == t)
            {
				Token op = current;
                MoveNext();

                // Construct the expected tree derived from Element1
                ExpressionNode treeElement1 = Element1();

                // Create a UnaryOperationNode with $treeElement1
                UnaryOperationNode opNode = new UnaryOperationNode(op, treeElement1);

                return opNode;
            }
            else
            {
                // Construct the expected tree derived from Element2
                ExpressionNode treeElement2 = Element2();

                return treeElement2;
            }
        }
        #endregion
        #region Methods Using Production Functions
        #region Exp
        private ExpressionNode Exp() { return ProductionCase1(AndExp, MoreAndExps); }
        #endregion
        #region MoreAndExps
        private ExpressionNode MoreAndExps(ExpressionNode n)
        {
            List<TokenType> lType = new List<TokenType>(new TokenType[] { TokenType.OR });
            return ProductionCase2(lType, n, AndExp, MoreAndExps);
        }
        #endregion
        #region AndExp
		private ExpressionNode AndExp() { return ProductionCase1(UnaryRelExp, MoreUnaryRelExps); }
        #endregion
        #region MoreUnaryRelExps
        private ExpressionNode MoreUnaryRelExps(ExpressionNode n)
        {
            List<TokenType> lType = new List<TokenType>(new TokenType[] { TokenType.AND });
            return ProductionCase2(lType, n, UnaryRelExp, MoreUnaryRelExps);
        }
        #endregion
        #region UnaryRelExp
		private ExpressionNode UnaryRelExp() { return ProductionCase3(TokenType.NOT, UnaryRelExp, RelExp); }
        #endregion
        #region RelExp
		private ExpressionNode RelExp() { return ProductionCase1(SumExp, MoreSumExps); }
        #endregion
        #region MoreSumExps
        private ExpressionNode MoreSumExps(ExpressionNode n)
        {
            List<TokenType> lType = new List<TokenType>(new TokenType[] { 
                TokenType.EQUAL, TokenType.INEQUAL, TokenType.GREATER, TokenType.GREATEREQ, TokenType.LESS, TokenType.LESSEQ 
            });
			return ProductionCase2(lType, n, SumExp, MoreSumExps);
        }
        #endregion
        #region SumExp
		private ExpressionNode SumExp() { return ProductionCase1(Term, MoreTerms); }
        #endregion
        #region MoreTerms
        private ExpressionNode MoreTerms(ExpressionNode n)
        {
            List<TokenType> lType = new List<TokenType>(new TokenType[] { TokenType.PLUS, TokenType.MINUS });
			return ProductionCase2(lType, n, Term, MoreTerms);
        }
        #endregion
        #region Term
		private ExpressionNode Term() { return ProductionCase1(UnaryExp, MoreUnaryExps); }
        #endregion
        #region MoreUnaryExps
        private ExpressionNode MoreUnaryExps(ExpressionNode n)
        {
            List<TokenType> lType = new List<TokenType>(new TokenType[] { TokenType.MUL, TokenType.DIV });
			return ProductionCase2(lType, n, UnaryExp, MoreUnaryExps);
        }
        #endregion
        #region UnaryExp
		private ExpressionNode UnaryExp() { return ProductionCase3(TokenType.MINUS, UnaryExp, Factor); }
        #endregion
        #endregion

        #region Factor
		/// <summary>
		/// Parse the Factor → IDE | ( Exp ) | Call | Const
		/// </summary>
		/// <returns>The <see cref="ExpressionNode" /> created analysing the Factor.</returns>
		/// <exception cref="System.FunwapException">Thrown when the parsed factor is invalid.</exception>
		private ExpressionNode Factor()
        {
            ExpressionNode result = null;

            // Switch on the current token type
            switch (current.Type)
            {
                case TokenType.ROUNDBR_OPEN:
					// Parse the Factor → ( Exp )
                    MoveNext();
                    // Construct the expected tree derived from a Exp
                    result = Exp();
                    this.MatchToken(TokenType.ROUNDBR_CLOSE);
					break;

                case TokenType.IDE:
					// Parse the Factor → IDE | Call
					Token ide = MatchToken(TokenType.IDE);

                    if (current.Type == TokenType.ROUNDBR_OPEN)
                        result = Call(ide);
                    else
                        result = new VarNode(ide);
					break;

                case TokenType.NUMBER: case TokenType.CHAR: case TokenType.STRING: case TokenType.URL: case TokenType.TRUE: case TokenType.FALSE:
					// Parse the Factor → Const, where Const → NUMBERC | CHARC | STRINGC | URLC | true | false
					Token cons = current;
                    result = new ConstantNode(cons);
                    MoveNext();
					break;

                default:
                    throw new System.FunwapException("ParserException: invalid factor, token: " + current + ".", current);
            }
			return result;
        }
        #endregion

        #endregion

        #region PUBLIC METHODS

        #region ParseTree
		/// <summary>
		/// Parse the source text and constructs an abstract syntax tree.
		/// </summary>
		/// <returns>A <see cref="Tuple" /> containing The "Main" Token and the root of the abstract syntax tree.</returns>
		/// <exception cref="System.FunwapException">>Thrown when found a Token different from the end of the file.</exception>
        public Tuple<Token, BlockNode> ParseTree()
        {
            // Construct the "Program" node
            BlockNode programNode = new BlockNode(null, "Program", BlockType.PROGRAM_BLOCK);

			// Parse Program → Main | DeclList Main
            DeclList(programNode);
			Token main = Main(programNode);

			if (current.Type != TokenType.EOF)
				throw new System.FunwapException("ParserException: " + this.current + " found has not expected type " + TokenType.EOF + ".", this.current);

            return new Tuple<Token, BlockNode>(main, programNode);
        }
        #endregion

        #endregion
    }
}
