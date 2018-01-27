using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FYJ.Winui.Controls
{
    public class ComboBoxEx : ComboBox
    {
        /// <summary>
        /// 获取或设置选择的值(线程安全)
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public new object SelectedValue
        {
            get
            {
                object obj = null;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    obj = base.SelectedValue;
                }));

                return obj;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.SelectedValue = value;
                }));
            }
        }

        /// <summary>
        /// 获取或设置选择的第一个索引(线程安全)
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public new int SelectedIndex
        {
            get
            {
                int index = -1;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    index = base.SelectedIndex;
                }));

                return index;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.SelectedIndex = value;
                }));
            }
        }

        /// <summary>
        /// 获取或设置选择的第一项(线程安全)
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public new object SelectedItem
        {
            get
            {
                object obj = null;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    obj = base.SelectedItem;
                }));

                return obj;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    base.SelectedItem = value;
                }));
            }
        }

        /// <summary>
        /// 获取或设置文本内容（线程安全）如果没有文本则会返回"" 
        /// </summary> 
        public new string Text
        {
            get
            {
                string text = "";
                this.Dispatcher.Invoke((Action)(() =>
                {
                    text = base.Text;
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
