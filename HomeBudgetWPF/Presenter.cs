using System;
using System.Collections.Generic;
using System.Text;
using Budget;
using System.Data.SQLite;
using System.Linq;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Presenter class is used to interact with model and call classes from the Views accordingly.
    /// </summary>
    public class Presenter
    {
        private readonly LaunchWindowInterface launchWindowView = null;
        private readonly ProjectDetailsWindowInterface projectDetailsWindowView = null;
        private CreateCategoryInterface createCategoryView = null;
        private CreateExpenseInterface createExpenseView = null;
        private UpdateExpenseInterface updateExpenseView = null;
        private MainWindowInterface mainWindowView = null;
        private DataViewInterface dataView = null;
        private HomeBudget model = null;
        private string currentFile;

        /// <summary>
        /// Initializes an instance of the presenter class
        /// </summary>
        /// <param name="launchWindowInterface">Instance of a class that implements the <see cref="LaunchWindowInterface"/>.</param>
        public Presenter(LaunchWindowInterface launchWindowInterface)
        {
            launchWindowView = launchWindowInterface;
        }
        /// <summary>
        /// Initializes an instance of the presenter class
        /// </summary>
        /// <param name="projectDetailsWindowInterface">Instance of a class that implements the <see cref="ProjectDetailsWindowInterface"/>.</param>
        public Presenter(ProjectDetailsWindowInterface projectDetailsWindowInterface)
        {
            projectDetailsWindowView = projectDetailsWindowInterface;
        }

        /// <summary>
        /// Sets the instance of the CreateCategoryView
        /// </summary>
        public CreateCategoryInterface CreateCategoryView
        {
            set { createCategoryView = value; }
        }
        /// <summary>
        /// Sets the instance of the CreateExpenseView
        /// </summary>
        public CreateExpenseInterface CreateExpenseView
        {
            set { createExpenseView = value; }
        }
        /// <summary>
        /// Sets the instance of the MainWindowView
        /// </summary>
        public MainWindowInterface MainWindowView
        {
            set { mainWindowView = value; }
        }

        /// <summary>
        /// Sets the instance of the DataView
        /// </summary>
        public DataViewInterface DataView
        {
            set { dataView = value; }
        }

        public UpdateExpenseInterface UpdateExpenseView
        {
            set { updateExpenseView = value; }
        }
        /// <summary>
        /// Returns the current file being worked on
        /// </summary>
        /// <returns>Path of file</returns>
        public string GetCurrentFile()
        {
            return currentFile;
        }
        /// <summary>
        /// Creates new HomeBudget given filename
        /// </summary>
        /// <param name="filename">The given filename</param>
        public void ProcessNewFile(string filename)
        {
            currentFile = filename;
            model = new HomeBudget(filename, true);
            projectDetailsWindowView.OpenMainCloseLaunch();
        }
        /// <summary>
        /// Opens existing HomeBudget given filename
        /// </summary>
        /// <param name="filename">The given filename</param>
        public void ProcessExistingFile(string filename)
        {
            currentFile = filename;
            model = new HomeBudget(filename, false);
            try
            {
                model.categories.List();
                launchWindowView.OpenMainCloseLaunch();
            }
            catch (Exception err)
            {
                launchWindowView.ShowError(err.Message);
            }

        }

        //----------------------------------
        //Create Category
        //----------------------------------
        /// <summary>
        /// Adds a category to the database with the passed information if it is valid.
        /// </summary>
        /// <param name="description">Short description about the category.</param>
        /// <param name="categoryType">The type of the category.</param>
        public void CreateCategory(string description, Category.CategoryType? categoryType)
        {
            string emptyError = "Fill all fields before submitting";
            if (description == "" || categoryType == null)
            {
                createCategoryView.ShowError(emptyError);
            }
            else
            {
                model.categories.Add(description, (Category.CategoryType)categoryType);
                createCategoryView.ShowSuccess();
            }
        }

        //----------------------------------
        //Create Expense
        //----------------------------------
        /// <summary>
        /// Adds an expense to the database with the passed information if it is valid
        /// </summary>
        /// <param name="date">Represents the day of creation of the item.</param>
        /// <param name="amount">Cost of the Expense</param>
        /// <param name="description">Short description of the Expense</param>
        /// <param name="category">The Id of the <see cref="Category"/>.</param>
        public void CreateExpense(DateTime? date, string amount, string description, int? category)
        {

            double amountDouble = 0;
            string emptyError = "Fill all fields before submitting";
            string invalidAmount = "Amount must be a positive number";

            bool result = double.TryParse(amount, out amountDouble);

            if (description == "" || amount == "" || category == null || date == null||category==-1)
            {
                createExpenseView.ShowError(emptyError);
            }
            else if (!result || amountDouble < 0)
            {
                createExpenseView.ShowError(invalidAmount);
            }
            else
            {
                model.expenses.Add((DateTime)date, (int)category + 1, amountDouble, description);
                createExpenseView.ShowSuccess();
            }
            mainWindowView.ContextChanged();
        }
        /// <summary>
        /// Binds the category list combo box in a create expense view with the list of categories
        /// </summary>
        /// <param name="_">CreateExpenseInterface indicating which view to use.</param>
        public void Populate(CreateExpenseInterface _)
        {
            List<Category> catList = model.categories.List();
            createExpenseView.BindCategories(catList);
        }

        //----------------------------------
        //Update Expense
        //----------------------------------
        /// <summary>
        /// Updates the fields of expense with the passed Id with the passed information if it is valid
        /// </summary>
        /// <param name="id">id of the expense to update</param>
        /// <param name="date">Represents the new day of creation of the item.</param>
        /// <param name="amount">New cost of the Expense</param>
        /// <param name="description">New short description of the Expense</param>
        /// <param name="category">The new Id of the <see cref="Category"/>.</param>
        public void UpdateExpense(int id, DateTime? date, string amount, string description, int? category)
        {
            double amountDouble = 0;
            string emptyError = "Fill all fields before submitting";
            string invalidAmount = "Amount must be a positive number";

            bool result = double.TryParse(amount, out amountDouble);

            if (description == "" || amount == "" || category == null || date == null)
            {
                updateExpenseView.ShowError(emptyError);
            }
            else if (!result || amountDouble < 0)
            {
                updateExpenseView.ShowError(invalidAmount);
            }
            else
            {
                model.expenses.UpdateProperties(id, (DateTime)date, description, amountDouble, (int)category + 1);
                updateExpenseView.Success();
            }
            mainWindowView.ContextChanged();
        }
        /// <summary>
        /// Gets the values of the fields of the expense with the passed id and calls the view's FillFields method with them.
        /// </summary>
        /// <param name="id">Id of the expense to get the fields of</param>
        public void SetFields(int id)
        {
            Expense expense = model.expenses.GetExpensesFromId(id);
            updateExpenseView.FillFields(expense.Date, expense.Description, expense.Amount, expense.Category - 1);
        }
        /// <summary>
        /// Bind category list with view with model category list
        /// </summary>
        /// <param name="_">UpdateExpenseInterface indicating which view to use.</param>
        public void Populate(UpdateExpenseInterface _)
        {
            List<Category> catList = model.categories.List();
            updateExpenseView.BindCategories(catList);
        }
        /// <summary>
        /// Deletes the expense with the passed Id
        /// </summary>
        /// <param name="id">Id of the expense to delete</param>
        public void DeleteExpense(int id)
        {
            model.expenses.Delete(id);
            mainWindowView.ContextChanged();
        }
        //-------------------------------------------------------------
        //Main Window
        //-------------------------------------------------------------
        /// <summary>
        /// Gets current Budget items, which is then filtered by the received parameters.
        /// </summary>
        /// <param name="start">Start date filter parameter</param>
        /// <param name="end">End date filter parameter</param>
        /// <param name="filterFlag">Boolean representing wheter to filter by CategoryID or not</param>
        /// <param name="categoryID">The CategoryId to filter by</param>
        public void PopulateDataView(DateTime? start, DateTime? end, bool filterFlag, int categoryID, bool isByCategory, bool isByMonth)
        {
            if (!isByCategory && !isByMonth)
            {
                List<BudgetItem> budgetItems = model.GetBudgetItems(start, end, filterFlag, categoryID);
                dataView.DataSource = budgetItems.Cast<object>().ToList();
                dataView.ShowData();
            }
            else if (isByCategory && !isByMonth)
            {
                List<BudgetItemsByCategory> budgetItemsByCategory = model.GetBudgetItemsByCategory(start, end, filterFlag, categoryID);
                dataView.DataSource = budgetItemsByCategory.Cast<object>().ToList();
                dataView.ShowDataByCategory();
            }
            else if (!isByCategory && isByMonth)
            {
                List<BudgetItemsByMonth> budgetItemsByMonth = model.GetBudgetItemsByMonth(start, end, filterFlag, categoryID);
                dataView.DataSource = budgetItemsByMonth.Cast<object>().ToList();
                dataView.ShowDataByMonth();
            }
            else
            {
                List<Dictionary<string, object>> budgetItemsDictionary = model.GetBudgetDictionaryByCategoryAndMonth(start, end, filterFlag, categoryID);
                dataView.DataSource = budgetItemsDictionary.Cast<object>().ToList();
                dataView.ShowDataByMonthByCategory();
            }

        }

        /// <summary>
        /// Bind category list with view with model category list
        /// </summary>
        /// <param name="_">MainWindowInterface indicating which view to use.</param>
        public void PopulateCategories(MainWindowInterface mainWindowInterface)
        {
            List<Category> catList = model.categories.List();
            mainWindowView.BindCategories(catList);
        }

        /// <summary>
        /// Gets the list of all categories in the model and returns it
        /// </summary>
        /// <returns>Returns a list of categories</returns>
        public List<string> GetCategoriesAsString()
        {
            List<Category> categories = model.categories.List();
            List<string> categoriesAsString = new List<string>();
            foreach (Category category in categories)
            {
                categoriesAsString.Add(category.Description);
            }
            return categoriesAsString;
        }

        /// <summary>
        /// Updates the selected item in the datagrid if the searchBoxResult is in the description or amount of any of the budgetItems in the list, calls ShowError if not
        /// </summary>
        /// <param name="startIndex">Index to start at when looping through the list</param>
        /// <param name="budgetItems">List of budgetItems to check</param>
        /// <param name="searchBoxResult">String to search for</param>
        public void UpdateSearchResult(int startIndex,List<BudgetItem> budgetItems, string searchBoxResult)
        {
            if (startIndex == -1)
                startIndex = 0;
            int index = startIndex + 1;
            if (startIndex == budgetItems.Count - 1)
                index = 0;
            if (budgetItems[startIndex].ShortDescription.Contains(searchBoxResult) || budgetItems[startIndex].Amount.ToString().Contains(searchBoxResult))
            {
                dataView.SelectDataGridItem(startIndex);
                return;
            }
            while (!(index == startIndex))
            {
                if (budgetItems[index].ShortDescription.Contains(searchBoxResult) || budgetItems[index].Amount.ToString().Contains(searchBoxResult))
                {
                    dataView.SelectDataGridItem(index);
                    return;
                }
                index++;
                if (index >= budgetItems.Count)
                    index = 0;
            }
            dataView.ShowError("No result found.");
        }
    }
}