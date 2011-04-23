using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace AutoDataSpider
{
    class Downloader
    {
        public static void DownloadPhotos(string PhotoURL, string ID, string PhotoName)
        {
            Directory.CreateDirectory("photos/" + ID);
            WebClient client = new WebClient();
            try
            {
                string file = "photos/" + ID + "/" + PhotoName + ".jpg";
                client.DownloadFile(PhotoURL, file);
            }
            catch (Exception e)
            {
                Console.WriteLine("DownloadPhotos");
                Console.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Method for image scaling. Image proportion not changes.
        /// Result image will be not greater than destHeight x destWidth
        /// </summary>
        /// <param name="imgSourcePath">Path (with name) to source file</param>
        /// <param name="imgDestPath">Path (with name) for out. Resulted image is in jpeg format</param>
        /// <param name="destWidth">Result image maximum width</param>
        /// <param name="destHeight">Result image maximum height</param>
        public static void Scale(String imgSourcePath, String imgDestPath, int destWidth, int destHeight)
        {
            Bitmap source = new Bitmap(imgSourcePath);
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            double widthScale = (double)sourceWidth / destWidth;
            double heightScale = (double)sourceHeight / destHeight;
            double scale = widthScale > heightScale ? widthScale : heightScale;

            int width = (int)(sourceWidth / scale + 0.5);
            int height = (int)(sourceHeight / scale + 0.5);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(source, new Rectangle(0, 0, width, height),
                new Rectangle(0, 0, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            bmPhoto.Save(imgDestPath, ImageFormat.Jpeg);
            source.Dispose();
            bmPhoto.Dispose();
        }
    }
}
