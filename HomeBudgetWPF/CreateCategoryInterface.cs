using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetWPF
{
    public interface CreateCategoryInterface
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
    }
}
