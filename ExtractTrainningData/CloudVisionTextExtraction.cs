using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtractTrainningData
{
    public class CloudVisionTextExtraction
    {
        public static CropBoxes GetTextExtraction(string item)
        {
            var client = ImageAnnotatorClient.Create();

            var image = Google.Cloud.Vision.V1.Image.FromFile(item);
            var response = client.DetectDocumentText(image);

            var cropBoxes = new CropBoxes();
            foreach (var page in response.Pages)
            {
                foreach (var block in page.Blocks)
                {
                    foreach (var paragraph in block.Paragraphs)
                    {
                        foreach (var word in paragraph.Words)
                        {
                            var wordBox = BoundigBoxToPoints(word.BoundingBox);
                            string palavra = string.Join(string.Empty, word.Symbols.Select(x => x.Text));
                            cropBoxes.Push(palavra, wordBox);
                        }
                    }
                }
            }
            return cropBoxes;
        }

        private static Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF> BoundigBoxToPoints(BoundingPoly bounding)
        {
            var v = bounding.Vertices;
            var p0 = new System.Drawing.PointF(v[0].X, v[0].Y);
            var p1 = new System.Drawing.PointF(v[1].X, v[1].Y);
            var p2 = new System.Drawing.PointF(v[2].X, v[2].Y);
            var p3 = new System.Drawing.PointF(v[3].X, v[3].Y);
            return new Tuple<System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF, System.Drawing.PointF>(p0, p1, p2, p3);
        }
    }
}
