
namespace IronAHK.Scripting
{
    partial class Parser
    {
        bool IsKeyword(string code)
        {
            switch (code.ToLowerInvariant())
            {
                case NotTxt:
                case OrTxt:
                case AndTxt:
                    return true;

                default:
                    return false;
            }
        }
    }
}
