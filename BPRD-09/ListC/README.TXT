Compiling and loading the list-C evaluator and parser (file ListC/README)
-------------------------------------------------------------------------

Archive listc.zip contains the files used below.


Building the list-C command line compiler listcc:

   fslex --unicode CLex.fsl
   fsyacc --module CPar CPar.fsy

   fsc --standalone -r ~/fsharp/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ListCC.fs -o listcc.exe
      or if you have fsharpc as the compiler instead of fsc, e.g., Mono on Mac:
   fsharpc --standalone -r ~/fsharp/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ListCC.fs -o listcc.exe

Using the list-C command line compiler to compile program ex30.lc
to listmachine code (in file ex30.out):

   ListCC ex30.lc
     or
   mono listcc.exe ex30.lc

The list-C virtual machine, as provided, should compile and run on
both Windows and Unix systems in both 32 and 64 bit.

A Visual Studio 19 project is provided for Windows. On Unix you simply
use gcc in the directory containing the C source files:
  listmachine.c
  utils_unix.c
  utils_win.c

Compiling on Windows open the Visual Studio project and use Build. You
can configure build for 32 or 64 bit. You locate the listmachine
executable and use that to run the compiled list-C programs.

On Unix you compile with gcc -o listmachine listmachine.c and use the
executable. You are of course not using the Visual Studio project
files on Unix and may delete them.

For instance on MacOS (Unix):

$ pwd
.../ListC/ListVM/ListVM
$ ll
total 40
...    256 Oct 27 10:54 .
...    192 Oct 27 10:44 ..
...   5708 Oct 27 10:44 ListVM.vcxproj
...    961 Oct 27 10:44 ListVM.vcxproj.filters
...    165 Oct 27 10:44 ListVM.vcxproj.user
...  15879 Oct 27 10:44 listmachine.c
...    816 Oct 27 10:44 utils_unix.c
...    965 Oct 27 10:44 utils_win.c
$ gcc -o listmachine listmachine.c 
$ ll
...
...  17840 Oct 27 10:54 listmachine
...  15879 Oct 27 10:44 listmachine.c
...    816 Oct 27 10:44 utils_unix.c
...    965 Oct 27 10:44 utils_win.c
$ ./listmachine 
listmachine for 64 bit architecture compiled with GCC
Usage: listmachine [-trace] <programfile> <arg1> ...
$ 

Run examples (assuming listmachine is a symbolic link to
ListVM/ListVM/listmachine.exe

   ./listmachine ex30.out 8

 
