using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
  /****************************************************/
  /*Information about symbols held in AST nodes and the symbol table*/
  /****************************************************/
  public abstract class Attributes
  {

    public Attributes()
    {

    }

  }
  public class TypeAttributes : Attributes
  {
    public TypeDescriptor type;
    public TypeAttributes(TypeDescriptor type)
    {
      this.type = type;
    }
  }
  public class ObjectTypeAttributes : Attributes
  {
    public ObjectTypeDescriptor type;
    public ObjectTypeAttributes(ObjectTypeDescriptor type)
    {
      this.type = type;
    }
  }
  public class ClassAttributes : ObjectTypeAttributes
  {
    public ClassAttributes(ObjectTypeDescriptor type) : base (type)
    {
      this.type = type;
    }
  }

  public class StructAttributes : ClassAttributes
  {
    public StructAttributes(ObjectTypeDescriptor type) : base(type)
    {
      this.type = type;
    }
  }
  public class VariableAttributes : Attributes
  {
    public string id;
    public TypeDescriptor type;
    public bool initialized;
    public int stackLoc;
    public bool isParam;
    public string foundIn;

    public VariableAttributes(TypeDescriptor type, string id) 
    {
      this.type = type;
      this.id = id;
      initialized = false;
      foundIn = "";
    }
  }

  public class FieldAttributes : VariableAttributes
  {
    public List<ModifierType> modifiers;
    public FieldAttributes(TypeDescriptor type, string id) : base (type, id)
    {
      modifiers = new List<ModifierType>();
    }
  }  

  public class ConstructorAttributes : Attributes
  {
    public List<MethodSignatureAttributes> signatures; // Could have made it as set...
    public List<ModifierType> modifiers;              // But I don't want to deal with Hashing these

    public ConstructorAttributes()
    {
      modifiers = new List<ModifierType>();
      signatures = new List<MethodSignatureAttributes>();
    }

  }

  public class MethodAttributes : ConstructorAttributes
  {
    public TypeDescriptor returnType;
    public bool isStatic;
    public string foundIn;

    public MethodAttributes()
    {
      foundIn = "";
    }
  }

  public class MethodSignatureAttributes : Attributes
  {
    public string name;
    public List<VariableAttributes> parameters; // empty for no param

    public MethodSignatureAttributes(string name)
    {
      this.name = name;
      parameters = new List<VariableAttributes>();
    }

    public override bool Equals(object other)
    {
      if (other.GetType() != typeof(MethodSignatureAttributes)) return false;
      MethodSignatureAttributes otherMS = (MethodSignatureAttributes) other;

      if (parameters.Count != otherMS.parameters.Count) return false;
      for (int i = 0; i < parameters.Count; i++)
        if (parameters[i].type.GetType() != otherMS.parameters[i].type.GetType())
          return false;

      return name == otherMS.name;
    }
  }

  public class MethodCallAttributes : Attribute
  {
    public MethodAttributes mAtr;
    public List<TypeDescriptor> sig;
    public string name;
    public string relativeName;

    public MethodCallAttributes()
    {
      sig = new List<TypeDescriptor>();
    }
  }

  public class ErrorAttributes : Attributes
  {
    public ErrorAttributes()
    {
    }
  }

  public class ReservedAttributes : MethodAttributes
  {
    public TypeDescriptor type;
    public ReservedAttributes(TypeDescriptor type, TypeDescriptor returnType)
    {
      this.type = type;
      this.returnType = returnType;
    }
  }

  public class BuiltInTypeAttributes : TypeAttributes
  {
    public BuiltInTypeAttributes(TypeDescriptor type) : base(type)
    {
    }
  }
}
