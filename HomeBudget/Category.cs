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
    // CLASS: Category
    //        - An individual category for budget program
    //        - Valid category types: Income, Expense, Credit, Saving
    // ====================================================================
    /// <summary>
    /// Category class is used to create unique categories to differenciate one from another. 
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets or sets the Id of the category.
        /// </summary>
        /// <value>Unique Id of a category.</value>
        public int Id { get; }

        /// <summary>
        /// Gets or sets the description of the category.
        /// </summary>
        /// <value>Short description about the current category.</value>
        public String Description { get; }

        /// <summary>
        /// Gets or sets the type of the Category.
        /// </summary>
        /// <value>The type of the category.</value>
        public CategoryType Type { get; }

        /// <summary>
        /// CategoryType defines the type that a category is.
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Is a positive amount of money.
            /// </summary>
            Income,
            /// <summary>
            /// A purchase representing a negative amount of money.
            /// </summary>
            Expense,
            /// <summary>
            /// A purchase made on a credit card, can be negative or positive amount of money.
            /// </summary>
            Credit,
            /// <summary>
            /// Saved amount of money, positive amount of money.
            /// </summary>
            Savings
        };

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Initializes an instance of Category class.
        /// </summary>
        /// <param name="id">Unique Id of the category.</param>
        /// <param name="description">Description of the category.</param>
        /// <param name="type">Type of the category.</param>
        /// <example>
        /// <code>
        /// Category category = new Category(1, "Food", Category.CategoryType.Expense);
        /// </code>
        /// </example>
        public Category(int id, String description, CategoryType type = CategoryType.Expense)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }
        // ====================================================================
        // Copy Constructor
        // ====================================================================
        /// <summary>
        /// Initializes an instance of Category class.
        /// </summary>
        /// <param name="category">Category to be copied.</param>
        /// <remarks>Constructor used to copy a category.</remarks>
        /// <example>
        /// <code>
        /// Category category = new Category(1, "Food", Category.CategoryType.Expense);
        /// Category categoryCopy = new Category(category);
        /// </code>
        /// </example>
        public Category(Category category)
        {
            this.Id = category.Id;;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================
        /// <summary>
        /// Overrides the ToString() method to return the description of the category.
        /// </summary>
        /// <returns>A string representing the description of the category.</returns>
        public override string ToString()
        {
            return Description;
        }

    }
}

