using FYJ.Winui.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FYJ.Winui
{
    /// <summary>
    /// ColorPickerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        private bool isLoaded = false;
        public ColorPickerWindow()
        {
            InitializeComponent();
            this.Loaded += delegate
            {
                //ChangePostion(this.CurrentColor);
                try
                {
                    Point point_hl = this.ellipsePixel_hl.TranslatePoint(new Point(0, 0), this.CanvImage_hl);
                    this.ellipsePixel_hl.Tag = point_hl.X + "," + point_hl.Y;
                    Point point_s = this.ellipsePixel_s.TranslatePoint(new Point(0, 0), this.CanvImage_s);
                    this.ellipsePixel_s.Tag = point_s.X + "," + point_s.Y;

                    ChangeColor();
                    InitSkinImage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                isLoaded = true;
            };
        }

        /// <summary>
        /// 加载初始图片选择器
        /// </summary>
        private void InitSkinImage()
        {
            WrapPanel wp = new WrapPanel();
            ControlTemplate skinTemplate = (ControlTemplate)App.Current.Resources["skinTemplate"];
            Button btn = new Button();
            btn.Template = skinTemplate;
            wp.Children.Add(btn);
        }

        public delegate void ColorChangedEventHandler(System.Windows.Media.Color color);
        public event ColorChangedEventHandler ColorChanged;

        public delegate void SkinChangedEventHandler(BitmapSource imageSource);
        public event SkinChangedEventHandler SkinChanged;

        public delegate void OpacityChangedEventHandler(int value);
        public event OpacityChangedEventHandler OpacityChanged;

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        private BitmapSource ChangeBitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            //IntPtr hBitmap = bitmap.GetHbitmap();

            //ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            //    hBitmap,
            //    IntPtr.Zero,
            //    Int32Rect.Empty,
            //    BitmapSizeOptions.FromEmptyOptions());

            //if (!DeleteObject(hBitmap))
            //{
            //    throw new System.ComponentModel.Win32Exception();
            //}

            //return wpfBitmap;
            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return bs;
        }

        private void ChangeColor()
        {
            //获取ellipsePixel_s相对于CanvImage_s坐标方法一
            //GeneralTransform generalTransform1 = this.CanvImage_s.TransformToVisual(this.ellipsePixel_s);
            // Point point_s = generalTransform1.Transform(new Point(0, 0));

            //获取ellipsePixel_s相对于CanvImage_s坐标方法二
            //Point point_s = this.ellipsePixel_s.TranslatePoint(new Point(0, 0), this.CanvImage_s);

            double x_s = Convert.ToDouble(this.ellipsePixel_s.Tag.ToString().Split(',')[0]);
            double y_s = Convert.ToDouble(this.ellipsePixel_s.Tag.ToString().Split(',')[1]);
            Point point_s = new Point(x_s, y_s);

            System.Drawing.Bitmap bm_hl = new System.Drawing.Bitmap(360, 100);
            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    bm_hl.SetPixel(i, j, new HSLColor(255, i, (point_s.X + 4) / 360.0, j / 100.0).Color);
                }
            }
            this.pickerImage_hl.Source = ChangeBitmapToImageSource(bm_hl);

            double x_hl = Convert.ToDouble(this.ellipsePixel_hl.Tag.ToString().Split(',')[0]);
            double y_hl = Convert.ToDouble(this.ellipsePixel_hl.Tag.ToString().Split(',')[1]);
            Point point_hl = new Point(x_hl, y_hl);

            System.Drawing.Bitmap bm_s = new System.Drawing.Bitmap(360, (int)this.pickerImage_s.Height);
            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < this.pickerImage_s.Height; j++)
                {
                    bm_s.SetPixel(i, j, new HSLColor(255, (int)point_hl.X + 4, (i + 4) / 360.0, (point_hl.Y + 4) / 100.0).Color);
                }
            }
            this.pickerImage_s.Source = ChangeBitmapToImageSource(bm_s);

            HSLColor hc = new HSLColor();
            hc.Hue = (int)point_hl.X + 4;
            hc.Saturation = (point_s.X + 4) / 360.0;
            hc.Lightness = (point_hl.Y + 4) / 100.0;
            System.Drawing.Color co = hc.Color;
            this.CurrentColor = hc.Color;
            System.Windows.Media.Color c = new Color();
            c.R = co.R;
            c.G = co.G;
            c.B = co.B;
            c.A = 255;
            if (ColorChanged != null && isLoaded)
            {
                ColorChanged(c);            //触发事件
            }
        }

        private void ChangePostion(System.Drawing.Color color)
        {
            try
            {
                HSLColor hsl = new HSLColor(color);

                this.ellipsePixel_hl.SetValue(Canvas.LeftProperty, hsl.Hue > 352 ? 352 : hsl.Hue);
                this.ellipsePixel_hl.SetValue(Canvas.TopProperty, hsl.Lightness > 0.91 ? 92 : hsl.Lightness * 100);

                this.ellipsePixel_s.SetValue(Canvas.LeftProperty, hsl.Saturation > 0.96 ? 352 : hsl.Saturation * 360);
                this.ellipsePixel_s.SetValue(Canvas.TopProperty, 6);
            }
            catch 
            {

            }
        }

        private System.Drawing.Color _currentColor = System.Drawing.Color.FromArgb(21, 160, 253);
        public System.Drawing.Color CurrentColor
        {
            get { return _currentColor; }
            set { _currentColor = value; }
        }

        public HSLColor CurrentHSL
        {
            get;
            set;
        }
        private void CanvImage_sMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((IInputElement)sender);
            this.ellipsePixel_s.SetValue(Canvas.LeftProperty, p.X - 4);
            this.ellipsePixel_s.SetValue(Canvas.TopProperty, p.Y - 4);
            this.ellipsePixel_s.Tag = (p.X - 4) + "," + (p.Y - 4);
            ChangeColor();
        }

        private void CanvImage_hlMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((IInputElement)sender);
            this.ellipsePixel_hl.SetValue(Canvas.LeftProperty, p.X - 4);
            this.ellipsePixel_hl.SetValue(Canvas.TopProperty, p.Y - 4);
            this.ellipsePixel_hl.Tag = (p.X - 4) + "," + (p.Y - 4);
            ChangeColor();
        }

        private void SkinBtnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            object tag = button.Tag;
            string imagePath = (string)tag;
            if (SkinChanged != null && tag != null)
            {
                BitmapSource imageSource = null;
                if (Regex.IsMatch(imagePath, "/.*?;"))
                {
                    if (!imagePath.StartsWith("pack://application:,,,"))
                    {
                        imagePath = "pack://application:,,," + imagePath;
                    }
                    imageSource = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    imagePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + imagePath;
                    FileStream fs = new FileStream(imagePath, FileMode.Open);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
                    fs.Close();
                    var bitmap = new System.Drawing.Bitmap(img);
                    imageSource = ChangeBitmapToImageSource(bitmap);
                }

                SkinChanged(imageSource);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OpacityChanged != null)
            {
                OpacityChanged(100 - (int)e.NewValue);
            }
        }

        private void btnAddPicClick(object sender, RoutedEventArgs e)
        {
            //Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            //if(op.ShowDialog().Value)
            //{
            //    MessageBox.Show("true");
            //}
            //else
            //{
            //    MessageBox.Show("false");
            //}

            System.Windows.Forms.OpenFileDialog fb = new System.Windows.Forms.OpenFileDialog();
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string imagePath = fb.FileName;
                if (imagePath.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase)
                           || imagePath.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase)
                           || imagePath.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase))
                {

                    FileStream fs = new FileStream(imagePath, FileMode.Open);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
                    fs.Close();
                    var bitmap = new System.Drawing.Bitmap(img);
                    BitmapSource imageSource = ChangeBitmapToImageSource(bitmap);
                    SkinChanged(imageSource);
                }
            }
        }
    }
}
