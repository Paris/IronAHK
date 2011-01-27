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

            var ignore = new List<string>();

            #endregion

            #region Methods

            foreach (var method in bcl.GetMethods())
            {
                if (!method.IsPublic || !method.IsStatic)
                    continue;

                var name = method.Name.ToLowerInvariant();

                if (ignore.Contains(name))
                    continue;

                var param = method.GetParameters();

                if (libMethods.ContainsKey(name))
                {
                    libMethods.Remove(name);
                    ignore.Add(name);
                }
                else 
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

            libProperties.Add(ErrorLevel.ToLowerInvariant(), ErrorLevel);

            #endregion
        }
    }
}
