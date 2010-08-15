using System.Collections.Generic;
using System.Reflection;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        Dictionary<string, ParameterInfo[]> libMethods;
        Dictionary<string, string> libProperties;

        void ScanLibrary()
        {
            #region Variables

            if (libMethods == null)
                libMethods = new Dictionary<string, ParameterInfo[]>();
            else
                libMethods.Clear();

            if (libProperties == null)
                libProperties = new Dictionary<string, string>();
            else
                libProperties.Clear();

            #endregion

            #region Methods

            foreach (var method in bcl.GetMethods())
            {
                if (!method.IsPublic || !method.IsStatic)
                    continue;

                var name = method.Name.ToLowerInvariant();
                var param = method.GetParameters();

                if (libMethods.ContainsKey(name))
                {
                    if (param.Length < libMethods[name].Length)
                        continue;

                    libMethods.Remove(name);
                }

                libMethods.Add(name, param);
            }

            #endregion

            #region Properties

            foreach (var property in bcl.GetProperties())
            {
                if (!property.Name.StartsWith("A_"))
                    continue;

                libProperties.Add(property.Name.ToLowerInvariant(), property.Name);
            }

            #endregion
        }
    }
}
