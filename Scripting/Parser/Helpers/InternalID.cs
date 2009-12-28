
namespace IronAHK.Scripting
{
    partial class Parser
    {
        int internalID = 0;

        string InternalID
        {
            get { return "e" + internalID++.ToString(); }
        }
    }
}
