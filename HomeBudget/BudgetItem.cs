using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    /// <summary>
    /// BudgetItem presents properties to get and set attributes.
    /// </summary>
    public class BudgetItem
    {
        /// <summary>
        /// Gets or sets the categoryID of the budget item.
        /// </summary>
        /// <value>Id of the category of the item.</value>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the expenseID of the budget item.
        /// </summary>
        /// <value>Id of the expense of the item.</value>
        public int ExpenseID { get; set; }

        /// <summary>
        /// Gets or sets the date of the budget item.
        /// </summary>
        /// <value>Represents the day of creation of the item.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the category of the budget item.
        /// </summary>
        /// <value>Category of the item.</value>
        public String Category { get; set; }

        /// <summary>
        /// Gets or sets the short description of the budget item.
        /// </summary>
        /// <value>Short description of what the item is.</value>
        public String ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the amount of the budget item.
        /// </summary>
        /// <value>Cost of the item.</value>
        public Double Amount { get; set; }

        /// <summary>
        /// Gets or sets the balance of the budget item.
        /// </summary>
        /// <value>Amount of money left after the amount was removed or added.</value>
        public Double Balance { get; set; }

    }
    /// <summary>
    /// BudgetItemsByMonth represents a group of Budget Items made in a month. Presents properties to get and set attributes of the group.
    /// </summary>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// Gets or sets the month of the group.
        /// </summary>
        /// <value>Year/month for the group.</value>
        public String Month { get; set; }

        /// <summary>
        /// Gets or sets the list of budget items for the group.
        /// </summary>
        /// <value>List of BudgetItems for the month.</value>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Gets or sets the total for the group.
        /// </summary>
        /// <value>Total money spent or gained during the month.</value>
        public Double Total { get; set; }

    }

    /// <summary>
    /// BudgetItemsByCategory represents a group of Budget Items for a category. Presents properties to get and set attributes of the group.
    /// </summary>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// Gets or sets the category of the group.
        /// </summary>
        /// <value>Name of the category for the group.</value>
        public String Category { get; set; }

        /// <summary>
        /// Gets or sets the list of budget items for the group.
        /// </summary>
        /// <value>List of BudgetItems for the category.</value>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Gets or sets the total for the group.
        /// </summary>
        /// <value>Total money spent or gained for the category.</value>
        public Double Total { get; set; }

    }


}
