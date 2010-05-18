using System.Collections;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        int Set(IEnumerable parts, int offset)
        {
            var e = parts.GetEnumerator();

            for (int i = -1; i < offset; i++)
                e.MoveNext();

            int[] levels = { 0, 0, 0 }; // parentheses, objects, arrays
            int position = offset;

            if (!(e.Current is string))
                return 0;

            var first = (string)e.Current;
            const char nul = '\0';
            char expect = nul;

            if (first.Length == 0)
                return 0;
            else if (first.Length == 1)
            {
                switch (first[0])
                {
                    case BlockOpen:
                        expect = BlockClose;
                        levels[1]++;
                        break;

                    case ArrayOpen:
                        expect = ArrayClose;
                        levels[2]++;
                        break;

                    case ParenOpen:
                        expect = ParenClose;
                        levels[0]++;
                        break;
                }
            }
            else if (first[first.Length - 1] == ParenOpen)
            {
                expect = ParenClose;
                levels[0]++;
            }

            if (expect == nul)
                return 0;

            while (e.MoveNext())
            {
                position++;

                if (!(e.Current is string))
                    continue;

                var current = (string)e.Current;

                if (current.Length == 0)
                    continue;
                else if (current.Length == 1)
                {
                    switch (current[0])
                    {
                        case BlockOpen:
                            levels[1]++;
                            break;

                        case BlockClose:
                            levels[1]--;
                            if (levels[1] < 0)
                                throw new ParseException(ExUnbalancedParens);
                            else if (expect == BlockClose && levels[0] == 0 && levels[1] == 0 && levels[2] == 0)
                                return position;
                            break;

                        case ArrayOpen:
                            levels[2]++;
                            break;

                        case ArrayClose:
                            levels[2]--;
                            if (levels[2] < 0)
                                throw new ParseException(ExUnbalancedParens);
                            else if (expect == ArrayClose && levels[0] == 0 && levels[1] == 0 && levels[2] == 0)
                                return position;
                            break;

                        case ParenOpen:
                            levels[0]++;
                            break;

                        case ParenClose:
                            levels[0]--;
                            if (levels[0] < 0)
                                throw new ParseException(ExUnbalancedParens);
                            else if (expect == ParenClose && levels[0] == 0 && levels[1] == 0 && levels[2] == 0)
                                return position;
                            break;
                    }
                }
                else if (current[current.Length - 1] == ParenOpen)
                {
                    levels[0]++;
                }
            }

            if (levels[0] != 0 || levels[1] != 0 || levels[2] != 0)
                throw new ParseException(ExUnbalancedParens);

            return 0;
        }
    }
}
