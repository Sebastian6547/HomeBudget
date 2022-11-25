using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    /// <summary>
    /// Categories class is used to read and write from a <see cref="Category"/> table in a database.
    /// </summary>
    public class Categories
    {
        private SQLiteConnection _connection;

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Initializes an instance of Categories class. abc  
        /// </summary>
        /// <param name="connection">Connection to a database.</param>
        /// <param name="newDb">Boolean that determines if the database is new.</param>
        /// <remarks>If the database is new, the categories and categoryTypes table will be filled with default values.</remarks>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// </code>
        /// </example>
        public Categories(SQLiteConnection connection, Boolean newDb)
        {
            _connection = connection;
            if (newDb)
            {             
                SetCategoryTypesToDefaults();
                SetCategoriesToDefaults();
            }

        }

        /// <summary>
        /// Looks for the <see cref="Category"/>  with the id <paramref name="i"/> in the categories table.
        /// </summary>
        /// <param name="i">Id of the object to look for.</param>
        /// <returns>The object with an Id corresponding to the passed Id.</returns>
        /// <exception cref="Exception">
        /// Thrown when a Category with the passed Id isn't found.
        /// </exception>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// Category search = categories.GetCategoryFromId(2);
        /// Console.WriteLine(search);
        /// </code>
        /// </example>
        public Category GetCategoryFromId(int i)
        {
            string findCategory = "select Id, Description, TypeId from categories where Id= @id;";
            SQLiteCommand com = new SQLiteCommand(findCategory, _connection);
            com.Parameters.AddWithValue("@id", i);
            SQLiteDataReader rdr = com.ExecuteReader();
            com.Dispose();
            Category foundCategory = null;

            while (rdr.Read())
            {
                Category.CategoryType foundType = (Category.CategoryType) (rdr.GetInt32(2) - 1); // Zero base
                foundCategory = new Category(rdr.GetInt32(0), rdr.GetString(1), foundType);
            }
            if (foundCategory == null)
            {
                throw new Exception("Cannot find category with id " + i);
            }
            return foundCategory;

        }
        /// <summary>
        /// Adds default values to the categoryTypes table.
        /// </summary>
        /// <remarks>
        /// Values that were previously in the categoryTypes table are deleted.
        /// </remarks>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// categories.SetCategoryTypesToDefaults();
        /// </code>
        /// </example>
        public void SetCategoryTypesToDefaults()
        {
            string deleteStatement = "delete from categoryTypes";
            SQLiteCommand deleteCommand = new SQLiteCommand(deleteStatement, _connection);
            deleteCommand.ExecuteNonQuery();
            deleteCommand.Dispose();

            Category.CategoryType[] enums = (Category.CategoryType[]) Enum.GetValues(typeof(Category.CategoryType));
            for (int i = 0; i < enums.Length; i++)
            {
                string sqlCommand = "insert into categoryTypes(Id, Description) values (@Id, @Description)";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
                command.Parameters.AddWithValue("@Id", i + 1);
                command.Parameters.AddWithValue("@Description", Enum.GetName(typeof(Category.CategoryType), i));
                command.ExecuteNonQuery();
                command.Dispose();
            }

        }

        private int CategoryTypeToId(Category.CategoryType category)
        {
            Category.CategoryType[] enums = (Category.CategoryType[])Enum.GetValues(typeof(Category.CategoryType));
            for (int i = 0; i < enums.Length; i++)
            {
                if (enums[i] == category)
                    return i + 1;
            }
            return -1;
        }

        /// <summary>
        /// Updates the category from the categories table with the passed id with the passed values.
        /// </summary>
        /// <remarks>
        /// If a category with the passed Id isn't found, an error will logged to the console.
        /// </remarks>
        /// <param name="id">Id of the category to be updated.</param>
        /// <param name="newDescr">New Description of the category.</param>
        /// <param name="categoryType">New CategoryType of the category.</param>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// categories.Add("Food", Category.CategoryType.Expense);
        /// categories.UpdateProperties(1,"Drink", Category.CategoryType.Expense);
        /// </code>
        /// </example>
        public void UpdateProperties(int id, string newDescr, Category.CategoryType categoryType)
        {
            try
            {
                GetCategoryFromId(id);
                string sqlCommand = "update categories set Description = @newDescr, TypeId = @categoryType  where Id = @id";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
                command.Parameters.AddWithValue("@newDescr", newDescr);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@categoryType", CategoryTypeToId(categoryType));
                command.ExecuteNonQuery();
                command.Dispose();

            }
            catch(Exception e)
            {
                Console.WriteLine(e); //This error would be sent to a log for an admin
            }
        }


        /// <summary>
        /// Adds default values to the categories table.
        /// </summary>
        /// <remarks>
        /// Values that were previously in the categories table are deleted.
        /// </remarks>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// categories.Add("Food", Category.CategoryType.Expense);
        /// categories.SetCategoriesToDefaults();
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            string sqlCommand = "delete from categories";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
            command.ExecuteNonQuery();
            command.Dispose();

            Add("Utilities", Category.CategoryType.Expense);
            Add("Rent", Category.CategoryType.Expense);
            Add("Food", Category.CategoryType.Expense);
            Add("Entertainment", Category.CategoryType.Expense);
            Add("Education", Category.CategoryType.Expense);
            Add("Miscellaneous", Category.CategoryType.Expense);
            Add("Medical Expenses", Category.CategoryType.Expense);
            Add("Vacation", Category.CategoryType.Expense);
            Add("Credit Card", Category.CategoryType.Credit);
            Add("Clothes", Category.CategoryType.Expense);
            Add("Gifts", Category.CategoryType.Expense);
            Add("Insurance", Category.CategoryType.Expense);
            Add("Transportation", Category.CategoryType.Expense);
            Add("Eating Out", Category.CategoryType.Expense);
            Add("Savings", Category.CategoryType.Savings);
            Add("Income", Category.CategoryType.Income);

        }

        /// <summary>
        /// Adds a category to the categories table.
        /// </summary>
        /// <remarks>Id of added categories are auto incremented</remarks>
        /// <param name="desc">Description of the new category.</param>
        /// <param name="type">Type of the new category.</param>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// categories.Add("Food", Category.CategoryType.Expense);
        /// </code>
        /// </example>
        public void Add(String desc, Category.CategoryType type)
        {
            string sqlCommand = "insert into categories (Description, TypeId) values (@desc , @type)";

            SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
            command.Parameters.AddWithValue("@desc", desc);
            command.Parameters.AddWithValue("@type", CategoryTypeToId(type));
            command.ExecuteNonQuery();
            command.Dispose();
        }

        // ====================================================================
        // Delete category
        // ====================================================================
        /// <summary>
        /// Removes the category with the passed id from the categories table.
        /// </summary>
        /// <remarks>
        /// If a category with the passed Id isn't found, an error will logged to the console.
        /// </remarks>
        /// <param name="Id">Id of the object to delete.</param>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// categories.Add("Food", Category.CategoryType.Expense);
        /// categories.Delete(1);
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            try
            {
                GetCategoryFromId(Id);
                string sqlCommand = "delete from categories where Id = @id";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
                command.Parameters.AddWithValue("@id", Id);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch(Exception e)
            {
                Console.WriteLine(e); //This error would be sent to a log for an admin
            }

        }
        /// <summary>
        /// Returns a list of all categories from the categories table.
        /// </summary>
        /// <returns>List of Categories</returns>
        /// <example>
        /// <code>
        /// Categories categories = new Categories(Database.dbConnection,false);
        /// foreach(Category category in categories.List())
        ///     Console.WriteLine(category);
        /// </code>
        /// </example>
        public List<Category> List()
        {
            string sqlQuery = "SELECT Id, Description, TypeId FROM categories";
            SQLiteCommand command = new SQLiteCommand(sqlQuery, _connection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Category> newList = new List<Category>();

            while (reader.Read())
            {
                Category category = new Category(reader.GetInt32(0), reader.GetString(1), (Category.CategoryType) reader.GetInt32(2) - 1); // Zero-Base
                newList.Add(category);

            }
            command.Dispose();
            return newList;
        }
    }
}

