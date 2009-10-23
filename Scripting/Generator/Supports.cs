using System;
using System.CodeDom.Compiler;

namespace IronAHK.Scripting
{
    partial class Generator
    {
        public bool Supports(GeneratorSupport supports)
        {
            switch (supports)
            {
                case GeneratorSupport.ArraysOfArrays: return true;
                case GeneratorSupport.AssemblyAttributes: return false;
                case GeneratorSupport.ChainedConstructorArguments: return false;
                case GeneratorSupport.ComplexExpressions: return true;
                case GeneratorSupport.DeclareDelegates: return false;
                case GeneratorSupport.DeclareEnums: return false;
                case GeneratorSupport.DeclareEvents: return false;
                case GeneratorSupport.DeclareIndexerProperties: return false;
                case GeneratorSupport.DeclareInterfaces: return false;
                case GeneratorSupport.DeclareValueTypes: return false;
                case GeneratorSupport.EntryPointMethod: return true;
                case GeneratorSupport.GenericTypeDeclaration: return false;
                case GeneratorSupport.GenericTypeReference: return false;
                case GeneratorSupport.GotoStatements: return true;
                case GeneratorSupport.MultidimensionalArrays: return false;
                case GeneratorSupport.MultipleInterfaceMembers: return false;
                case GeneratorSupport.NestedTypes: return false;
                case GeneratorSupport.ParameterAttributes: return false;
                case GeneratorSupport.PartialTypes: return false;
                case GeneratorSupport.PublicStaticMembers: return true;
                case GeneratorSupport.ReferenceParameters: return false;
                case GeneratorSupport.Resources: return true;
                case GeneratorSupport.ReturnTypeAttributes: return false;
                case GeneratorSupport.StaticConstructors: return false;
                case GeneratorSupport.TryCatchStatements: return false;
                case GeneratorSupport.Win32Resources: return true;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
