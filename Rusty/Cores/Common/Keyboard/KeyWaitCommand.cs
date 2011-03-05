using System.Threading;
using System.Windows.Forms;

namespace IronAHK.Rusty.Cores.Common.Keyboard
{
    public class KeyWaitCommand
    {
        #region Fields

        readonly KeyboardHook _keyboardHook;
        bool timeOut = false;
        object timeOutLock = new object();
        System.Threading.Timer _timeoutTimer;
        int? _timeOutVal = null;

        #endregion

        public KeyWaitCommand(KeyboardHook hook) {
            _keyboardHook = hook;
        }

        /// <summary>
        /// Wait until the key gets pressed.
        /// </summary>
        /// <param name="k"></param>
        public void Wait(Keys k) {
            _timeoutTimer = new System.Threading.Timer(new System.Threading.TimerCallback(OnTimoutTick));
            _timeoutTimer.Change(this.TimeOutVal.Value, Timeout.Infinite);
            // may better listen to pressed event...
            while(!_keyboardHook.IsPressed(k)) {
                lock(timeOutLock) {
                    if(timeOut)
                        break;
                }
                Thread.Sleep(2);
            }
        }

        
        public int? TimeOutVal {
            get { return _timeOutVal; }
            set { _timeOutVal = value; }
        }

        #region Event Handlers

        void OnTimoutTick(object state) {
            var timeoutTimer = (System.Threading.Timer)state;
            timeoutTimer.Dispose();
            lock(timeOutLock) {
                timeOut = true;
            }
        }

        #endregion
    }
}
