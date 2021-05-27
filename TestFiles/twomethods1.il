.assembly extern mscorlib {}
.assembly test1 { }
.class public twomethods1
{
    .method public static void check (int32) cil managed
    {
        .maxstack 20
        ldstr "The argument to check was "
        call void [mscorlib]System.Console::Write (string)
        ldarg 0
        call void [mscorlib]System.Console::WriteLine (int32)
        ret
    }
    .method public static void main ()
    {
        .entrypoint
        .maxstack 20
        ldc.i4 12345
        call void twomethods1::check(int32)
        ldstr "Back in main"
        call void [mscorlib]System.Console::WriteLine (string)
        ret
    }
}
