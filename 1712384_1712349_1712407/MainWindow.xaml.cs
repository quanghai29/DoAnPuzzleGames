using System;
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
        int[,] win = {{0, 1, 2},
                        {3, 4, 5},
                        {6, 7, 9}};
        int startX = 100;
        int startY = 100;
        int number = 3;// Biến lưu cái số lượng mảnh cắt ra
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
                    numCut = number
                };
                if (timer != null)
                {
                    ResetTimer(21);
                }
                CropImage(Game);
            }
        }

        private int countSmallerNumber(int[,] a, int n, int x, int y)
        {      
            int count = 0;
            for (int j = y + 1; j < n; j++)
            {
                if (a[x, j] < a[x, y] && a[x,y] != 9)
                    count++;
            }
            for (int i = x + 1; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (a[i, j] < a[x, y] && a[x,y] != 9)
                        count++;
                }
            }
            return count;
        }

        //Thuật toán kiểm tra tráo hình có thể chuyển về vị trí ban đầu hay không: https://yinyangit.wordpress.com/2010/12/11/algorithm-tim-hi%E1%BB%83u-v%E1%BB%81-bai-toan-n-puzzle-updated/
        private Boolean CheckValid(int[,] a, int n)
        {
            List<int> smaller = new List<int>();
            int N = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    smaller.Add(countSmallerNumber(a, n, i, j));
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
                        if (a[i, j] == 9)
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

        private Tuple<int,int> getIndex(int[,]a , int n, int x)
        {
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if (a[i, j] == x)
                        return new Tuple<int, int>(i, j);
                }
            }
            return new Tuple<int, int>(-1, -1);
        }

        private Boolean checkWin(int[,]a, int n)
        {
            for(int i = 0; i< n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if (a[i, j] != _puzzle[i, j])
                        return false;
                }
            }
            return true;
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
                    Canvas.SetLeft(listImages[k], j * (w + 2) + startX);
                    Canvas.SetTop(listImages[k], i * (h + 2) +startY);


                    _puzzle[i, j] = k;

                    listImages[k].PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                    listImages[k].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
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

            listImages.Clear();
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


            while (!CheckValid(_puzzle,num) && !checkWin(_puzzle,num))
            {
                table.Children.Clear();
                swapImage(num,w,h);
            }

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    Debug.Write(_puzzle[i,j] + " ");
                }
                Debug.WriteLine("");
            }
            ResultImage.Source = image.Source;
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
            int x = (int)(position.X - startX) / (w + 2) * (w + 2);
            int y = (int)(position.Y - startY) / (h + 2) * (h + 2);

            //Tọa độ thật 
            
           
            bool validPosition = true;
            var image = sender as Image;
            var (i, j) = image.Tag as Tuple<int, int>;
            var (n, m) = getIndex(_puzzle, number, i * number + j);
            //Xét vị trí hợp lệ -------------------------------
            // Xét lần lượt các biên sau

            //Biên bên trái
            if (x < 0)
            {
                x = 0;
                validPosition = false;
            }
            //Biên bên phải
            if (x > (w + 2) * (number - 1))
            {
                x =  (w + 2) * (number - 1);
                validPosition = false;
            }
            //Biên bên trên cùng
            if (y < 0)
            {
                y = 0;
                validPosition = false;
            }
            //Biên bên dưới
            if (y > (h + 2) * (number - 1))
            {
                y = (h + 2) * (number - 1);
                validPosition = false;
            }

            var indexY = y / h;
            var indexX = x / w;
            //Xét hướng di chuyển m ,n 
            //Xét không cho đi chéo đi trùng ô

            //áp dụng công thức tính khoảng cách giữa hai điểm
            // căn((a2-a1)^2 +(b2-b1)^2))

            var denta = Math.Sqrt(Math.Pow((indexY - n), 2) + Math.Pow((indexX - m), 2));
            if (_puzzle[indexY, indexX] != 9 || denta!=1)
            {
                x =  (w + 2) * m;
                y =  (h + 2) * n;
                validPosition = false;
            }
            Canvas.SetLeft(image, x+startX);
            Canvas.SetTop(image, y+startY);

            
          
            
            //-------------------------------------------------------
            //Nếu snap hợp lệ rồi mới được tráo mảng puzzle
            if (validPosition)
            {
                var (s, t) = getIndex(_puzzle, 3, win[i, j]);
                int tmp = _puzzle[y / h, x / w];
                _puzzle[y / h, x / w] = win[i, j];
                _puzzle[s, t] = tmp;

                for (int k = 0; k < 3; k++)
                {
                    for (int p = 0; p < 3; p++)
                    {
                        Debug.Write(_puzzle[k, p] + " ");
                    }
                    Debug.WriteLine("");
                }
                //CountDown();
            }
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
                if (timer != null)
                {
                    ResetTimer(21);
                }
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

        int sec=21;//Số giây 
        /// <summary>
        /// Hiển thị bộ đếm thời gian sau mỗi giây trôi qua
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            sec--;
            Dispatcher.Invoke(() => lblTimer.Content=FormatTimer(sec));

            if (sec<=0)
            {
                timer.Stop();
                timer.Dispose();
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

        private void ResetTimer(int resetSec)
        {
            timer.Stop();
            timer.Dispose();
            sec = resetSec;
        }
    }
}
