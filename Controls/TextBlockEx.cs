using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FYJ.Winui.Controls
{
    public class TextBlockEx : TextBlock
    {
        /// <summary>
        /// 获取或设置文本内容（线程安全）如果没有文本则会返回"" 
        /// </summary> 
        [Bindable(true)]
        [Category("Behavior")]
        public new string Text
        {
            get
            {
                string text="";
                this.Dispatcher.Invoke((Action)(() =>
                {
                    text= base.Text;
                }));

                return text;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.Text = value;
                }));
            }
        }
    }
}
