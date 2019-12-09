using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace _1712384_1712349_1712407
{
    public abstract class ImageOperation
    {
        //-------Attribute
        public BitmapImage Source { set; get; }

        public string Image { get; set; } 
        //chiều ngang mảnh cắt
        public int cropWidth => (int)Source.Width / numCut;

        // chiều dài mảnh cắt
        public int cropHeight => (int)Source.Height / numCut;

        public int numCut;// số mảnh sẽ cắt ra 

        
        //-------Method
       
    }

    public class playImage : ImageOperation
    {

    }

   
}
