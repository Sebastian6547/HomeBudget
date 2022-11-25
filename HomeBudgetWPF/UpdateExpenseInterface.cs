using System;
using System.Collections.Generic;
using System.Text;
using Budget;

namespace HomeBudgetWPF
{
    public interface UpdateExpenseInterface
    {
        /// <summary>
        /// Called when an error is encountered when adding an expense. Alerts the user that there was an error updating the expense.
        /// </summary>
        /// <param name="msg">Message documenting the error that occured.</param>
        void ShowError(string msg);
        /// <summary>
        /// Called to refresh a list of categories with the current results. Binds a list of categories to a combo box.
        /// </summary>
        /// <param name="categories">List of the current categories.</param>
        void BindCategories(List<Category> categories);
        /// <summary>
        /// Fills all input fields with the original values of the expense
        /// </summary>
        /// <param name="date"></param>
        /// <param name="description"></param>
        /// <param name="amount"></param>
        /// <param name="categoryIndex"></param>
        void FillFields(DateTime date, string description, double amount, int categoryIndex);
        /// <summary>
        /// Closes the current window. Called when update was successful
        /// </summary>
        void Success();
    }
}
