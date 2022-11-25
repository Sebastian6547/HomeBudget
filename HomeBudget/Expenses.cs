using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Globalization;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: expenses
    //        - A collection of expense items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// Expenses class is used to read and write from a <see cref="Expense"/> table in a database.
    /// </summary>
    public class Expenses
    {
        private SQLiteConnection _connection;

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Initializes an instance of Expenses class. 
        /// </summary>
        /// <param name="connection">Connection to a database.</param>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(Database.dbConnection);
        /// </code>
        /// </example>
        public Expenses(SQLiteConnection connection)
        { 
            _connection = connection;
        }

        /// <summary>
        /// Looks for the <see cref="Expense"/>  with the id <paramref name="i"/> in the expenses table.
        /// </summary>
        /// <param name="i">Id of the object to look for.</param>
        /// <returns>The object with an Id corresponding to the passed Id.</returns>
        /// <exception cref="Exception">
        /// Thrown when an expense with the passed Id isn't found.
        /// </exception>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(Database.dbConnection);
        /// Expense search = expenses.GetCategoryFromId(2);
        /// Console.WriteLine(search);
        /// </code>
        /// </example>
        public Expense GetExpensesFromId(int i)
        {
            string findExpense = "select Id, Date, Description, Amount, CategoryId from expenses where Id= @id;";
            SQLiteCommand com = new SQLiteCommand(findExpense, _connection);
            com.Parameters.AddWithValue("@id", i);
            SQLiteDataReader rdr = com.ExecuteReader();
            com.Dispose();
            Expense foundExpense = null;

            while (rdr.Read())
            {
                DateTime date = DateTime.ParseExact(rdr.GetString(1), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                foundExpense = new Expense(rdr.GetInt32(0), date, rdr.GetInt32(4), rdr.GetDouble(3), rdr.GetString(2));
            }
            if (foundExpense == null)
            {
                throw new Exception("Cannot find expense with id " + i);
            }
            return foundExpense;

        }

        // ====================================================================
        // Add expense
        // ====================================================================

        /// <summary>
        /// Adds an expense to the expenses table.
        /// </summary>
        /// <remarks>Id of added expenses are auto incremented.</remarks>
        /// <param name="date">Represents the day of creation of the new expense.</param>
        /// <param name="category">The Id of the <see cref="Category"/>.</param>
        /// <param name="amount">Cost of the expense.</param>
        /// <param name="description">Description of the new expense.</param>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(Database.dbConnection);
        /// expenses.Add(DateTime.Now, 1, 20, "Eating Out");
        /// </code>
        /// </example>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            string sqlCommand = "insert into expenses (Date, Description, Amount, CategoryId) values (@date , @description, @amount, @category)";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
            command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@category", category);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        // ====================================================================
        // Delete expense
        // ====================================================================
        /// <summary>
        /// Removes the expense with the passed id from the expenses table.
        /// </summary>
        /// <remarks>
        /// If a category with the passed Id isn't found, an error will logged to the console.
        /// </remarks>
        /// <param name="Id">Id of the object to delete.</param>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(Database.dbConnection);
        /// expenses.Add(DateTime.Now, 1, 20, "Eating Out");
        /// expenses.Delete(1);
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            try
            {
                GetExpensesFromId(Id);
                string sqlCommand = "delete from expenses where Id = @id";
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
        /// Updates the expense from the expenses table with the passed id with the passed values.
        /// </summary>
        /// <remarks>
        /// If a category with the passed Id isn't found, an error will logged to the console.
        /// </remarks>
        /// <param name="id">Id of the expense to be updated.</param>
        /// <param name="newDate">New date of the expense.</param>
        /// <param name="newDescr">New Desciption of the expense.</param>
        /// <param name="amount">New amount of the expense.</param>
        /// <param name="newCategoryId">New Id of the category corresponding to the expense.</param>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(Database.dbConnection);
        /// expenses.Add(DateTime.Now, 1, 20, "Eating Out");
        /// expenses.UpdateProperties(1,DateTime.Now, 2, 10, "Buying a Shirt");
        /// </code>
        /// </example>
        public void UpdateProperties(int id, DateTime newDate, string newDescr, Double amount ,int newCategoryId)
        {
            try
            {
                GetExpensesFromId(id);
                string sqlCommand = "update expenses set Date= @newDate, Description = @newDecr, Amount= @amount, CategoryId = @newCategoryId where Id = @id";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
                command.Parameters.AddWithValue("@newDate", newDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@newDecr", newDescr);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@newCategoryId", newCategoryId);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch(Exception e)
            {
                Console.WriteLine(e); //This error would be sent to a log for an admin
            }
        }

        // ====================================================================
        // Return list of expenses
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Returns a list of all expenses from the expenses table.
        /// </summary>
        /// <returns>List of Expenses</returns>
        /// <example>
        /// <code>
        /// Expenses expenses = new Expenses(Database.dbConnection);
        /// foreach(Expense expense in expenses.List())
        ///     Console.WriteLine(expense);
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            string sqlQuery = "SELECT Id, Date, Amount, Description, CategoryId FROM expenses;";
            SQLiteCommand command = new SQLiteCommand(sqlQuery, _connection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Expense> newList = new List<Expense>();

            while (reader.Read())
            {
                string stringDate = reader.GetString(1);
                DateTime date = DateTime.ParseExact(stringDate, "yyyy-MM-dd",CultureInfo.InvariantCulture);
                Expense expense = new Expense(reader.GetInt32(0),date, reader.GetInt32(4), reader.GetDouble(2), reader.GetString(3));
                newList.Add(expense);
            }
            command.Dispose();
            return newList;
        }

    }
}

