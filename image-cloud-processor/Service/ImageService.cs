using System;
using System.Collections.Generic;
using System.Drawing;
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

        public byte[] CropImage(byte[] imageEdit, Tuple<PointF, PointF, PointF, PointF> sexoBox, float resizeX, float resizeY)
        {
            var result = default(byte[]);
            Image image = Image.FromStream(new MemoryStream(imageEdit));

            RectangleF cropRect = new RectangleF(sexoBox.Item1.X, sexoBox.Item1.Y, 
                (sexoBox.Item2.X - sexoBox.Item1.X) * resizeX, 
                (sexoBox.Item3.Y - sexoBox.Item1.Y) * resizeY);


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
    }
}
