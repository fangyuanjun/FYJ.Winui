using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FYJ.Winui.Controls
{
    public class ListBoxEx : ListBox
    {
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new ItemCollection Items
        {
            get
            {
                ItemCollection items = null;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    items = base.Items;
                }));

                return items;
            }
        }
    }
}
