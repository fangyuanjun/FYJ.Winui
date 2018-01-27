using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FYJ.Winui
{
    /// <summary>
    /// ButtonUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonUserControl : Button, INotifyPropertyChanged
    {
        public ButtonUserControl()
        {
            InitializeComponent();
        }

        //不使用绑定 依赖属性没有意义
        public static readonly DependencyProperty ImgSourceProperty = DependencyProperty.Register("UCImgSource", typeof(ImageSource), typeof(ButtonUserControl), null);
        public static readonly DependencyProperty MouseEnterImageProperty = DependencyProperty.Register("UCMouseEnterImgSource", typeof(ImageSource), typeof(ButtonUserControl), null);

        public event PropertyChangedEventHandler PropertyChanged;

        [Browsable(true)]
        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set
            {
                SetValue(ImgSourceProperty, value);
                if (this.PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ImgSource"));
                }
            }
        }

        [Description("获取或设置鼠标进入时的图片")]
        [Category("Common Properties")]
        [Browsable(true)]
        public ImageSource MouseEnterImgSource
        {
            get { return (ImageSource)GetValue(MouseEnterImageProperty); }
            set
            {
                SetValue(MouseEnterImageProperty, value);
                if (this.PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MouseEnterImgSource"));
                }
            }
        }

    }
}
