using System.Drawing;

namespace IronAHK.Rusty
{
    partial class Core
    {
        internal abstract class MouseController
        {
            public class ClickInfo
            {
                /// <summary>
                /// A mouse button.
                /// </summary>
                public enum Buttons
                {
                    Left, Middle, Right,
                    X1, X2,
                    WheelUp, WheelDown, WheelLeft, WheelRight,
                }

                /// <summary>
                /// Send a down click event.
                /// </summary>
                public bool Down { get; set; }

                /// <summary>
                /// Send an up click event.
                /// </summary>
                public bool Up { get; set; }

                /// <summary>
                /// Number of times to perform a click.
                /// Use zero to move the mouse.
                /// </summary>
                public int Count { get; set; }

                /// <summary>
                /// Specifies <see cref="Location"/> as relative offsets from the current mouse position.
                /// </summary>
                public bool Relative { get; set; }

                /// <summary>
                /// The coordinates on the screen to send the click or mouse movement.
                /// Leave blank to use the current position.
                /// </summary>
                public Point Location { get; set; }

                /// <summary>
                /// The type of click to send.
                /// </summary>
                public Buttons Button { get; set; }

                public ClickInfo()
                {
                    Down = true;
                    Up = true;
                    Count = 1;
                    Relative = false;
                    Button = ClickInfo.Buttons.Left;
                }
            }

            /// <summary>
            /// The absolute screen coordinates of the current mouse locaiton.
            /// </summary>
            public abstract Point Location { get; }

            public abstract void SendEvent(ClickInfo info);
        }
    }
}
