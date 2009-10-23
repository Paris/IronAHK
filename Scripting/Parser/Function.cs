using System.IO;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        bool IsFunction(string code, string next)
        {
            if (code.Length == 0 || code[0] == ParenOpen)
                return false;

            var reader = new StringReader(code);
            int stage = 0;
            bool str = false;

            while (reader.Peek() != -1)
            {
                char sym = (char)reader.Read();

                switch (stage)
                {
                    case 0:
                        if (sym == ParenOpen)
                            stage++;
                        else if (!IsIdentifier(sym))
                            return false;
                        break;

                    case 1:
                        if (sym == StringBound)
                            str = !str;
                        else if (!str && sym == ParenClose)
                            stage++;
                        break;

                    case 2:
                        if (sym == BlockOpen)
                            return true;
                        else if (!IsSpace(sym))
                            return false;
                        break;
                }
            }

            if (next.Length == 0)
                return false;

            reader = new StringReader(next);

            while (reader.Peek() != -1)
            {
                char sym = (char)reader.Read();

                if (sym == BlockOpen)
                    return true;
                else if (!IsSpace(sym))
                    return false;
            }

            return false;
        }
    }
}
