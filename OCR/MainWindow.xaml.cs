﻿using Microsoft.Win32;
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
        private int                 grayLimit = 200;
        private List<string>        dictionary = new List<string>();

        public Bitmap bitmap { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            try {
                dictionary = Directory.GetFiles("../..//assets").ToList();
            } catch (DirectoryNotFoundException e) {
                Console.WriteLine("ERROR: Directory not found");
            }
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
                this.bitmap = getImageBitmap(filename);
                this.bitmap = cropImage(this.bitmap);
                picture.Source = imgSource;
                pictureRes.Source = Tools.ToBitmapImage(this.bitmap);

                /* KNN */
                List<double []>[] letters = new List<double []>[26];
                List<int> outputList = new List<int>();

                //getting each file in assets, crop, and get vectors
                foreach (string letterPath in dictionary) {
                    FileInfo file = new FileInfo(letterPath);

                    switch (file.Name[0]) {
                        case 'a':
                            if (letters[0] == null) {
                                letters[0] = new List<double[]>();
                            }
                            double[] v2 = getVectors(cropImage(getImageBitmap(file.FullName)))[0];
                            letters[0].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'b':
                            if (letters[1] == null) {
                                letters[1] = new List<double[]>();
                            }
                            double[] v3 = getVectors(cropImage(getImageBitmap(file.FullName)))[0];
                            letters[1].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'c':
                            if (letters[2] == null) {
                                letters[2] = new List<double[]>();
                            }
                            letters[2].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'd':
                            if (letters[3] == null) {
                                letters[3] = new List<double[]>();
                            }
                            letters[3].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'e':
                            if (letters[4] == null) {
                                letters[4] = new List<double[]>();
                            }
                            letters[4].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'f':
                            if (letters[5] == null) {
                                letters[5] = new List<double[]>();
                            }
                            letters[5].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'g':
                            if (letters[6] == null) {
                                letters[6] = new List<double[]>();
                            }
                            letters[6].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'h':
                            if (letters[7] == null)  {
                                letters[7] = new List<double[]>();
                            }
                            letters[7].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'i':
                            if (letters[8] == null) {
                                letters[8] = new List<double[]>();
                            }
                            letters[8].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'j':
                            if (letters[9] == null) {
                                letters[9] = new List<double[]>();
                            }
                            letters[9].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'k':
                            if (letters[10] == null) {
                                letters[10] = new List<double[]>();
                            }
                            letters[10].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'l':
                            if (letters[11] == null) {
                                letters[11] = new List<double[]>();
                            }
                            letters[11].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'm':
                            if (letters[12] == null) {
                                letters[12] = new List<double[]>();
                            }
                            letters[12].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'n':
                            if (letters[13] == null) {
                                letters[13] = new List<double[]>();
                            }
                            letters[13].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'o':
                            if (letters[14] == null) {
                                letters[14] = new List<double[]>();
                            }
                            letters[14].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'p':
                            if (letters[15] == null) {
                                letters[15] = new List<double[]>();
                            }
                            letters[15].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'q':
                            if (letters[16] == null) {
                                letters[16] = new List<double[]>();
                            }
                            letters[16].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'r':
                            if (letters[17] == null) {
                                letters[17] = new List<double[]>();
                            }
                            letters[17].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 's':
                            if (letters[18] == null) {
                                letters[18] = new List<double[]>();
                            }
                            letters[18].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 't':
                            if (letters[19] == null){
                                letters[19] = new List<double[]>();
                            }
                            letters[19].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'u':
                            if (letters[20] == null) {
                                letters[20] = new List<double[]>();
                            }
                            letters[20].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'v':
                            if (letters[21] == null) {
                                letters[21] = new List<double[]>();
                            }
                            letters[21].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'w':
                            if (letters[22] == null) {
                                letters[22] = new List<double[]>();
                            }
                            letters[22].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'x':
                            if (letters[23] == null) {
                                letters[23] = new List<double[]>();
                            }
                            letters[23].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'y':
                            if (letters[24] == null) {
                                letters[24] = new List<double[]>();
                            }
                            letters[24].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                        case 'z':
                            if (letters[25] == null) {
                                letters[25] = new List<double[]>();
                            }
                            letters[25].Add(getVectors(cropImage(getImageBitmap(file.FullName)))[0]);
                            break;
                    }
                }

                int totlaLetters = 0;
                int totlaLettersTypes = 0;
                
                //get input array size
                for (int i = 0; i < letters.Length; ++i) {
                    if (letters[i] != null) {
                        foreach (double[] letter in letters[i]) {
                            totlaLetters++;
                        }
                        totlaLettersTypes++;
                    }
                }
                double[][] inputs = new double[totlaLetters][];
                int j = 0;

                for (int i = 0; i < letters.Length; ++i) {
                    if (letters[i] != null) {
                        foreach (double[] letter in letters[i]) {
                            inputs[j++] = letter;
                            outputList.Add(i);
                        }
                    }
                }

                KNearestNeighbors knn = new KNearestNeighbors(k: 4, classes: totlaLettersTypes, inputs: inputs, outputs: outputList.ToArray());
                double[] v1 = getVectors(getImageBitmap(filename))[0];
                int answer = knn.Compute(getVectors(getImageBitmap(filename))[0]);
                Console.WriteLine("a = " + answer);
            }
        }

        private double[][] getVectors(Bitmap source) {
            int x;
            int y;
            int xp = 0;
            int yp = 0;
            double[][] res = new double[2][];
            List<double>[] vectors = new List<double>[2];

            vectors[0] = new List<double>();
            vectors[1] = new List<double>();
            for (x = 0; x < this.bitmap.Width; x++) {
                vectors[0].Add(0);
                for (y = 0; y < this.bitmap.Height; y++) {
                    System.Drawing.Color pixelColor = this.bitmap.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit) {
                        vectors[0][xp] += 1;
                    }
                }
                xp++;
            }
            for (y = 0; y < this.bitmap.Height; y++) {
                vectors[1].Add(0);
                for (x = 0; x < this.bitmap.Width; x++) {
                    System.Drawing.Color pixelColor = this.bitmap.GetPixel(x, y);
                    if (pixelColor.R <= this.grayLimit) {
                        vectors[1][yp] += 1;
                    }
                }
                yp++;
            }
            res[0] = vectors[0].ToArray();
            res[1] = vectors[1].ToArray();
            return res;
        }

        private Bitmap cropImage(Bitmap bitmap)
        {
            int x, y;
            int top, bottom, left, right;
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