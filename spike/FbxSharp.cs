using System;
using System.IO;
using CppSharp;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.AST;
using CppSharp.Generators.CLI;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;
using CppSharp.Types;
using CppAbi = CppSharp.Parser.AST.CppAbi;

namespace FbxSharp
{
    class FbxSharp : ILibrary
    {
        public void Setup(Driver driver)
        {
            driver.Options.GeneratorKind = GeneratorKind.CSharp;
            driver.Options.LibraryName = "FbxSdk";
            driver.Options.Headers.Add("fbxsdk.h");
            driver.Options.SharedLibraryName = "libfbxsdk.dylib";
            driver.Options.OutputDir = "out/cppsharp";
            driver.Options.CompileCode = true;

            driver.ParserOptions.Abi = CppAbi.Itanium;
            driver.ParserOptions.AddIncludeDirs(Path.Combine(Directory.GetCurrentDirectory(), "2016.0/include"));
            driver.ParserOptions.AddLibraryDirs(Path.Combine(Directory.GetCurrentDirectory(), "2016.0/lib/clang/release"));
        }

        public void SetupPasses(Driver driver)
        {
            //driver.Context.TranslationUnitPasses.RenameDeclsUpperCase(RenameTargets.Any);
            //driver.Context.TranslationUnitPasses.AddPass(new FunctionToInstanceMethodPass());
            //driver.Context.TranslationUnitPasses.RemovePrefix("SDL_");
            //driver.Context.TranslationUnitPasses.RemovePrefix("SCANCODE_");
            //driver.Context.TranslationUnitPasses.RemovePrefix("SDLK_");
            //driver.Context.TranslationUnitPasses.RemovePrefix("KMOD_");
            //driver.Context.TranslationUnitPasses.RemovePrefix("LOG_CATEGORY_");
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
            //ctx.IgnoreEnumWithMatchingItem("SDL_FALSE");
            //ctx.IgnoreEnumWithMatchingItem("DUMMY_ENUM_VALUE");

            //ctx.SetNameOfEnumWithMatchingItem("SDL_SCANCODE_UNKNOWN", "ScanCode");
            //ctx.SetNameOfEnumWithMatchingItem("SDLK_UNKNOWN", "Key");
            //ctx.SetNameOfEnumWithMatchingItem("KMOD_NONE", "KeyModifier");
            //ctx.SetNameOfEnumWithMatchingItem("SDL_LOG_CATEGORY_CUSTOM", "LogCategory");

            //ctx.GenerateEnumFromMacros("InitFlags", "SDL_INIT_(.*)").SetFlags();
            //ctx.GenerateEnumFromMacros("Endianness", "SDL_(.*)_ENDIAN");
            //ctx.GenerateEnumFromMacros("InputState", "SDL_RELEASED", "SDL_PRESSED");
            //ctx.GenerateEnumFromMacros("AlphaState", "SDL_ALPHA_(.*)");
            //ctx.GenerateEnumFromMacros("HatState", "SDL_HAT_(.*)");

            //ctx.IgnoreHeadersWithName("SDL_atomic*");
            //ctx.IgnoreHeadersWithName("SDL_endian*");
            //ctx.IgnoreHeadersWithName("SDL_main*");
            //ctx.IgnoreHeadersWithName("SDL_mutex*");
            //ctx.IgnoreHeadersWithName("SDL_stdinc*");
            //ctx.IgnoreHeadersWithName("SDL_error");

            //ctx.IgnoreEnumWithMatchingItem("SDL_ENOMEM");
            //ctx.IgnoreFunctionWithName("SDL_Error");
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
            //ctx.SetNameOfEnumWithName("PIXELTYPE", "PixelType");
            //ctx.SetNameOfEnumWithName("BITMAPORDER", "BitmapOrder");
            //ctx.SetNameOfEnumWithName("PACKEDORDER", "PackedOrder");
            //ctx.SetNameOfEnumWithName("ARRAYORDER", "ArrayOrder");
            //ctx.SetNameOfEnumWithName("PACKEDLAYOUT", "PackedLayout");
            //ctx.SetNameOfEnumWithName("PIXELFORMAT", "PixelFormats");
            //ctx.SetNameOfEnumWithName("assert_state", "AssertState");
            //ctx.SetClassBindName("assert_data", "AssertData");
            //ctx.SetNameOfEnumWithName("eventaction", "EventAction");
            //ctx.SetNameOfEnumWithName("LOG_CATEGORY", "LogCategory");
        }
    }
}

static class Program
{
    public static void Main(string[] args)
    {
        ConsoleDriver.Run(new FbxSharp.FbxSharp());
    }
}
