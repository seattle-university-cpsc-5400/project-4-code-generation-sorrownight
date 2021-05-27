.assembly extern mscorlib {}
.assembly test1 { }
.class public hello
{
    .method public static void main () cil managed
    {
        .entrypoint
        .maxstack 20
        ldstr "Hello, World"
        call void [mscorlib]System.Console::WriteLine (string)
        ret
    }
}
