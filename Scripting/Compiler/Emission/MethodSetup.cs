using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class MethodWriter
    {
        ILGenerator Generator;

        MethodInfo ForceDecimal;
        MethodInfo ForceLong;
        MethodInfo ForceInt;
        MethodInfo ForceBool;
        MethodInfo ForceString;
        
        MethodInfo SetVariable;
        MethodInfo GetVariable;
        LocalBuilder VarsProperty;

        MethodCollection Lookup;
        ILMirror Mirror;

        public bool IsEntryPoint;
        public MethodBuilder Method;
        public Dictionary<string, MethodWriter> Methods; // Set in TypeEmitter.cs
        public Dictionary<string, Type[]> ParameterTypes;
        public CodeMemberMethod Member;

        int Depth;

        Stack<LoopMetadata> Loops;

        Dictionary<string, LocalBuilder> Locals;
        Dictionary<string, LabelMetadata> Labels;

        public MethodWriter(TypeBuilder Parent, CodeMemberMethod Member, MethodCollection Lookup, ILMirror Mirror)
        {
            Loops = new Stack<LoopMetadata>();

            this.Member = Member;
            this.Lookup = Lookup;
            this.Mirror = Mirror;

            if(Member is CodeEntryPointMethod)
            {
                Method = Parent.DefineMethod("Main", MethodAttributes.Private | MethodAttributes.Static, typeof(void), null);
                IsEntryPoint = true;
            }
            else Method = Parent.DefineMethod(Member.Name, MethodAttributes.Public | MethodAttributes.Static, typeof(object), new[] { typeof(object[]) });
            
            Generator = Method.GetILGenerator();

            ForceString = Mirror.GrabMethod(typeof(Script).GetMethod("ForceString"));
            ForceDecimal = Mirror.GrabMethod(typeof(Script).GetMethod("ForceDecimal"));
            ForceLong = Mirror.GrabMethod(typeof(Script).GetMethod("ForceLong"));
            ForceInt = Mirror.GrabMethod(typeof(Script).GetMethod("ForceInt"));
            ForceBool = Mirror.GrabMethod(typeof(Script).GetMethod("ForceBool"));
            
            Locals = new Dictionary<string, LocalBuilder>();
            Labels = new Dictionary<string, LabelMetadata>();
            
            Mirror.Sources.Add(typeof(Script.Variables));
            Type Variables = Mirror.GrabType(typeof(Script.Variables), true, false);
            
            if(IsEntryPoint)
                GenerateEntryPointHeader(Generator);
            
            // "Item" is the property for this-indexers
            SetVariable = Mirror.GrabMethod(typeof(Script.Variables).GetMethod("set_Item"));
            GetVariable = Mirror.GrabMethod(typeof(Script.Variables).GetMethod("get_Item"));
            
            VarsProperty = Generator.DeclareLocal(Variables);
            Generator.Emit(OpCodes.Call, Mirror.GrabMethod(typeof(Script).GetMethod("get_Vars")));
            Generator.Emit(OpCodes.Stloc, VarsProperty);            
        }

        void GenerateEntryPointHeader(ILGenerator Generator)
        {
            ConstructorInfo StatThreadConstructor = typeof(STAThreadAttribute).GetConstructor(Type.EmptyTypes);
            var Attribute = new CustomAttributeBuilder(StatThreadConstructor, new object[] {});
            Method.SetCustomAttribute(Attribute);
            
            Mirror.SimulateStaticConstructors(Generator);
        }

        [Conditional("DEBUG")]
        void Debug(string Message)
        {
            Console.Write(new string(' ', Depth));
            Console.WriteLine(Message);
        }
    }
}
