.assembly extern mscorlib {}
.assembly test1 { }
.class public iftest
{
    .method public static void main ()
    {
        .entrypoint
        .maxstack 20
        .locals init(int32)
        .locals init(int32)
        ldc.i4 0
        stloc 0
        ldc.i4 1
        stloc 1
        ldstr "TCCL if test"
        call void [mscorlib]System.Console::WriteLine (string)
        ldloc 0
        ldc.i4 10
        blt T1
        ldc.i4 0
        br E1
        T1: ldc.i4 1
        E1:
        brfalse F0
        ldstr "In then part"
        call void [mscorlib]System.Console::WriteLine (string)
        br E0
        F0:
        ldloc 1
        stloc 0
        E0:
        ldloc 0
        ldc.i4 10
        beq T3
        ldc.i4 0
        br E3
        T3: ldc.i4 1
        E3:
        brfalse F2
        ldloc 0
        stloc 1
        ldstr "This statement is in then part and should not be executed"
        call void [mscorlib]System.Console::WriteLine (string)
        br E2
        F2:
        ldstr "In else part"
        call void [mscorlib]System.Console::WriteLine (string)
        E2:
        ret
    }
}
