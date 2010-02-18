
namespace IronAHK.Scripting
{
    partial class Parser
    {
        const bool LaxExpressions =
#if LEGACY
 true
#endif
#if !LEGACY
 false
#endif
;

        const bool LegacyIf = LaxExpressions;
    }
}
