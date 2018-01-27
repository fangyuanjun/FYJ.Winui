using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace FYJ.Winui.Controls
{
    public class ProgressBarEx : ProgressBar
    {

        [Bindable(true)]
        [Category("Behavior")]
        public new double Maximum
        {
            get
            {
                double d = 0;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    d = base.Maximum;
                }));

                return d;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.Maximum = value;
                }));
            }
        }

        [Bindable(true)]
        [Category("Behavior")]
        public new double Minimum
        {
            get
            {
                double d = 0;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    d = base.Minimum;
                }));

                return d;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.Minimum = value;
                }));
            }
        }

        [Bindable(true)]
        [Category("Behavior")]
        public new double Value
        {
            get
            {
                double d = 0;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    d = base.Value;
                }));

                return d;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.Value = value;
                }));
            }
        }

        /// <summary>
        /// 获取或设置可见性(线程安全)
        /// </summary>
        [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
        public new Visibility Visibility
        {
            get
            {
                Visibility v = 0;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    v = base.Visibility;
                }));

                return v;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.Visibility = value;
                }));
            }
        }

        public bool Visible
        {
            get
            {
                bool b = true;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    b =(base.Visibility==Visibility.Visible);
                }));

                return b;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if(value)
                    {
                        base.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        base.Visibility = Visibility.Hidden;
                    }
                }));
            }
        }
    }
}
