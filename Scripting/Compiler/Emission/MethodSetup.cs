using System;
using System.Diagnostics;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
        TypeBuilder Parent;
        CodeMemberMethod Member;
        ILGenerator Generator;

        MethodInfo ForceFloat;
        MethodInfo ForceDecimal;
        MethodInfo ForceLong;
        MethodInfo ForceInt;
        MethodInfo ForceBool;
        MethodInfo ForceString;

        MethodCollection Lookup;

        public bool IsEntryPoint = false;
        public MethodBuilder Method;

        int Depth = 0;

        Stack<LoopMetadata> Loops;

        Dictionary<string, LocalBuilder> Locals;
        Dictionary<string, LabelMetadata> Labels;

        public MethodWriter(TypeBuilder Parent, CodeMemberMethod Member, MethodCollection Lookup)
        {
            Loops = new Stack<LoopMetadata>();

            this.Parent = Parent;
            this.Member = Member;
            this.Lookup = Lookup;

            if(Member is CodeEntryPointMethod)
            {
                Method = Parent.DefineMethod("Main", MethodAttributes.Private | MethodAttributes.Static,
                                                          typeof(void), new Type[] { typeof(string[]) });
                IsEntryPoint = true;
            }
            else Method = Parent.DefineMethod(Member.Name, MethodAttributes.Static);

            Generator = Method.GetILGenerator();

            if(IsEntryPoint)
                GenerateEntryPointHeader();

            ForceFloat = typeof(Script).GetMethod("ForceFloat");
            ForceString = typeof(Script).GetMethod("ForceString");
            ForceDecimal = typeof(Script).GetMethod("ForceDecimal");
            ForceLong = typeof(Script).GetMethod("ForceLong");
            ForceInt = typeof(Script).GetMethod("ForceInt");
            ForceBool = typeof(Script).GetMethod("ForceBool");
            
            Locals = new Dictionary<string, LocalBuilder>();
            Labels = new Dictionary<string, LabelMetadata>();
        }

        void GenerateEntryPointHeader()
        {
            ConstructorInfo StatThreadConstructor = typeof(System.STAThreadAttribute).GetConstructor(Type.EmptyTypes);
            CustomAttributeBuilder Attribute = new CustomAttributeBuilder(StatThreadConstructor, new object[] {});
            Method.SetCustomAttribute(Attribute);

            //Assembly Winforms = Assembly.LoadWithPartialName("System.Windows.Forms");
            const string WinForms = "System.Windows.Forms";
            Assembly Winforms = Assembly.Load(WinForms + ", Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Type Application = Winforms.GetType(WinForms + ".Application");

            if (Application == null)
                return;

            MethodInfo Enable = Application.GetMethod("EnableVisualStyles");
            Generator.Emit(OpCodes.Call, Enable);
        }

        [Conditional("DEBUG")]
        void Debug(string Message)
        {
            Console.Write(new string(' ', Depth));
            Console.WriteLine(Message);
        }
    }
}
