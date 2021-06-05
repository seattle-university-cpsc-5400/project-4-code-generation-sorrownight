using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
  public class SemanticsVisitor
  {
    //Uncomment the next line once you include your symbol table implementation
    protected static Symtab table;

    // Used to signal error
    ErrorTypeDescriptor errorType = new ErrorTypeDescriptor();
   
    // used to produce a readable trace when desired
    protected String prefix = "";

    public SemanticsVisitor(String oldPrefix = "")
    // Parameter oldPrefix allows creation of this visitor during a traversal
    {
      prefix = oldPrefix + "   ";
      table = new Symtab();
    }

    public virtual string ClassName()
    {
      return this.GetType().Name;
    }

    private bool TraceFlag = true;  // Set to true or false to control trace

    private void Trace(AbstractNode node, string suffix = "")
    {
      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(this.prefix + ">> In VisitNode for " + node.ClassName() + " " + suffix);
        Console.ResetColor();
      }
    }

    // The following are not needed now; they follow implementation from Chapter 8

    //protected static MethodAttributes currentMethod = null;
    //protected void SetCurrentMethod(MethodAttributes m)
    //{
    //    currentMethod = m;
    //}
    //protected MethodAttributes GetCurrentMethod()
    //{
    //    return currentMethod;
    //}

    //protected static ClassAttributes currentClass = null;

    //protected void SetCurrentClass(ClassAttributes c)
    //{
    //    currentClass = c;
    //}
    //protected ClassAttributes GetCurrentClass()
    //{
    //    return currentClass;
    //}

    // Call this method to begin the semantic checking process
    public void CheckSemantics(dynamic node)
    {
      if (node == null) return;

      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(this.prefix + "Started Check Semantics for " + node.ClassName());
        Console.ResetColor();
      }

      VisitNode(node);
    }

    // This version of VisitNode is invoked if a more specialized one is not defined below.
    public virtual void VisitNode(AbstractNode node)
    {
      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(this.prefix + ">> In general VisitNode for " + node.ClassName());
        Console.ResetColor();
      }
      VisitChildren(node);
    }

    public virtual void VisitChildren(AbstractNode node)
    {
      // Nothing to do if node has no children
      if (node.Child == null) return;

      if (TraceFlag) {
        Console.ForegroundColor = ConsoleColor.Green;
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

    //Starting Node of an AST
    public void VisitNode(CompilationUnit node)
    {
      Trace(node);

      VisitChildren(node);
    }

    // This method is needed to implement special handling of certain expressions
    public virtual void VisitNode(Expression node)
    {
      Trace(node);

      VisitChildren(node);

      dynamic lhs = node.Child;
      dynamic rhs = lhs.Sib;

      switch (node.exprKind) {
        case ExprKind.EQUALS:
          if (!(lhs is QualifiedName || lhs is Identifier)) {
            err("Attempting to assign to unassignable token.");
            node.type = errorType;
            break;
          }
          if (lhs.atr != null && lhs.atr is VariableAttributes) {
            if (lhs.type.Equals(errorType) || rhs.type.Equals(errorType)) {
              node.type = errorType;
              break;
            }
            if (lhs.type.GetType() != rhs.type.GetType()) {
              err("Unsupported assignment of " + rhs.type.GetType() + " to " 
                  + lhs.type.GetType());
                node.type = errorType;
            }
            node.type = lhs.type;
            ((VariableAttributes)lhs.atr).initialized = true;
          } else if (lhs.type == errorType || rhs.type == errorType) {
            node.type = errorType;
          } else {
            if (lhs.atr != null && !(lhs.atr is VariableAttributes)) 
              err("Unassignable token" + lhs.atr.GetType());
            node.type = errorType;
          }
          break;

        case ExprKind.PLUSOP:
        case ExprKind.MINUSOP:
        case ExprKind.ASTERISK:
        case ExprKind.RSLASH:
        case ExprKind.PERCENT:
          if ((lhs is QualifiedName && (VariableAttributes)lhs.atr != null &&
            !(((VariableAttributes)lhs.atr).initialized)) 
            || (rhs is QualifiedName && (VariableAttributes)rhs.atr != null &&
            !(((VariableAttributes)rhs.atr).initialized))) {
            err("Trying to use undeclared variables");
            node.type = errorType;
            break;
          }
          if (lhs.type.GetType() == typeof(ErrorTypeDescriptor) || 
              lhs.type.GetType() == typeof(ErrorTypeDescriptor)) {
            node.type = errorType;
          } else if (lhs.type.GetType() != typeof(IntegerTypeDescriptor) ||
              rhs.type.GetType() != typeof(IntegerTypeDescriptor)) {
            node.type = errorType;
            err("Unsupported operation. Expected type INT.");
          } else node.type = lhs.type;
          break;

        case ExprKind.OP_LOR:
        case ExprKind.OP_LAND:
        case ExprKind.PIPE:
        case ExprKind.HAT:
        case ExprKind.AND:
          if (lhs.type.GetType() == typeof(ErrorTypeDescriptor) ||
              rhs.type.GetType() == typeof(ErrorTypeDescriptor) ) {
            node.type = errorType;
          } else if (lhs.type.GetType() != typeof(BooleanTypeDescriptor) ||
              rhs.type.GetType() != typeof(BooleanTypeDescriptor)) {
            node.type = errorType;
            err("Unsupported operation. Expected type BOOLEAN.");
          } else node.type = ((TypeAttributes)table.lookup("BOOLEAN")).type;
          break;
        case ExprKind.OP_EQ:
        case ExprKind.OP_NE:
        case ExprKind.OP_GT:
        case ExprKind.OP_LT:
        case ExprKind.OP_LE:
        case ExprKind.OP_GE:        
          if (lhs.type.GetType() == typeof(ErrorTypeDescriptor) ||
              rhs.type.GetType() == typeof(ErrorTypeDescriptor)) {
            node.type = errorType;
          } else if (!(lhs.type is PrimitiveValueTypeDescriptor) ||
              !(rhs.type is PrimitiveValueTypeDescriptor) ||
              !lhs.type.GetType().Equals(rhs.type.GetType())) {
            node.type = errorType;
            err("Unsupported operation. Expected same types across the operand with INT or STRING.");
          } else node.type = ((TypeAttributes)table.lookup("BOOLEAN")).type;
          break;
      }
    }

    public void VisitNode(INT_CONST node)
    {
      Trace(node);

      node.type = ((TypeAttributes)table.lookup("INT")).type;
    }

    //================================================================//
    public void err(string s)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("---ERROR: " + s);
      Console.ResetColor();
    }

    public void getModifiers(Modifiers node, List<ModifierType> mods)
    {
      foreach (ModifierType mod in node.ModifierTokens)
        mods.Add(mod);
    }

    public void VisitNode(ClassDeclaration node)
    {
      Trace(node);

      ClassTypeDescriptor classType = new ClassTypeDescriptor();
      ClassAttributes classAtr = new ClassAttributes(classType);

      dynamic mods = node.Child;
      dynamic name = mods.Sib;
      dynamic body = name.Sib;

      classType.className = name.Name; // Set type name      
      classType.type = classType.className;

      name.type = classType;
      if (!table.enter(classType.className, classAtr)) // outer scope, enter symbol
        Console.WriteLine("Redeclaration of symbol " + classType.className);

      classType.locals = table.openScope(); // Associate inner scope with class type
      table.changeScopeContext(classAtr);

      getModifiers(mods, classType.modifiers);

      VisitNode(body);
      table.closeScope();
      node.type = name.type;
    }

    public void VisitNode(ClassBody node)
    {
      Trace(node);     

      VisitChildren(node);
    }

    public void VisitNode(StructDeclaration node)
    {
      Trace(node);

      StructTypeDescriptor classType = new StructTypeDescriptor();
      StructAttributes classAtr = new StructAttributes(classType);

      dynamic mods = node.Child;
      dynamic name = mods.Sib;
      dynamic body = name.Sib;

      classType.className = name.Name; // Set type name   
      ObjectTypeAttributes outer = (ObjectTypeAttributes)table.getScopeContext();
      classType.type = outer.type.type + "/" + classType.className;

      name.type = classType;
      if (!table.enter(classType.className, classAtr)) // outer scope, enter symbol
        Console.WriteLine("Redeclaration of symbol " + classType.className);

      classType.locals = table.openScope(); // Associate inner scope with class type
      table.changeScopeContext(classAtr);

      getModifiers(mods, classType.modifiers);

      VisitNode(body);
      table.closeScope();
      node.type = name.type;
    }

    public void VisitNode(PrimitiveType node)
    {
      Trace(node);

      if (node.Type == EnumPrimitiveType.VOID) {
        node.type = ((TypeAttributes)table.lookup("VOID")).type;
      } else if (node.Type == EnumPrimitiveType.BOOLEAN) {
        node.type = ((TypeAttributes)table.lookup("BOOLEAN")).type;
      } else if (node.Type == EnumPrimitiveType.INT) {
        node.type = ((TypeAttributes)table.lookup("INT")).type;
      }
    }   

    public void VisitNode(QualifiedName node)
    {
      Trace(node);

      Lookupable ctx = table;
      Identifier prev = null;
      foreach (Identifier id in node.Children()) {
        ctx = VisitContext(id, ctx);
        node.Name += id.Name + " ";
        node.type = id.type;
        node.atr = id.atr;
        if (prev != null) {
          node.invokedOn = id.Name;

          id.invokedOn = prev.Name;
        }
        prev = id;
        // TODO: Error report when doing ClassA.a.b.c where a doesn't return a context
      }
    }

    // Returns null when no context can be referenced from
    public SymScope VisitContext(Identifier node, Lookupable curCtx)
    {
      node.type = errorType;
      if (curCtx == null) return null;
      node.atr = curCtx.lookup(node.Name);

      if (node.atr == null) {
        err(node.Name + " not found!");
      } else {        
        if (node.atr is ObjectTypeAttributes) {
          ObjectTypeDescriptor type = ((ObjectTypeAttributes)node.atr).type;
          node.type = type;

          return type.locals;
        } else if (node.atr is MethodAttributes) {
          node.type = ((MethodAttributes)node.atr).returnType;

          return null;
        } else if (node.atr is VariableAttributes) {
          node.type = ((VariableAttributes)node.atr).type;

          if (node.type is ObjectTypeDescriptor)
            return ((ObjectTypeDescriptor)node.type).locals;

          return null;
        } else if (node.atr is BuiltInTypeAttributes) {
          node.type = ((BuiltInTypeAttributes)node.atr).type;

          return null;
        } else {
          node.type = errorType;
        }
      }

      return null;
    }

    public void VisitNode (ArraySpecifier node)
    {
      // Resolve array type... account for multi-dimensional arrays (not supported by the grammar)
      VisitChildren(node);
      TypeDescriptor arrType = node.Child.type;
      if (arrType == errorType)
        node.type = errorType;
      else
        node.type = new ArrayTypeDescriptor(arrType);
    }

    public void VisitNode(FieldDeclaration node)
    {
      Trace(node);

      dynamic modifiers = node.Child;
      dynamic typeSpec = modifiers.Sib;
      VisitNode(typeSpec);
      dynamic fieldNames = typeSpec.Sib;

      TypeDescriptor type = typeSpec.type;
      if (type.GetType() == typeof(VoidTypeDescriptor)){
        err("Undeclarable type VOID");
        type = errorType;
      }

      node.type = type;

      foreach (Identifier id in fieldNames.Children()) {
        // Insert into symbol table with correct type
        // Symbol Table will handle redeclarations
        FieldAttributes atr = new FieldAttributes(type, id.Name);
        foreach (ModifierType mod in modifiers.ModifierTokens)
          atr.modifiers.Add(mod);

        atr.foundIn = ((ObjectTypeAttributes)table.getScopeContext()).type.type;
        atr.type = type;

        id.type = type;
        id.atr = atr;
        if (!table.enter(id.Name, atr)) {
          node.type = errorType;
          Console.WriteLine("Redeclaration of symbol " + id.Name);
        }
      }
    }

    public void VisitNode(LocalVariableDeclaration node)
    {
      Trace(node);

      if (node.sr == null) {
        dynamic typeSpec = node.Child;
        VisitNode(typeSpec);
        dynamic fieldNames = typeSpec.Sib;

        TypeDescriptor type = typeSpec.type;
        if (type.GetType() == typeof(VoidTypeDescriptor)) {
          err("Undeclarable type VOID");
          type = errorType;
        }

        foreach (Identifier id in fieldNames.Children()) {
          // Insert into symbol table with correct type
          // Symbol Table will handle redeclarations
          VariableAttributes atr = new VariableAttributes(type, id.Name);
          id.type = type;
          atr.isLocal = true;
          id.atr = atr;
          

          if (!table.enter(id.Name, atr)) {
            node.type = errorType;
            Console.WriteLine("Redeclaration of symbol " + id.Name);
          }
        }
      } else {
        // TODO: Visit struct
      }
    }

    public MethodSignatureAttributes getSignature(MethodSignature sig, SymScope scope)
    {
      dynamic id = sig.Child;
      dynamic paraList = id.Sib;      

      MethodSignatureAttributes atr = new MethodSignatureAttributes(id.Name);
      sig.atr = atr;
      if (paraList != null) {
        foreach(Parameter para in paraList.Children()) {
          dynamic typeSpec = para.Child;
          dynamic pID = typeSpec.Sib;

          VisitNode(typeSpec);
          pID.type = typeSpec.type;
          VariableAttributes varAtr = new VariableAttributes(pID.type, pID.Name);
          varAtr.isParam = true;
          varAtr.isLocal = true;
          varAtr.initialized = true;
          pID.atr = varAtr;
          if (!scope.enter(pID.Name, varAtr)) {
            pID.type = errorType;
            Console.WriteLine("Redeclaration of symbol " + pID.Name);
          }
          atr.parameters.Add(varAtr);
        }
      }

      return atr;
    }
/*
    public void resolveParamType(QualifiedName node)
    {
      // lookup type
    }

    public void resolveParamType(PrimitiveType node)
    {
      VisitNode(node);
    }*/

    public void VisitNode(ConstructorDeclaration node)
    {
      Trace(node);

      ConstructorAttributes atr = new ConstructorAttributes();
      node.atr = atr;

      dynamic mods = node.Child;
      dynamic typeSpec = mods.Sib;
      VisitNode(typeSpec);
      dynamic sig = typeSpec.Sib;
      dynamic block = sig.Sib;

      getModifiers(mods, atr.modifiers);

      // Extract Signature
      MethodSignatureAttributes sigAtr = getSignature(sig, table.openScope());

      if (atr.signatures.Contains(sigAtr)) { // Check of overloadable
        node.type = errorType;
        err("Method of same signature already exists.");
      } else {
        atr.signatures.Add(sigAtr);

        table.changeScopeContext(atr);
        if (table.getOuterContext().GetType() == typeof(ClassAttributes)) {
          ((ClassTypeDescriptor)((ClassAttributes)table.getOuterContext()).type).constructor = atr;
          sig.Child.type = ((ClassAttributes)table.getOuterContext()).type;
        } else {
          err("Constructor declared outside of class context!");
          sig.Child.type = errorType;
        }
      }      

      // Visit Block
      VisitChildren(block);

      table.closeScope();
    }

    public void VisitNode(MethodDeclaration node)
    {
      Trace(node);

      MethodAttributes atr = new MethodAttributes();
      node.atr = atr;

      dynamic mods = node.Child;
      dynamic typeSpec = mods.Sib;
      VisitNode(typeSpec);
      dynamic sig = typeSpec.Sib;
      dynamic block = sig.Sib;

      getModifiers(mods, atr.modifiers);

      // Extract return type
      atr.returnType = typeSpec.type;
      atr.foundIn = ((ObjectTypeAttributes)table.getScopeContext()).type.type;

      // Extract Signature
      MethodSignatureAttributes sigAtr = getSignature(sig, table.openScope());

      if (atr.signatures.Contains(sigAtr)) { // Check of overloadable
        node.type = errorType;
        err("Method of same signature already exists.");
      } else {
        atr.signatures.Add(sigAtr);
        sig.Child.type = atr.returnType;

        table.changeScopeContext(atr);
        if (!table.enterIntoOuterScope(sigAtr.name, atr)) {
          Console.WriteLine("Redeclaration of symbol " + sigAtr.name);
          node.type = errorType;
        }

        node.type = typeSpec.type;
      }
      // Visit Block
      VisitChildren(block);

      table.closeScope();
    }
    public void VisitNode(MethodCall node)
    {
      Trace(node);

      dynamic name = node.Child;
      dynamic argList = name.Sib;

      VisitChildren(node);
      node.invokedOn = name.invokedOn;
      // Find method in symbol table
      Attributes atr = name.atr;
      if (atr == null) {
        node.type = errorType;
        err("Reference " + name.Name + " not found.");
      } else if (!(atr is MethodAttributes)) {
        node.type = errorType;
        err("Reference " + name.Name + " is not a Method.");
      } else {
        node.type = name.type;

        MethodAttributes mAtr = (MethodAttributes)atr;
        // Check signature
        bool found = false;

        node.atr = new MethodCallAttributes();
        node.atr.mAtr = mAtr;

        List<TypeDescriptor> sigList;

        if (argList == null) sigList = new List<TypeDescriptor>();
        else sigList = argList.sig;

        foreach (MethodSignatureAttributes sig in mAtr.signatures) {
          if (sig.parameters.Count == sigList.Count) {
            bool flag = true;
            for (int i = 0; i < sigList.Count && flag; i++) {
              if (!sig.parameters[i].type.GetType().Equals(sigList[i].GetType())) {
                flag = false;
                /*Console.WriteLine("Expected: " + sig.parameters[i].type.GetType()
                                + ". Actual: " + argList.sig[i].GetType());*/
              }
            }
            if (flag) {
              node.atr.sig = sigList;
              node.atr.name = name.Name;
              dynamic last = null;
              foreach (Identifier id in name.Children()) last = id;
              node.atr.relativeName = last.Name;              
              found = true;
              break;
            }
          }
        }

        if (!found) {
          node.type = errorType;
          string args = "";
          if (argList != null) args = argList.ToString();
          err("Cannot find " + name.Name + "(" + args + ")");
        }
      }
    }

    public void VisitNode(ArgumentList node)
    {
      Trace(node);

      VisitChildren(node);
      //Console.WriteLine(node.Child.type.GetType());
      foreach (dynamic child in node.Children())
        node.sig.Add(child.type);
    }

    public void VisitNode(STR_CONST node)
    {
      Trace(node);
      node.type = ((TypeAttributes)table.lookup("String")).type;
    }

    public void VisitNode(IterationStatement node)
    {
      Trace(node);
      table.openScope();
      VisitChildren(node);
      table.closeScope();
      node.type = errorType;

      dynamic expr = node.Child;

      if (expr.type.GetType() != typeof(BooleanTypeDescriptor)
        && expr.type.GetType() != typeof(ErrorTypeDescriptor)) {
        err("Loop condition is not of type BOOL.");
      } else {
        node.type = expr.type;
      }
    }

    public void VisitNode(ReturnStatement node)
    {
      Trace(node);
      VisitChildren(node);
      node.type = errorType;
      Attributes ctx = table.getScopeContext();

      if ((ctx == null || !(ctx is MethodAttributes))
        && ( (ctx = table.getOuterContext()) == null || !(ctx is MethodAttributes))) {
          err("Return statement outside of Method context.");
      } else {
        MethodAttributes atr = (MethodAttributes)ctx;
        node.type = atr.returnType;
        if (atr.returnType.GetType() != node.type.GetType()) {

          err("Incorrect return type. Expecting " + atr.returnType.GetType() 
            + " got " + node.type.GetType());

          node.type = errorType;
        }
      }      
    }

    public void VisitNode(SelectionStatement node)
    {
      Trace(node);
      table.openScope();
      VisitChildren(node);
      table.closeScope();

      node.type = errorType;

      dynamic expr = node.Child;

      if (expr.type.GetType() != typeof(BooleanTypeDescriptor)
        && expr.type.GetType() != typeof(ErrorTypeDescriptor)) {
        err("IF-statement condition is not of type BOOL.");
      } else {
        node.type = expr.type;
      }
    }
  }

}

