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
    // ====================================================================
    // CLASS: Expense
    //        - An individual expense for budget program
    // ====================================================================
    /// <summary>
    /// Expense class is used to create unique expenses to differenciate one from another. 
    /// </summary>
    public class Expense
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets the Id of the Expense.
        /// </summary>
        /// <value>Unique Id for every Expense.</value>
        public int Id { get; }

        /// <summary>
        /// Gets the Date of the Expense.
        /// </summary>
        /// <value>Represents the day of creation of the item.</value>
        public DateTime Date { get;  }
        /// <summary>
        /// Gets and sets the amount of the Expense
        /// </summary>
        /// <value>Cost of the Expense</value>
        public Double Amount { get; }
        /// <summary>
        /// Gets and sets the description of the Expense
        /// </summary>
        /// <value>Short description of the Expense</value>
        public String Description { get; }
        /// <summary>
        /// Gets and sets the Category of the Expense
        /// </summary>
        /// <value>The number of the <see cref="Category.CategoryType"/></value>
        public int Category { get; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the expense category exists in the
        //        categories object
        // ====================================================================
        /// <summary>
        /// Initializes an instance of Expense class.
        /// </summary>
        /// <param name="id">Unique id of the expense.</param>
        /// <param name="date">Date of the expense.</param>
        /// <param name="category">Number of the category of the expense.</param>
        /// <param name="amount">Cost of the expense.</param>
        /// <param name="description">Description of the expense.</param>
        /// <example>
        /// <code>
        /// Expense expense = new Expense(1, DateTime.Now, (int)Category.CategoryType.Expense, 20, "Eating Out");
        /// </code>
        /// </example>
        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            this.Id = id;
            this.Date = date;
            this.Category = category;
            this.Amount = amount;
            this.Description = description;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================
        /// <summary>
        /// Initializes an instance of Expense class.
        /// </summary>
        /// <param name="obj">Expense to be copied.</param>
        /// <remarks>Constructor used to copy an expense.</remarks>
        /// <example>
        /// <code>
        /// Expense expense = new Expense(1, DateTime.Now, (int)Category.CategoryType.Expense, 20, "Eating Out");
        /// Expense expenseCopy = new Expense(expense);
        /// </code>
        /// </example>
        public Expense (Expense obj)
        {
            this.Id = obj.Id;
            this.Date = obj.Date;
            this.Category = obj.Category;
            this.Amount = obj.Amount;
            this.Description = obj.Description;
           
        }
    }
}
