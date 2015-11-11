using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using Accord.MachineLearning;
using System.Drawing.Drawing2D;
namespace OCR
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 
    static class Tools
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

    }

    public partial class MainWindow : Window
    {
        private int                 grayLimit = 250;
        private List<string>        dictionary = new List<string>();


        public MainWindow()
        {
            InitializeComponent();
            try {
                dictionary = Directory.GetFiles("../../assets").ToList();
            } catch (DirectoryNotFoundException e) {
                Console.WriteLine("ERROR: Directory not found");
            }
        }
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            Graphics g = Graphics.FromImage(newBitmap);

            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
               });

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }


        private void browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "All images|*.jpeg;*.png;*.jpg;*.bmp|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                Uri uri = new Uri(filename);
                BitmapImage imgSource = new BitmapImage(uri);

                FileNameTextBox.Text = filename;
                picture.Source = imgSource;

                /* KNN */
                List<double []> letters = new List<double[]>();
                List<int> outputList = new List<int>();
                List<string> indexes = new List<string>();

                foreach (string letterPath in dictionary) {
                    FileInfo file = new FileInfo(letterPath);
                    string nameClean = file.Name.Substring(0, file.Name.Length - 4);

                    while (Char.IsDigit(nameClean[nameClean.Length - 1])) {
                        nameClean = nameClean.Substring(0, nameClean.Length - 1);
                    }
                    int i = indexes.IndexOf(nameClean);

                    if (i <= -1) {
                        indexes.Add(nameClean);
                        outputList.Add(indexes.Count - 1);
                    } else {
                        outputList.Add(i);
                    }
                    letters.Add(getVectors(cropImage(getImageBitmap(file.FullName))));
                }

                DateTime start = DateTime.Now;

                KNearestNeighbors knn = new KNearestNeighbors(k: 3, classes: indexes.Count, inputs: letters.ToArray(), outputs: outputList.ToArray());
                int answer = knn.Compute(getVectors(cropImage(MakeGrayscale(getImageBitmap(filename)))));
                string res = indexes[answer];
                lettre.Content = res;
                timeSpent.Content = "Execution time: " + (DateTime.Now - start);
            }
        }

        private double[] getVectors(Bitmap source) {
            int x;
            int y;
            List<double> vector = new List<double>();

            for (x = 0; x < source.Width; x++) {
                vector.Add(0);
                for (y = 0; y < source.Height; y++) {
                    System.Drawing.Color pixelColor = source.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit) {
                        vector[x] += 1;
                    }
                }
            }
            for (y = 0; y < source.Height; y++) {
                vector.Add(0);
                for (x = 0; x < source.Width; x++) {
                    System.Drawing.Color pixelColor = source.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit) {
                        vector[vector.Count - 1] += 1;
                    }
                }
            }
            return vector.ToArray();
        }

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
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
        private Bitmap cropImage(Bitmap bitmap)
        {
            int x, y;
            int top, bottom, left, right;
            int testlimit = (bitmap.Height > bitmap.Width) ? bitmap.Width : bitmap.Height;
            top = bitmap.Height;
            left = bitmap.Width;
            right = bottom = 0;
            for (x = 0; x < bitmap.Width; x++)
            {
                for (y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit)
                    {
                        if (x < left) { left = x; }
                        if (x > right) { right = x; }
                        if (y < top) { top = y; }
                        if (y > bottom) { bottom = y; }

                    }
                }
            }
            var crop = new System.Drawing.Rectangle(left, top, right - left, bottom - top);
            var bmp = new Bitmap(crop.Width, crop.Height);

            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return ResizeImage(bmp, 50, 50);
        }

        private Bitmap getImageBitmap(string fileName)
        {
            Bitmap image = null;
            if (fileName != null && fileName != "")
            {
                image = new Bitmap(fileName);
            }
            return image;
        }
    }
}
