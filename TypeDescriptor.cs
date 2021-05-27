using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
  /******************************************/
  /*Describes type of node for decorated AST*/
  /******************************************/
  public abstract class TypeDescriptor
  {
    // All types include a string representation for code generation purposes.
    public string type;
    public bool isStatic;

    public void PrintType()
    {
      Console.WriteLine("    " + this.GetType().ToString());
    }
  }
  /******************************************/
  /*********Basic Primitive Types************/
  /******************************************/
  public class PrimitiveValueTypeDescriptor : TypeDescriptor
  {
    public PrimitiveValueTypeDescriptor()
    {
    }
  }
  public class IntegerTypeDescriptor : PrimitiveValueTypeDescriptor
  {
    public IntegerTypeDescriptor()
    {
      type = "int32";
    }
  }
  public class StringTypeDescriptor : PrimitiveValueTypeDescriptor
  {
    public StringTypeDescriptor()
    {
      type = "string";
    }
  }
  public class BooleanTypeDescriptor : TypeDescriptor
  {
    public BooleanTypeDescriptor()
    {
      type = "bool";
    }
  }
  public class VoidTypeDescriptor : TypeDescriptor
  {
    public VoidTypeDescriptor()
    {
      type = "void";
    }
  }


  /*****MSCoreLib Type Descriptor for mscorlib code generation*****/
  public class MSCorLibTypeDescriptor : TypeDescriptor
  {
    public MSCorLibTypeDescriptor()
    {
      type = "void [mscorlib]System.Console::";
    }
  }
  /*****Error Type Descriptor*****/
  public class ErrorTypeDescriptor : TypeDescriptor
  {
  }

  /*****Custom Type Descriptor*****/

  public class ObjectTypeDescriptor : TypeDescriptor
  {
    public string className;
    public SymScope locals;
    public List<ModifierType> modifiers;

    public ObjectTypeDescriptor()
    {
      type = "struct";
      modifiers = new List<ModifierType>();
    }
  }

  public class StructTypeDescriptor : ObjectTypeDescriptor
  {
    public StructTypeDescriptor()
    {
      type = "struct";
    }
  } 

  public class ClassTypeDescriptor : ObjectTypeDescriptor
  {
    public ConstructorAttributes constructor; // if empty, compiler generates

    public ClassTypeDescriptor()
    {
      type = "class";      
    }
  }

  // Each array is associated with its own TypeDescriptor (not shared)
  public class ArrayTypeDescriptor : TypeDescriptor
  {
    public int size; // Not supported by the grammar...
    public TypeDescriptor elementType;
    public ArrayTypeDescriptor(TypeDescriptor elementType)
    {
      this.elementType = elementType;
    }
  }
 }
