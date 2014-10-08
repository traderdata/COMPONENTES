using System.Windows.Controls;

namespace ModulusFE.PaintObjects
{
    internal interface IPaintObject
    {
        /// <summary>
        /// This method gets called before adding paint object to canvas
        /// Here we can setup different objects style that will get added
        /// </summary>
        /// <param name="args"></param>
        void SetArgs(params object[] args);
        /// <summary>
        /// After the object created and initialized it will be added to Canvas
        /// </summary>
        /// <param name="canvas"></param>
        void AddTo(Canvas canvas);
        void RemoveFrom(Canvas canvas);

        int ZIndex { get; set; }
    }
}

