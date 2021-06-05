.assembly extern mscorlib {}
.assembly test1 { }
.class public structPoint
{
    .class nested public sequential ansi sealed beforefieldinit point extends [System.Private.CoreLib]System.ValueType
    {
        .field public int32 x
        .field public int32 y
        .method public void 'init' (int32, int32) cil managed
        {
            .maxstack 20
            ldarg 0
            ldarg 1
            stfld int32 structPoint/point::x
            ldarg 0
            ldarg 2
            stfld int32 structPoint/point::y
            ret
        }
        .method public int32 DotProduct (valuetype structPoint/point) cil managed
        {
            .maxstack 20
            ldarg 0
            ldfld int32 structPoint/point::x
            ldarg 1
            ldfld int32 structPoint/point::x
            mul
            ldarg 0
            ldfld int32 structPoint/point::y
            ldarg 1
            ldfld int32 structPoint/point::y
            mul
            add
            ret
        }
    }
    .field static valuetype structPoint/point p1
    .field static valuetype structPoint/point p2
    .method public static void main ()
    {
        .entrypoint
        .maxstack 20
        ldsflda valuetype  structPoint/point structPoint::p1
        ldc.i4 5
        ldc.i4 8
        call instance void structPoint/point::'init'(int32, int32)
        ldsflda valuetype  structPoint/point structPoint::p2
        ldc.i4 2
        ldc.i4 20
        call instance void structPoint/point::'init'(int32, int32)
        ldsflda valuetype  structPoint/point structPoint::p1
        ldsfld valuetype  structPoint/point structPoint::p2
        call instance int32 structPoint/point::DotProduct(valuetype structPoint/point)
        call void [mscorlib]System.Console::WriteLine (int32)
        ret
    }
}
