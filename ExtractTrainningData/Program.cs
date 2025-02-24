﻿using Google.Cloud.Vision.V1;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ExtractTrainningData
{
    class Program
    {
        static void Main(string[] args)
        {
            //var srcPath = @"C:\Users\a.de.melo.pinheiro\Downloads\drive-download-20201226T220426Z-001\Teste0";
            var srcPath = @"C:\Users\a.de.melo.pinheiro\Downloads\drive-download-20201226T220426Z-001\Massas de teste";
            var dstPath = @$"{srcPath}\Crops3";
            InitializeGoogleCredentials();

            //var fileList = System.IO.Directory.EnumerateFiles(srcPath);
            var fileList = new System.Collections.Generic.List<string>();
            fileList.Add(@"C:\Users\a.de.melo.pinheiro\Downloads\drive-download-20201226T220426Z-001\Massas de teste\ROI_save.png");
            var index = 0;
            var total = fileList.Count();

            foreach (var item in fileList)
            {
                Console.WriteLine($"Progress: {index}/{total} - {(index / total) * 100}%");

                var btm = DrawUtils.GetBitmapFromPath(item);
                btm.Save(Path.Combine(dstPath, Path.GetFileName(item)));
                var cropBoxes = CloudVisionTextExtraction.GetTextExtraction(item, btm);

                ExtractFields(btm, dstPath, item, cropBoxes);

                ExtractOptionsFields(btm, dstPath, item, cropBoxes);
                index++;
            }
        }

        private static void ExtractOptionsFields(Bitmap btm, string dstPath, string item, CropBoxes cropBoxes)
        {
            foreach (var field in cropBoxes.GetOptionsBoxes())
            {
                var dimension = CropBoxes.GetOptionsFieldDimension(field);

                string filenameFinal = Path.Combine(dstPath, field.ToString());
                CropAndSaveImage(btm, item, filenameFinal, cropBoxes.GetOptionsBox(field), dimension.Item1, dimension.Item2, dimension.Item3, dimension.Item4);
            }
        }

        private static void ExtractFields(Bitmap btm, string dstPath, string item, CropBoxes cropBoxes)
        {
            foreach (var field in cropBoxes.GetBoxes())
            {
                var dimension = CropBoxes.GetFieldDimension(field);

                string filenameFinal = Path.Combine(dstPath, field.ToString());
                CropAndSaveImage(btm, item, filenameFinal, cropBoxes.GetBox(field), dimension.Item1, dimension.Item2, dimension.Item3, dimension.Item4);
            }
        }

        // Parametros do CROP
        // Sexo 8x4
        // Raça 12x4

        public static void CropAndSaveImage(Bitmap btm, string fullPathImage, string fullPath, Tuple<PointF, PointF, PointF, PointF> sexoBox, float hResize, float vResize, float xTranslate = 0f, float yTranslate = 0f)
        {
            //var image = System.Drawing.Image.FromFile(fullPathImage) as Bitmap;
            var image = btm;//System.Drawing.Image.FromFile(fullPathImage) as Bitmap;

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


                Console.WriteLine("Saving: " + fullPath);
                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

                //target.Save(Path.Combine(fullPath, Path.GetFileName(fullPathImage)));
                target.Save(Path.Combine(fullPath, Path.GetFileName(fullPathImage)));
            }
        }

        private static void InitializeGoogleCredentials()
        {
            // TODO: Put this path in appsettings
            var googleCredential = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrEmpty(googleCredential))
            {
                Console.WriteLine("Google Credentials not SET - Loading for Dev Enviroment");
                string credential_path = @"C:\Users\a.de.melo.pinheiro\Documents\CESAR School\projeto-recopa\api-auth\API Project-64e82001381f.json";
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
            }
            googleCredential = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

            Console.WriteLine("Google Credentials set:" + googleCredential);
        }
    }
}
