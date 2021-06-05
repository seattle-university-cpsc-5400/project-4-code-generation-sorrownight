using System.Collections.Generic;

namespace ASTBuilder
{

  public class CompilationUnit : AbstractNode
  {
    // just for the CompilationUnit because it's the top node
    //public override AbstractNode LeftMostSibling => this;
    public override AbstractNode Sib => null;

    public CompilationUnit(AbstractNode classDecl)
    {
      adoptChildren(classDecl);
    }

  }

  public class ClassDeclaration : AbstractNode
  {
    public ClassDeclaration(
        AbstractNode modifiers,
        AbstractNode className,
        AbstractNode classBody)
    {
      adoptChildren(modifiers);
      adoptChildren(className);
      adoptChildren(classBody);
    }

  }
  // Modified
  public class ClassBody : AbstractNode
  {
    public ClassBody()
    {

    }
    public ClassBody(AbstractNode members)
    {
      adoptChildren(members);
    }
  }
  public enum ModifierType { PUBLIC, STATIC, PRIVATE }

  public class Modifiers : AbstractNode
  {
    public List<ModifierType> ModifierTokens { get; set; } = new List<ModifierType>();

    public void AddModType(ModifierType type)
    {
      ModifierTokens.Add(type);
    }

    public Modifiers(ModifierType type) : base()
    {
      AddModType(type);
    }

  }
  public class Identifier : AbstractNode
  {
    public string Name;
    public Attributes atr;
    public string invokedOn;
    public Identifier(string s)
    {
      invokedOn = "this";
      Name = s;
    }
  }
  public class INT_CONST : AbstractNode
  {
    public virtual string IntVal { get; protected set; }

    public INT_CONST(string s)
    {
      IntVal = s;
    }
  }

  public class STR_CONST : AbstractNode
  {
    public virtual string StrVal { get; protected set; }

    public STR_CONST(string s)
    {
      StrVal = s;
    }
  }


  public class MethodDeclaration : AbstractNode
  {
    public MethodAttributes atr;
    public MethodDeclaration(
        AbstractNode modifiers,
        AbstractNode typeSpecifier,
        AbstractNode methodSignature,
        AbstractNode methodBody)
    {
      adoptChildren(modifiers);
      adoptChildren(typeSpecifier);
      adoptChildren(methodSignature);
      adoptChildren(methodBody);
    }

  }

  public enum EnumPrimitiveType { BOOLEAN, INT, VOID }
  public class PrimitiveType : AbstractNode
  {
    public EnumPrimitiveType Type { get; set; }
    public PrimitiveType(EnumPrimitiveType type)
    {
      Type = type;
    }

  }

  public class Parameter : AbstractNode
  {
    public Parameter(AbstractNode typeSpec, AbstractNode declName) : base()
    {
      adoptChildren(typeSpec);
      adoptChildren(declName);
    }
  }

  public class ParameterList : AbstractNode
  {
    public ParameterList(AbstractNode parameter) : base()
    {
      adoptChildren(parameter);
    }
  }

  public class MethodSignature : AbstractNode
  {
    public MethodSignatureAttributes atr;
    public MethodSignature(AbstractNode name)
    {
      adoptChildren(name);
    }

    public MethodSignature(AbstractNode name, AbstractNode paramList)
    {
      adoptChildren(name);
      adoptChildren(paramList);
    }
  }

  public enum ExprKind
  {
    EQUALS, OP_LOR, OP_LAND, PIPE, HAT, AND, OP_EQ,
    OP_NE, OP_GT, OP_LT, OP_LE, OP_GE, PLUSOP, MINUSOP,
    ASTERISK, RSLASH, PERCENT, UNARY, PRIMARY
  }
  public class Expression : AbstractNode
  {
    public ExprKind exprKind { get; set; }
    public Expression(ExprKind kind)
    {
      exprKind = kind;
    }
    public Expression(AbstractNode lhs, ExprKind kind, AbstractNode rhs)
    {
      adoptChildren(lhs);
      adoptChildren(rhs);
      exprKind = kind;
    }

  }

  // NEW NODE TYPES

  public class VariableDeclaration : AbstractNode
  {
    public AbstractNode nodeType;    
  }

  public class FieldDeclaration : VariableDeclaration
  {
    public FieldDeclaration(AbstractNode modifiers, AbstractNode typeSpecifier, AbstractNode fieldNames)
    {
      adoptChildren(modifiers);
      adoptChildren(typeSpecifier);
      adoptChildren(fieldNames);
      nodeType = typeSpecifier;
    }
  }
  public class StructDeclaration : AbstractNode
  {
    public StructDeclaration(AbstractNode modifiers, AbstractNode fieldNames, AbstractNode classBody)
    {
      adoptChildren(modifiers);
      adoptChildren(fieldNames);
      adoptChildren(classBody);
    }
  }

  public class ConstructorDeclaration : AbstractNode
  {
    public ConstructorAttributes atr;
    public ConstructorDeclaration(AbstractNode modifiers, AbstractNode methodSig, AbstractNode block)
    {
      adoptChildren(modifiers);
      adoptChildren(methodSig);
      adoptChildren(block);
    }
  }

  public class LocalVariableDeclaration : VariableDeclaration
  {
    public NameList ids;
    public StructDeclaration sr;

    public LocalVariableDeclaration(AbstractNode structDcl)
    {
      adoptChildren(structDcl);
      sr = (StructDeclaration)structDcl;
    }

    public LocalVariableDeclaration(AbstractNode typeSpecifier, AbstractNode localVariableNames)
    {
      adoptChildren(typeSpecifier);
      adoptChildren(localVariableNames);

      nodeType = typeSpecifier;
      ids = (NameList)localVariableNames;
    }
  }

  public class EmptyStatement : AbstractNode
  {
    public EmptyStatement()
    {
    }    
  }

  public class SelectionStatement : AbstractNode
  {
    public SelectionStatement(AbstractNode expr, AbstractNode stmt1, AbstractNode stmt2)
    {
      adoptChildren(expr);
      adoptChildren(stmt1);
      adoptChildren(stmt2);
    }

    public SelectionStatement(AbstractNode expr, AbstractNode stmt)
    {
      adoptChildren(expr);
      adoptChildren(stmt);
    }
  }

  public class IterationStatement : AbstractNode
  {
    public IterationStatement(AbstractNode expr, AbstractNode stmt)
    {
      adoptChildren(expr);
      adoptChildren(stmt);
    }
  }


  public class ReturnStatement : AbstractNode
  {
    public ReturnStatement()
    {
    }
    public ReturnStatement(AbstractNode expr)
    {
      adoptChildren(expr);
    }
  }

  public class Block : AbstractNode
  {
    public Block()
    {
    }

    public Block(AbstractNode locItems)
    {
      adoptChildren(locItems);
    }
  }

  public class ArgumentList : AbstractNode
  {
    public List<TypeDescriptor> sig;
    public ArgumentList(AbstractNode expr)
    {
      sig = new List<TypeDescriptor>();
      adoptChildren(expr);
    }
    public override string ToString()
    {
      string s = "";
      foreach (TypeDescriptor p in sig)
        s += p.GetType() + " ";

      return s;
    }
  }

  public class FieldAccess : AbstractNode
  {
    public FieldAccess(AbstractNode name, AbstractNode id)
    {
      adoptChildren(name);
      adoptChildren(id);
    }
  }

  public class QualifiedName : AbstractNode
  {
    public string Name;
    public Attributes atr;
    public string invokedOn;

    public QualifiedName(AbstractNode id)
    {
      invokedOn = "this";
      adoptChildren(id);
    }
  }

  public class MethodCall : AbstractNode
  {
    public MethodCallAttributes atr;
    public string invokedOn;
    public MethodCall(AbstractNode mRef, AbstractNode argList)
    {
      adoptChildren(mRef);
      adoptChildren(argList);
    }

    public MethodCall(AbstractNode mRef)
    {
      adoptChildren(mRef);
    }
  }

  public class NameList : AbstractNode
  {
    public NameList(AbstractNode id)
    {
      adoptChildren(id);
    }
  }

  public class ArraySpecifier : AbstractNode
  {
    public ArraySpecifier(AbstractNode typeName)
    {
      adoptChildren(typeName);
    }
  }
  public enum ReservedNames
  {
    THIS, NULL
  }
  public class SpecialName : AbstractNode
  {
    public ReservedNames encName;

    public SpecialName(ReservedNames name)
    {
      encName = name;
    }
  }
}

