using System.CodeDom;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Generator
    {
        public string GetTypeOutput(CodeTypeReference type)
        {
            var name = new StringBuilder(type.BaseType.Length + type.ArrayRank * 2);

            string basename = type.BaseType;
            int z = basename.IndexOf('`');
            if (z != -1)
                basename = basename.Substring(0, z);
            name.Append(basename);

            if (type.TypeArguments.Count > 0)
            {
                name.Append('<');

                for (int i = 0; i < type.TypeArguments.Count; i++)
                {
                    name.Append(GetTypeOutput(type.TypeArguments[i]));
                    if (i > 0)
                        name.Append(", ");
                }

                name.Append('>');
            }

            for (int i = 0; i < type.ArrayRank; i++)
                name.Append("[]");

            return name.ToString();
        }
    }
}
