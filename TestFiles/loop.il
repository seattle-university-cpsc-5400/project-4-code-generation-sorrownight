.assembly extern mscorlib {}
.assembly test1 { }
.class public loop
{
    .method public static void main () cil managed
    {
        .entrypoint
        .maxstack 20
        .locals init(int32)
        .locals init(int32)
        ldc.i4 0
        stloc 0
        ldc.i4 1
        stloc 1
        ldstr "Beginning loop test"
        call void [mscorlib]System.Console::WriteLine (string)
        LOOP_0:
        ldloc 0
        ldc.i4 10
        blt T1
        ldc.i4 0
        br E1
        T1: ldc.i4 1
        E1:
        brfalse E0
        ldloc 1
        call void [mscorlib]System.Console::WriteLine (int32)
        ldloc 0
        ldc.i4 1
        add
        stloc 0
        ldloc 1
        ldloc 0
        mul
        stloc 1
        br LOOP_0
        E0:
        ret
    }
}
