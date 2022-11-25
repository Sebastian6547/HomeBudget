using System;
using Xunit;
using HomeBudgetWPF;
using System.IO;
using Budget;
using System.Collections.Generic;

namespace TestHomeBudgetWPF
{
    //Test views
    [Collection("Sequential")]
    public class TestLaunchWindowView : LaunchWindowInterface
    {
        public bool openMainCalled = false;
        public bool getExistingFileCalled = false;
        public bool showErrorCalled = false;

        public string GetExistingFile()
        {
            getExistingFileCalled = true;
            return "test";
        }

        public void OpenMainCloseLaunch()
        {
            openMainCalled = true;
        }

        public void ShowError(string message)
        {
            showErrorCalled = true;
        }
    }
    public class TestProjectDetailsView : ProjectDetailsWindowInterface
    {
        public bool openMainCalled = false;
        public void OpenMainCloseLaunch()
        {
            openMainCalled = true;
        }
    }
    public class TestCreateCategoryView : CreateCategoryInterface
    {
        public bool clearFormCalled = false;
        public bool showErrorCalled = false;
        public void ShowSuccess()
        {
            clearFormCalled = true;
        }

        public void ShowError(string msg)
        {
            showErrorCalled = true;
        }
    }
    public class TestCreateExpenseWindow : CreateExpenseInterface
    {
        public bool calledBindCategories = false;
        public bool calledShowSuccess = false;
        public bool calledShowError = false;
        public string errorMessage = "";
        public void BindCategories(List<Category> categories)
        {
            calledBindCategories = true;
        }
        public void ShowSuccess()
        {
            calledShowSuccess = true;
        }
        public void ShowError(string msg)
        {
            calledShowError = true;
            errorMessage = msg;
        }
    }
    public class TestUpdateExpenseWindow : UpdateExpenseInterface
    {
        public bool calledShowError = false;
        public bool calledSuccess = false;
        public bool calledBindCategories = false;
        public bool calledFilledFields = false;
        public string errorMessage = "";

        public void BindCategories(List<Category> categories)
        {
            calledBindCategories = true;
        }

        public void FillFields(DateTime date, string description, double amount, int categoryIndex)
        {
            calledFilledFields = true;
        }

        public void ShowError(string msg)
        {
            calledShowError = true;
            errorMessage = msg;
        }

        public void Success()
        {
            calledSuccess=true;
        }
    }
    public class TestMainWindow : MainWindowInterface
    {
        public bool calledBindCategories = false;
        public bool calledShowDataGridWithBudgetItems = false;
        public bool calledShowDataGridWithBudgetItemsByMonth = false;
        public bool calledShowDataGridWithBudgetItemsByCategory = false;
        public bool calledShowDataGridWithDictionary = false;
        public bool calledContextChanged = false;
        public void BindCategories(List<Category> categories)
        {
            calledBindCategories = true;
        }

        public void ContextChanged()
        {
            calledContextChanged = true;
        }

        public void ShowDataGrid(List<BudgetItem> budgetItems)
        {
            calledShowDataGridWithBudgetItems = true;
        }

        public void ShowDataGrid(List<BudgetItemsByMonth> budgetItemsByMonth)
        {
            calledShowDataGridWithBudgetItemsByMonth = true;
        }

        public void ShowDataGrid(List<BudgetItemsByCategory> budgetItemsByCategory)
        {
            calledShowDataGridWithBudgetItemsByCategory = true;
        }

        public void ShowDataGrid(List<Dictionary<string, object>> budgetItemsByMonthByCategory)
        {
            calledShowDataGridWithDictionary = true;
        }
    }
    public class TestDataView : DataViewInterface
    {
        private List<object>_dataSource;
        public List<object> DataSource { get => _dataSource; set => _dataSource = value; }

        public bool calledShowData = false;
        public bool calledShowDataByCategory = false;
        public bool calledShowDataByMonth = false;
        public bool calledShowDataByMonthByCategory = false;
        public bool calledShowError = false;
        public bool calledSelectDataGridItem = false;

        public int index;

        public void ShowData()
        {
            calledShowData = true;
        }

        public void ShowDataByCategory()
        {
            calledShowDataByCategory = true;
        }

        public void ShowDataByMonth()
        {
            calledShowDataByMonth = true;
        }

        public void ShowDataByMonthByCategory()
        {
            calledShowDataByMonthByCategory = true;
        }

        public void ShowError(string msg)
        {
            calledShowError = true;
        }

        public void SelectDataGridItem(int index)
        {
            calledSelectDataGridItem = true;
            this.index = index;
        }
    }

    public class TestPresenter
    {
        //--------------------------------------------------------------------------------------------------------------------------
        //Project details view and launch window view
        //--------------------------------------------------------------------------------------------------------------------------
        [Fact]
        public void TestProcessExistingFileSuccess()
        {
            // Create new file to test existing file

            TestProjectDetailsView newView = new TestProjectDetailsView();
            Presenter tempPresenter = new Presenter(newView);
            string filename = "test";
         
            tempPresenter.ProcessNewFile(filename);

            TestLaunchWindowView view = new TestLaunchWindowView();
            Presenter presenter = new Presenter(view);

            presenter.ProcessExistingFile(view.GetExistingFile());

            Assert.True(view.openMainCalled);
            Assert.True(view.getExistingFileCalled);
            Assert.False(view.showErrorCalled);
        }
        [Fact]
        public void TestProcessExistingFileFail()
        {
            TestLaunchWindowView view = new TestLaunchWindowView();
            Presenter presenter = new Presenter(view);
            string filename = "test.pdf";

            if (!File.Exists(filename))
                File.Create(filename).Dispose();
                      
            presenter.ProcessExistingFile(filename);

            Assert.False(view.openMainCalled);
            Assert.False(view.getExistingFileCalled);
            Assert.True(view.showErrorCalled);
        }
        [Fact]
        public void TestProccessNewFile()
        {
            TestProjectDetailsView view = new TestProjectDetailsView();
            Presenter presenter = new Presenter(view);
            string filename = "fakeFile";

            presenter.ProcessNewFile(filename);

            Assert.True(view.openMainCalled);
        }

        [Fact]
        public void TestProccessNewFileFail()
        {
            TestProjectDetailsView view = new TestProjectDetailsView();
            Presenter presenter = new Presenter(view);

            Assert.Throws<ArgumentNullException>(() => presenter.ProcessNewFile(null));
            Assert.False(view.openMainCalled);
        }
        //--------------------------------------------------------------------------------------------------------------------------
        //Create Category View
        //--------------------------------------------------------------------------------------------------------------------------
        [Fact]
        public void TestCreateCategorySuccess()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            TestCreateCategoryView createCategoryView = new TestCreateCategoryView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            
            string filename = "test";

            if (!File.Exists(filename))
                File.Create(filename).Dispose();

            presenter.ProcessNewFile(filename);
            presenter.CreateCategoryView = createCategoryView;

            string description = "this is a test";
            presenter.CreateCategory(description, Category.CategoryType.Income);

            Assert.True(createCategoryView.clearFormCalled);
            Assert.False(createCategoryView.showErrorCalled);
        }
        [Fact]
        public void TestCreateCategoryEmptyDescription()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            TestCreateCategoryView createCategoryView = new TestCreateCategoryView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            string filename = "test";

            if (!File.Exists(filename))
                File.Create(filename).Dispose();

            presenter.ProcessNewFile(filename);
            presenter.CreateCategoryView = createCategoryView;

            string description = "";
            presenter.CreateCategory(description, Category.CategoryType.Income);

            Assert.False(createCategoryView.clearFormCalled);
            Assert.True(createCategoryView.showErrorCalled);
        }
        [Fact]
        public void TestCreateCategoryNullCategoryType()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            TestCreateCategoryView createCategoryView = new TestCreateCategoryView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            
            string filename = "test";

            if (!File.Exists(filename))
                File.Create(filename).Dispose();

            presenter.ProcessNewFile(filename);
            presenter.CreateCategoryView = createCategoryView;

            string description = "";
            presenter.CreateCategory(description, null);

            Assert.False(createCategoryView.clearFormCalled);
            Assert.True(createCategoryView.showErrorCalled);
        }
        //--------------------------------------------------------------------------------------------------------------------------
        //Create Expense View
        //--------------------------------------------------------------------------------------------------------------------------
        [Fact]
        public void TestCreateExpensePopulate()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            presenter.Populate(testCreateExpenseWindow);

            Assert.True(testCreateExpenseWindow.calledBindCategories);
            Assert.False(testCreateExpenseWindow.calledShowSuccess);
            Assert.False(testCreateExpenseWindow.calledShowError);

        }
        [Fact]
        public void TestCreateExpenseSuccess()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            DateTime date = DateTime.Now;
            string amount = "20";
            string validDescription = "validDescription";
            int category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testCreateExpenseWindow.calledBindCategories);
            Assert.True(testCreateExpenseWindow.calledShowSuccess);
            Assert.False(testCreateExpenseWindow.calledShowError);
        }
        [Fact]
        public void TestCreateExpenseNullAmount()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;
            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            DateTime date = DateTime.Now;
            string amount = "";
            string validDescription = "valid";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            string errorMessage = "Fill all fields before submitting";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testCreateExpenseWindow.calledBindCategories);
            Assert.False(testCreateExpenseWindow.calledShowSuccess);
            Assert.True(testCreateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testCreateExpenseWindow.errorMessage);
        }

        [Fact]
        public void TestCreateExpenseNullDescription()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;
            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            DateTime date = DateTime.Now;
            string amount = "20";
            string validDescription = "";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            string errorMessage = "Fill all fields before submitting";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testCreateExpenseWindow.calledBindCategories);
            Assert.False(testCreateExpenseWindow.calledShowSuccess);
            Assert.True(testCreateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testCreateExpenseWindow.errorMessage);
        }
        [Fact]
        public void TestCreateExpenseNullCategoryId()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;
            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            DateTime date = DateTime.Now;
            string amount = "20";
            string validDescription = "valid";
            int? category = null;
            presenter.CreateExpense(date, amount, validDescription, category);

            string errorMessage = "Fill all fields before submitting";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testCreateExpenseWindow.calledBindCategories);
            Assert.False(testCreateExpenseWindow.calledShowSuccess);
            Assert.True(testCreateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testCreateExpenseWindow.errorMessage);
        }
        [Fact]
        public void TestCreateExpenseNullDate()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;
            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            DateTime? date = null;
            string amount = "20";
            string validDescription = "valid";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            string errorMessage = "Fill all fields before submitting";

            Assert.False(testCreateExpenseWindow.calledBindCategories);
            Assert.False(testCreateExpenseWindow.calledShowSuccess);
            Assert.True(testCreateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testCreateExpenseWindow.errorMessage);
        }

        [Fact]
        public void TestCreateExpenseInvalidAmountInput()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;
            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            DateTime date = DateTime.Now;
            string amount = "abc";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            string errorMessage = "Amount must be a positive number";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testCreateExpenseWindow.calledBindCategories);
            Assert.False(testCreateExpenseWindow.calledShowSuccess);
            Assert.True(testCreateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testCreateExpenseWindow.errorMessage);
        }
        //--------------------------------------------------------------------------------------------------------------------------
        //Update expense view
        //--------------------------------------------------------------------------------------------------------------------------
        [Fact]
        public void TestUpdateExpensePopulate()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();
            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            presenter.Populate(testUpdateExpenseWindow);

            Assert.True(testUpdateExpenseWindow.calledBindCategories);
            Assert.False(testUpdateExpenseWindow.calledSuccess);
            Assert.False(testUpdateExpenseWindow.calledShowError);
        }
        [Fact]
        public void TestUpdateExpenseSuccess()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            int id = 1;
            //Add an expense to the db
            DateTime date = DateTime.Now;
            string amount = "200";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            //Update the expense that was added
            DateTime newDate = DateTime.Now;
            string newAmount = "100";
            string newValidDescription = "newValidDescription";
            int? newCategory = 2;
            presenter.UpdateExpense(id, newDate, newAmount, newValidDescription, newCategory);

            Assert.True(testMainWindow.calledContextChanged);
            Assert.True(testUpdateExpenseWindow.calledSuccess);
            Assert.False(testUpdateExpenseWindow.calledShowError);
        }
        [Fact]
        public void TestUpdateExpenseNullAmount()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            int id = 1;
            //Add an expense to the db
            DateTime date = DateTime.Now;
            string amount = "200";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            //Update the expense that was added
            DateTime newDate = DateTime.Now;
            string newAmount = "";
            string newValidDescription = "newValidDescription";
            int? newCategory = 2;
            presenter.UpdateExpense(id, newDate, newAmount, newValidDescription, newCategory);
            //Set the expected error message
            string errorMessage = "Fill all fields before submitting";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testUpdateExpenseWindow.calledSuccess);
            Assert.True(testUpdateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testUpdateExpenseWindow.errorMessage);
        }
        [Fact]
        public void TestUpdateExpenseNullDescrption()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            int id = 1;
            //Add an expense to the db
            DateTime date = DateTime.Now;
            string amount = "200";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            //Update the expense that was added
            DateTime newDate = DateTime.Now;
            string newAmount = "100";
            string newValidDescription = "";
            int? newCategory = 2;
            presenter.UpdateExpense(id, newDate, newAmount, newValidDescription, newCategory);
            //Set the expected error message
            string errorMessage = "Fill all fields before submitting";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testUpdateExpenseWindow.calledSuccess);
            Assert.True(testUpdateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testUpdateExpenseWindow.errorMessage);
        }
        [Fact]
        public void TestUpdateExpenseNullCategoryId()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            int id = 1;
            //Add an expense to the db
            DateTime date = DateTime.Now;
            string amount = "200";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            //Update the expense that was added
            DateTime newDate = DateTime.Now;
            string newAmount = "100";
            string newValidDescription = "newValidDescription";
            int? newCategory = null;
            presenter.UpdateExpense(id, newDate, newAmount, newValidDescription, newCategory);
            //Set the expected error message
            string errorMessage = "Fill all fields before submitting";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testUpdateExpenseWindow.calledSuccess);
            Assert.True(testUpdateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testUpdateExpenseWindow.errorMessage);
        }
        [Fact]
        public void TestUpdateExpenseNullDate()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            int id = 1;
            //Add an expense to the db
            DateTime date = DateTime.Now;
            string amount = "200";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            //Update the expense that was added
            DateTime? newDate = null;
            string newAmount = "100";
            string newValidDescription = "newValidDescription";
            int? newCategory = 2;
            presenter.UpdateExpense(id, newDate, newAmount, newValidDescription, newCategory);
            //Set the expected error message
            string errorMessage = "Fill all fields before submitting";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testUpdateExpenseWindow.calledSuccess);
            Assert.True(testUpdateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testUpdateExpenseWindow.errorMessage);
        }
        [Fact]
        public void TestUpdateExpenseInvalidAmount()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            int id = 1;
            //Add an expense to the db
            DateTime? date = DateTime.Now;
            string amount = "200";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            //Update the expense that was added
            DateTime newDate = DateTime.Now;
            string newAmount = "abc";
            string newValidDescription = "newValidDescription";
            int? newCategory = 2;
            presenter.UpdateExpense(id, newDate, newAmount, newValidDescription, newCategory);
            //Set the expected error message
            string errorMessage = "Amount must be a positive number";

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testUpdateExpenseWindow.calledSuccess);
            Assert.True(testUpdateExpenseWindow.calledShowError);
            Assert.Equal(errorMessage, testUpdateExpenseWindow.errorMessage);
        }
        [Fact]
        public void TestUpdateExpenseSetFields()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            TestUpdateExpenseWindow testUpdateExpenseWindow = new TestUpdateExpenseWindow();
            presenter.UpdateExpenseView = testUpdateExpenseWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            int id = 1;
            //Add an expense to the db
            DateTime? date = DateTime.Now;
            string amount = "100";
            string validDescription = "validDescription";
            int? category = 1;
            presenter.CreateExpense(date, amount, validDescription, category);

            presenter.SetFields(id);

            Assert.True(testMainWindow.calledContextChanged);
            Assert.False(testUpdateExpenseWindow.calledSuccess);
            Assert.False(testUpdateExpenseWindow.calledShowError);
            Assert.True(testUpdateExpenseWindow.calledFilledFields);
        }
        //--------------------------------------------------------------------------------------------------------------------------
        // Main window view
        //--------------------------------------------------------------------------------------------------------------------------
        [Fact]
        public void TestMainWindowPopulate()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            presenter.MainWindowView = testMainWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            presenter.PopulateCategories(testMainWindow);

            Assert.True(testMainWindow.calledBindCategories);
        }

        [Fact]
        public void PopulateDataViewDefault()
        {
            Presenter presenter = InitializePopulatedPresenter();

            TestDataView testDataView = new TestDataView();
            presenter.DataView = testDataView;

            presenter.PopulateDataView(null, null, false, 0, false, false);

            Assert.True(testDataView.DataSource.Count != 0);
            Assert.True(testDataView.calledShowData);
            Assert.False(testDataView.calledShowDataByCategory);
            Assert.False(testDataView.calledShowDataByMonth);
            Assert.False(testDataView.calledShowDataByMonthByCategory);

        }

        [Fact]
        public void PopulateDataViewByCategory()
        {
            Presenter presenter = InitializePopulatedPresenter();
            TestMainWindow testMainWindow = new TestMainWindow();
            presenter.MainWindowView = testMainWindow;
            TestDataView testDataView = new TestDataView();
            presenter.DataView = testDataView;

            presenter.PopulateDataView(null, null, false, 0, true, false);

            Assert.True(testDataView.DataSource.Count != 0);
            Assert.False(testDataView.calledShowData);
            Assert.True(testDataView.calledShowDataByCategory);
            Assert.False(testDataView.calledShowDataByMonth);
            Assert.False(testDataView.calledShowDataByMonthByCategory);

        }

        [Fact]
        public void PopulateDataViewByMonth()
        {
            Presenter presenter = InitializePopulatedPresenter();
            TestMainWindow testMainWindow = new TestMainWindow();
            presenter.MainWindowView = testMainWindow;
            TestDataView testDataView = new TestDataView();
            presenter.DataView = testDataView;


            presenter.PopulateDataView(null, null, false, 0, false, true);

            Assert.True(testDataView.DataSource.Count != 0);
            Assert.False(testDataView.calledShowData);
            Assert.False(testDataView.calledShowDataByCategory);
            Assert.True(testDataView.calledShowDataByMonth);
            Assert.False(testDataView.calledShowDataByMonthByCategory);

        }

        [Fact]
        public void PopulateDataViewByMonthByCategory()
        {
            Presenter presenter = InitializePopulatedPresenter();
            TestMainWindow testMainWindow = new TestMainWindow();
            presenter.MainWindowView = testMainWindow;
            TestDataView testDataView = new TestDataView();
            presenter.DataView = testDataView;

            presenter.PopulateDataView(null, null, false, 0, true, true);

            Assert.True(testDataView.DataSource.Count != 0);
            Assert.False(testDataView.calledShowData);
            Assert.False(testDataView.calledShowDataByCategory);
            Assert.False(testDataView.calledShowDataByMonth);
            Assert.True(testDataView.calledShowDataByMonthByCategory);

        }

        [Fact]
        public void TestDataGridUpdateSearchResult()
        {
            Presenter presenter = InitializePopulatedPresenter();
            TestMainWindow testMainWindow = new TestMainWindow();
            presenter.MainWindowView = testMainWindow;
            TestDataView testDataView = new TestDataView();
            presenter.DataView = testDataView;

            List<BudgetItem> budgetItems = new List<BudgetItem>();
            BudgetItem budgetItem1 = new BudgetItem();
            budgetItem1.ShortDescription = "Hi";
            budgetItem1.Amount = 12;
            budgetItems.Add(budgetItem1);
            BudgetItem budgetItem2 = new BudgetItem();
            budgetItem2.ShortDescription = "Hello";
            budgetItem2.Amount = 12;
            budgetItems.Add(budgetItem2);

            presenter.UpdateSearchResult(0, budgetItems, "Hello");

            Assert.True(testDataView.calledSelectDataGridItem);
            Assert.Equal(1, testDataView.index);
        }
        [Fact]
        public void TestDataGridShowErrorDueToResultNotFound()
        {
            Presenter presenter = InitializePopulatedPresenter();
            TestMainWindow testMainWindow = new TestMainWindow();
            presenter.MainWindowView = testMainWindow;
            TestDataView testDataView = new TestDataView();
            presenter.DataView = testDataView;

            List<BudgetItem> budgetItems = new List<BudgetItem>();
            BudgetItem budgetItem1 = new BudgetItem();
            budgetItem1.ShortDescription = "Hi";
            budgetItems.Add(budgetItem1);
            BudgetItem budgetItem2 = new BudgetItem();
            budgetItem2.ShortDescription = "Hello";
            budgetItems.Add(budgetItem2);

            presenter.UpdateSearchResult(0, budgetItems, "Non-Existant String");

            Assert.True(testDataView.calledShowError);
        }


        private Presenter InitializeTestPresenter()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();
            TestCreateCategoryView testCreateCategoryView = new TestCreateCategoryView();

            presenter.CreateCategoryView = testCreateCategoryView;
            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;
            string fileName = "test";
            presenter.ProcessNewFile(fileName);

            return presenter;
        }

        private Presenter InitializePopulatedPresenter()
        {
            TestProjectDetailsView testProjectDetailsView = new TestProjectDetailsView();
            Presenter presenter = new Presenter(testProjectDetailsView);
            TestMainWindow testMainWindow = new TestMainWindow();
            TestCreateExpenseWindow testCreateExpenseWindow = new TestCreateExpenseWindow();

            presenter.CreateExpenseView = testCreateExpenseWindow;
            presenter.MainWindowView = testMainWindow;

            string fileName = "test";
            presenter.ProcessNewFile(fileName);
            presenter.CreateExpense(DateTime.Today, "400", "Dummy Expense 1", 1);
            presenter.CreateExpense(DateTime.Today, "20", "Dummy Expense 4", 2);
            presenter.CreateExpense(DateTime.Today, "190", "Dummy Expense 3", 3);
            presenter.CreateExpense(DateTime.Today, "4.50", "Dummy Expense 4", 4);
            return presenter;
        }
    }
}
