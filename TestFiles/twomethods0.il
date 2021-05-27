.assembly extern mscorlib {}
.assembly test1 { }
.class public twomethods0
{
    .method public static void check () cil managed
    {
        .maxstack 20
        ldstr "In the check method"
        call void [mscorlib]System.Console::WriteLine (string)
        ret
    }
    .method public static void main ()
    {
        .entrypoint
        .maxstack 20
        call void twomethods0::check()
        ldstr "Back in main"
        call void [mscorlib]System.Console::WriteLine (string)
        ret
    }
}
