
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ex13
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ex13.exe
// MVID: {A7CBF56E-697D-44AA-9AB4-7AEDB11970CD}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03120000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ex13
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Main(string[] args) cil managed
  {
    .entrypoint
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldelem.ref
    IL_0003:  call       int32 [mscorlib]System.Int32::Parse(string)
    IL_0008:  stloc.0
    IL_0009:  ldc.i4     0x761
    IL_000e:  stloc.1
    IL_000f:  br.s       IL_002f

    IL_0011:  ldloc.1
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.4
    IL_0017:  rem
    IL_0018:  brtrue.s   IL_002f

    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.s   100
    IL_001d:  rem
    IL_001e:  brtrue.s   IL_0029

    IL_0020:  ldloc.1
    IL_0021:  ldc.i4     0x190
    IL_0026:  rem
    IL_0027:  brtrue.s   IL_002f

    IL_0029:  ldloc.1
    IL_002a:  call       void InOut::PrintI(int32)
    IL_002f:  ldloc.1
    IL_0030:  ldloc.0
    IL_0031:  blt.s      IL_0011

    IL_0033:  ldc.i4.s   10
    IL_0035:  call       void InOut::PrintC(int32)
    IL_003a:  ret
  } // end of method ex13::Main

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ex13::.ctor

} // end of class ex13

.class private auto ansi beforefieldinit InOut
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  PrintI(int32 i) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        [mscorlib]System.Int32
    IL_0006:  ldstr      " "
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::Write(string)
    IL_0015:  ret
  } // end of method InOut::PrintI

  .method public hidebysig static void  PrintC(int32 c) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  conv.u2
    IL_0002:  call       void [mscorlib]System.Console::Write(char)
    IL_0007:  ret
  } // end of method InOut::PrintC

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method InOut::.ctor

} // end of class InOut


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
