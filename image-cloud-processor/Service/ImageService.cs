using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Service
{
    public class ImageService
    {
        public byte[] CreateBoundingBox(byte[] streamedFileContent, List<Tuple<PointF, PointF, PointF, PointF>> boxes, Color colorBase)
        {
            var result = default(byte[]);
            Image image = Image.FromStream(new MemoryStream(streamedFileContent));
            using (var g = Graphics.FromImage(image))
            {
                foreach (var box in boxes)
                {
                    Color customColor = Color.FromArgb(80, colorBase);
                    SolidBrush shadowBrush = new SolidBrush(customColor);
                    g.DrawPolygon(new Pen(shadowBrush, 7f), new PointF[] { box.Item1, box.Item2, box.Item3, box.Item4 });
                }

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    result = ms.ToArray();
                }
                return result;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }



        public byte[] CropImage(byte[] imageEdit, Tuple<PointF, PointF, PointF, PointF> sexoBox, float hResize, float vResize, float xTranslate = 0f, float yTranslate = 0f)
        {
            var result = default(byte[]);
            Image image = Image.FromStream(new MemoryStream(imageEdit));


            var width = (sexoBox.Item2.X - sexoBox.Item1.X);
            var heigth = (sexoBox.Item3.Y - sexoBox.Item1.Y);

            RectangleF cropRect = new RectangleF(sexoBox.Item1.X + (width * xTranslate), sexoBox.Item1.Y + (heigth * yTranslate),
                width * hResize,
                heigth * vResize);

            Bitmap src = image as Bitmap;
            Bitmap target = new Bitmap(
                (int)cropRect.Width,
                (int)cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);

                using (var ms = new MemoryStream())
                {
                    target.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    result = ms.ToArray();
                }

            }
            return result;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }

        }
    }
}
