
namespace IronAHK.Scripting
{
    partial class Parser
    {
        class BreakLabels
        {
            string breakLabel, continueLabel;

            public BreakLabels(string breakLabel, string continueLabel)
            {
                this.breakLabel = breakLabel;
                this.continueLabel = continueLabel;
            }

            public string Break
            {
                get { return breakLabel; }
            }

            public string Continue
            {
                get { return continueLabel; }
            }
        }
    }
}
