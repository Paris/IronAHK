
namespace IronAHK.Scripting
{
    partial class Parser
    {
        const string Legacy = "LEGACY";

        const bool LaxExpressions =
#if LEGACY
 true
#endif
#if !LEGACY
 false
#endif
;

        const bool LegacyIf = LaxExpressions;

        const bool LegacyLoop = LaxExpressions;

        bool DynamicVars = LaxExpressions;
    }
}
