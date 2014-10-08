using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModulusFE.Controls
{
    ///<summary>
    /// A Dialog
    ///</summary>
    public abstract class DialogX
    {
        private bool _isShowing;
        private Popup _popup;
        private Grid _grid;
        private Canvas _canvas;

        ///<summary>
        /// AppRoot for dialog.
        ///</summary>
        public Panel AppRoot { get; set; } //here we must add popup in order to work fine

        ///<summary>
        /// OnCLose
        ///</summary>
        public event EventHandler OnClose = delegate { };

        ///<summary>
        ///</summary>
        public enum DialogStyle
        {
            ///<summary>
            ///</summary>
            NonModal,
            ///<summary>
            ///</summary>
            Modal,
            ///<summary>
            ///</summary>
            ModalDimmed
        }

        ///<summary>
        /// Show Dialog
        ///</summary>
        ///<param name="style"></param>
        ///<exception cref="InvalidOperationException"></exception>
        public void Show(DialogStyle style)
        {
            if (_isShowing)
                throw new InvalidOperationException();

            _isShowing = true;

            EnsurePopup(style);

            _popup.IsOpen = true;

#if SILVERLIGHT
            Application.Current.Host.Content.Resized += OnPluginSizeChanged;
#endif
        }

        ///<summary>
        /// Close
        ///</summary>
        public void Close()
        {
            _isShowing = false;

            if (_popup == null) return;
            _popup.IsOpen = false;
#if SILVERLIGHT
            Application.Current.Host.Content.Resized -= OnPluginSizeChanged;
#endif

            AppRoot.Children.Remove(_popup);
            //OnClose(this, EventArgs.Empty);
        }

        /// <summary>
        /// Override this method to add your content to the dialog
        /// </summary>
        /// <returns></returns>
        protected abstract FrameworkElement GetContent();

        /// <summary>
        /// Override this method if you want to do something (e.g. call Close) when you click 
        /// outside of the content
        /// </summary>
        protected virtual void OnClickOutside() { }

        private void EnsurePopup(DialogStyle style)
        {
            if (_popup != null) return;

            _popup = new Popup();
            Debug.Assert(AppRoot != null);
            AppRoot.Children.Add(_popup);
            _grid = new Grid();
            _popup.Child = _grid;

            if (style != DialogStyle.NonModal)
            {
                _canvas = new Canvas();
                _canvas.MouseLeftButtonDown += (sender, e) => OnClickOutside();

                switch (style)
                {
                    case DialogStyle.Modal:
                        _canvas.Background = new SolidColorBrush(Colors.Transparent);
                        break;
                    case DialogStyle.ModalDimmed:
                        _canvas.Background = new SolidColorBrush(Color.FromArgb(0x20, 0x80, 0x80, 0x80));
                        break;
                }

                _grid.Children.Add(_canvas);
            }
            _grid.Children.Add(GetContent());

            UpdateSize();
        }

        private void UpdateSize()
        {
#if SILVERLIGHT
            _grid.Width = Application.Current.Host.Content.ActualWidth;
            _grid.Height = Application.Current.Host.Content.ActualHeight;
#endif

            if (_canvas == null) return;
            _canvas.Width = _grid.Width;
            _canvas.Height = _grid.Height;
        }

        private void OnPluginSizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }
    }
}
