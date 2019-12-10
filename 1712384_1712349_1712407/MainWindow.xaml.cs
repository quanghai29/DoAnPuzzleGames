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
using System.Windows.Media.Animation;


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
        ImageOperation _games =null;

        List<Image> listImages = new List<Image>();

        int number = 3;// Biến lưu cái số lượng mảnh cắt ra
        int[,] _puzzle;
        int[,] win;
        int startX = 50;
        int startY = 20;
        int sizeWidth = 660;
        int sizeHeight = 660;
        int sec = 300;//Số giây 
        int resetSec = 300;
        private void initArray(int n)
        {
            _puzzle = new int[n, n];
            win = new int[n, n];
            int k = 0;
            for(int i = 0;i< n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    win[i, j] = k++;
                }
            }
            win[n - 1, n - 1] = n * n;
        }

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
                _games = null;
                initArray(number);
                setTimeLevel();
                var Game = new playImage()
                {
                    Source = new BitmapImage(),
                    Image=screen.FileName,
                    numCut = number
                };
                Game.Source.BeginInit();
                Game.Source.DecodePixelHeight = sizeWidth;
                Game.Source.DecodePixelWidth = sizeHeight;
                Game.Source.UriSource = new Uri(screen.FileName, UriKind.Absolute);
                Game.Source.EndInit();
                _games = Game;
                
                if (timer != null)
                {
                    ResetTimer(resetSec);
                }
                CropImage(Game);
            }
        }

       

        private int countSmallerNumber(int[,] a, int n, int x, int y)
        {      
            int count = 0;
            for (int j = y + 1; j < n; j++)
            {
                if (a[x, j] < a[x, y] && a[x,y] != n*n)
                    count++;
            }
            for (int i = x + 1; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (a[i, j] < a[x, y] && a[x,y] != n*n)
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
                        if (a[i, j] == n*n)
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
                    if (a[i, j] != win[i, j])
                        return false;
                }
            }
            return true;
        }

        private void  swapImage(int num,int w,int h)
        {
            int[] a = new int[num * num];
            for(int i = 0; i < num * num; i++)
            {
                a[i] = i;
            }
            a[num * num - 1] = num * num;
            List<int> Indexes = new List<int>(a);

            Random r = new Random();

            int k;
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    k = Indexes[r.Next(0, Indexes.Count)];
                    Indexes.Remove(k);

                    
                    var x = j * (w + 2) + startX;
                    var y = i * (h + 2) + startY;

                    Canvas.SetLeft(listImages[k],x);
                    Canvas.SetTop(listImages[k], y);

                    table.Children.Add(listImages[k]);

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
            initArray(number);
            _games = image;

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
                swapImage(num, w, h);


                while (!CheckValid(_puzzle, num) && !checkWin(_puzzle, num))
                {
                    table.Children.Clear();
                    swapImage(num, w, h);
                }

                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num; j++)
                    {
                        Debug.Write(_puzzle[i, j] + " ");
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
            if (_games==null)
                return;
            _isDragging = false;
            var position = e.GetPosition(this);

            var w = _games.cropWidth;
            var h = _games.cropHeight;
            int x = (int)(position.X - startX) / (w + 2) * (w + 2);
            int y = (int)(position.Y - startY) / (h + 2) * (h + 2);

            //Tọa độ thật 
            
           
            bool validPosition = true;
            var image = sender as Image;
            var (i, j) = image.Tag as Tuple<int, int>;
            var (n, m) = getIndex(_puzzle, number, i * number + j);
            Panel.SetZIndex(image, (int)100);
            
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
            if (_puzzle[indexY, indexX] != number*number || denta > 1)
            {
                x =  (w + 2) * m;
                y =  (h + 2) * n;
                validPosition = false;
            }
            Canvas.SetLeft(image, x+startX);
            Canvas.SetTop(image, y+startY);
           
            Panel.SetZIndex(image, (int)(-100));


            //-------------------------------------------------------
            //Nếu snap hợp lệ rồi mới được tráo mảng puzzle
            if (validPosition)
            {
                var (s, t) = getIndex(_puzzle, number, win[i, j]);
                int tmp = _puzzle[y / h, x / w];
                _puzzle[y / h, x / w] = win[i, j];
                _puzzle[s, t] = tmp;

                for (int k = 0; k < number; k++)
                {
                    for (int p = 0; p < number; p++)
                    {
                        Debug.Write(_puzzle[k, p] + " ");
                    }
                    Debug.WriteLine("");
                }
                //CountDown();
            }

            MessageWin();
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
                _games = null;
                initArray(number);
                setTimeLevel();
                var Game = new playImage()
                {
                    Source = new BitmapImage(),
                    Image=screen.SourceData,
                    numCut = number
                };

                Game.Source.BeginInit();
                Game.Source.DecodePixelHeight = sizeWidth;
                Game.Source.DecodePixelWidth = sizeHeight;
                Game.Source.UriSource = new Uri(screen.SourceData, UriKind.Absolute);
                Game.Source.EndInit();

                _games = Game;
                if (timer != null)
                {
                    ResetTimer(resetSec);
                }
                CropImage(Game);
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
            lblTimer.Visibility = Visibility.Visible;
        }

        
        /// <summary>
        /// Hiển thị bộ đếm thời gian sau mỗi giây trôi qua
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            sec--;
            Dispatcher.Invoke(() => lblTimer.Content=FormatTimer(sec));

            if (sec==0)//người chơi thua cuộc(hết thời gian)
            {
                timer.Stop();
                timer.Dispose();
                Dispatcher.Invoke(() => Notify("You lose\n Unfortunately! :( Try again", true));
                Dispatcher.Invoke(() => ResetGame());
                _games = null;
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

        //xét thời gian cho từng level
        private void setTimeLevel()
        {
            if(number==3)
            {
                sec = 300;
                resetSec = 300;
            }
            else if(number==4)
            {
                sec = 500;
                resetSec = 500;
            }
            else if(number==5)
            {
                sec = 700;
                resetSec = 700;
            }
                
        }

        private void ResetGame()
        {
            table.Children.Clear();
            listImages.Clear();
            _games = null;
        }

        //Lưu lại game
        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            if(_games==null)
            {
                Notify("Please Start a game to save game!",false);
                return;
            }
            var path = PathToProject() + "save.txt";
            using (StreamWriter writetext = new StreamWriter(path))
            {
               
                writetext.WriteLine(number);//số mảnh cắt
                writetext.WriteLine(_games.Image);//tên ảnh đang chơi
                writetext.WriteLine(sec.ToString());//lưu lại thời gian hiện tại 
                for(int i=0;i<number;i++)
                {
                    for(int j=0;j<number;j++)
                    {
                        writetext.WriteLine(_puzzle[i,j]);//số trong mảng _puzzle
                    }
                }
            }
            Notify("Save is completed!", false);
        }

       

        //Hàm hỗ trợ lấy đường dẫn tuyệt đối
        private string PathToProject()
        {
            var currentPath = AppDomain.CurrentDomain.BaseDirectory;//đường dẫn đến thư mục Debug/bin
            const string Separator = "\\";
            var tokens = currentPath.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            var targetPath = "";
            for (int i = 0; i < tokens.Length - 2; i++)
            {
                targetPath = targetPath + tokens[i] + "\\";
            }
            return targetPath;
        }

        //Load lại game
        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            
            var path = PathToProject() + "save.txt";
            try
            {
                using (StreamReader readtext = new StreamReader(path))
                {
                    if (readtext == null)
                        return;
                    
                    //Lấy dữ liệu từ file save.txt
                    
                    number = int.Parse(readtext.ReadLine());
                    var image = readtext.ReadLine();
                    var currentSec = int.Parse(readtext.ReadLine());

                    for (int i = 0; i < number; i++)
                    {
                        for (int j = 0; j < number; j++)
                        {
                            _puzzle[i, j] = int.Parse(readtext.ReadLine());
                        }
                    }

                    Debug.WriteLine($"{number} - {image}");
                    for (int i = 0; i < number; i++)
                    {
                        for (int j = 0; j < number; j++)
                        {
                            Debug.WriteLine($"{_puzzle[i, j]} ");
                        }
                        Debug.WriteLine("");
                    }

                    //Xóa trước khi tải hình vào

                    table.Children.Clear();
                    listImages.Clear();
                    //Tạo lại game
                    var Game = new playImage()
                    {
                        Source = new BitmapImage(),
                        Image = image,
                        numCut = number
                    };
                    Game.Source.BeginInit();
                    Game.Source.DecodePixelHeight = sizeWidth;
                    Game.Source.DecodePixelWidth = sizeHeight;
                    Game.Source.UriSource = new Uri(image, UriKind.Absolute);
                    Game.Source.EndInit();

                    //Gán lại hình mẫu 
                    ResultImage.Source = Game.Source;
                    var w = Game.cropWidth;
                    var h = Game.cropHeight;
                    var source = Game.Source;

                    _games = Game;
                    //Reset lại thời gian
                    if (timer != null)
                    {
                        ResetTimer(currentSec);
                    }
                    CountDown();
                    

                    //Cắt hình trước
                    //crop Image
                    for (int i = 0; i < number; i++)
                    {
                        for (int j = 0; j < number; j++)
                        {
                            var rect = new Int32Rect(j * w, i * h, w, h);
                            var cropbitmap = new CroppedBitmap(source, rect);
                            var cropImage = new Image();
                            Canvas.SetLeft(cropImage, j * (w + 2) + startX);
                            Canvas.SetTop(cropImage, i * (h + 2) + startY);
                            cropImage.Width = w;
                            cropImage.Height = h;
                            cropImage.Source = cropbitmap;
                            listImages.Add(cropImage);
                            cropImage.Tag = new Tuple<int, int>(i, j);
                        }
                    }
                    Notify("Load game is already", true);

                    // Để lại hình theo trật tự cũ
                    for (int i = 0; i < number; i++)
                    {
                        for (int j = 0; j < number; j++)
                        {
                            var value = _puzzle[i, j];
                            if (value != number * number)
                            {
                                //Xét vị trí và xử lý xự kiện
                                Canvas.SetLeft(listImages[value], j * (w + 2) + startX);
                                Canvas.SetTop(listImages[value], i * (h + 2) + startY);
                                table.Children.Add(listImages[value]);
                                listImages[value].PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                                listImages[value].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                            }
                        }
                    }
                }
            }catch(Exception es)
            {
                Debug.WriteLine(es.Message);
                Notify("Please save game first",false);
            }
           
        }

        //4 button hỗ trợ cho bốn mũi tên
        private void Arrowup_Click(object sender, RoutedEventArgs e)
        {
            arrow("up");
            if (checkWin(_puzzle, number))
                MessageWin();
        }

        private void Arrowleft_Click(object sender, RoutedEventArgs e)
        {
            arrow("left");
            MessageWin();
        }

        private void Arrowright_Click(object sender, RoutedEventArgs e)
        {
            arrow("right");
            MessageWin();
        }

        private void Arrowdown_Click(object sender, RoutedEventArgs e)
        {
            arrow("down");
            MessageWin();
        }
        private void arrow(string v)
        {
            var (i, j) = getIndex(_puzzle, number, number * number);
            int k = i;// đại diện cho i mới
            int l = j;// đại diện cho j mới
            if (v == "up")
            {
                if (i == number - 1)
                    return;
                k = i + 1;
            }
            else if (v == "down")
            {
                if (i == 0)
                    return;
                k = i - 1;
            }
            else if (v == "right")
            {
                if (j == 0)
                    return;
                l = j - 1;
            }
            else if (v == "left")
            {
                if (j == number - 1)
                    return;
                l = j + 1;
            }



            var valuechange = _puzzle[k, l];
            var value = number * number;

            var w = _games.cropWidth;
            var h = _games.cropHeight;

            
            

            Canvas.SetLeft(listImages[valuechange], j * (w + 2) + startX);
            Canvas.SetTop(listImages[valuechange], i * (h + 2) + startY);

            //Tráo trong mảng puzzle
            (_puzzle[i, j], _puzzle[k, l]) = (_puzzle[k, l], _puzzle[i, j]);
            
        }

        //Hàm hỗ trợ gọi messagebox viết sẵn
        private void Notify(string noty,bool animation)
        {
            var screen = new NottifyDiaglog(noty,animation);
            screen.ShowDialog();
        }
        private void MessageWin()
        {
            if (checkWin(_puzzle, number))
            {
                timer.Stop();
                timer.Dispose();
                Dispatcher.Invoke(() => Notify("Congratulation! You win",true));
                Dispatcher.Invoke(() => lblTimer.Content = "00:00:00");
                Dispatcher.Invoke(() => ResetGame());
            }
        }

        //button exit game
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Ba nút thể hiện mức độ chơi 
        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            number = 3;
        }

        private void Medium_Click(object sender, RoutedEventArgs e)
        {
            number = 4;   
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            number = 5;
        }
    }
}
