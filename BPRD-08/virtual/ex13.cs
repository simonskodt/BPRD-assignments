// micro-C example 13, in C#

class ex13 {
  static void Main(string[] args) {
    int n = int.Parse(args[0]);
    int y;
    y = 1889;
    while (y < n) {
      y = y + 1;
      if (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0))
        InOut.PrintI(y);
    }
    InOut.PrintC(10);
  }
}

class InOut {
  public static void PrintI(int i) { 
    System.Console.Write(i + " "); 
  }

  public static void PrintC(int c) { 
    System.Console.Write((char)c); 
  }
}

/*

VS2010/.NET 4.0.30319.1 on 2012-02-11 on WinXP SP3 32-bit,
Release build with optimization: 

--- C:\Documents and Settings\Administrator\My Documents\Visual Studio 2010\Projects\ConsoleApplication1\ConsoleApplication1\Program.cs 
      int n = int.Parse(args[0]);
00000000  push        ebp 
00000001  mov         ebp,esp 
00000003  push        edi 
00000004  push        esi 
00000005  cmp         dword ptr [ecx+4],0 
00000009  jbe         00000082 
0000000b  mov         esi,dword ptr [ecx+0Ch] 
0000000e  call        7692F580 
00000013  push        eax 
00000014  mov         ecx,esi 
00000016  mov         edx,7 
0000001b  call        76946400 
00000020  mov         edi,eax 
      int y;
      y = 1889;
00000022  mov         esi,761h 
      while (y < n) {
00000027  cmp         edi,761h 
0000002d  jle         0000006A 
        y = y + 1;
0000002f  inc         esi 
        if (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0))
00000030  mov         eax,esi 
00000032  and         eax,80000003h 
00000037  jns         0000003E 
00000039  dec         eax 
0000003a  or          eax,0FFFFFFFCh 
0000003d  inc         eax 
0000003e  test        eax,eax 
00000040  jne         00000066 
00000042  mov         eax,esi 
00000044  mov         ecx,64h 
00000049  cdq 
0000004a  idiv        eax,ecx 
0000004c  test        edx,edx 
0000004e  jne         0000005E 
00000050  mov         eax,esi 
00000052  mov         ecx,190h 
00000057  cdq 
00000058  idiv        eax,ecx 
0000005a  test        edx,edx 
0000005c  jne         00000066 
          InOut.PrintI(y);
0000005e  mov         ecx,esi 
00000060  call        dword ptr ds:[009B38A4h] 
      while (y < n) {
00000066  cmp         esi,edi 
00000068  jl          0000002F 
      }
      InOut.PrintC(10);
0000006a  call        76957060 
0000006f  mov         ecx,eax 
00000071  mov         edx,0Ah 
00000076  mov         eax,dword ptr [ecx] 
00000078  mov         eax,dword ptr [eax+2Ch] 
0000007b  call        dword ptr [eax+1Ch] 
0000007e  pop         esi 
    }
0000007f  pop         edi 
00000080  pop         ebp 
00000081  ret 
00000082  call        7627922C 
00000087  int         3 

--------------------------------------------------

Just the while loop and the statement before it:

Register esi holds y and register edi holds n.  Presumably the
convoluted way of computing y%4 in lines 32-3d is faster than the
general code using integer division in lines 44-4a and lines 52-58.  

22 mov  esi,761h            // y = 1889;
27 cmp  edi,761h                            
2d jle  0000006A            // if (n<=1889) skip 
2f inc  esi                 // y = y + 1;
30 mov  eax,esi            
32 and  eax,80000003h      
37 jns  0000003E           
39 dec  eax                
3a or   eax,0FFFFFFFCh     
3d inc  eax                
3e test eax,eax            
40 jne  00000066            // y%4!=0
42 mov  eax,esi            
44 mov  ecx,64h            
49 cdq                      // (sign-extend eax into edx)
4a idiv eax,ecx             // (int division, remainder in edx) 
4c test edx,edx            
4e jne  0000005E            // y%100!=0
50 mov  eax,esi            
52 mov  ecx,190h           
57 cdq                     
58 idiv eax,ecx            
5a test edx,edx            
5c jne  00000066            // y%400!=0
5e mov  ecx,esi 
60 call dword ptr ds:[...]  // InOut.PrintI(y)
66 cmp  esi,edi 
68 jl   0000002F            // while (y<n) ...
6a ...

*/
