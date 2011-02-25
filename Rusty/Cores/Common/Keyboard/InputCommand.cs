using System;
using System.Threading;

namespace IronAHK.Rusty.Cores.Common.Keyboard
{
    public class IAInputCommand
    {
        #region Fields

        readonly KeyboardHook _keyboardHook;
        bool _visible = false;
        int _keyLimit = 0;

        string catchedText = "";
        bool catchDone = false;
        object catchDoneLock = new object();

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

        /// <summary>
        /// Are the Keys consumed or Visible for furter processing?
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
            catchedText += e.Typed;

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

            // Past Condition 2
            // ToDo....
            return false;
        }


        #endregion
    }
}
