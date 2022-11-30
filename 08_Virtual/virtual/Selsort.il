.assembly extern mscorlib
{
  .ver 4:0:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly 'Selsort'
{
  .custom instance void class [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::'.ctor'(int32) =  (01 00 08 00 00 00 00 00 ) // ........

  .custom instance void class [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::'.ctor'() =  (
		01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
		63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01       ) // ceptionThrows.

  .custom instance void class [mscorlib]System.Diagnostics.DebuggableAttribute::'.ctor'(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) =  (01 00 02 00 00 00 00 00 ) // ........

  .hash algorithm 0x00008004
  .ver  0:0:0:0
}
.module Selsort.exe // GUID = {C1611979-BBEE-49E1-8ADA-19C1569AB5F9}


  .class private auto ansi beforefieldinit Selsort
  	extends [mscorlib]System.Object
  {
    .field  public static initonly  class [mscorlib]System.Random rnd

    // method line 1
    .method public static hidebysig 
           default void Main (string[] args)  cil managed 
    {
        // Method begins at RVA 0x2050
	.entrypoint
	// Code size 87 (0x57)
	.maxstack 4
	.locals init (
		int32	V_0,
		int32[]	V_1,
		int32	V_2,
		int32	V_3)
	IL_0000:  ldc.i4.s 0x0a
	IL_0002:  stloc.0 
	IL_0003:  ldloc.0 
	IL_0004:  newarr [mscorlib]System.Int32
	IL_0009:  stloc.1 
	IL_000a:  ldc.i4.0 
	IL_000b:  stloc.2 
	IL_000c:  br.s IL_0024

	IL_000e:  ldloc.1 
	IL_000f:  ldloc.2 
	IL_0010:  ldsfld class [mscorlib]System.Random Selsort::rnd
	IL_0015:  ldc.i4 1000000
	IL_001a:  callvirt instance int32 class [mscorlib]System.Random::Next(int32)
	IL_001f:  stelem.i4 
	IL_0020:  ldloc.2 
	IL_0021:  ldc.i4.1 
	IL_0022:  add 
	IL_0023:  stloc.2 
	IL_0024:  ldloc.2 
	IL_0025:  ldloc.0 
	IL_0026:  blt.s IL_000e

	IL_0028:  ldloc.1 
	IL_0029:  call void class Selsort::SelectionSort(int32[])
	IL_002e:  ldc.i4.0 
	IL_002f:  stloc.3 
	IL_0030:  br.s IL_004d

	IL_0032:  ldloc.1 
	IL_0033:  ldloc.3 
	IL_0034:  ldelem.i4 
	IL_0035:  box [mscorlib]System.Int32
	IL_003a:  ldstr " "
	IL_003f:  call string string::Concat(object, object)
	IL_0044:  call void class [mscorlib]System.Console::Write(string)
	IL_0049:  ldloc.3 
	IL_004a:  ldc.i4.1 
	IL_004b:  add 
	IL_004c:  stloc.3 
	IL_004d:  ldloc.3 
	IL_004e:  ldloc.0 
	IL_004f:  blt.s IL_0032

	IL_0051:  call void class [mscorlib]System.Console::WriteLine()
	IL_0056:  ret 
    } // end of method Selsort::Main

    // method line 2
    .method public static hidebysig 
           default void SelectionSort (int32[] arr)  cil managed 
    {
        // Method begins at RVA 0x20b4
	// Code size 57 (0x39)
	.maxstack 4
	.locals init (
		int32	V_0,
		int32	V_1,
		int32	V_2,
		int32	V_3)
	IL_0000:  ldc.i4.0 
	IL_0001:  stloc.0 
	IL_0002:  br.s IL_0032

	IL_0004:  ldloc.0 
	IL_0005:  stloc.1 
	IL_0006:  ldloc.0 
	IL_0007:  ldc.i4.1 
	IL_0008:  add 
	IL_0009:  stloc.3 
	IL_000a:  br.s IL_001a

	IL_000c:  ldarg.0 
	IL_000d:  ldloc.3 
	IL_000e:  ldelem.i4 
	IL_000f:  ldarg.0 
	IL_0010:  ldloc.1 
	IL_0011:  ldelem.i4 
	IL_0012:  bge.s IL_0016

	IL_0014:  ldloc.3 
	IL_0015:  stloc.1 
	IL_0016:  ldloc.3 
	IL_0017:  ldc.i4.1 
	IL_0018:  add 
	IL_0019:  stloc.3 
	IL_001a:  ldloc.3 
	IL_001b:  ldarg.0 
	IL_001c:  ldlen 
	IL_001d:  conv.i4 
	IL_001e:  blt.s IL_000c

	IL_0020:  ldarg.0 
	IL_0021:  ldloc.0 
	IL_0022:  ldelem.i4 
	IL_0023:  stloc.2 
	IL_0024:  ldarg.0 
	IL_0025:  ldloc.0 
	IL_0026:  ldarg.0 
	IL_0027:  ldloc.1 
	IL_0028:  ldelem.i4 
	IL_0029:  stelem.i4 
	IL_002a:  ldarg.0 
	IL_002b:  ldloc.1 
	IL_002c:  ldloc.2 
	IL_002d:  stelem.i4 
	IL_002e:  ldloc.0 
	IL_002f:  ldc.i4.1 
	IL_0030:  add 
	IL_0031:  stloc.0 
	IL_0032:  ldloc.0 
	IL_0033:  ldarg.0 
	IL_0034:  ldlen 
	IL_0035:  conv.i4 
	IL_0036:  blt.s IL_0004

	IL_0038:  ret 
    } // end of method Selsort::SelectionSort

    // method line 3
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x20f9
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method Selsort::.ctor

    // method line 4
    .method private static hidebysig specialname rtspecialname 
           default void '.cctor' ()  cil managed 
    {
        // Method begins at RVA 0x2101
	// Code size 11 (0xb)
	.maxstack 8
	IL_0000:  newobj instance void class [mscorlib]System.Random::'.ctor'()
	IL_0005:  stsfld class [mscorlib]System.Random Selsort::rnd
	IL_000a:  ret 
    } // end of method Selsort::.cctor

  } // end of class Selsort

