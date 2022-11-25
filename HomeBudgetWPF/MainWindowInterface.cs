using System;
using System.Collections.Generic;
using System.Text;
using Budget;
namespace HomeBudgetWPF
{
    public interface MainWindowInterface
    {
        /// <summary>
        /// Called to refresh a list of categories with the current results.
        /// </summary>
        /// <param name="categories">List of the current categories.</param>
        void BindCategories(List<Category> categories);
        /// <summary>
        /// Called to refresh the data view
        /// </summary>
        void ContextChanged();
    }
}
