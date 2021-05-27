using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ASTBuilder
{
  class CodeGenVisitor
  {

    private StreamWriter file;  // IL code written to this file
    private SymScope curClass;
    private int localStackLoc = 0;
    private bool inRhs = false;
    private int branchLoc = 0;
    private string currentClass = "";
    private bool inReturn = false;
    private int offset = 0;
    public string methodCtx = "";

    // used to produce a readable trace when desired
    protected String prefix = "";
    protected String otherPrefix = "";


    public virtual string ClassName()
    {
      return this.GetType().Name;
    }

    private bool TraceFlag = true;  // Set to true or false to control trace

    private void Trace(AbstractNode node, string suffix = "")
    {
      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(this.prefix + ">> In VistNode for " + node.ClassName() + " " + suffix);
        Console.ResetColor();
      }
    }
    public void GenerateCode(dynamic node, string filename)
    // node is the root of the AST for the entire TCCL program
    {
      if (node == null) return;  // No code generation for empty AST

      file = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), filename + ".il"));

      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(this.prefix + "Started code Generation for " + filename + ".txt");
        Console.ResetColor();
      }
      // Since node is the root node of the AST, this call begins the code generation traversal
      VisitNode(node);

      file.Close();

    }

    public virtual void VisitNode(AbstractNode node)
    {
      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(this.prefix + ">> In general VistNode for " + node.ClassName());
        Console.ResetColor();
      }

      VisitChildren(node);
    }
    public virtual void VisitChildren(AbstractNode node)
    {
      // Nothing to do if node has no children
      if (node.Child == null) return;

      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(this.prefix + " └─In VistChildren for " + node.ClassName());
        Console.ResetColor();
      }
      // Increase prefix while visiting children
      String oldPrefix = this.prefix;
      this.prefix += "   ";

      foreach (dynamic child in node.Children()) {
        VisitNode(child);
      };

      this.prefix = oldPrefix;
    }
    public virtual void VisitNode(CompilationUnit node)
    {
      Trace(node);

      prefix = "";

      // The two lines that follow generate the prelude required in all .il files
      file.WriteLine(".assembly extern mscorlib {}");
      file.WriteLine(".assembly test1 { }");

      VisitChildren(node);

      // The following lines are present so that an executable .il file is generated even
      // before you have implemented any VisitNode routines.  It generated the body for the
      // hello.txt program regardless of what file is parsed to create the AST.
      // DELETE THESE LINES ONCE YOU HAVE IMPLEMENTED VisitNode FOR MethodDeclaration
      /*file.WriteLine(".method static void main()");
      file.WriteLine("{");
      file.WriteLine("   .entrypoint");
      file.WriteLine("   .maxstack 1");
      file.WriteLine("   ldstr \"Hello world!\"");
      file.WriteLine("   call void [mscorlib]System.Console::WriteLine(string)");
      file.WriteLine("   ret");
      file.WriteLine("}");*/
      // DELETE TRHOUGH HERE

    }

    private string getModString(ModifierType mod)
    {
      switch (mod) {
        case ModifierType.PUBLIC:
          return "public";
        case ModifierType.PRIVATE:
          return "private";
        default:
          return "static";
      }
    }

    public void VisitNode(ClassDeclaration node)
    {
      ClassTypeDescriptor type = (ClassTypeDescriptor)node.type;

      string prevClass = currentClass;
      currentClass = type.className;
      file.Write(otherPrefix + ".class");
      foreach (ModifierType mod in type.modifiers)
        file.Write(" " + getModString(mod));
      file.WriteLine(" " + type.className);
      file.WriteLine(otherPrefix + "{");

      curClass = type.locals;

      String oldPrefix = this.otherPrefix;
      otherPrefix += "    ";

      dynamic body = node.Child.Sib.Sib;
      VisitNode(body);

      otherPrefix = oldPrefix;

      file.WriteLine(otherPrefix + "}");
      currentClass = prevClass;
    }

    public void VisitNode(ClassBody node)
    {
      VisitChildren(node);
    }
    public void VisitNode(MethodDeclaration node)
    {
      MethodAttributes atr = node.atr;
      dynamic sig = node.Child.Sib.Sib;
      dynamic body = node.Child.Sib.Sib.Sib;
      localStackLoc = 0;

      file.Write(otherPrefix + ".method");
      foreach (ModifierType mod in atr.modifiers) {
        if (mod == ModifierType.STATIC) {
          atr.isStatic = true;
        }
        file.Write(" " + getModString(mod));
      }

      if (!atr.isStatic)
        methodCtx = atr.foundIn;

      if (!atr.isStatic && sig.atr.name.Equals("main"))
        file.Write(" static");

      file.Write(" " + atr.returnType.type);

      VisitNode(sig);
      file.WriteLine(otherPrefix + "{");

      String oldPrefix = this.otherPrefix;
      otherPrefix += "    ";

      if (sig.atr.name.Equals("main"))
        file.WriteLine(otherPrefix + ".entrypoint");

      file.WriteLine(otherPrefix + ".maxstack 20");

      if (!atr.isStatic)  // First arg reserved for instance
        offset = 1;
      VisitNode(body);
      offset = 0;

      if (atr.returnType.GetType() == typeof(VoidTypeDescriptor))
        file.WriteLine(otherPrefix + "ret");

      otherPrefix = oldPrefix;

      file.WriteLine(otherPrefix + "}");
    }

    public void VisitNode(MethodSignature node)
    {
      MethodSignatureAttributes atr = node.atr;
      if (atr.name.Equals("init")) atr.name = "\'" + atr.name + "\'";
      file.Write(" " + atr.name + " (");

      if (atr.parameters.Count != 0)
        file.Write(atr.parameters[0].type.type);
      for (int i = 1; i < atr.parameters.Count; i++)
        file.Write(", " + atr.parameters[i].type.type);

      if (!atr.name.Equals("main")) file.WriteLine(") cil managed");
      else file.WriteLine(")");

      foreach (VariableAttributes varAtr in atr.parameters) {
        varAtr.stackLoc = localStackLoc++; // We also need space for our arguments
      }
    }

    public void VisitNode(ReturnStatement node)
    {
      inReturn = true;
      inRhs = true;
      VisitChildren(node);
      inRhs = false;
      file.WriteLine(otherPrefix + "ret");
    }

    public void VisitNode(MethodCall node)
    {
      MethodCallAttributes callAtr = node.atr;      

      if (currentClass.Equals(methodCtx)) { // Load the instance from arg one
        file.WriteLine(otherPrefix + "ldarg 0");
      }
      bool prevRhs = inRhs;

      inRhs = false;
      VisitNode(node.Child); // get the scope thing for struct

      inRhs = true;
      if (node.Child.Sib != null) // Load arguments
        VisitNode((ArgumentList)node.Child.Sib);

      inRhs = prevRhs;


      file.Write(otherPrefix + "call");

      if (callAtr.mAtr.GetType() == typeof(ReservedAttributes)) {
        file.Write(" " + ((ReservedAttributes)callAtr.mAtr).type.type + callAtr.name + "(");
      } else { // Remove trailing whitespace + replace other whitespaces with scope operator
        if (!callAtr.mAtr.isStatic) {          

          if (callAtr.relativeName.Equals("init")) callAtr.relativeName = "\'" + callAtr.relativeName + "\'";

          file.Write(" instance " + callAtr.mAtr.returnType.type + " " + callAtr.mAtr.foundIn + "::"
            + callAtr.relativeName + "(");

        } else {
          // Next-level bruteforce/bad-practice
          string methodName = callAtr.name.Remove(callAtr.name.Length - 1).Replace(" ", "::");
          if (!methodName.Contains("::"))
            methodName = currentClass + "::" + methodName;

          file.Write(" " + callAtr.mAtr.returnType.type + " " + methodName + "(");
        }        
      }

      if (callAtr.sig.Count != 0)
        file.Write(callAtr.sig[0].type);
      for (int i = 1; i < callAtr.sig.Count; i++)
        file.Write(", " + callAtr.sig[i].type);

      file.WriteLine(")");
    }

    public void VisitNode(ArgumentList node)
    {
      foreach(dynamic child in node.Children()) {
        if (child is STR_CONST) {
          file.WriteLine(otherPrefix + "ldstr \"" + ((STR_CONST)child).StrVal + "\"");
        } else {
          VisitNode(child);
        }
      }
    }

    public void VisitNode(INT_CONST node)
    {
      file.WriteLine(otherPrefix + "ldc.i4 " + node.IntVal);
    }

    public void VisitNode(Expression node)
    {
      dynamic lhs = node.Child;
      dynamic rhs = lhs.Sib;

      if (node.exprKind == ExprKind.EQUALS && currentClass.Equals(methodCtx)) {
        if (currentClass.Equals(methodCtx)) { // Load the instance from arg one
          file.WriteLine(otherPrefix + "ldarg 0");
        }
        inRhs = true;
        VisitNode(rhs);
      } else VisitChildren(node);      

      switch (node.exprKind) {
        case ExprKind.EQUALS: // Assuming rhs already on stack... store in variable
          if (((VariableAttributes)lhs.atr).isParam) 
            file.WriteLine(otherPrefix + "starg " + ((VariableAttributes)lhs.atr).stackLoc + offset);
          else if (!((VariableAttributes)lhs.atr).foundIn.Equals(""))
            file.WriteLine(otherPrefix + "stfld " + ((VariableAttributes)lhs.atr).type.type
                          + " " + ((VariableAttributes)lhs.atr).foundIn + "::" + ((VariableAttributes)lhs.atr).id);
          else file.WriteLine(otherPrefix + "stloc " + ((VariableAttributes)lhs.atr).stackLoc);
          inRhs = false;
          break;
        case ExprKind.PLUSOP:
          file.WriteLine(otherPrefix + "add");
          break;
        case ExprKind.MINUSOP:
          file.WriteLine(otherPrefix + "sub");
          break;
        case ExprKind.ASTERISK:
          file.WriteLine(otherPrefix + "mul");
          break;
        case ExprKind.RSLASH:
          file.WriteLine(otherPrefix + "div");
          break;
        case ExprKind.PERCENT:
          file.WriteLine(otherPrefix + "rem");
          break;
        case ExprKind.OP_LOR: // This will probably work please don't do implicit type conversion here!!!
        case ExprKind.PIPE:
          file.WriteLine(otherPrefix + "or");
          break;
        case ExprKind.OP_LAND:
        case ExprKind.AND:
          file.WriteLine(otherPrefix + "and");
          break;
        //case ExprKind.HAT:
        case ExprKind.OP_EQ: // Generate boolean for consistency
          file.WriteLine(otherPrefix + "beq T" + branchLoc);
          emitBranchCond();
          break;
        case ExprKind.OP_NE:
          file.WriteLine(otherPrefix + "bne.un T" + branchLoc);
          emitBranchCond();
          break;
        case ExprKind.OP_GT:
          file.WriteLine(otherPrefix + "bgt T" + branchLoc);
          emitBranchCond();
          break;
        case ExprKind.OP_LT:
          file.WriteLine(otherPrefix + "blt T" + branchLoc);
          emitBranchCond();
          break;
        case ExprKind.OP_LE:
          file.WriteLine(otherPrefix + "ble T" + branchLoc);
          emitBranchCond();
          break;
        case ExprKind.OP_GE:
          file.WriteLine(otherPrefix + "bge T" + branchLoc);
          emitBranchCond();
          break;
      }
    }

    public void emitBranchCond()
    {
      file.WriteLine(otherPrefix + "ldc.i4 0");
      file.WriteLine(otherPrefix + "br E" + branchLoc);
      file.WriteLine(otherPrefix + "T" + branchLoc + ": ldc.i4 1");
      file.WriteLine(otherPrefix + "E" + branchLoc + ":");
      branchLoc++;
    }

    public void VisitNode(LocalVariableDeclaration node)
    {
      // On declaration, reserve location for variable
      

      foreach(Identifier id in node.ids.Children()) {
        file.WriteLine(otherPrefix + ".locals init(" + id.type.type + ")");
        ((VariableAttributes)id.atr).stackLoc = localStackLoc++;
      }
    }

    public void VisitNode(Identifier node)
    {
      if (node.type is ObjectTypeDescriptor) {
        file.WriteLine(otherPrefix + "ldsflda valuetype  " + node.type.type
                      + " " + currentClass + "::" + node.Name);
      }
      // If RHS, extract location
      else if (inRhs) {
        // Unary doesn't work here for some reason...
        if (((VariableAttributes)node.atr).isParam) 
          file.WriteLine(otherPrefix + "ldarg " + (((VariableAttributes)node.atr).stackLoc + offset));
        else if (!((VariableAttributes)node.atr).foundIn.Equals(""))
          file.WriteLine(otherPrefix + "ldfld " + ((VariableAttributes)node.atr).type.type 
                        + " " + ((VariableAttributes)node.atr).foundIn + "::" + ((VariableAttributes)node.atr).id);
        else file.WriteLine(otherPrefix + "ldloc " + ((VariableAttributes)node.atr).stackLoc);
      } 
    }

    public void VisitNode(SelectionStatement node)
    {
      dynamic cond = node.Child;
      dynamic tBlock = cond.Sib;
      dynamic fBlock = tBlock.Sib;

      int ourLabel = branchLoc;
      branchLoc++;

      inRhs = true;
      VisitNode(cond); // Generate boolean value on the stack
      inRhs = false;

      file.WriteLine(otherPrefix + "brfalse F" + ourLabel);
      // TRUE BRANCH
      VisitNode(tBlock);
      if (!inReturn)
        file.WriteLine(otherPrefix + "br E" + ourLabel);

      if (fBlock != null) {
        // FALSE BRANCH
        file.WriteLine(otherPrefix + "F" + ourLabel + ":");
        VisitNode(fBlock);
      }
      if (!inReturn)
        file.WriteLine(otherPrefix + "E" + ourLabel + ":");
      inReturn = false;
    }

    public void VisitNode(IterationStatement node)
    {
      dynamic cond = node.Child;
      dynamic tBlock = cond.Sib;

      int ourLabel = branchLoc;
      branchLoc++;

      file.WriteLine(otherPrefix + "LOOP_" + ourLabel + ":");

      inRhs = true;
      VisitNode(cond); // Generate boolean value on the stack
      inRhs = false;

      file.WriteLine(otherPrefix + "brfalse E" + ourLabel);
      // Loop body
      VisitNode(tBlock);

      file.WriteLine(otherPrefix + "br LOOP_" + ourLabel);

      file.WriteLine(otherPrefix + "E" + ourLabel + ":");
    }
    public void VisitNode(StructDeclaration node)
    {
      // Treat as a class
      StructTypeDescriptor type = (StructTypeDescriptor)node.type;

      string prevClass = currentClass;
      currentClass = type.type;

      file.Write(otherPrefix + ".class nested");
      foreach (ModifierType mod in type.modifiers)
        file.Write(" " + getModString(mod));
      file.Write(" sequential ansi sealed beforefieldinit");
      file.WriteLine(" " + type.className + " extends [System.Private.CoreLib]System.ValueType");
      file.WriteLine(otherPrefix + "{");

      curClass = type.locals;

      String oldPrefix = this.otherPrefix;
      otherPrefix += "    ";

      dynamic body = node.Child.Sib.Sib;

      VisitNode(body);
      otherPrefix = oldPrefix;

      file.WriteLine(otherPrefix + "}");
      currentClass = prevClass;
    }

    public void VisitNode(FieldDeclaration node)
    {
      /*List<ModifierType> mods = ((FieldAttributes)((Identifier)node.Child).atr).modifiers;*/
      string modString = "";
      dynamic mods = node.Child;
      foreach (ModifierType mod in mods.ModifierTokens)
        modString += " " + getModString(mod);

      dynamic ids = mods.Sib.Sib;

      string val = "";

      if (node.type is ObjectTypeDescriptor) val = "valuetype ";

      foreach(Identifier id in ids.Children()) {
        file.WriteLine(otherPrefix + ".field" + modString + " " + val + node.type.type + " " + id.Name);
      }

    }
  }  
}
