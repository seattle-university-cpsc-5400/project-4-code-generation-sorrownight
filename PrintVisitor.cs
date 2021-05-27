﻿using System;

namespace ASTBuilder
{
  class PrintVisitor
  {
    // The Visitor pattern implementation works by using the 'dynamic' type specification
    // to pass any node type to the function that initiates the visiting process.
    // The calls to VisitNode with 'node' as an argument then trigger dynamic lookup 
    // to find the appropriate version of that method.


    // This PrintTree method begins the tree printing process and handles
    // production of the tree structure prefix.  It will be called recursively
    // to produce indented output for each level of the abstract syntax tree.

    public void PrintTree(dynamic node, string prefix = "")
    {
      // This check is here to simplify calling code, VisitChildren 
      if (node == null) {
        return;
      }
      // Print appropriate prefix before calling VisitNode

      // The form of the prefix at this level and in the call to
      // VisitChildren depends on whether the current node is
      // the last sibling among the children of its parent node
      bool isLastChild = (node.Sib == null);

      Console.ForegroundColor = ConsoleColor.Magenta;
      Console.Write(prefix);
      Console.Write(isLastChild ? "└─ " : "├─ ");
      Console.ResetColor();

      this.VisitNode(node);

      // Propagate tree traversal to the children of node
      VisitChildren(node, prefix + (isLastChild ? "   " : "│ "));
    }

    // This method isn't strictly necessary, since its simple body
    // could be incorporated into PrintTree.  It is included here to be
    // consistent with the pseudocode in the textbook.
    public void VisitChildren(AbstractNode node, String prefix)
    {
      foreach (AbstractNode child in node.Children()) {
        PrintTree(child, prefix);
      }
    }

    // VisitNode is defined here for a parameter of the parent class
    // AbstractNode, so it will be invoked on any node when there is not
    // a specialized method defined below.
    public void VisitNode(AbstractNode node)
    {
      Console.Write("<" + node.ClassName() + ">");
      if (node.type != null)
        Console.Write(" ---type: " + node.type.GetType());
      Console.WriteLine();
    }

    // Here are three specialized VisitNode methods for terminals plus
    // one for Expression.  You will be adding more methods here for 
    // other nodes that hold information of interest beyond the class name.
    //
    // Note that the only differences in these methods are on the third line, 
    // where the special information for the node type is written out.  
    // Just what must be included at this point in the method depends on
    // what information is stored in the node.
    public void VisitNode(Identifier node)
    {
      Console.Write("<" + node.ClassName() + ">: ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write(node.Name);
      Console.WriteLine(" ---type: " + node.type.GetType());
      Console.ResetColor();
    }

    public void VisitNode(INT_CONST node)
    {
      Console.Write("<" + node.ClassName() + ">: ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write(node.IntVal);
      Console.WriteLine(" ---type: " + node.type.GetType());
      Console.ResetColor();
    }

    public void VisitNode(STR_CONST node)
    {
      Console.Write("<" + node.ClassName() + ">: ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write(node.StrVal);
      Console.WriteLine(" ---type: " + node.type.GetType());
      Console.ResetColor();
    }

    public void VisitNode(Expression node)
    {
      Console.Write("<" + node.ClassName() + ">: ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write(node.exprKind);
      Console.WriteLine(" ---type: " + node.type.GetType());
      Console.ResetColor();
    }
    // Added methods
    public void VisitNode(PrimitiveType node)
    {
      Console.Write("<" + node.ClassName() + ">: ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write(node.Type);
      Console.WriteLine(" ---type: " + node.type.GetType());
      Console.ResetColor();
    }

    public void VisitNode(Modifiers node)
    {
      Console.Write("<" + node.ClassName() + ">: ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      foreach (ModifierType type in node.ModifierTokens)
        Console.Write(type + " ");
      Console.WriteLine();
      Console.ResetColor();
    }

    public void VisitNode(SpecialName node)
    {
      Console.Write("<" + node.ClassName() + ">: ");
      Console.ForegroundColor = ConsoleColor.Yellow;      
      Console.WriteLine(node.encName);
      Console.ResetColor();
    }

  }

}

