using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        List<object> Dissect(List<object> parts, int start, int end)
        {
            var extracted = new List<object>(end - start);

            for (int i = start; i < end; i++)
            {
                extracted.Add(parts[start]);
                parts.RemoveAt(start);
            }

            return extracted;
        }
    }
}
