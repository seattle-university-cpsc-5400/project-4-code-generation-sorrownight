.assembly extern mscorlib {}
.assembly test1 { }
.class public fact2
{
    .method public static int32 factorial (int32) cil managed
    {
        .maxstack 20
        ldarg 0
        ldc.i4 0
        beq T1
        ldc.i4 0
        br E1
        T1: ldc.i4 1
        E1:
        brfalse F0
        ldc.i4 1
        ret
        F0:
        ldarg 0
        ldarg 0
        ldc.i4 1
        sub
        call int32 fact2::factorial(int32)
        mul
        ret
    }
    .method public static void main ()
    {
        .entrypoint
        .maxstack 20
        .locals init(int32)
        .locals init(int32)
        ldc.i4 0
        stloc 0
        ldstr "TCCL recursive factorial test"
        call void [mscorlib]System.Console::WriteLine (string)
        LOOP_2:
        ldloc 0
        ldc.i4 15
        blt T3
        ldc.i4 0
        br E3
        T3: ldc.i4 1
        E3:
        brfalse E2
        ldloc 0
        call int32 fact2::factorial(int32)
        stloc 1
        ldloc 1
        call void [mscorlib]System.Console::WriteLine (int32)
        ldloc 0
        ldc.i4 1
        add
        stloc 0
        br LOOP_2
        E2:
        ret
    }
}
