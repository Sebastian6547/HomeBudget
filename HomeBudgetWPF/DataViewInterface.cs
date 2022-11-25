using System;
using System.Collections.Generic;
using System.Text;
using Budget;

namespace HomeBudgetWPF
{
    public interface DataViewInterface
    {
        /// <summary>
        /// Called to show data of BudgetItems filtered by month given a list of BudgetItemsByMonth.
        /// </summary>
        void ShowData();

        /// <summary>
        /// Called to show data of BudgetItems filtered by month given a list of BudgetItemsByMonth.
        /// </summary>
        void ShowDataByMonth();

        /// <summary>
        /// Called to show data of BudgetItems filtered by category given a list of BudgetItemsByCategory.
        /// </summary>
        void ShowDataByCategory();

        /// <summary>
        /// Called to show data of BudgetItems filtered by month and category given a list of dictionaries containing BudgetItems filtered by month and category.
        void ShowDataByMonthByCategory();
        
        /// <summary>
        /// Getter/setter used to either set or get the DataSource of the DataView
        /// </summary>
        List<object> DataSource { get; set; }

        /// <summary>
        /// Outputs an error message to the user
        /// </summary>
        /// <param name="msg"></param>
        void ShowError(string msg);

        /// <summary>
        /// Selects an item from a datagrid with the passed index
        /// </summary>
        /// <param name="index">Index of the item to select</param>
        void SelectDataGridItem(int index);
    }
}
