using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        internal abstract class WindowManager
        {
            #region Properties

            int delay = 100;

            public int Delay
            {
                get { return delay; }
                set { delay = value; }
            }

            #endregion

            #region Find

            #region Criteria

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
                    if (mixed.IndexOf(Keyword_ahk, StringComparison.OrdinalIgnoreCase) == -1)
                        return new SearchCriteria { Title = mixed };

                    var criteria = new SearchCriteria();
                    var i = 0;
                    var t = false;

                    while ((i = mixed.IndexOf(Keyword_ahk, i, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        if (!t)
                        {
                            var pre = i == 0 ? string.Empty : mixed.Substring(0, i).Trim(Keyword_Spaces);

                            if (pre.Length != 0)
                                criteria.Title = pre;

                            t = true;
                        }

                        var z = mixed.IndexOfAny(Keyword_Spaces, i);

                        if (z == -1)
                            break;

                        var word = mixed.Substring(i, z - i);

                        var e = mixed.IndexOf(Keyword_ahk, ++i, StringComparison.OrdinalIgnoreCase);
                        var arg = (e == -1 ? mixed.Substring(z) : mixed.Substring(z, e - z)).Trim();
                        long n;

                        switch (word.ToLowerInvariant())
                        {
                            case Keyword_ahk_class: criteria.ClassName = arg; break;
                            case Keyword_ahk_group: criteria.Group = arg; break;

                            case Keyword_ahk_id:
                                if (long.TryParse(arg, out n))
                                    criteria.ID = new IntPtr(n);
                                break;

                            case Keyword_ahk_pid:
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

            #endregion

            public abstract IntPtr LastFound { get; set; }

            public abstract IntPtr[] AllWindows { get; }

            public abstract IntPtr ActiveWindow { get; }

            public abstract IntPtr PreviousWindow { get; }

            public abstract IntPtr FindWindow(SearchCriteria criteria);

            public WindowManager FindWindow(string title, string text, string excludeTitle, string excludeText)
            {
                var criteria = SearchCriteria.FromString(title, text, excludeTitle, excludeText);
                var ids = FindWindow(criteria);
                return CreateWindow(ids);
            }

            #endregion

            #region Window

            public IntPtr ID { get; set; }

            public abstract WindowManager CreateWindow(IntPtr id);

            #region Helpers

            public bool IsSpecified
            {
                get { return ID != IntPtr.Zero; }
            }

            public bool Equals(SearchCriteria criteria)
            {
                if (!IsSpecified)
                    return false;

                if (criteria.ID != IntPtr.Zero && ID != criteria.ID)
                    return false;

                if (criteria.PID != IntPtr.Zero && PID != criteria.PID)
                    return false;

                var comp = StringComparison.OrdinalIgnoreCase;

                if (!string.IsNullOrEmpty(criteria.ClassName))
                {
                    if (!ClassName.Equals(criteria.ClassName, comp))
                        return false;
                }

                if (!string.IsNullOrEmpty(criteria.Title))
                {
                    if (!TitleCompare(Title, criteria.Title))
                        return false;
                }

                if (!string.IsNullOrEmpty(criteria.Text))
                {
                    foreach (var text in Text)
                        if (text.IndexOf(criteria.Text, comp) == -1)
                            return false;
                }

                if (!string.IsNullOrEmpty(criteria.ExcludeTitle))
                {
                    if (Title.IndexOf(criteria.ExcludeTitle, comp) != -1)
                        return false;
                }

                if (!string.IsNullOrEmpty(criteria.ExcludeText))
                {
                    foreach (var text in Text)
                        if (text.IndexOf(criteria.ExcludeText, comp) != -1)
                            return false;
                }

                return true;
            }

            bool TitleCompare(string a, string b)
            {
                var comp = StringComparison.OrdinalIgnoreCase;

                switch (A_TitleMatchMode.ToLowerInvariant())
                {
                    case "1":
                        return a.StartsWith(b, comp);

                    case "2":
                        return a.IndexOf(b, comp) != -1;

                    case "3":
                        return a.Equals(b, comp);

                    case Keyword_RegEx:
                        return new Regex(b).IsMatch(a);
                }

                return false;
            }

            #endregion

            public abstract IntPtr PID { get; }

            public abstract bool Active { get; set; }

            public abstract bool Close();

            public abstract bool Exists { get; }

            public abstract string ClassName { get; }

            public abstract Point Location { get; set; }

            public abstract Size Size { get; set; }

            public abstract bool SelectMenuItem(params string[] items);

            public abstract string Title { get; set; }

            public abstract string[] Text { get; }

            public abstract bool Hide();

            public abstract bool Kill();

            public abstract bool AlwaysOnTop { get; set; }

            public abstract bool Bottom { set; }

            public abstract bool Enabled { get; set; }

            public abstract bool Redraw();

            public abstract int Style { get; set; }

            public abstract int ExStyle { get; set; }

            public abstract void SetTransparency(byte level, Color color);

            public abstract bool Show();

            public abstract FormWindowState WindowState { get; set; }

            #region Wait

            public bool Wait(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (!this.Exists)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }

                return true;
            }

            public bool WaitActive(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (!this.Active)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }

                return true;
            }

            public bool WaitClose(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (this.Exists)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }

                return true;
            }

            public bool WaitNotActive(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (this.Active)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }

                return true;
            }

            #endregion

            #endregion
        }
    }
}
