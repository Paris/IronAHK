using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace IronAHK.Rusty
{

    /// <summary>Provides Next Coord to process
    /// 
    /// </summary>
    public sealed class CoordProvider
    {
        #region Private Data

        private Size mMaxMovement;
        private Point mCurrent;
        private bool mDone;

        #endregion

        #region Constructor

        public CoordProvider(Size uSourceSize, Size uNeedleSize) {
            mMaxMovement = new Size(uSourceSize.Width - uNeedleSize.Width, uSourceSize.Height - uNeedleSize.Height);
            mCurrent = new Point(-1, 0);
            mDone = false;
        }

        #endregion

        #region Public Methods

        /// <summary>Returns the next Workitem
        /// 
        /// </summary>
        /// <returns>Next Coord (Point) or Null if the work is done.</returns>
        public Point? Next() {
            if(mDone)
                return null;

            mCurrent.X++;
            if(mCurrent.X > mMaxMovement.Width) {
                mCurrent.X = 0;
                mCurrent.Y++;
                if(mCurrent.Y > mMaxMovement.Height) {
                    mDone = true;
                    return null;
                }
            }
            return mCurrent;
        }

        #endregion
    }
}
