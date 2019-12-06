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
                    numCut = 4
                };
               
                CropImage(Game);
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
                    table.Children.Add(cropImage);
                    Canvas.SetLeft(cropImage, j * (w + 2));
                    Canvas.SetTop(cropImage, i * (h + 2));

                    cropImage.PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                    cropImage.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;


                    cropImage.Tag = new Tuple<int, int>(i, j);
                }
            }
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
                    numCut = 4
                };
                
                CropImage(Game1);
               
            }
            else
            {
                //MessageBox.Show("Failed");
            }
            
        }

    }
}
