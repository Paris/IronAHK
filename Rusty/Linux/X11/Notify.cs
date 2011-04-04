using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Linux.X11
{

    internal enum NotifyMode
    {
        NotifyNormal = 0,
        NotifyGrab = 1,
        NotifyUngrab = 2
    }

    internal enum NotifyDetail
    {
        NotifyAncestor = 0,
        NotifyVirtual = 1,
        NotifyInferior = 2,
        NotifyNonlinear = 3,
        NotifyNonlinearVirtual = 4,
        NotifyPointer = 5,
        NotifyPointerRoot = 6,
        NotifyDetailNone = 7
    }
}
