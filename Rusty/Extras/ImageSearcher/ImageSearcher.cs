using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace IronAHK.Rusty
{

    /// <summary>
    /// Provides easy to use image comparisation and search Algorythms
    /// </summary>
    public class SearchableImage
    {

        #region Private Data

        private int mVariation = 0; // 0-255
        private Bitmap mSourceImage = null;
        #endregion

        #region Constructors

        public SearchableImage(Bitmap uSourceImage) {
            this.mSourceImage = uSourceImage;
        }

        #endregion

        #region Public Propertys

        /// <summary>Variation of matcher (0 = exact match, 255 = every color matches)
        /// 
        /// </summary>
        public int Variation {
            get { return mVariation; }
            set {
                if(value >= 0 && value <= 255) {
                    mVariation = value;
                } else
                    throw new ArgumentException("Submitted variation is invalid!");
            }
        }

        #endregion

        #region Pubic Methods

        /// <summary>Searches the image.
        /// Searches a image in antoher image
        /// 
        /// 1. search the first pixel of the needle in the source
        /// 2. if found --> lookup required size for performance optimisation
        /// 3. if step 2. passed --> begin pixel per pixel comparisation, with (Variation)
        /// 
        /// Further optimisations may include hash comparisation
        /// </summary>
        /// <returns>If nothing is found [Point?]=null is returned, otherwise upperleft corner of found position.</returns>
        public Point? SearchImage(Bitmap NeedleImage) {

            // Point? ImageLocation = null;

            if(mSourceImage == null || NeedleImage == null)
                throw new InvalidOperationException();

            var Unit = GraphicsUnit.Pixel;
            //Is the needle image conatined (physical size) in the source image?
            if(mSourceImage.GetBounds(ref Unit).Contains(NeedleImage.GetBounds(ref Unit))) {

                var MaxMovement = new Size(mSourceImage.Size.Width - NeedleImage.Size.Width, mSourceImage.Size.Height - NeedleImage.Size.Height);

                for(int row = 0; row <= MaxMovement.Height; row++)
                    for(int col = 0; col <= MaxMovement.Width; col++) {
                        if(CompareImage(NeedleImage, new Point(col, row)))
                            return new Point(col, row);
                    }
            }
            return null;
        }

        /// <summary>Searches a Pixel in a Image.
        /// 
        /// </summary>
        /// <param name="ColorId"></param>
        /// <returns>If nothing is found [Point?]=null is returned, otherwise upperleft corner of found position.</returns>
        public Point? SearchPixel(int ColorId) {

            var c = CreateComparerMask(ColorId);

            for(int row = 0; row < mSourceImage.Size.Height; row++)
                for(int col = 0; col < mSourceImage.Size.Width; col++) {
                    Color pix = mSourceImage.GetPixel(col, row);

                    if(CompareWithMask(pix, c))
                        return new Point(col, row);
                }
            return null;
        }


        /// <summary>Searches the Needle Image withhin the Source Image at given Point. Variation and other Parameters are taken into account.
        /// 
        /// </summary>
        /// <param name="NeedleImageLocation"></param>
        /// <returns></returns>
        public bool CompareImage(Bitmap NeedleImage, Point NeedleImageLocation) {
            Color NeedlePix;
            Color SourcePix;

            // Check if the Needleimage at given Position fits into SourceImage
            var SourceBox = new Rectangle(new Point(0, 0), mSourceImage.Size);
            var NeedleBox = new Rectangle(NeedleImageLocation, NeedleImage.Size);
            if(!SourceBox.Contains(NeedleBox))
                return false;

            //If so, perform a comarisation for each pixel from needle:
            for(int row = 0; row < NeedleImage.Size.Height; row++)
                for(int col = 0; col < NeedleImage.Size.Width; col++) {
                    NeedlePix = NeedleImage.GetPixel(col, row);
                    SourcePix = mSourceImage.GetPixel(col + NeedleImageLocation.X, row + NeedleImageLocation.Y);

                    if(!CompareWithMask(SourcePix, CreateComparerMask(NeedlePix.ToArgb()))) {
                        //they don't match
                        return false;
                    }
                }
            return true;
        }

        public Bitmap ToBitmap() {
            return mSourceImage;
        }

        #endregion

        #region Private Methods

        /// <summary>Create a Comarer Mask with given Variation
        /// 
        /// </summary>
        /// <param name="ColorID">Source Color</param>
        /// <returns></returns>
        private int[,] CreateComparerMask(int ColorID) {
            var c = new int[3, 2];

            for(int i = 0; i < 3; i++) {
                int t = ColorID >> (2 - i) * 8 & 0xff;
                c[i, 0] = t - this.Variation; c[i, 1] = t + this.Variation;
            }
            return c;
        }

        /// <summary>Compares given Color with Comparer Mask
        /// 
        /// </summary>
        /// <param name="GivenColor"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool CompareWithMask(Color GivenColor, int[,] c) {
            return (GivenColor.R >= c[0, 0] && GivenColor.R <= c[0, 1] &&
                         GivenColor.G >= c[1, 0] && GivenColor.G <= c[1, 1] &&
                         GivenColor.B >= c[2, 0] && GivenColor.B <= c[2, 1]);
        }
        #endregion
    }
}