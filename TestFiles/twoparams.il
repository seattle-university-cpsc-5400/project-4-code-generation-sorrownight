.assembly extern mscorlib {}
.assembly test1 { }
.class public twoparams
{
    .method public static void outTwoInts (int32, int32) cil managed
    {
        .maxstack 20
        ldarg 0
        call void [mscorlib]System.Console::Write (int32)
        ldstr " is the value of the first parameter"
        call void [mscorlib]System.Console::WriteLine (string)
        ldarg 1
        call void [mscorlib]System.Console::Write (int32)
        ldstr " is the value of the second parameter"
        call void [mscorlib]System.Console::WriteLine (string)
        ret
    }
    .method public static void main ()
    {
        .entrypoint
        .maxstack 20
        .locals init(int32)
        .locals init(int32)
        .locals init(int32)
        ldstr "TCCL test of method with two parameters test"
        call void [mscorlib]System.Console::WriteLine (string)
        ldc.i4 3
        ldc.i4 4
        add
        stloc 2
        ldc.i4 5
        ldc.i4 7
        mul
        stloc 1
        ldloc 2
        ldloc 1
        call void twoparams::outTwoInts(int32, int32)
        ret
    }
}
