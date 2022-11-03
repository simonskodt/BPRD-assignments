Illustrating the Java Virtual Machine and the .NET Common Language Runtime
--------------------------------------------------------------------------

Archive virtual.zip contains the files mentioned below:


A. An implementation of selection sort in Java as well as C#.  

   This will compile the C# version and then disassemble its .NET
   Common Language Runtime bytecode into text file Selsort.il:

     csc /o Selsort.cs
     ildasm /text Selsort.exe > Selsort.il

   This will compile the Java version and then disassemble its Java
   Virtual Machine bytecode into text file Selsort.jvmbytecode:

     javac Selsort.java
     javap -verbose -c Selsort > Selsort.jvmbytecode


B. The programs CircularQueue.java and CircularQueue.cs implement a
   generic class CircularQueue<T>.  Compiling and then disassembling
   them to bytecode shows that the type parameters such as T have been
   erased in the Java bytecode, whereas they are preserved in the .NET
   Common Language Runtime bytecode, although renamed to !T or similar.

     javac CircularQueue.java 
     javap -verbose -c CircularQueue > CircularQueue.jvmbytecode

     csc /o CircularQueue.cs
     ildasm /text CircularQueue.exe > CircularQueue.il
   

C. The programs StringConcatSpeed.java and StringConcatSpeed.cs
   demonstrate how repeated string concatenation using the (+)
   operator is very slow and stresses the garbage collector.
 
