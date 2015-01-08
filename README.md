# Funw@p
Interpreter and Compiler for the Funw@p language.
====

##Introduction

The software was built with the goal to simulate the behaviour of an <b>IDE</b> (Integrated development environment).<br />
It is an interpreter and a compiler, written in <b>C#</b>, for the Funw@p language.<br /><br />

Thanks to the graphical interface, it is really simple to use. You need just to run it and it will appear a Windows Form.<br />
It contains a menu containing the items:
 - <b>File</b> → where you can open the file of your program written in the Funw@p language.
 - <b>Execute</b> → you can geynerete the Abstract Syntax Tree, compile or interpret your code.
 - <b>?</b> → containing a Help and About buttons.

If you need more information, you can read the file: <i>"Documentation.pdf"</i>.

In order to understand the Funw@p language, you can read the file: <i>"Funw@p - Grammar.pdf"</i>.<br />
Since it can be difficult to read the grammar, in the <i>Example</i> directory you can find 
a list of working examples that allows you to see the potential of the language.

####Key Features:
- Simple and clear graphical interface; it is even shown a graphical representation of the Abstract Syntax Tree
- Code full of comments and supports the Sandcastle Documentation
- No dependencies to external libraries

##Example
```c#
  try
  {
    // Scanner phase
    Scanner s = new Scanner();
    s.Initialize("func Main(){ println(\"Hello World!\"); }");
    IEnumerable<Token> tokens = s.Tokenize();
    
    // Trasform the token enumerator in list and remove all the comments
    List<Token> tokenlist = new List<Token>();
		foreach (var token in tokens)
			if (token.Type != TokenType.COMMENT)
				tokenlist.Add(token);
		
		// Parser phase
		Parser p = new Parser(tokenlist);
    
    // Create the Abstract Syntax Tree.
		System.Tuple<Token, BlockNode> pair = p.ParseTree();
  }
  catch (System.FunwapException ex){ System.Console.Write(ex.Message); }
```

##Supported Operation
| Operator |       Description       |
|:--------:|:-----------------------:|
|   OR     |	Logical OR       |
|   AND    |	Logical AND    |
|   NOT    |	Logical NOT |
|    =     |	Equality       |
|    ≠     |	Inequality       |
|    >     |	Greater than    |
|    ≥     |	Greater than or equal to operator |
|    <     |	Less than       |
|    ≤     |	Less than or equal to       |
|    +     |	Addition       |
|    -     |	Subtraction    |
|    ×     |	Multiplication |
|    ÷     |	Division       |
