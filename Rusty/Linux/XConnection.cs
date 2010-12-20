using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace IronAHK.Rusty.Linux
{
	internal delegate void XEventHandler(LinuxAPI.XEvent Event);
	
	// Singleton class to keep track of all active windows and their 
	// events to help the hotkey and window management code
	public class XConnectionSingleton : IDisposable
	{
		#region Singleton boilerplate
		static XConnectionSingleton Instance;
		
		public static XConnectionSingleton GetInstance()
		{
			if(Instance == null)
				Instance = new XConnectionSingleton();
			
			return Instance;
		}
		#endregion
		
		// These are the events we subscribe to, in order to allow hotkey/hotstring support
        readonly static LinuxAPI.EventMasks SelectMask = LinuxAPI.EventMasks.KeyPress |
			LinuxAPI.EventMasks.FocusChange | LinuxAPI.EventMasks.SubstructureNofity  | 
			LinuxAPI.EventMasks.KeyRelease | LinuxAPI.EventMasks.Exposure;
		IntPtr Display;
		Thread Listener;
		LinuxAPI.XErrorHandler OldHandler;
		List<int> Windows;
		
		internal event XEventHandler OnEvent;
		
        // HACK: X sometimes throws a BadWindow Error because windows are quickly deleted
        // We set a placeholder errorhandler for some time and restore it later
		bool mSurpressErrors = false;
		bool Success = true;
		public bool SurpressErrors
		{
			set 
			{
				if(value && !mSurpressErrors) 
					OldHandler = LinuxAPI.X11.XSetErrorHandler(delegate { Success = false; return 0; });
				else if(!value && mSurpressErrors)
					LinuxAPI.X11.XSetErrorHandler(OldHandler);
				mSurpressErrors = value;
			}
			get { return mSurpressErrors; }
		}
		
		public IntPtr Handle {
			get { return Display; }
		}
		
		private XConnectionSingleton ()
		{
			Windows = new List<int>();
			
            // Kick off a thread listening to X events
            Listener = new Thread(Listen);
            Listener.Start();
		}
		
		public void Dispose()
		{
			Listener.Abort();
            LinuxAPI.X11.XCloseDisplay(Display);
		}
		
		void Listen()
        {
            Display = LinuxAPI.X11.XOpenDisplay(IntPtr.Zero);
            
            // Select all the windows already present
			SurpressErrors = true;
            RecurseTree(Display, LinuxAPI.X11.XDefaultRootWindow(Display));
			SurpressErrors = false;
            
            while (true)
            {
                FishEvent();
                Thread.Sleep(10); // Be polite
            }
        }
        
        private void FishEvent()
        {
            var Event = new LinuxAPI.XEvent();
            LinuxAPI.X11.XNextEvent(Display, ref Event);
			
			if(OnEvent != null)
				OnEvent(Event);
			
			if (Event.type == LinuxAPI.XEventName.CreateNotify)
			{
				int Window = Event.CreateWindowEvent.window;
				Success = true;
				
				Windows.Add(Window);
    
				SurpressErrors = true;
				if(Success)
					RecurseTree(Display, Window);
				
				SurpressErrors = false;
			}
			else if(Event.type == LinuxAPI.XEventName.DestroyNotify)
			{
				Windows.Remove(Event.DestroyWindowEvent.window);
			}
        }            
        
        // In the X Window system, windows can have sub windows. This function crawls a
        // specific function, and then recurses on all child windows. It is called to 
        // select all initial windows. It make some time (~0.5s)
        void RecurseTree(IntPtr Display, int RootWindow)
        {
            int RootWindowRet, ParentWindow, NChildren;
            IntPtr ChildrenPtr;
            int[] Children;
			
			if(!Windows.Contains(RootWindow))
				Windows.Add(RootWindow);
			
			// Request all children of the given window, along with the parent
            LinuxAPI.X11.XQueryTree(Display, RootWindow, out RootWindowRet, out ParentWindow, out ChildrenPtr, out NChildren);

            if (NChildren != 0)
            {
                // Fill the array with zeroes to prevent NullReferenceException from glue layer
                Children = new int[NChildren];
                Marshal.Copy(ChildrenPtr, Children, 0, NChildren);

                LinuxAPI.X11.XSelectInput(Display, RootWindow, SelectMask);

                // Subwindows shouldn't be forgotten, especially since everything is a subwindow of RootWindow
                for (int i = 0; i < NChildren; i++) 
                {
                    if (Children[i] != 0)
                    {
                        LinuxAPI.X11.XSelectInput(Display, Children[i], SelectMask);
                        RecurseTree(Display, Children[i]);
                    }
                }
            }
        }
	}
}

