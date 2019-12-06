using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace _1712384_1712349_1712407
{
    /// <summary>
    /// Interaction logic for SelectDialog.xaml
    /// </summary>
    public partial class SelectDialog : Window
    {
        public SelectDialog()
        {
            InitializeComponent();
        }

        public class Picture
        {
            public string Source { get; set; }
        }

        BindingList<Picture> _picture = null;

        public class PictureDAO
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static BindingList<Picture> GetAll()
            {
                BindingList<Picture> result = null;
                var lines = File.ReadAllLines("Source.txt");
                var count = int.Parse(lines[0]);

                if(count>0)
                {
                    result = new BindingList<Picture>();

                    for(int i=0;i<count;i++)
                    {
                        var picture = new Picture()
                        {
                            Source = lines[i + 1]
                        };
                        result.Add(picture);
                    }

                }
                return result;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _picture = PictureDAO.GetAll();
            pictureComboBox.ItemsSource = _picture;
        }

        private void OkButon_Click(object sender, RoutedEventArgs e)
        {
            if(SourceData!="")
            {
                SourceData = "Images\\"+SourceData;
                //MessageBox.Show(SourceData);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please choose a picture");
            }
         
        }

        public string SourceData="";
        private void pictureComboBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedIndex;
            if(item>=0)
            {
                SourceData = _picture[item].Source;
            }
            //MessageBox.Show(_picture[item].Source);
            //MessageBox.Show(item.ToString());
            //if (item!=null)
            //{
            //    MessageBox.Show(item.ToString());
            //}
         
        }
    }
}
