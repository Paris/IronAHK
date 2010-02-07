using System;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        void MoveToEOL(string code, ref int i)
        {
            while (i < code.Length)
            {
                switch (code[i])
                {
                    case CR:
                        int n = i + 1;
                        if (n < code.Length && code[n] == LF)
                            i = n;
                        goto case LF;

                    case LF:
                        return;

                    default:
                        i++;
                        break;
                }
            }
        }
    }
}
