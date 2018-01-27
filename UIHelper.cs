using FYJ.Winui;
using FYJ.Winui.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
   public class UIHelper 
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
        private Window window;
        #endregion

        public UIHelper(Window window)
        {
            this.window = window;
        }

        public void Init()
        {
            window.Style = (Style)App.Current.Resources["BaseWindowStyle"];
            window.SourceInitialized += delegate(object sender, EventArgs e)
            {
                this.hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                this.hwndSource.AddHook(Win32.WindowProc);
            };

            window.Loaded += delegate
            {
                try
                {
                    ControlTemplate mainWindowTemplate = window.Template;

                    ButtonUserControl btnMin = (ButtonUserControl)mainWindowTemplate.FindName("btnMin", window);
                    ButtonUserControl btnMax = (ButtonUserControl)mainWindowTemplate.FindName("btnMax", window);
                    ButtonUserControl closeBtn = (ButtonUserControl)mainWindowTemplate.FindName("btnClose", window);
                    ButtonUserControl btnMenu = (ButtonUserControl)mainWindowTemplate.FindName("btnMenu", window);
                    ButtonUserControl btnSkin = (ButtonUserControl)mainWindowTemplate.FindName("btnSkin", window);
                    //隐藏菜单按钮
                    if (btnMenu != null)
                    {
                        btnMenu.Visibility = Visibility.Collapsed;
                    }
                    //隐藏调色按钮
                    if (btnSkin != null)
                    {
                        btnSkin.Visibility = Visibility.Collapsed;
                    }

                    if (window.ResizeMode == ResizeMode.NoResize)
                    {
                        btnMax.Visibility = Visibility.Collapsed;
                        btnMin.Visibility = Visibility.Collapsed;
                    }

                    if (window.ResizeMode == ResizeMode.CanMinimize)
                    {
                        btnMax.IsEnabled = false;
                    }

                    if (window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip)
                    {
                        ImageSourceConverter imgConv = new ImageSourceConverter();
                        if (window.WindowState == WindowState.Normal)
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

                    btnMin.Click += delegate
                    {
                        oldRect = new Rect(window.Left, window.Top, window.Width, window.Height);
                        window.WindowState = WindowState.Minimized;
                    };

                    btnMax.Click += delegate
                    {
                        ImageSourceConverter imgConv = new ImageSourceConverter();
                        if (window.WindowState == WindowState.Normal)
                        {
                            oldRect = new Rect(window.Left, window.Top, window.Width, window.Height);
                            window.WindowState = WindowState.Maximized;
                            btnMax.ImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_restore_normal.png");
                            btnMax.MouseEnterImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_restore_highlight.png");
                            btnMax.ToolTip = "向下还原";
                        }
                        else
                        {
                            window.WindowState = WindowState.Normal;
                            btnMax.ImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_max_normal.png");
                            btnMax.MouseEnterImgSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,,/FYJ.Winui;component/Resources/btn_max_highlight.png");
                            btnMax.ToolTip = "最大化";
                        }
                    };

                    closeBtn.Click += delegate
                    {
                        window.Close();
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载失败:" + ex.Message);
                }
            };

            window.StateChanged += delegate
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    System.Drawing.Size size = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
                    window.Width = size.Width;
                    window.Height = size.Height;
                }
                else
                {
                    window.Width = oldRect.Width;
                    window.Height = oldRect.Height;
                    window.Top = oldRect.Top;
                    window.Left = oldRect.Left;
                }
            };

            window.MouseLeftButtonDown += Window1_MouseLeftButtonDown;
            window.MouseMove += Window1_MouseMove;
            window.MouseLeave += Window1_MouseLeave;
        }

        private void Window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Win32.POINT p;
            if (!Win32.GetCursorPos(out p))
                return;
            if (window.Left + RESIZE_BORDER < p.x && window.Left + window.ActualWidth - RESIZE_BORDER > p.x && window.Top + RESIZE_BORDER < p.y && window.Top + window.ActualHeight - RESIZE_BORDER > p.y)
            {
                window.DragMove();
            }

            //双击最大化或还原
            if (window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                ControlTemplate mainWindowTemplate = window.Template;
                Button maxBtn = (Button)mainWindowTemplate.FindName("btnMax", window);
                if (e.ClickCount >= 2)
                {
                    maxBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }

        private void Window1_MouseMove(object sender, MouseEventArgs e)
        {
            if (window.ResizeMode != ResizeMode.CanResize)
            {
                return;
            }

            Win32.POINT p;
            if (!Win32.GetCursorPos(out p))
                return;
            if (window.Left <= p.x && window.Left + RESIZE_BORDER >= p.x
                && window.Top <= p.y && window.Top + RESIZE_BORDER >= p.y)
            {
                window.Cursor = Cursors.SizeNWSE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61444), IntPtr.Zero);
            }
            else if (window.Left <= p.x && window.Left + RESIZE_BORDER >= p.x
                && window.Top + window.ActualHeight - RESIZE_BORDER <= p.y && window.Top + window.ActualHeight >= p.y)
            {
                window.Cursor = Cursors.SizeNESW;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61447), IntPtr.Zero);
            }
            else if (window.Left + window.ActualWidth - RESIZE_BORDER <= p.x && window.Left + window.ActualWidth >= p.x
                && window.Top <= p.y && window.Top + RESIZE_BORDER >= p.y)
            {
                window.Cursor = Cursors.SizeNESW;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61445), IntPtr.Zero);
            }
            else if (window.Left + window.ActualWidth - RESIZE_BORDER <= p.x && window.Left + window.ActualWidth >= p.x
                && window.Top + window.ActualHeight - RESIZE_BORDER <= p.y && window.Top + window.ActualHeight >= p.y)
            {
                window.Cursor = Cursors.SizeNWSE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61448), IntPtr.Zero);
            }
            else if (window.Top <= p.y && window.Top + RESIZE_BORDER >= p.y)
            {
                window.Cursor = Cursors.SizeNS;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61443), IntPtr.Zero);
            }
            else if (window.Left <= p.x && window.Left + RESIZE_BORDER >= p.x)
            {
                window.Cursor = Cursors.SizeWE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61441), IntPtr.Zero);
            }
            else if (window.Top + window.ActualHeight - RESIZE_BORDER <= p.y && window.Top + window.ActualHeight >= p.y)
            {
                window.Cursor = Cursors.SizeNS;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61446), IntPtr.Zero);
            }
            else if (window.Left + window.ActualWidth - RESIZE_BORDER <= p.x && window.Left + window.ActualWidth >= p.x)
            {
                window.Cursor = Cursors.SizeWE;
                if (e.LeftButton == MouseButtonState.Pressed)
                    Win32.SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61442), IntPtr.Zero);
            }
            else
            {
                window.Cursor = Cursors.Arrow;
            }
            if (window.WindowState == WindowState.Normal)
            {
                if (this.isHidded)
                {
                    if (window.Left < p.x && window.Left + window.ActualWidth > p.x && window.Top < p.y && window.Top + window.ActualHeight > p.y)
                    {
                        window.Top = 0;
                        window.Topmost = false;
                    }
                }
                else
                {
                    if (window.Top <= 0 && window.Left <= 0)
                    {
                        window.Left = 0;
                        window.Top = HIDE_BORDER - window.ActualHeight;
                        this.isHidded = true;
                        window.Topmost = true;
                    }
                    else if (window.Top <= 0 && window.Left >= SystemParameters.VirtualScreenWidth - window.ActualWidth)
                    {
                        window.Left = SystemParameters.VirtualScreenWidth - window.ActualWidth;
                        window.Top = HIDE_BORDER - window.ActualHeight;
                        this.isHidded = true;
                        window.Topmost = true;
                    }
                    else if (window.Top <= 0)
                    {
                        window.Top = HIDE_BORDER - window.ActualHeight;
                        this.isHidded = true;
                        window.Topmost = true;
                    }
                }
            }
        }

        private void Window1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (window.ResizeMode != ResizeMode.CanResize)
            {
                return;
            }

            if (window.WindowState == WindowState.Normal)
            {
                if (this.isHidded)
                {
                    window.Top = HIDE_BORDER - window.ActualHeight;
                    this.isHidded = true;
                    window.Topmost = true;
                }
            }
        }
    }
}
