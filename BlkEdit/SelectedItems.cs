using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

namespace MR.BlkEdit
{
    public class SelectedItems
    {
        public int[] WorkItemIDs;
        public string SuitBintoChange;

        private static SelectedItems _selectedItems;

        public static SelectedItems Instance 
        { 
            get
            {
                if (_selectedItems == null)
                {
                    _selectedItems = new SelectedItems();
                }
                return _selectedItems;
            }
         }
    }
}
