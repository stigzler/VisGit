using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VisGit.Controls
{
    internal class ComboBoxNullable : ComboBox
    {
        public string NullValueText { get; set; } = "null";

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Items.Add(NullValueText);
            base.OnItemsSourceChanged(oldValue, newValue);
        }
    }
}