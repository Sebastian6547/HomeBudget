using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Budget
{
    /// <summary>
    /// HomeBudget class is used to combine <see cref="Categories"/> class and <see cref="Expenses"/> class in a database.
    /// </summary>
    /// <example>
    /// <code>
    /// HomeBudget homebudget=new HomeBudget("C:\\Users\\exampleDb.db",true);
    /// </code>
    /// </example>
    public class HomeBudget
    {
        private Categories _categories;
        private Expenses _expenses;

        // Properties (categories and expenses object)
        /// <summary>
        /// Gets the Categories of the HomeBudget.
        /// </summary>
        /// <value>Categories in the current HomeBudget</value>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Gets the Expenses of the HomeBudget.
        /// </summary>
        /// <value>Expenses in the current HomeBudget</value>
        public Expenses expenses { get { return _expenses; } }

        // -------------------------------------------------------------------
        // Constructor (existing budget ... must specify file)
        // -------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of HomeBudget class.
        /// </summary>
        /// <param name="databaseFile">Name of the database file.</param>
        /// <param name="newDB">Boolean that determines if the database is new.</param>
        /// <remarks>
        /// If newDb is true and the database file already exists, tables from the database will be replaced with new ones.
        /// </remarks>
        /// <example>
        /// <code>
        /// HomeBudget homebudget=new HomeBudget("C:\\Users\\exampleDb.db",true);
        /// </code>
        /// </example>
        public HomeBudget(String databaseFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }
            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }
            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);
            // create the _expenses course
            _expenses = new Expenses(Database.dbConnection);
        }


        #region GetList

        /// <summary>
        /// Gets a list of BudgetItems within a time frame.
        /// </summary>
        /// <param name="Start">Start of the time frame.</param>
        /// <param name="End">End of the time frame</param>
        /// <param name="FilterFlag">If true, limits the ones that have the same CategoryId as the passed <paramref name="CategoryID"/>, otherwise all the results are returned.</param>
        /// <param name="CategoryID">Id of the wanted category.</param>
        /// <returns>List of BudgetItems within the time frame.</returns>
        /// <example>
        /// <code>
        /// HomeBudget homebudget=new Homebudget("C:\\Users\\exampleDb.db",true);
        /// homeBudget.categories.Add("Food", Category.CategoryType.Expense);
        /// List&lt;BudgetItem&gt; list = homeBudget.GetBudgetItems(null, null, false, 0);
        /// foreach(BudgetItem budgetItem in list)
        /// {
        ///     Console.WriteLine(String.Format("{0} {1} {2:C} {3:C}",budgetItem.Date.ToString("yyyy-MMM-dd"),budgetItem.ShortDescription,budgetItem.Amount, budgetItem.Balance));  
        /// }        /// </code>
        /// </example>
        public List<BudgetItem> GetBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);


            string query = "SELECT e.CategoryId, e.Id, e.Description, e.Date, e.Amount, c.Description from categories as c inner join expenses as e on e.CategoryId = c.Id where e.Date >= @realStart AND e.Date <= @realEnd";

            // Add filter
            if (FilterFlag)
                query += (" AND e.CategoryId = @CategoryID order by e.Date");
            else
                query += " order by e.Date";

            SQLiteCommand command = new SQLiteCommand(query, Database.dbConnection);
            command.Parameters.AddWithValue("@realStart", realStart.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@realEnd", realEnd.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CategoryID", CategoryID);
            SQLiteDataReader reader = command.ExecuteReader();

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItem> items = new List<BudgetItem>();
            Double total = 0;
            while (reader.Read())
            {
                total += reader.GetDouble(4); // Get Amount
                items.Add(new BudgetItem
                {
                    CategoryID = reader.GetInt32(0),
                    ExpenseID = reader.GetInt32(1),
                    ShortDescription = reader.GetString(2),
                    Date = reader.GetDateTime(3),
                    Amount = reader.GetDouble(4),
                    Category = reader.GetString(5),
                    Balance = total
                });
            }
            command.Dispose();
            return items;
        }

        // ============================================================================
        // Group all expenses month by month (sorted by year/month)
        // returns a list of BudgetItemsByMonth which is 
        // "year/month", list of budget items, and total for that month
        // ============================================================================
        /// <summary>
        /// Groups each BudgetItem by month into seperate lists and returns a list of the groups within a time frame.
        /// </summary>
        /// <param name="Start">Start of the time frame.</param>
        /// <param name="End">End of the time frame</param>
        /// <param name="FilterFlag">If true, limits the ones that have the same CategoryId as <paramref name="CategoryID"/>, otherwise all the results are returned.</param>
        /// <param name="CategoryID">Id of the wanted category.</param>
        /// <returns><see cref="BudgetItemsByMonth"/> list within the time frame</returns>
        /// <example>
        /// <code>
        /// HomeBudget homebudget=new Homebudget("C:\\Users\\exampleDb.db",true);
        /// homeBudget.categories.Add("Salary", Category.CategoryType.Income);
        /// List&lt;BudgetItemsByMonth&gt; list = homeBudget.GeBudgetItemsByMonth(null, null, false, 0);
        /// foreach(BudgetItemsByMonth budgetItems in list)
        /// {
        ///     foreach(BudgetItem budgetItem in budgetItems.Details)
        ///     {
        ///         Console.WriteLine(String.Format("{0} {1} {2:C} {3:C}",budgetItem.Date.ToString("yyyy/MMM/dd"),budgetItem.ShortDescription,budgetItem.Amount, budgetItem.Balance));  
        ///     }
        /// }
        /// </code>
        /// </example>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);

            // -----------------------------------------------------------------------
            // Get all date subgroups
            // -----------------------------------------------------------------------
            string dateQuery = "select DISTINCT substr(Date, 1, 7) from expenses where Date between @realStart and @realEnd";
            if (FilterFlag)
                dateQuery += " and CategoryId = @CategoryId;";

            SQLiteCommand command = new SQLiteCommand(dateQuery, Database.dbConnection);
            command.Parameters.AddWithValue("@realStart", realStart);
            command.Parameters.AddWithValue("@realEnd", realEnd);
            command.Parameters.AddWithValue("@CategoryId", CategoryID);
            SQLiteDataReader reader = command.ExecuteReader();
            command.Dispose();

            List<string> keys = new List<string>();
            while (reader.Read())
            {
                keys.Add(reader.GetString(0));
            }
            reader.Close();


            var summary = new List<BudgetItemsByMonth>();

            // Loop through subgroups
            foreach (string key in keys)
            {
                // Replace hyphen with backslash
                string changedKey = key.Replace('-', '/');


                // Find Start and End Date
                int year = int.Parse(changedKey.Split('/')[0]);
                int month = int.Parse(changedKey.Split("/")[1]);
                var startDate = new DateTime(year, month, 1);
                var lastDay = DateTime.DaysInMonth(year, month);
                var endDate = new DateTime(year, month, lastDay);

                // Total for the subgroup

                string totalQuery = "select sum(Amount) from expenses where Date between @startDate and @endDate";
                if (FilterFlag)
                    totalQuery += " and CategoryId = @CategoryId group by substr(Date, 1, 7);";
                else
                    totalQuery += " group by substr(Date, 1, 7);";
                command = new SQLiteCommand(totalQuery, Database.dbConnection);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);
                command.Parameters.AddWithValue("@CategoryId", CategoryID);
                reader = command.ExecuteReader();


                double total = 0;
                while (reader.Read())
                    total += reader.GetDouble(0);

                // Create and add BudgetItemsByMonth
                summary.Add(new BudgetItemsByMonth
                {
                    Month = changedKey,
                    Details = GetBudgetItems(startDate, endDate, FilterFlag, CategoryID),
                    Total = total
                }); ;

            }
            return summary;
        }

        // ============================================================================
        // Group all expenses by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Groups each BudgetItem by CategoryId into seperate lists and returns a list of the groups within a time frame
        /// </summary>
        /// <param name="Start">Start of the time frame.</param>
        /// <param name="End">End of the time frame</param>
        /// <param name="FilterFlag">If true, limits the ones that have the same CategoryId as <paramref name="CategoryID"/>, otherwise all the results are returned.</param>
        /// <param name="CategoryID">Id of the wanted category.</param>
        /// <returns><see cref="BudgetItemsByCategory"/> list within the time frame</returns>
        /// <example>
        /// <code>
        /// HomeBudget homeBudget = new HomeBudget("C:\\Users\\exampleDb.db",true);
        /// homeBudget.categories.Add("Salary", Category.CategoryType.Income);
        /// List&lt;BudgetItemsByCategory&gt; list = homeBudget.GeBudgetItemsByCategory(null, null, false, 0);
        /// foreach(BudgetItemsByCategory budgetItems in list)
        /// {
        ///     foreach(BudgetItem budgetItem in budgetItems.Details)
        ///     {
        ///         Console.WriteLine(String.Format("{0} {1} {2:C} {3:C}",budgetItem.Date.ToString("yyyy/MMM/dd"),budgetItem.ShortDescription,budgetItem.Amount, budgetItem.Balance));  
        ///     }
        /// }
        /// </code>
        /// </example>
        public List<BudgetItemsByCategory> GetBudgetItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);

            // -----------------------------------------------------------------------
            // Get all date subgroups
            // -----------------------------------------------------------------------
            string dateQuery = "select DISTINCT CategoryId from expenses where Date between @realStart and @realEnd";
            if (FilterFlag)
                dateQuery += " and CategoryId = @CategoryId;";

            SQLiteCommand command = new SQLiteCommand(dateQuery, Database.dbConnection);
            command.Parameters.AddWithValue("@realStart", realStart);
            command.Parameters.AddWithValue("@realEnd", realEnd);
            command.Parameters.AddWithValue("@CategoryId", CategoryID);
            SQLiteDataReader reader = command.ExecuteReader();
            command.Dispose();

            // List of subgroups of each category in expenses
            List<int> categoryIDs = new List<int>();
            while (reader.Read())
            {
                categoryIDs.Add(reader.GetInt32(0));
            }
            reader.Close();


            var summary = new List<BudgetItemsByCategory>();

            // Loop through subgroups
            foreach (int categoryId in categoryIDs)
            {

                // Total for the subgroup
                
                string totalQuery = "select sum(Amount) from expenses where Date between @realStart and @realEnd and CategoryId = @category";
                command = new SQLiteCommand(totalQuery, Database.dbConnection);
                command.Parameters.AddWithValue("@realStart", realStart);
                command.Parameters.AddWithValue("@realEnd", realEnd);
                command.Parameters.AddWithValue("@category", categoryId);
                reader = command.ExecuteReader();


                double total = 0;
                while (reader.Read())
                    total += reader.GetDouble(0);

                // Get category description
                string categoryDesc = "select Description from categories where id = @id";
                command = new SQLiteCommand(categoryDesc, Database.dbConnection);
                command.Parameters.AddWithValue("@id", categoryId);
                reader = command.ExecuteReader();
                string categoryString = "";

                while (reader.Read())
                    categoryString = reader.GetString(0);

                // Create and add BudgetItemsByCategory
                summary.Add(new BudgetItemsByCategory
                {
                    Category = categoryString,
                    Details = GetBudgetItems(realStart, realEnd, true, categoryId), // Technically since its by category, we are always filtering by category
                    Total = total
                }); ;

            }
            return summary;
        }



        // ============================================================================
        // Group all expenses by category and Month
        // creates a list of ExpandoObjects... which are objects that can have
        //   properties added to it on the fly.
        // ... for each element of the list (expenses by month), the ExpandoObject will have a property
        //     Month = (year/month) (string)
        //     Total = Double total for that month
        //     and for each category that had an entry in that month...
        //     1) Name of category , 
        //     2) and a property called "details: <name of category>" 
        //  
        // ... the last element of the list will contain an ExpandoObject
        //     with the properties for each category, equal to the totals for that
        //     category, and the name of the "Month" property will be "Totals"
        // ============================================================================

        /// <summary>
        /// Groups each BudgetItem by CategoryId and month into seperate lists and returns a list of the groups within a time frame.
        /// </summary>
        /// <param name="Start">Start of the time frame.</param>
        /// <param name="End">End of the time frame</param>
        /// <param name="FilterFlag">If true, limits the ones that have the same CategoryId as <paramref name="CategoryID"/>, otherwise all the results are returned.</param>
        /// <param name="CategoryID">Id of the wanted category.</param>
        /// <returns>Dictionary&lt;string,object&gt; list within the time frame</returns>
        /// <example>
        /// <code>
        /// HomeBudget homeBudget = new HomeBudget("C:\\Users\\exampleDb.db",true);
        /// homeBudget.categories.Add("Salary", Category.CategoryType.Income);
        /// List&lt;Dictionary&lt;string,object&gt;&gt; list=homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0);
        /// </code>
        /// </example>
        public List<Dictionary<string,object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["Total"] = MonthGroup.Total;

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total = total + item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] =  details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out Double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
