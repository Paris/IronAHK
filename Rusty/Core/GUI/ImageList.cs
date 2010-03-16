using System;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise ImageList.cs

        /// <summary>
        /// Adds an icon or picture to the specified ImageListID and returns the new icon's index (1 is the first icon, 2 is the second, and so on). Filename is the name of an icon (.ICO), cursor (.CUR), or animated cursor (.ANI) file (animated cursors will not actually be animated when displayed in a ListView). Other sources of icons include the following types of files: EXE, DLL, CPL, SCR, and other types that contain icon resources. To use an icon group other than the first one in the file, specify its number for IconNumber. In the following example, the default icon from the second icon group would be used: IL_Add(ImageListID, "C:\My Application.exe", 2).
        /// </summary>
        /// <param name="ImageListID"></param>
        /// <param name="Filename"></param>
        /// <param name="IconNumber"></param>
        /// <param name="ResizeNonIcon"></param>
        public static void IL_Add(int ImageListID, string Filename, int IconNumber, bool ResizeNonIcon)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new ImageList, initially empty, and returns the unique ID of the ImageList (or 0 upon failure). InitialCount is the number of icons you expect to put into the list immediately (if omitted, it defaults to 2). GrowCount is the number of icons by which the list will grow each time it exceeds the current list capacity (if omitted, it defaults to 5). LargeIcons should be a numeric value: If non-zero, the ImageList will contain large icons. If zero, it will contain small icons (this is the default when omitted). Icons added to the list are scaled automatically to conform to the system's dimensions for small and large icons.
        /// </summary>
        /// <param name="InitialCount"></param>
        /// <param name="GrowCount"></param>
        /// <param name="LargeIcons"></param>
        /// <returns></returns>
        public static int IL_Create(string InitialCount, string GrowCount, int LargeIcons)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified ImageList and returns 1 upon success and 0 upon failure. It is normally not necessary to destroy ImageLists because once attached to a ListView, they are destroyed automatically when the ListView or its parent window is destroyed. However, if the ListView shares ImageLists with other ListViews (by having 0x40 in its options), the script should explicitly destroy the ImageList after destroying all the ListViews that use it. Similarly, if the script replaces one of a ListView's old ImageLists with a new one, it should explicitly destroy the old one.
        /// </summary>
        /// <param name="ImageListID"></param>
        public static void IL_Destroy(int ImageListID)
        {
            throw new NotImplementedException();
        }
    }
}
