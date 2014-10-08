using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ModulusFE.SL
{
    ///<summary>
    /// Extensions
    ///</summary>
    public static class Extensions
    {
        ///<summary>
        /// Inflates a <see cref="Rect"/> structure by the specified amount.
        ///</summary>
        ///<param name="self"></param>
        ///<param name="width"></param>
        ///<param name="height"></param>
        public static void Inflate(this Rect self, double width, double height)
        {
            self.X -= width / 2;
            self.Width += width / 2;
            self.Y -= height / 2;
            self.Height += height / 2;
        }

        ///<summary>
        /// Translates the <see cref="Point"/> by the specified amount.
        ///</summary>
        ///<param name="self"></param>
        ///<param name="offsetX"></param>
        ///<param name="offsetY"></param>
        public static Point Offset(this Point self, double offsetX, double offsetY)
        {
            self.X += offsetX;
            self.Y += offsetY;

            return self;
        }

        ///<summary>
        /// Finds the position of an element using a given predicate
        ///</summary>
        ///<param name="self"></param>
        ///<param name="predicat"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public static int FindIndex<T>(this List<T> self, Func<T, bool> predicat)
        {
            int index = 0;
            foreach (var t in self)
            {
                if (predicat(t))
                    return index;
                index++;
            }
            return -1;
        }
        internal static void Freeze(this Brush self)
        {
            //Nothing, SL doesn't not support IFreezable interface
        }

        internal static object GetAsFrozen(this Brush self)
        {
            //Nothing, SL doesn't not support IFreezable interface
            return self;
        }

        internal static object GetAsFrozen(this GeometryGroup self)
        {
            return self;
        }
    }
}
