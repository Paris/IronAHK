using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IronAHK.Rusty.Common
{
    partial class Keyboard
    {
        // ToDo: Handle IgnoreIAGeneratedInput
        // ToDo: Test TimeOut
        // ToDo: Test Multithreaded Access to this Singleton

        /// <summary>
        /// Input Command Handler (Singleton)
        /// </summary>
        internal class IAInputCommand : Singleton<IAInputCommand>
        {
            #region Fields

            KeyboardHook _keyboardHook;
            bool _visible = false;
            int _keyLimit = 0;
            bool _ignoreBackSpace = false;
            bool _recognizeModifiedKeystrockes = false;
            string catchedText = "";
            Keys endKeyReason = Keys.None;
            AbortReason abortReason = AbortReason.Fail;

            bool isCatching = false;
            object catchingLock = new object();
            int? _timeout = null;
            bool _ignoreIAGeneratedInput = false;

            List<Keys> _endkeys = new List<Keys>();

            List<string> _endMatchings = new List<string>();
            bool _caseSensitive = false;
            bool _findAnywhere = false;

            System.Threading.Timer _timeoutTimer;

            #endregion

            #region Singleton Config Properties

            // as long as this Class depends on other non Singleton Objects,
            // it must be configured else where

            /// <summary>
            /// Set the KeyboardHook to use to catch userinput
            /// </summary>
            public KeyboardHook Hook
            {
                set
                {
                    if (value == null)
                        throw new ArgumentException("Hook");
                    _keyboardHook = value;
                }
                private get { return _keyboardHook; }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Start Logging Text. Blocks calling Thread until user input meets an abort condition.
            /// </summary>
            /// <returns>Returns the catched Userinput</returns>
            public AbortInformation StartCatching()
            {

                lock (catchingLock)
                {
                    if (isCatching)
                    {
                        throw new NotImplementedException("Pending Input interruption not implemented yet!");
                    }

                    Hook.IAKeyEvent += OnKeyPressedEvent;
                    isCatching = true;

                    if (TimeOutVal.HasValue)
                    {
                        if (_timeoutTimer != null)
                            _timeoutTimer.Dispose();

                        _timeoutTimer = new System.Threading.Timer(new System.Threading.TimerCallback(OnTimoutTick));
                        _timeoutTimer.Change(this.TimeOutVal.Value, Timeout.Infinite);
                    }
                }



                while (true)
                {
                    lock (catchingLock)
                    {
                        if (!isCatching)
                            break;
                    }
                    Application.DoEvents(); // This is necessary if the StartCatching Method gets called on the Main GUI Thread
                    Thread.Sleep(2);
                }
                Hook.IAKeyEvent -= OnKeyPressedEvent; // we no longer need to get notified about keys...
                var ret = new AbortInformation(abortReason, endKeyReason, catchedText);
                return ret;
            }

            public void Reset()
            {
                _visible = false;
                _keyLimit = 0;
                _ignoreBackSpace = false;
                _recognizeModifiedKeystrockes = false;
                catchedText = "";
                endKeyReason = Keys.None;
                abortReason = AbortReason.Fail;

                isCatching = false;
                catchingLock = new object();
                _timeout = 0;
                _ignoreIAGeneratedInput = false;
                _endkeys.Clear();

                _endMatchings.Clear();
                _caseSensitive = false;
                _findAnywhere = false;

                if (_timeoutTimer != null)
                    _timeoutTimer.Dispose();
            }

            #endregion

            #region Properties

            #region Behaviour Properties

            /// <summary>
            /// A list of Keys which terminates the key catching
            /// </summary>
            public List<Keys> Endkeys
            {
                get { return _endkeys; }
            }

            /// <summary>
            /// Normally, the user's input is blocked (hidden from the system).
            /// Use this option to have the user's keystrokes sent to the active window.
            /// </summary>
            public bool Visible
            {
                get { return _visible; }
                set { _visible = value; }
            }

            /// <summary>
            /// Max Keys which are catched. 0 Means no Limit.
            /// </summary>
            public int KeyLimit
            {
                get { return _keyLimit; }
                set { _keyLimit = value; }
            }

            /// <summary>
            /// Modified keystrokes such as Control-A through Control-Z are recognized and
            /// transcribed if they correspond to real ASCII characters.
            /// </summary>
            public bool RecognizeModifiedKeystrockes
            {
                get { return _recognizeModifiedKeystrockes; }
                set { _recognizeModifiedKeystrockes = value; }
            }

            /// <summary>
            /// Backspace is ignored. Normally, pressing backspace during an Input will remove
            /// the most recently pressed character from the end of the string.
            /// 
            /// Note: If the input text is visible (such as in an editor) and the arrow keys or other
            /// means are used to navigate within it, backspace will still remove the last character
            /// rather than the one behind the caret (insertion point).
            /// </summary>
            public bool IgnoreBackSpace
            {
                get { return _ignoreBackSpace; }
                set { _ignoreBackSpace = value; }
            }

            /// <summary>
            /// The number of milliseconds to wait before terminating the Input and setting ErrorLevel to the word Timeout.
            /// If the Input times out, OutputVar will be set to whatever text the user had time to enter. 
            /// 
            /// Null means that there is no TimeOut. (default)
            /// </summary>
            public int? TimeOutVal
            {
                get { return _timeout; }
                set { _timeout = value; }
            }

            /// <summary>
            /// Strings which force to abort Logging
            /// </summary>
            public List<string> EndMatches
            {
                get { return _endMatchings; }
            }

            /// <summary>
            /// Normally, what the user types must exactly match one of the MatchList phrases for a match to occur.
            /// Use this option to find a match more often by searching the entire length of the input text.
            /// </summary>
            public bool FindAnyWhere
            {
                get { return _findAnywhere; }
                set { _findAnywhere = value; }
            }

            /// <summary>
            /// Is MatchList case sensitive? 
            /// </summary>
            public bool CaseSensitive
            {
                get { return _caseSensitive; }
                set { _caseSensitive = value; }
            }

            /// <summary>
            /// Ignore Scripts own Send Input
            /// </summary>
            public bool IgnoreIAGeneratedInput
            {
                get { return _ignoreIAGeneratedInput; }
                set { _ignoreIAGeneratedInput = value; }
            }

            #endregion

            /// <summary>
            /// Is User Input catching running?
            /// </summary>
            public bool IsBusy
            {
                get
                {
                    lock (catchingLock)
                    {
                        return isCatching;
                    }
                }
            }

            #endregion

            #region Event Handlers

            void OnKeyPressedEvent(object sender, IAKeyEventArgs e)
            {
                if (e.Block || e.Handeled || !e.Down)
                    return;

                lock (catchingLock)
                {
                    if (!isCatching)
                        return;
                }

                // Tell how to proceed whit this key
                if (!Visible)
                    e.Block = true;

                // Check for Post Abort Conditions
                if (PostAbortCondition(e))
                {
                    CatchingDone();
                    return;
                }

                // Handle Input
                if (e.Keys == System.Windows.Forms.Keys.Back)
                {
                    if (!_ignoreBackSpace)
                    {
                        catchedText = catchedText.Substring(0, catchedText.Length - 2);
                    }
                }
                else
                {
                    catchedText += e.Typed;
                }

                // Check for Past Abort Conditions
                if (PastAbortCondition(e))
                {
                    CatchingDone();
                }
            }

            public void AbortCatching()
            {
                abortReason = AbortReason.NewInput;
                CatchingDone();
            }

            void CatchingDone()
            {
                lock (catchingLock)
                {
                    isCatching = false;
                }
            }

            void OnTimoutTick(object state)
            {
                var timeoutTimer = (System.Threading.Timer)state;
                timeoutTimer.Dispose();
                CatchingDone();
            }


            #endregion

            #region Abort Conditions

            /// <summary>
            /// Checks if we are done with logging.
            /// </summary>
            /// <param name="e"></param>
            /// <returns></returns>
            bool PostAbortCondition(IAKeyEventArgs e)
            {
                if (Endkeys.Contains(e.Keys))
                {
                    endKeyReason = e.Keys;
                    abortReason = AbortReason.EndKey;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Checks if we are done with logging.
            /// </summary>
            /// <returns></returns>
            bool PastAbortCondition(IAKeyEventArgs e)
            {

                // Past Condition: Key Limit
                if (KeyLimit != 0)
                {
                    if (catchedText.Length >= KeyLimit)
                    {
                        abortReason = AbortReason.Max;
                        return true;
                    }
                }

                bool abort = false;
                // Past Condition Matchlist
                foreach (var match in _endMatchings)
                {
                    if (CaseSensitive)
                    {
                        if (!FindAnyWhere && match == catchedText)
                            abort = true;
                        if (FindAnyWhere && catchedText.Contains(match))
                            abort = true;
                    }
                    else
                    {
                        if (!FindAnyWhere && match.Equals(catchedText, StringComparison.OrdinalIgnoreCase))
                            abort = true;
                        if (FindAnyWhere && catchedText.ToLowerInvariant().Contains(match.ToLowerInvariant()))
                            abort = true;
                    }
                    if (abort)
                        break;
                }
                if (abort)
                    abortReason = AbortReason.Match;

                return abort;
            }


            #endregion
        }

        internal class AbortInformation
        {
            public AbortReason Reason;
            public Keys EndKey;
            public string CatchedText;

            public AbortInformation()
            {
                Reason = AbortReason.Fail;
                EndKey = Keys.None;
                CatchedText = "";
            }
            public AbortInformation(AbortReason reason, Keys endkey, string catchedText)
            {
                Reason = reason;
                EndKey = endkey;
                CatchedText = catchedText;
            }
        }

        /// <summary>
        /// Reason why Catching of Userinput was stopped
        /// </summary>
        internal enum AbortReason : int
        {
            Success = 0,
            Fail = 1,
            NewInput = 2,
            Max = 3,
            Timeout = 4,
            Match = 5,
            EndKey = 6,
        }

    }
}
