using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    class MethodCollection : List<MethodInfo>
    {
        public ILMirror Mirror;
        
        public MethodCollection()
        {
            Mirror = new ILMirror();
        }
        
        public MethodInfo BestMatch(string name, int length)
        {
            MethodInfo result = null;
            var last = int.MaxValue;

            foreach (var writer in this)
            {
                // find method with same name (case insensitive)
                if (!name.Equals(writer.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                var param = writer.GetParameters().Length;

                if (param == length) // perfect match when parameter count is the same
                {
                    return writer;
                }
                else if (param > length && param < last) // otherwise find a method with the next highest number of parameters
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
