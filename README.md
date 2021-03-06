# Funw@p
Interpreter and Compiler for the Funw@p language.
====

## Project specifications

Funw@p <i>(read fun with AP)</i> is a domain specific programming language designed to support general mathematical and logical operations. The language has native support for <b>higher order functions</b> and <b>parallel programming</b>.<br/>
Funw@p helps you <b>distribute</b> your computationally expensive or network related problems across multiple machines in a cluster. Those machines can execute their portion of the problem and then send results back to the master program.<br/>
For more information, read the file: [PA_Proj_Midterm2014-15.pdf](./PA_Proj_Midterm2014-15.pdf)

## Introduction

The software was built with the goal to simulate the behaviour of an <b>IDE</b> (Integrated development environment).<br />
It is an interpreter and a compiler, written in <b>C#</b>, for the Funw@p language.<br />

Thanks to the graphical interface, it is really simple to use. You need just to run it and it will appear a Windows Form.<br />
It contains a menu containing the items:
 - <b>File</b> → where you can open the file of your program written in the Funw@p language.
 - <b>Execute</b> → you can geynerete the Abstract Syntax Tree, compile or interpret your code.
 - <b>?</b> → containing a Help and About buttons.

The image below shows the Abstract Syntax Tree of the factorial program generated by the application.

<img src="./demo.png" />

If you need more information, you can read the file: [Documentation.pdf](./Documentation.pdf).

In order to understand the Funw@p language, you can read the file: [Funw@p - Grammar.pdf](./Funw@p - Grammar.pdf).<br />
Since it can be difficult to read the grammar, in the [Examples](./Examples) directory you can find 
a list of working examples that allows you to see the potential of the language.

#### Key Features:
- Simple and clear graphical interface; it is even shown a graphical representation of the Abstract Syntax Tree
- Code full of comments and supports the Sandcastle Documentation
- No dependencies to external libraries

## Example
```c#
public IDE()
{
   InitializeComponent();
   Editor.AcceptsTab = true;
   try
   {
      // Scanner phase
      scanner = new Scanner();
      scanner.Initialize("func Main(){ println(\"Hello World!\"); }");
      tokens = scanner.Tokenize();
      
      // Trasform the token enumerator in list and remove all the comments
      tokenlist = new List<Token>();
      foreach (var token in tokens)
         if (token.Type != TokenType.COMMENT)
            tokenlist.Add(token);

      // Parser phase
      parser = new Parser(tokenlist);
      
      // Create the Abstract Syntax Tree.
      System.Tuple<Token, BlockNode> pair = parser.ParseTree();
      main = pair.Item1;
      root = pair.Item2;
      nameFile = "Hello_World";
   }
   catch (System.FunwapException ex) { System.Console.Write(ex.Message); }
}
```

## Supported Types
|   Types  |       Description       |
|:--------:|:-----------------------:|
|   void   |	No type                 |
|   int    |	Type integer            |
|   bool   |	Type boolean            |
|   char   |	Type character          |
|  string  |	Type string             |
|   url    |	Type network address    |
|   fun    |	Type function           |


## Supported Operation
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

### License
The MIT License (MIT)

Copyright (c) 2015 Federico Conte

http://opensource.org/licenses/MIT
