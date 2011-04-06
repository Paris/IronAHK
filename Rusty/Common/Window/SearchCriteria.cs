using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Common
{
    partial class Window
    {
        public class SearchCriteria
        {
            public string Title { get; set; }
            public string Text { get; set; }
            public string ExcludeTitle { get; set; }
            public string ExcludeText { get; set; }

            public string ClassName { get; set; }
            public IntPtr ID { get; set; }
            public IntPtr PID { get; set; }
            public string Group { get; set; }

            public bool HasExcludes
            {
                get { return !string.IsNullOrEmpty(ExcludeTitle) || !string.IsNullOrEmpty(ExcludeText); }
            }

            public bool HasID
            {
                get { return ID != IntPtr.Zero || PID != IntPtr.Zero; }
            }

            public bool IsEmpty
            {
                get { return !HasID && !HasExcludes && string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(ClassName); }
            }


            public static SearchCriteria FromString(string mixed)
            {
                if (mixed.IndexOf(Core.Keyword_ahk, StringComparison.OrdinalIgnoreCase) == -1)
                    return new SearchCriteria { Title = mixed };

                var criteria = new SearchCriteria();
                var i = 0;
                var t = false;

                while ((i = mixed.IndexOf(Core.Keyword_ahk, i, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    if (!t)
                    {
                        var pre = i == 0 ? string.Empty : mixed.Substring(0, i).Trim(Core.Keyword_Spaces);

                        if (pre.Length != 0)
                            criteria.Title = pre;

                        t = true;
                    }

                    var z = mixed.IndexOfAny(Core.Keyword_Spaces, i);

                    if (z == -1)
                        break;

                    var word = mixed.Substring(i, z - i);

                    var e = mixed.IndexOf(Core.Keyword_ahk, ++i, StringComparison.OrdinalIgnoreCase);
                    var arg = (e == -1 ? mixed.Substring(z) : mixed.Substring(z, e - z)).Trim();
                    long n;

                    switch (word.ToLowerInvariant())
                    {
                        case Core.Keyword_ahk_class: criteria.ClassName = arg; break;
                        case Core.Keyword_ahk_group: criteria.Group = arg; break;

                        case Core.Keyword_ahk_id:
                            if (long.TryParse(arg, out n))
                                criteria.ID = new IntPtr(n);
                            break;

                        case Core.Keyword_ahk_pid:
                            if (long.TryParse(arg, out n))
                                criteria.PID = new IntPtr(n);
                            break;
                    }

                    i++;
                }

                return criteria;
            }

            public static SearchCriteria FromString(string title, string text, string excludeTitle, string excludeText)
            {
                var criteria = FromString(title);
                criteria.Text = text;
                criteria.ExcludeTitle = excludeTitle;
                criteria.ExcludeText = excludeText;
                return criteria;
            }
        }
    }
}
