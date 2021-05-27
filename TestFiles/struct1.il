.assembly extern mscorlib {}
.assembly test1 { }
.class public structTest
{
    .class nested public sequential ansi sealed beforefieldinit test extends [System.Private.CoreLib]System.ValueType
    {
        .field public int32 control
        .method public void 'init' (int32) cil managed
        {
            .maxstack 20
            ldarg 0
            ldarg 1
            stfld int32 structTest/test::control
            ret
        }
        .method public int32 factorial (int32) cil managed
        {
            .maxstack 20
            ldarg 1
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
            ldarg 1
            ldarg 0
            ldarg 1
            ldc.i4 1
            sub
            call instance int32 structTest/test::factorial(int32)
            mul
            ret
        }
    }
    .field private static valuetype structTest/test t
    .method public static void main ()
    {
        .entrypoint
        .maxstack 20
        .locals init(int32)
        ldstr "TCCL test of factorial in a struct"
        call void [mscorlib]System.Console::WriteLine (string)
        ldc.i4 0
        call instance void structTest/test::'init'(int32)
        LOOP_2:
        ldfld structTest/test structTest::t
        ldfld int32 structTest/test::control
        ldc.i4 15
        blt T3
        ldc.i4 0
        br E3
        T3: ldc.i4 1
        E3:
        brfalse E2
        ldfld structTest/test structTest::t
        ldfld int32 structTest/test::control
        call instance int32 structTest/test::factorial(int32)
        stloc 0
        ldloc 0
        call void [mscorlib]System.Console::WriteLine (int32)
        ldfld structTest/test structTest::t
        ldfld int32 structTest/test::control
        ldc.i4 1
        add
        stfld int32 structTest/test::control
        br LOOP_2
        E2:
        ret
    }
}
