using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Controls;
using FYJ.Winui.Util;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;

namespace FYJ.Winui
{
    /// <summary>
    /// Windows窗体基类
    /// </summary>
    public class BaseWindow : Window
    {
        #region 常量
        private const int RESIZE_BORDER = 8;
        private const int HIDE_BORDER = 3;
        private const int FLASH_TIME = 500;
        private const int MAGNET_BORDER = 20;
        #endregion

        #region 变量
        private HwndSource hwndSource;
        private bool isHidded = false;
        private Rect oldRect;
        private ColorPickerWindow pickerWindow;
        #endregion


        [Browsable(true)]
        [Description("是否显示换肤按钮")]
        public bool IsShowSkinButton
        {
            get;
            set;
        }

        [Browsable(true)]
        [Description("是否显示菜单按钮")]
        public bool IsShowMenuButton
        {
            get;
            set;
        }

        [Browsable(true)]
        [Description("是否关闭应用程序")]
        public bool IsCloseApplication
        {
            get;
            set;
        }

        [Browsable(true)]
        [Description("标题栏菜单")]
        public ContextMenu TitleContextMenu
        {
            get;
            set;
        }

        #region 构造函数
        public BaseWindow()
        {
            this.AllowDrop = true;
            //this.Style = (Style)App.Current.Resources["MainWindowStyle"];
            this.SourceInitialized += delegate(object sender, EventArgs e)
            {
                this.hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                this.hwndSource.AddHook(Win32.WindowProc);
            };

            this.Loaded += delegate
            {
                try
                {
                    //ControlTemplate mainWindowTemplate = (ControlTemplate)App.Current.Resources["BaseWindowControlTemplate"];
                    ControlTemplate mainWindowTemplate = this.Template;

                    ButtonUserControl btnMenu = (ButtonUserControl)mainWindowTemplate.FindName("btnMenu", this);
                    ButtonUserControl btnSkin = (ButtonUserControl)mainWindowTemplate.FindName("btnSkin", this);
                    ButtonUserControl btnMin = (ButtonUserControl)mainWindowTemplate.FindName("btnMin", this);
                    ButtonUserControl btnMax = (ButtonUserControl)mainWindowTemplate.FindName("btnMax", this);

                    if (!IsShowMenuButton)
                    {
                        btnMenu.Visibility = Visibility.Collapsed;
                    }
                    if (IsShowSkinButton)
                    {
                        pickerWindow = new ColorPickerWindow();
                        ChangeColor(Color.FromArgb(pickerWindow.CurrentColor.A, pickerWindow.CurrentColor.R, pickerWindow.CurrentColor.G, pickerWindow.CurrentColor.B));
                        pickerWindow.ColorChanged += delegate(System.Windows.Media.Color c)
                        {
                            ChangeColor(c);
                        };

                        pickerWindow.SkinChanged += delegate(BitmapSource bitmapSource)
                        {
                            ChangeSkin(bitmapSource);
                        };

                        pickerWindow.OpacityChanged += delegate(int value)
                        {
                            this.Opacity = Math.Round(value / 100.0, 2);
                        };
                    }
                    else
                    {
                        //2015-7-13
                        System.Drawing.Color _currentColor = System.Drawing.Color.FromArgb(21, 160, 253);
                        ChangeColor(Color.FromArgb(_currentColor.A, _currentColor.R, _currentColor.G, _currentColor.B));

                        btnSkin.Visibility = Visibility.Collapsed;
                    }

                    if (this.ResizeMode == ResizeMode.NoResize)
                    {
                        btnMax.Visibility = Visibility.Collapsed;
                        btnMin.Visibility = Visibility.Collapsed;
                    }

                    if (this.ResizeMode == ResizeMode.CanMinimize)
                    {
                        btnMax.IsEnabled = false;
                    }

                    if (this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip)
                    {
                        ImageSourceConverter imgConv = new ImageSourceConverter();
                        if (this.WindowState == WindowState.Normal)
                        {
                            btnMax.ImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_max_normal.png");
                            btnMax.MouseEnterImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_max_highlight.png");
                            btnMax.ToolTip = "最大化";
                        }
                        else
                        {
                            btnMax.ImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_restore_normal.png");
                            btnMax.MouseEnterImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_restore_highlight.png");
                            btnMax.ToolTip = "向下还原";
                        }
                    }

                    InitializeEvent();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载失败:" + ex.Message);
                }
            };

            this.StateChanged += delegate
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    System.Drawing.Size size = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
                    this.Width = size.Width;
                    this.Height = size.Height;
                }
                else
                {
                    if(oldRect.Width!=0)
                    {
                        this.Width = oldRect.Width;
                        this.Height = oldRect.Height;
                        this.Top = oldRect.Top;
                        this.Left = oldRect.Left;
                    }
                }
            };

            this.Closed += delegate
            {
                if (pickerWindow != null)
                {
                    pickerWindow.Close();
                }
                if (IsCloseApplication)
                {
                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
            };
        }
        #endregion

        private void ChangeColor(Color c)
        {
            ControlTemplate mainWindowTemplate = this.Template;
            SolidColorBrush brush = new SolidColorBrush(c);
            Grid grid = (Grid)mainWindowTemplate.FindName("gridTitle", this);
            if (grid != null)
            {
                grid.Background = brush;
            }
        }

        private void ChangeSkin(BitmapSource bitmapSource)
        {
            ControlTemplate mainWindowTemplate = this.Template;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapSource;
            brush.TileMode = TileMode.Tile;
            //brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            //brush.Viewport = new Rect(0, 0, 1, 1);
            //brush.ViewboxUnits = BrushMappingMode.Absolute;
            //brush.Viewbox = new Rect(0, 0, 0, 18);
            Grid grid = (Grid)mainWindowTemplate.FindName("gridTitle", this);
            if (grid != null)
            {
                grid.Background = brush;
            }
        }

        private void InitializeEvent()
        {
            ControlTemplate mainWindowTemplate = this.Template;
            Button minBtn = (Button)mainWindowTemplate.FindName("btnMin", this);
            ButtonUserControl maxBtn = (ButtonUserControl)mainWindowTemplate.FindName("btnMax", this);
            Button closeBtn = (Button)mainWindowTemplate.FindName("btnClose", this);
            Button skinBtn = (Button)mainWindowTemplate.FindName("btnSkin", this);
            Button menuBtn = (Button)mainWindowTemplate.FindName("btnMenu", this);

            minBtn.Click += delegate
            {
                oldRect = new Rect(this.Left, this.Top, this.Width, this.Height);
                this.WindowState = WindowState.Minimized;
            };

            maxBtn.Click += delegate
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();
                if (this.WindowState == WindowState.Normal)
                {
                    oldRect = new Rect(this.Left, this.Top, this.Width, this.Height);
                    this.WindowState = WindowState.Maximized;
                    maxBtn.ImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_restore_normal.png");
                    maxBtn.MouseEnterImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_restore_highlight.png");
                    maxBtn.ToolTip = "向下还原";
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    maxBtn.ImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_max_normal.png");
                    maxBtn.MouseEnterImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_max_highlight.png");
                    maxBtn.ToolTip = "最大化";
                }
            };

            closeBtn.Click += delegate
            {
                this.Close();
                //接下来的在Closed事件里处理
            };

            menuBtn.Click += delegate
            {
                if (this.TitleContextMenu != null)
                {
                    this.TitleContextMenu.IsOpen = true;
                }
            };
            //Border borderTitle = (Border)mainWindowTemplate.FindName("borderTitle", this);
            //borderTitle.MouseMove += delegate(object sender, MouseEventArgs e)
            //{
            //    if (e.LeftButton == MouseButtonState.Pressed)
            //    {
            //        this.DragMove();
            //    }
            //};
            //borderTitle.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            //{
            //    if (e.ClickCount >= 2)
            //    {
            //        maxBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            //    }
            //};

            skinBtn.Click += delegate
            {
                pickerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                pickerWindow.Top = this.Top + 18;
                pickerWindow.Left = this.Left + this.Width - 138 - pickerWindow.Width;
                pickerWindow.Show();
            };
        }

        #region 事件
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (pickerWindow != null)
            {
                pickerWindow.Hide();
            }

            base.OnMouseRightButtonDown(e);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (pickerWindow != null)
            {
                pickerWindow.Hide();
            }

            Win32.POINT p;
            if (!Win32.GetCursorPos(out p))
                return;
            if (this.Left + RESIZE_BORDER < p.x && this.Left + this.ActualWidth - RESIZE_BORDER > p.x && this.Top + RESIZE_BORDER < p.y && this.Top + this.ActualHeight - RESIZE_BORDER > p.y)
            {
                this.DragMove();
            }

            //双击最大化或还原
            if (this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                ControlTemplate mainWindowTemplate = this.Template;
                Button maxBtn = (Button)mainWindowTemplate.FindName("btnMax", this);
                if (e.ClickCount >= 2)
                {
                    maxBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.ResizeMode != ResizeMode.CanResize)
            {
                base.OnMouseMove(e);
                return;
            }

            Win32.POINT p;
            if (!Win32.GetCursorPos(out p))
                return;
            if (this.Left <= p.x && this.Left + RESIZE_BORDER >= p.x
                && this.Top <= p.y && this.Top + RESIZE_BORDER >= p.y)
            {
                this.Cursor = Cursors.SizeNWSE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61444), IntPtr.Zero);
            }
            else if (this.Left <= p.x && this.Left + RESIZE_BORDER >= p.x
                && this.Top + this.ActualHeight - RESIZE_BORDER <= p.y && this.Top + this.ActualHeight >= p.y)
            {
                this.Cursor = Cursors.SizeNESW;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61447), IntPtr.Zero);
            }
            else if (this.Left + this.ActualWidth - RESIZE_BORDER <= p.x && this.Left + this.ActualWidth >= p.x
                && this.Top <= p.y && this.Top + RESIZE_BORDER >= p.y)
            {
                this.Cursor = Cursors.SizeNESW;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61445), IntPtr.Zero);
            }
            else if (this.Left + this.ActualWidth - RESIZE_BORDER <= p.x && this.Left + this.ActualWidth >= p.x
                && this.Top + this.ActualHeight - RESIZE_BORDER <= p.y && this.Top + this.ActualHeight >= p.y)
            {
                this.Cursor = Cursors.SizeNWSE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61448), IntPtr.Zero);
            }
            else if (this.Top <= p.y && this.Top + RESIZE_BORDER >= p.y)
            {
                this.Cursor = Cursors.SizeNS;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61443), IntPtr.Zero);
            }
            else if (this.Left <= p.x && this.Left + RESIZE_BORDER >= p.x)
            {
                this.Cursor = Cursors.SizeWE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61441), IntPtr.Zero);
            }
            else if (this.Top + this.ActualHeight - RESIZE_BORDER <= p.y && this.Top + this.ActualHeight >= p.y)
            {
                this.Cursor = Cursors.SizeNS;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61446), IntPtr.Zero);
            }
            else if (this.Left + this.ActualWidth - RESIZE_BORDER <= p.x && this.Left + this.ActualWidth >= p.x)
            {
                this.Cursor = Cursors.SizeWE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61442), IntPtr.Zero);
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }
            if (this.WindowState == WindowState.Normal)
            {
                if (this.isHidded)
                {
                    if (this.Left < p.x && this.Left + this.ActualWidth > p.x && this.Top < p.y && this.Top + this.ActualHeight > p.y)
                    {
                        this.Top = 0;
                        this.Topmost = false;
                    }
                }
                else
                {
                    if (this.Top <= 0 && this.Left <= 0)
                    {
                        this.Left = 0;
                        this.Top = HIDE_BORDER - this.ActualHeight;
                        this.isHidded = true;
                        this.Topmost = true;
                    }
                    else if (this.Top <= 0 && this.Left >= SystemParameters.VirtualScreenWidth - this.ActualWidth)
                    {
                        this.Left = SystemParameters.VirtualScreenWidth - this.ActualWidth;
                        this.Top = HIDE_BORDER - this.ActualHeight;
                        this.isHidded = true;
                        this.Topmost = true;
                    }
                    else if (this.Top <= 0)
                    {
                        this.Top = HIDE_BORDER - this.ActualHeight;
                        this.isHidded = true;
                        this.Topmost = true;
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.ResizeMode != ResizeMode.CanResize)
            {
                base.OnMouseLeave(e);
                return;
            }

            if (this.WindowState == WindowState.Normal)
            {
                if (this.isHidded)
                {
                    this.Top = HIDE_BORDER - this.ActualHeight;
                    this.isHidded = true;
                    this.Topmost = true;
                }
            }
            base.OnMouseLeave(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            e.Effects = System.Windows.DragDropEffects.Move;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
             if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                List<String> tempList = new List<string>();
                foreach (String s in files)
                {
                    if (File.Exists(s))
                    {
                        if (s.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase)
                            || s.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase)
                            || s.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase))
                        {
                            FileStream fs = new FileStream(s, FileMode.Open);
                            System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
                            fs.Close();
                            var bitmap = new System.Drawing.Bitmap(img);
                            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            this.ChangeSkin(bs);
                            //this.BackgroundImage = Image.FromFile(s);
                            //MyFormTemp.CurrentBackgroundImage = this.BackgroundImage;
                        }
                    }

                    //if (Directory.Exists(s))
                    //{
                    //    foreach (String f in Directory.GetFiles(s))
                    //    {
                    //        tempList.Add(f);
                    //    }
                    //}
                }
            }
        }
        #endregion
    }
}
