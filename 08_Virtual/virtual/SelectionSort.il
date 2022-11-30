    // method line 2
    .method public static hidebysig            
           default void SelectionSort (int32[] arr)  cil managed 
    {
        // Method begins at RVA 0x20b4
	// Code size 57 (0x39)
	.maxstack 4                   // related to analysis of program, not to size of stack
	.locals init (           
		int32	V_0,              // int i     
		int32	V_1,              // int least
		int32	V_2,              // int tmp
		int32	V_3)              // int j
	IL_0000:  ldc.i4.0            // push 0 onto the stack as int32
	IL_0001:  stloc.0             // pop a value from stack into local variable 0 (i = 0)
	IL_0002:  br.s IL_0032        // conditional jump, goes to IL_0032 (i < arr.Length)

    // least = i
	IL_0004:  ldloc.0             // load local variable 0 onto stack (i)
	IL_0005:  stloc.1             // pop a value from stack into local variable 1 (least = i)

    // push i+1
	IL_0006:  ldloc.0             // load local variable 0 onto stack (i)
	IL_0007:  ldc.i4.1            // push 1 onto the stack as int32
	IL_0008:  add                 // add two values, pushing a new value (i+1)

    // j = i+1
	IL_0009:  stloc.3             // pop a value from stack into local variable 3
	IL_000a:  br.s IL_001a        // conditional jump, goes to IL_001a (j < arr.Length)

    // arr[j] < arr[least]
	IL_000c:  ldarg.0             // load argument 0 onto the stack (arr)
	IL_000d:  ldloc.3             // load local variable 3 onto stack (j)
	IL_000e:  ldelem.i4           // load the array element with type int32 at index onto the top of the stack as an int32 (arr[j])
	IL_000f:  ldarg.0             // load argument 0 onto the stack (arr)
	IL_0010:  ldloc.1             // load local variable 1 onto stack (least)
	IL_0011:  ldelem.i4           // load the array element with type int32 at index onto the top of the stack as an int32 (arr[least])
	IL_0012:  bge.s IL_0016       // branch to target if greater than or equal to (arr[j] < arr[least])

    // least = j
	IL_0014:  ldloc.3             // load local variable 3 onto stack (j)
	IL_0015:  stloc.1             // pop a value from stack into local variable 1 (least = j)

    // j++
	IL_0016:  ldloc.3             // load local variable 3 onto stack (j)
	IL_0017:  ldc.i4.1            // push 1 onto the stack as int32
	IL_0018:  add                 // add two values, pushing a new value (j+1)
	IL_0019:  stloc.3             // pop a value from stack into local variable 3 (j++)

    // j < arr.Length 
	IL_001a:  ldloc.3             // load local variable 3 onto stack (j)
	IL_001b:  ldarg.0             // load argument 0 onto the stack (arr)
	IL_001c:  ldlen               // push the length of array on the stack (arr.Length)
	IL_001d:  conv.i4             // convert to int32, pushing int32 on stack
	IL_001e:  blt.s IL_000c       // branch to target IL_000c is less than

    // tmp = arr[i]
	IL_0020:  ldarg.0             // load argument 0 onto the stack (arr) 
	IL_0021:  ldloc.0             // load local variable 0 onto stack (i)
	IL_0022:  ldelem.i4           // load the element with type int32 at index onto the top of the stack as an int32 (arr[i])
	IL_0023:  stloc.2             // pop a value from stack into local variable 2 (tmp = arr[i])

    // arr[i] = arr[j]
	IL_0024:  ldarg.0             // load argument 0 onto the stack (arr) 
	IL_0025:  ldloc.0             // load local variable 0 onto stack (i)
	IL_0026:  ldarg.0             // load argument 0 onto the stack (arr) 
	IL_0027:  ldloc.1             // load local variable 1 onto stack (index)
	IL_0028:  ldelem.i4           // load the element with type int32 at index onto the top of the stack as an int32 (arr[least])
	IL_0029:  stelem.i4           // replace array element at index with the int32 value on the stack (arr[i] = arr[j])

    // arr[least] = tmp
	IL_002a:  ldarg.0             // load argument 0 onto the stack (arr) 
	IL_002b:  ldloc.1             // load local variable 1 onto stack (least)
	IL_002c:  ldloc.2             // load local variable 2 onto stack (tmp)
	IL_002d:  stelem.i4           // replace array element at index with the int32 value on the stack (arr[least] = tmp)

    // i++
	IL_002e:  ldloc.0             // load local variable 0 onto stack (i)
	IL_002f:  ldc.i4.1            // push 1 onto the stack as int32 
	IL_0030:  add                 // add two values, pushing a new value (i+1)
	IL_0031:  stloc.0             // pop a value from stack into local variable 0 (i = i + 1)

    // i < arr.Length
	IL_0032:  ldloc.0             // load local variable 0 onto stack (i)
	IL_0033:  ldarg.0             // load argument 0 onto the stack (arr) 
	IL_0034:  ldlen               // push the length of array on the stack (arr.Length)
	IL_0035:  conv.i4             // convert to int32, pushing int32 on stack
	IL_0036:  blt.s IL_0004       // branch to target IL_0004 is less than (i < arr.Length)

	IL_0038:  ret                 // return from method with no value
    } // end of method Selsort::SelectionSort