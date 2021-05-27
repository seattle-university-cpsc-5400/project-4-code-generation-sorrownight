namespace ASTBuilder
{
  public interface Lookupable
  {
    Attributes lookup(string id);
  }
  public interface SymtabInterface : Lookupable
  {
    /// Open a new nested symbol table scope
    SymScope openScope();

    /// Close an existng nested scope
    /// There must be an existng scope to close
    void closeScope();

    int CurrentNestLevel { get; }

    bool enter(string id, Attributes attr);
    bool enterIntoOuterScope(string s, Attributes info);
    Attributes getOuterContext();
  }


}
