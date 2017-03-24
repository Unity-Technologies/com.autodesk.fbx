#!/bin/sh -v

mcs -lib:./lib/Release_x64 -r:CppSharp.AST.dll -r:CppSharp.dll -r:CppSharp.Generator.dll -r:CppSharp.Parser.dll -r:CppSharp.Parser.CSharp.dll -lib:Mono.Cecil.0.9.6.4/lib/net45 -r:Mono.Cecil.dll ./FbxSharp.cs
MONO_PATH=lib/Release_x64 mono FbxSharp.exe
