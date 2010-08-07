using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace IronAHK.Rusty
{

    /// <summary>Provides Next Coord to process
    /// Creation:       02.08.2010 - IsNull
    /// Last Changes:   07.08.2010 - IsNull
    /// -----------------------------------
    /// </summary>
    public sealed class CoordProvider
    {
        #region Private Data

        private Size mMaxMovement;
        private Point mCurrent;
        private bool mDone;

        private object Locker = new object();

        #endregion

        #region Constructor

        /// <summary>Create new CoordProvider with given Settings.
        /// 
        /// </summary>
        /// <param name="uSourceSize">Size of Searchable Image Area</param>
        /// <param name="uNeedleSize">Size of Needle Image</param>
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

            lock(Locker) {
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
        }

        #endregion
    }
}
