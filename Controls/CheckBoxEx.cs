using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FYJ.Winui.Controls
{
    public class CheckBoxEx : CheckBox
    {
        /// <summary>
        /// 获取或设置是否选中(线程安全)
        /// </summary>
        public new bool? IsChecked
        {
            get
            {
                bool? b = null;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    b = base.IsChecked;
                }));

                return b;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.IsChecked = value;
                }));
            }
        }
    }
}
