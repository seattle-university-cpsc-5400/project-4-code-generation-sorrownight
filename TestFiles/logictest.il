.assembly extern mscorlib {}
.assembly test1 { }
.class public logictest
{
    .method public static void outStrln (string) cil managed
    {
        .maxstack 20
        ldarg 0
        call void [mscorlib]System.Console::WriteLine (string)
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
        ldc.i4 1
        stloc 1
        ldstr "TCCL logic test"
        call void logictest::outStrln(string)
        ldloc 0
        ldc.i4 10
        blt T1
        ldc.i4 0
        br E1
        T1: ldc.i4 1
        E1:
        ldloc 1
        ldc.i4 1
        beq T2
        ldc.i4 0
        br E2
        T2: ldc.i4 1
        E2:
        and
        brfalse F0
        ldstr "In then part of & test"
        call void logictest::outStrln(string)
        br E0
        F0:
        ldloc 1
        stloc 0
        E0:
        ldloc 0
        ldc.i4 10
        blt T4
        ldc.i4 0
        br E4
        T4: ldc.i4 1
        E4:
        ldloc 1
        ldc.i4 0
        beq T5
        ldc.i4 0
        br E5
        T5: ldc.i4 1
        E5:
        and
        brfalse F3
        ldstr "In then part of 2nd & test erroneously"
        call void logictest::outStrln(string)
        br E3
        F3:
        ldstr "In else part of 2nd & test"
        call void logictest::outStrln(string)
        E3:
        ldloc 0
        ldc.i4 10
        beq T7
        ldc.i4 0
        br E7
        T7: ldc.i4 1
        E7:
        ldloc 1
        ldloc 0
        beq T8
        ldc.i4 0
        br E8
        T8: ldc.i4 1
        E8:
        or
        brfalse F6
        ldloc 0
        stloc 1
        br E6
        F6:
        ldstr "In else part of false | test"
        call void logictest::outStrln(string)
        E6:
        ldloc 0
        ldc.i4 1
        beq T10
        ldc.i4 0
        br E10
        T10: ldc.i4 1
        E10:
        ldloc 1
        ldc.i4 1
        beq T11
        ldc.i4 0
        br E11
        T11: ldc.i4 1
        E11:
        or
        brfalse F9
        ldstr "In then part of true | test"
        call void logictest::outStrln(string)
        br E9
        F9:
        ldstr "In else part of true | test erroneously"
        call void logictest::outStrln(string)
        E9:
        ret
    }
}
