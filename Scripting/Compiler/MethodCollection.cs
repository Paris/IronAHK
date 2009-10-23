using System;
using System.Reflection;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    internal enum ArgType { Variable, Literal, Expression }
    
    internal class MethodCollection : List<MethodInfo>
    {
        public MethodCollection() 
        {
        }

        public MethodInfo BestMatch(string Name, ArgType[] Args)
        {
            // HACK: need to normalise method name and parameters in lexing stage so compiler can find perfect match

            MethodInfo result = null;
            int resultParams = -1;

            foreach (MethodInfo writer in this)
            {
                if (!string.Equals(writer.Name, Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                ParameterInfo[] parameters = writer.GetParameters();

                if (parameters.Length == Args.Length)
                    return writer;
                else if (parameters.Length > resultParams)
                {
                    result = writer;
                    resultParams = parameters.Length;
                }
            }

            return result;
        }
    }
}