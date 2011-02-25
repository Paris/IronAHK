using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IronAHK.Rusty.Cores.Common.Keyboard
{
    // ToDO: Handle IgnoreIAGeneratedInput

    /// <summary>
    /// Input Command Handler
    /// </summary>
    public class IAInputCommand
    {
        #region Fields

        readonly KeyboardHook _keyboardHook;
        bool _visible = false;
        int _keyLimit = 0;
        bool _ignoreBackSpace = false;
        bool _recognizeModifiedKeystrockes = false;
        string catchedText = "";
        bool catchDone = false;
        object catchDoneLock = new object();
        float _timeout = 0;
        bool _ignoreIAGeneratedInput = false;
        
        List<Keys> _endkeys = new List<Keys>();

        List<string> _endMatchings = new List<string>();
        bool _caseSensitive = false;
        bool _findAnywhere = false;

        #endregion

        #region Constructor

        internal IAInputCommand(KeyboardHook keyboardHook) {
            if(keyboardHook == null)
                throw new ArgumentNullException("keyboardHook");
            _keyboardHook = keyboardHook;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start Logging Text. Blocks calling Thread until user input meets an abort condition.
        /// </summary>
        /// <returns>Returns the catched Userinput</returns>
        public string StartCatching() {
            _keyboardHook.KeyPressedEvent += OnKeyPressedEvent;
            catchDone = false;
            while(true)
            {
                lock(catchDoneLock) {
                    if(catchDone)
                        break;
                }
                Thread.Sleep(2);
            }
            _keyboardHook.KeyPressedEvent -= OnKeyPressedEvent; // we no longer need to get notified about keys...
            return catchedText;
        }

        #endregion

        #region Properties

        public List<Keys> Endkeys {
            get { return _endkeys; }
        }

        /// <summary>
        /// Normally, the user's input is blocked (hidden from the system).
        /// Use this option to have the user's keystrokes sent to the active window.
        /// </summary>
        public bool Visible {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// Max Keys which are catched. 0 Means no Limit.
        /// </summary>
        public int KeyLimit {
            get { return _keyLimit; }
            set { _keyLimit = value; }
        }

        /// <summary>
        /// Modified keystrokes such as Control-A through Control-Z are recognized and
        /// transcribed if they correspond to real ASCII characters.
        /// </summary>
        public bool RecognizeModifiedKeystrockes {
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
        public bool IgnoreBackSpace {
            get { return _ignoreBackSpace; }
            set { _ignoreBackSpace = value; }
        }
        
        /// <summary>
        /// The number of seconds to wait before terminating the Input and setting ErrorLevel to the word Timeout.
        /// If the Input times out, OutputVar will be set to whatever text the user had time to enter. 
        /// 
        /// Null means that there is no TimeOut. (default)
        /// </summary>
        public float TimeOut {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Strings which force to abort Logging
        /// </summary>
        public List<string> EndMatches {
            get { return _endMatchings; }
        }

        /// <summary>
        /// Normally, what the user types must exactly match one of the MatchList phrases for a match to occur.
        /// Use this option to find a match more often by searching the entire length of the input text.
        /// </summary>
        public bool FindAnyWhere {
            get { return _findAnywhere; }
            set { _findAnywhere = value; }
        }

        /// <summary>
        /// Is MatchList case sensitive? 
        /// </summary>
        public bool CaseSensitive {
            get { return _caseSensitive; }
            set { _caseSensitive = value; }
        }

        /// <summary>
        /// Ignore Scripts own Send Input
        /// </summary>
        public bool IgnoreIAGeneratedInput {
            get { return _ignoreIAGeneratedInput; }
            set { _ignoreIAGeneratedInput = value;}
        }


        #endregion

        #region Event Handlers

        void OnKeyPressedEvent(object sender, IAKeyEventArgs e) {
            if(e.Block || e.Handeled ||!e.Down)
                return;

            // Tell how to proceed whit this key
            if(!Visible)
                e.Block = true;

            // Check for Post Abort Conditions
            if(PostAbortCondition(e)) {
                lock(catchDoneLock) {
                    catchDone = true;
                }
                return;
            }

            // Handle Input
            if(e.Keys == System.Windows.Forms.Keys.Back) {
                if(!_ignoreBackSpace) {
                    catchedText = catchedText.Substring(0, catchedText.Length - 2);
                } 
            } else {
                catchedText += e.Typed;
            }

            // Check for Past Abort Conditions
            if(PastAbortCondition(e)) {
                lock(catchDoneLock) {
                    catchDone = true;
                }
            }
        }


        #endregion

        #region Abort Conditions

        bool PostAbortCondition(IAKeyEventArgs e) {

            if(Endkeys.Contains(e.Keys))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if we are done with logging.
        /// </summary>
        /// <returns></returns>
        bool PastAbortCondition(IAKeyEventArgs e) {
            
            // Past Condition: Key Limit
            if(KeyLimit != 0) {
                if(catchedText.Length >= KeyLimit){
                    return true;
                }
            }

            // Past Condition Matchlist
            foreach(var match in _endMatchings) {
                if(CaseSensitive) {
                    if(!FindAnyWhere && match == catchedText)
                        return true;
                    if(FindAnyWhere && catchedText.Contains(match))
                        return true;
                } else {
                    if(!FindAnyWhere && match.ToLowerInvariant() == catchedText.ToLowerInvariant())
                        return true;
                    if(FindAnyWhere && catchedText.ToLowerInvariant().Contains(match.ToLowerInvariant()))
                        return true;
                }
            }
            return false;
        }


        #endregion
    }
}
