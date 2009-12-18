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

        public MethodInfo BestMatch(string name, ArgType[] args)
        {
            MethodInfo result = null;
            int length = args.Length, last = -1;

            foreach (MethodInfo writer in this)
            {
                // find method with same name (case insensitive)
                if (!name.Equals(writer.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                var param = writer.GetParameters().Length;

                if (param == length) // perfect match when parameter count is the same
                    return writer;
                else if (param < length && param > last) // otherwise find a method with the next highest number of parameters
                {
                    result = writer;
                    last = param;
                }
                else if (result == null) // return the first method with excess parameters as a last resort 
                    result = writer;
            }

            return result;
        }
    }
}