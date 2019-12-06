﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using System.Timers;

namespace _1712384_1712349_1712407
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        bool _isDragging = false;
        Point _lastPosition;
        Image _selectedBitmap = null;
        BindingList<ImageOperation> _games = new BindingList<ImageOperation>();

        List<Image> listImages = new List<Image>();
        int[,] _puzzle = new int[3, 3];

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }


        /// <summary>
        /// Mở hộp thoại dialog để chọn ảnh trên máy tính người chơi
        /// Gọi hàm CropImage để cắt ảnh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                var Game = new playImage()
                {
                    Source = new BitmapImage(
                   new Uri(screen.FileName, UriKind.Absolute)),
                    numCut = 3
                };

                CropImage(Game);
            }
        }

        private int countSmallerNumber(int[,] a, int n, int x, int y)
        {      
            int count = 0;
            for (int j = y + 1; j < n; j++)
            {
                if (a[x, j] < a[x, y] && a[y,x] != 9)
                    count++;
            }
            for (int i = x + 1; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (a[i, j] < a[x, y] && a[y, x] != 9)
                        count++;
                }
            }
            return count;
        }

        //Thuật toán: https://yinyangit.wordpress.com/2010/12/11/algorithm-tim-hi%E1%BB%83u-v%E1%BB%81-bai-toan-n-puzzle-updated/
        private Boolean CheckValid(int[,] a, int n)
        {
            List<int> smaller = new List<int>();
            int N = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    smaller.Add(countSmallerNumber(a, n, j, i));
                }
            }
            for (int i = 0; i < n * n - 1; i++)
            {
                N = N + smaller[i];
            }

            if ((n * n - 1) % 2 == 1)
            {
                if (N % 2 == 0)
                    return true;
                return false;
            }
            else
            {
                int[] T = new int[2];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (a[j, i] == 9)
                        {
                            T[0] = j; T[1] = i;
                            break;
                        }
                    }
                }

                if (N % 2 == 0)
                {
                    if ((T[1] + 1) % 2 == 0)
                        return true;
                    return false;
                }
                else
                {
                    if ((T[1] + 1) % 2 == 1)
                        return true;
                    return false;
                }
            }
        }

        private void  swapImage(int num,int w,int h)
        {
            List<int> Indexes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 });
            Random r = new Random();

            int k = 0;
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    k = Indexes[r.Next(0, Indexes.Count)];
                    Indexes.Remove(k);

                    table.Children.Add(listImages[k]);
                    Canvas.SetLeft(listImages[k], j * (w + 2));
                    Canvas.SetTop(listImages[k], i * (h + 2));

                    _puzzle[j, i] = k;


                    listImages[k].PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                    listImages[k].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;

                    //var rect = new Int32Rect(j * w, i * h, w, h);
                    //var cropbitmap = new CroppedBitmap(source, rect);

                    //var cropImage = new Image();

                    //cropImage.Width = w;
                    //cropImage.Height = h;
                    //cropImage.Source = cropbitmap;
                    //table.Children.Add(cropImage);
                    //Canvas.SetLeft(cropImage, j * (w + 2));
                    //Canvas.SetTop(cropImage, i * (h + 2));

                    //cropImage.PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                    //cropImage.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;

                    //cropImage.Tag = new Tuple<int, int>(i, j);
                }
            }
        }


        /// <summary>
        /// Cắt ảnh
        /// </summary>
        /// <param name="image"></param>
        private void CropImage(playImage image)
        {
            _games.Add(image);

            int num = image.numCut;
            int w = image.cropWidth;
            int h = image.cropHeight;
            var source = image.Source;
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    var rect = new Int32Rect(j * w, i * h, w, h);
                    var cropbitmap = new CroppedBitmap(source, rect);

                    var cropImage = new Image();

                    cropImage.Width = w;
                    cropImage.Height = h;
                    cropImage.Source = cropbitmap;
                    listImages.Add(cropImage);

                    cropImage.Tag = new Tuple<int, int>(i, j);
                }
            }


            // lưu hình trống
            var cropImage1 = new Image();

            cropImage1.Width = w;
            cropImage1.Height = h;

            listImages.Add(cropImage1);

            table.Children.Clear();
            swapImage(num,w,h);

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    Debug.Write(_puzzle[j, i] + " ");
                }
                Debug.WriteLine("");
            }

            while (!CheckValid(_puzzle,num))
            {
                table.Children.Clear();
                swapImage(num,w,h);
            }

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    Debug.Write(_puzzle[j,i] + " ");
                }
                Debug.WriteLine("");
            }
            CountDown();
        }
        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);
        }

        private void CropImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_games.Count() == 0)
                return;
            var index=0;
            _isDragging = false;
            var position = e.GetPosition(this);

            var w = _games[index].cropWidth;
            var h = _games[index].cropHeight;
            int x = (int)(position.X) / (w + 2) * (w + 2);
            int y = (int)(position.Y) / (h + 2) * (h + 2);

            var image = sender as Image;
            var (i, j) = image.Tag as Tuple<int, int>;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            if (_isDragging)
            {
                var dx = position.X - _lastPosition.X;
                var dy = position.Y - _lastPosition.Y;

                var lastleft = Canvas.GetLeft(_selectedBitmap);
                var lasttop = Canvas.GetTop(_selectedBitmap);

                Canvas.SetLeft(_selectedBitmap, lastleft + dx);
                Canvas.SetTop(_selectedBitmap, lasttop + dy);

                _lastPosition = position;
            }
        }

        /// <summary>
        /// Mở hộp thoại để người dùng chọn ảnh(đã chuẩn bị sẵn)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var screen = new SelectDialog();
            if (screen.ShowDialog() == true)
            {
                var Game1 = new playImage()
                {
                    Source = new BitmapImage(
                   new Uri(screen.SourceData, UriKind.Relative)),
                    numCut = 3
                };
                
                CropImage(Game1);
               
            }
            else
            {
                //MessageBox.Show("Failed");
            }
            
        }

        Timer timer;
        /// <summary>
        /// Đếm ngược thời gian
        /// </summary>
        private void CountDown()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        int sec=650;
        /// <summary>
        /// Hiển thị bộ đếm thời gian sau mỗi giây trôi qua
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            sec--;
            Dispatcher.Invoke(() =>lblTimer.Content=FormatTimer(sec));

            if (sec == 0)
            {
                timer.Stop();
                MessageBox.Show("You lose");
            }
        }

        /// <summary>
        /// Trả về chuỗi định dạng "00:00:00"
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        private string FormatTimer(int second)
        {
            string result = "";
            int hour = sec / 3600;
            int min = (sec-hour*3600)/60;
            int cpysec =(sec-hour*3600-min*60);

            string h = "";
            string m = "";
            string s = "";

            h = hour.ToString();
            m = min.ToString();
            s = cpysec.ToString();

            if(hour<10)
            {
                h = "0" + h;
            }
            if(min<10)
            {
                m = "0" + m;
            }
            if(cpysec<10)
            {
                s = "0" + s;
            }

            result = h + ":" + m + ":" + s;
            return result;
        }
    }
}
