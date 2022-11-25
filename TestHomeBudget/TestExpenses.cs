using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        String testInputFile = TestConstants.testExpensesInputFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [Fact]
        public void ExpensesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Expenses expenses = new Expenses(conn);

            // Assert 
            Assert.IsType<Expenses>(expenses);
        }

        // ========================================================================

        [Fact]
        public void ExpensesObject_New_CreatesDefaultExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Expenses expenses = new Expenses(conn);

            // Assert 
            Assert.True(expenses.List().Count == 0, "Empty table");

        }

        // ========================================================================

 /**       [Fact]
        public void ExpensesMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(existingDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Expenses expenses = new Expenses(conn);
            List<Expense> list = expenses.List();
            Expense firstExpense = list[0];

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);
            Assert.Equal(firstExpenseInFile.Id, firstExpense.Id);
            Assert.Equal(firstExpenseInFile.Description, firstExpense.Description);

        }
 **/
        // ========================================================================

        [Fact]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);

            // Act
            List<Expense> list = expenses.List();

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);

        }


        // ========================================================================

        [Fact]
        public void ExpensesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
            string descr = "Electricity";
            DateTime date = DateTime.Now;
            // Act
            expenses.Add(date, 1, 300, descr);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expenses.List().Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile + 1, sizeOfList);
            Assert.Equal(descr, expensesList[sizeOfList - 1].Description);

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
            int IdToDelete = 3;

            // Act
            expenses.Delete(IdToDelete);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile - 1, sizeOfList);
            Assert.False(expensesList.Exists(e => e.Id == IdToDelete), "correct Category item deleted");

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
            int IdToDelete = 9999;
            int sizeOfList = expenses.List().Count;

            // Act
            try
            {
                expenses.Delete(IdToDelete);
                Assert.Equal(sizeOfList, expenses.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_GetExpenseFromId()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
            int expID = 1;

            // Act
            Expense expense = expenses.GetExpensesFromId(expID);

            // Assert
            Assert.Equal(expID, expense.Id);

        }


        // ========================================================================

        [Fact]
        public void ExpensesMethod_UpdateExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Expenses expenses = generateTestExpense(newDB);
            String newDescr = "Presents";
            int id = 1;
            int typeId = 1;
            DateTime date = DateTime.Now;
       
            // Act
            expenses.UpdateProperties(id, date, newDescr, 303.2, typeId);
            Expense expense = expenses.GetExpensesFromId(id);

            // Assert 
            Assert.Equal(newDescr, expense.Description);
            Assert.Equal(typeId, expense.Category);

        }

        private Expenses generateTestExpense(String filename)
        {
            Database.newDatabase(filename);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, true);
            Expenses expenses = new Expenses(conn);
            DateTime time = DateTime.Now;
            expenses.Add(time, 2, 550, "Electricity");
            expenses.Add(time, 1, 500, "Stimmy Check");
            expenses.Add(time, 2, 200, "Water");
            expenses.Add(time, 1, 100, "Food");
            return expenses;

        }
    }
}

