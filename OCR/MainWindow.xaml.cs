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
        public int grayLimit = 200;

        public Bitmap bitmap { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".jpg";
            dlg.Filter = "All images|*.jpeg;*.png;*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;
                this.bitmap = getImageBitmap(filename);
                Uri uri = new Uri(filename);
                BitmapImage imgSource = new BitmapImage(uri);
                picture.Source = imgSource;
                this.bitmap = this.cropImage();
                pictureRes.Source = Tools.ToBitmapImage(this.bitmap);
                this.test();
                /* code here */
            }
        }
        private void test()
        {
            int x, y;
            int xp, yp;
            xp = yp = 0;
            List<int>[] vectors = new List<int>[2];
            vectors[0] = new List<int>();
            vectors[1] = new List<int>();

            for (x = 0; x < this.bitmap.Width; x++)
            {
                vectors[0].Add(0);
                for (y = 0; y < this.bitmap.Height; y++)
                {
                    System.Drawing.Color pixelColor = this.bitmap.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit)
                    {
                        vectors[0][xp] += 1;
                    }
                }
                xp++;
            }

            for (y = 0; y < this.bitmap.Height; y++)
            {
                vectors[1].Add(0);
                for (x = 0; x < this.bitmap.Width; x++)
                {
                    System.Drawing.Color pixelColor = this.bitmap.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit)
                    {
                        vectors[1][yp] += 1;
                    }
                }
                yp++;
            }
            vectors[0].ForEach(i => Console.Write("{0}\t", i));
            Console.WriteLine(vectors[0].ToString());
        }
        private Bitmap cropImage()
        {
            int x, y;
            int top, bottom, left, right;
            top = this.bitmap.Height;
            left = this.bitmap.Width;
            right = bottom = 0;
            for (x = 0; x < this.bitmap.Width; x++)
            {
                for (y = 0; y < this.bitmap.Height; y++)
                {
                    System.Drawing.Color pixelColor = this.bitmap.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit)
                    {
                        if (x < left) { left = x; }
                        if (x > right) { right = x; }
                        if (y < top) { top = y; }
                        if (y > bottom) { bottom = y; }

                    }
                }
            }
            Console.WriteLine(top.ToString());
            Console.WriteLine(bottom.ToString());
            Console.WriteLine(left.ToString());
            Console.WriteLine(right.ToString());

            var crop = new System.Drawing.Rectangle(left, top, right - left, bottom - top);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(this.bitmap, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
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
