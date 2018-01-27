using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;


namespace FYJ.Winui.Controls
{
    public class RichTextBoxEx : RichTextBox
    {
        /// <summary>
        /// 获取或设置文本内容（线程安全） 如果没有文本则会返回""  
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public  string Text
        {
            get
            {
                string text = "";
                this.Dispatcher.Invoke((Action)(() =>
                {
                    TextRange textRange = new TextRange(this.Document.ContentStart, this.Document.ContentEnd);
                    text= textRange.Text;
                }));

                return text;
            }
            set
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    this.Document.Blocks.Clear();
                    this.AppendText(value);
                }));
            }
        }
    }
}
