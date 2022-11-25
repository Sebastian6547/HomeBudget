using System;
using System.Collections.Generic;
using System.Text;
using Budget;

namespace HomeBudgetWPF
{
    public interface CreateExpenseInterface
    {
        /// <summary>
        /// Called after creating an expense is successful.
        /// </summary>
        void ShowSuccess();
        /// <summary>
        /// Called when an error is encountered when adding an expense.
        /// </summary>
        /// <param name="msg">Message documenting the error that occured.</param>
        void ShowError(string msg);
        /// <summary>
        /// Called to refresh a list of categories with the current results.
        /// </summary>
        /// <param name="categories">List of the current categories.</param>
        void BindCategories(List<Category> categories);
    }
}
