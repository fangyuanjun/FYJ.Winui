using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FYJ.Winui.Controls
{
    public class LogListBox : ListBox
    {
        private int errorCount;
        private int warnCount;
        /// <summary>
        /// 错误个数
        /// </summary>
        public int ErrorCount
        {
            get { return errorCount; }
        }

        /// <summary>
        /// 警告个数
        /// </summary>
        public int WarnCount
        {
            get { return warnCount; }
        }
        private string DateTimeNowString
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss    "); }
        }
        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="msg"></param>
        public void AddErrorItem(string msg)
        {
            this.Dispatcher.Invoke((Action)delegate()
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = DateTimeNowString + msg;
                    item.Foreground = Brushes.Red;
                    errorCount++;
                    this.Items.Add(item);
                });
        }

        /// <summary>
        /// 添加警告信息
        /// </summary>
        /// <param name="msg"></param>
        public void AddWarnItem(string msg)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = DateTimeNowString + msg;
                item.Foreground = Brushes.Yellow;
                warnCount++;
                this.Items.Add(item);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void AddItem(string msg)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = DateTimeNowString + msg;
                this.Items.Add(item);
            });
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.ContextMenu = CreateMenu();
        }

        private ContextMenu CreateMenu()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem clearItem = new MenuItem();
            clearItem.Header = "清除";
            clearItem.Click += (sender, e) =>
            {
                errorCount = 0;
                warnCount = 0;
                this.Items.Clear();
            };
            MenuItem copyMenu = new MenuItem();
            copyMenu.Header = "复制";
            copyMenu.Click += (sender, e) =>
            {
                ListBoxItem item = this.SelectedItem as ListBoxItem;
                if (item != null)
                {
                    Clipboard.SetText(item.Content.ToString());
                }
            };

            MenuItem exportMenu = new MenuItem();
            exportMenu.Header = "导出...";
            exportMenu.Click += (sender, e) =>
            {
                SaveFileDialog sf = new SaveFileDialog();

                sf.Filter = "*.txt|*.txt";
                if (sf.ShowDialog() == true)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (ListBoxItem item in this.Items)
                    {
                        sb.AppendLine(item.Content.ToString());
                    }
                    StreamWriter write = new StreamWriter(sf.FileName, false, Encoding.Default, 40960);
                    write.Write(sb.ToString());
                    write.Close();
                }
            };

            menu.Items.Add(clearItem);
            menu.Items.Add(copyMenu);
            menu.Items.Add(exportMenu);

            return menu;
        }
    }
}
