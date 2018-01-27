using System;
using System.Collections.Generic;
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

namespace FYJ.Winui.Controls
{
    /// <summary>
    /// ImageButton.xaml 的交互逻辑
    /// </summary>
    public partial class ImageButton : UserControl
    {
        public ImageButton()
        {
            InitializeComponent();
           
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(ImageButton_IsEnabledChanged);
            this.btn.Click += btn_Click;
        }

        public event RoutedEventHandler Click;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if(Click!=null)
            {
                Click(sender, e);
            }
        }

        void ImageButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled && ImageSource != null)
            {
                innerImage.Source = ImageSource;
            }
            else if (!this.IsEnabled && DisabledImageSource != null)
            {
                innerImage.Source = DisabledImageSource;
            }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton), new UIPropertyMetadata(null));


        public ImageSource DisabledImageSource
        {
            get { return (ImageSource)GetValue(DisabledImageSourceProperty); }
            set { SetValue(DisabledImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GrayImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisabledImageSourceProperty =
            DependencyProperty.Register("DisabledImageSource", typeof(ImageSource), typeof(ImageButton), new UIPropertyMetadata(null));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.IsEnabled && ImageSource != null)
            {
                innerImage.Source = ImageSource;
            }
            else if (!this.IsEnabled && DisabledImageSource != null)
            {
                innerImage.Source = DisabledImageSource;
            }

            if(this.ImageSource!=null)
            {
                this.innerImage.Height = this.ImageSource.Height;
                this.innerImage.Width = this.ImageSource.Width;
            }
        }
    }
}
