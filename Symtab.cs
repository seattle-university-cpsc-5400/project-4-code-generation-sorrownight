using System;
using System.Collections.Generic;

namespace ASTBuilder
{

  public class SymScope : Lookupable
  {
    public Dictionary<string, Attributes> symbols;
    public Attributes curContext;

    public SymScope()
    {
      symbols = new Dictionary<string, Attributes>();
    }

    public Attributes lookup(string s)
    {
      Attributes tmp;
      if (symbols.TryGetValue(s, out tmp)) {
        return tmp;
      }
      return null;
    }

    public bool enter(string s, Attributes info)
    {
      if (symbols.ContainsKey(s)) {        
        return false;
      }
      symbols.Add(s, info);
      return true;
    }
  }

  /// Your implementation of Symtab should be MERGED with this file.
  /// Note what has been added to the constructor here.

  public class Symtab : SymtabInterface
  {
    /// The name makes the use of this field obvious
    /// It should never have a negative integer as its value
    public int CurrentNestLevel { get; private set; }
    public List<SymScope> scopes;

    public Symtab()
    {
      this.CurrentNestLevel = 0;
      scopes = new List<SymScope>();
      scopes.Add(new SymScope());

      // *** Do any setup necessary to create a global name scope
      // *** and then initialize it with built-in names ...
      EnterPredefinedNames();

    }
    /// <summary>
    /// Enter predefined names into symbol table.  
    /// </summary>
    public void EnterPredefinedNames()
    {
      TypeAttributes attr = new TypeAttributes(new IntegerTypeDescriptor());
      TypeDescriptor tmpInt = attr.type;
      enter("INT", attr);
      attr = new BuiltInTypeAttributes(new BooleanTypeDescriptor());
      enter("BOOLEAN", attr);
      attr = new BuiltInTypeAttributes(new VoidTypeDescriptor());
      enter("VOID", attr);
      TypeDescriptor voidType = attr.type;
      attr = new BuiltInTypeAttributes(new StringTypeDescriptor());
      enter("String", attr);

      ReservedAttributes tmpAtr = new ReservedAttributes(new MSCorLibTypeDescriptor(), voidType);
      MethodSignatureAttributes sig = new MethodSignatureAttributes("Write");
      tmpAtr.signatures.Add(sig);
      tmpAtr.modifiers.Add(ModifierType.PUBLIC);
      tmpAtr.modifiers.Add(ModifierType.STATIC);
      sig.parameters.Add(new VariableAttributes(attr.type, "s"));

      enter("Write", tmpAtr);

      sig = new MethodSignatureAttributes("Write");
      tmpAtr.signatures.Add(sig);
      tmpAtr.modifiers.Add(ModifierType.PUBLIC);
      tmpAtr.modifiers.Add(ModifierType.STATIC);
      sig.parameters.Add(new VariableAttributes(tmpInt, "num"));

      enter("Write", tmpAtr);

      tmpAtr = new ReservedAttributes(new MSCorLibTypeDescriptor(), voidType);
      sig = new MethodSignatureAttributes("WriteLine");
      tmpAtr.signatures.Add(sig);
      tmpAtr.modifiers.Add(ModifierType.PUBLIC);
      tmpAtr.modifiers.Add(ModifierType.STATIC);
      sig.parameters.Add(new VariableAttributes(attr.type, "s"));

      enter("WriteLine", tmpAtr);

      sig = new MethodSignatureAttributes("WriteLine");
      tmpAtr.signatures.Add(sig);
      tmpAtr.modifiers.Add(ModifierType.PUBLIC);
      tmpAtr.modifiers.Add(ModifierType.STATIC);
      sig.parameters.Add(new VariableAttributes(tmpInt, "num"));

      enter("WriteLine", tmpAtr);
    }

    public virtual void changeScopeContext(Attributes atr)
    {
      scopes[CurrentNestLevel].curContext = atr;
    }

    public virtual Attributes getScopeContext()
    {
      return scopes[CurrentNestLevel].curContext;
    }


    /// <summary>
    /// Opens a new scope, retaining outer ones 
    /// MODIFIED: return current scope when invoked
    /// </summary>
    public virtual SymScope openScope()
    {
      SymScope cur = new SymScope();
      CurrentNestLevel++;
      scopes.Add(cur);

      return cur;
    }

    /// <summary>
    /// Closes the innermost scope </summary>
    /// </summary>
    public virtual void closeScope()
    {
      if (CurrentNestLevel == 0)
        err("No inner scope to close!");
      scopes.RemoveAt(CurrentNestLevel--);
    }

    /// <summary>
    /// Enter the given symbol information into the symbol table.  If the given
    ///    symbol is already present at the current nest level, produce an error 
    ///    message, but do NOT throw any exceptions from this method.
    /// </summary>
    public virtual bool enter(string s, Attributes info)
    {
      return scopes[CurrentNestLevel].enter(s, info);
    }

    public virtual bool enterIntoOuterScope(string s, Attributes info)
    {
      return scopes[CurrentNestLevel-1].enter(s, info);
    }

    public virtual Attributes getOuterContext()
    {
      if (CurrentNestLevel == 0) return null;
      return scopes[CurrentNestLevel - 1].curContext;
    }

    /// <summary>
    /// Returns the information associated with the innermost currently valid
    ///     declaration of the given symbol.  If there is no such valid declaration,
    ///     return null.  Do NOT throw any excpetions from this method.
    /// </summary>
    public virtual Attributes lookup(string s)
    {
      Attributes tmp;

      for (int i = CurrentNestLevel; i >= 0; i--) {
        if (scopes[i].symbols.TryGetValue(s, out tmp)) {
          return tmp;
        }
      }
      return null;
    }

    public virtual void @out(string s)
    {
      string tab = "";
      for (int i = 1; i <= CurrentNestLevel; ++i) {
        tab += "  ";
      }
      Console.WriteLine(tab + s);
    }

    public virtual void err(string s)
    {
      @out("Error: " + s);
      //Console.Error.WriteLine("Error: " + s);
      Environment.Exit(-1);
    }

  }


}

