using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModulusFE.SL;

using System.Windows.Input;

using System.Windows.Shapes;
using ImageTools.IO.Jpeg;
using ImageTools;

namespace ModulusFE
{
    public partial class StockChartX
    {
        ///<summary>
        /// Defines the types of image to be exported from chart
        ///</summary>
        public enum ImageExportType
        {
            ///<summary>
            /// Portable network graphic (PNG)
            ///</summary>
            Png,
            Jpg
        }

        /// <summary>
        /// Returns a raw byte array of image
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        public byte[] GetBytes(ImageExportType exportType)
        {
            return GetBytesInternal(this, exportType);
        }

        /// <summary>
        /// Save image to a local filename
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        public bool SaveToFile(ImageExportType exportType)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            switch (exportType)
            {
                case ImageExportType.Png:
                    sfd.Filter = "Png files (*.png)|*.png|All files (*.*)|*.*";
                    break;
                case ImageExportType.Jpg:
                    sfd.Filter = "Jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
                    break;
                default:
                    throw new NotSupportedException();
            }
            sfd.FilterIndex = 1;

            if (sfd.ShowDialog() == false)
                return false;

            if (exportType == ImageExportType.Png)
            {
                using (Stream stream = sfd.OpenFile())
                {

                    byte[] binaryData = GetBytes(exportType);
                    stream.Write(binaryData, 0, binaryData.Length);

                    stream.Close();
                }
            }
            else if (exportType == ImageExportType.Jpg)
            {         
                //ExtendedImage myImage = new ExtendedImage();
                
               // JpegEncoder jpeg = new JpegEncoder();
                //jpeg.Encode(
                //ExtendedImage img = new ExtendedImage();
                //using (Image img = Image.FromStream(new MemoryStream(raw)))
                //{
                //    img.Save("foo.jpg", ImageFormat.Jpeg);
                //}
            }

            return true;
        }

        //07-06-2012 - Adicionado por Felipe Soares
        /// <summary>
        /// Save image to a local filename
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        public byte[] GetBytesAsExportType(ImageExportType exportType)
        {
            return GetBytes(exportType);
        }
        //End

        private static byte[] GetBytesInternal(UIElement element, ImageExportType exportType)
        {
            switch (exportType)
            {
                case ImageExportType.Png:
                    return GetBytesInternalPng(element);
                case ImageExportType.Jpg:
                    return GetBytesInternalJpg(element);
            }
            throw new NotSupportedException();
        }

        private static byte[] GetBytesInternalPng(UIElement element)
        {
            WriteableBitmap w = new WriteableBitmap(element, new TranslateTransform());
            EditableImage imageData = new EditableImage(w.PixelWidth, w.PixelHeight);

            try
            {
                for (int y = 0; y < w.PixelHeight; ++y)
                {
                    for (int x = 0; x < w.PixelWidth; ++x)
                    {
                        int pixel = w.Pixels[w.PixelWidth * y + x];
                        imageData.SetPixel(x, y,

                                           (byte)((pixel >> 16) & 0xFF),
                                           (byte)((pixel >> 8) & 0xFF),
                                           (byte)(pixel & 0xFF), (byte)((pixel >> 24) & 0xFF)
                          );
                    }
                }
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show("Cannot print images from other domains");
                return null;
            }

            Stream pngStream = imageData.GetStream();
            StreamReader sr = new StreamReader(pngStream);
            byte[] binaryData = new Byte[pngStream.Length];
            pngStream.Read(binaryData, 0, (int)pngStream.Length);

            return binaryData;
        }

        private static byte[] GetBytesInternalJpg(UIElement element)
        {
            WriteableBitmap w = new WriteableBitmap(element, new TranslateTransform());
            EditableImage imageData = new EditableImage(w.PixelWidth, w.PixelHeight);

            try
            {
                for (int y = 0; y < w.PixelHeight; ++y)
                {
                    for (int x = 0; x < w.PixelWidth; ++x)
                    {
                        int pixel = w.Pixels[w.PixelWidth * y + x];
                        imageData.SetPixel(x, y,

                                           (byte)((pixel >> 16) & 0xFF),
                                           (byte)((pixel >> 8) & 0xFF),
                                           (byte)(pixel & 0xFF), (byte)((pixel >> 24) & 0xFF)
                          );
                    }
                }
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show("Cannot print images from other domains");
                return null;
            }

            Stream jpgStream = imageData.GetStream();
            StreamReader sr = new StreamReader(jpgStream);
            byte[] binaryData = new Byte[jpgStream.Length];
            jpgStream.Read(binaryData, 0, (int)jpgStream.Length);

            return binaryData;
        }


    }
}

