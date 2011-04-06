using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Adds an image to the specified ImageList and returns its index.
        /// </summary>
        /// <param name="id">The ImageList ID.</param>
        /// <param name="file">The source of the image.</param>
        /// <param name="index">The index of the icon if <paramref name="file"/> is a multi-icon or resource format.</param>
        public static void IL_Add(long id, string file, int index = 0)
        {
            InitGui();

            if (!imageLists.ContainsKey(id))
                imageLists.Add(id, new ImageList());

            var icon = GetIcon(file, index);
            imageLists[id].Images.Add(icon);
        }

        /// <summary>
        /// Creates a new ImageList.
        /// </summary>
        /// <returns>The unique ID of the ImageList.</returns>
        public static long IL_Create()
        {
            InitGui();

            var il = new ImageList();
            var ptr = il.Handle.ToInt64();
            imageLists.Add(ptr, il);
            return ptr;
        }

        /// <summary>
        /// Deletes an ImageList.
        /// </summary>
        /// <param name="id">The ImageList ID.</param>
        /// <returns><c>true</c> if the specified ImageList was deleted, <c>falsle</c> otherwise.</returns>
        public static bool IL_Destroy(int id)
        {
            InitGui();

            if (!imageLists.ContainsKey(id))
                return false;

            imageLists[id].Dispose();
            imageLists.Remove(id);
            return true;
        }
    }
}
