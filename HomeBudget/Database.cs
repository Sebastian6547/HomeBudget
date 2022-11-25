using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using System.IO;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleToAttribute("TestHomeBudget")]
// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Budget
{
    internal class Database
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <value>The connection to the database currently in use.</value>
        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
        /// <summary>
        /// Opens a connection to the passed database file and creates all HomeBudget tables.
        /// </summary>
        /// <remarks>If there are already HomeBudget tables in the database, they will be cleared.</remarks>
        /// <param name="filename">Name of the database file.</param>
        /// <example>
        /// <code>
        /// newDatabase("C:\\Users\\newDB.db");
        /// CloseDatabaseAndReleaseFile();
        /// </code>
        /// </example>
        public static void newDatabase(string filename)
        {

            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();
            if (!File.Exists(filename))
                SQLiteConnection.CreateFile(filename);
            else
            {
                _connection = new SQLiteConnection($"Data Source={filename}; Foreign Keys=1");
                _connection.Open();


                string removeTable = "drop table if exists expenses;";
                SQLiteCommand com = new SQLiteCommand(removeTable, _connection);
                com.ExecuteNonQuery();
                com.Dispose();

                removeTable = "drop table if exists categories;";
                com = new SQLiteCommand(removeTable, _connection);
                com.ExecuteNonQuery();
                com.Dispose();
            
                removeTable = "drop table if exists categoryTypes;";
                com = new SQLiteCommand(removeTable, _connection);
                com.ExecuteNonQuery();
                com.Dispose();

                _connection.Close();
            }

            _connection = new SQLiteConnection($"Data Source={filename}; Foreign Keys=1");
            _connection.Open();

            string sqlCommand = "create table categoryTypes (Id INTEGER PRIMARY KEY, Description text)";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, _connection);
            command.ExecuteNonQuery();
            command.Dispose();

            sqlCommand = "create table categories (Id INTEGER PRIMARY KEY AUTOINCREMENT, Description text, TypeId int, FOREIGN KEY (TypeId) REFERENCES categoryTypes(Id))";
            command = new SQLiteCommand(sqlCommand, _connection);
            command.ExecuteNonQuery();
            command.Dispose();

            sqlCommand = "create table expenses (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date text, Description text, Amount real, CategoryId int, FOREIGN KEY (categoryId) REFERENCES categories(Id))";
            command = new SQLiteCommand(sqlCommand, _connection);
            command.ExecuteNonQuery();
            command.Dispose();

           
        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        /// <summary>
        /// Opens a connection to the passed database file.
        /// </summary>
        /// <param name="filename">Name of the database file.</param>
        /// <example>
        /// <code>
        /// existingDatabase("C:\\Users\\oldDb.db");
        /// CloseDatabaseAndReleaseFile();
        /// </code>
        /// </example>
        public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();
            if (File.Exists(filename))
            {
                _connection = new SQLiteConnection($"Data Source={filename}; Foreign Keys=1");
                _connection.Open();
            }
            else
                throw new FileNotFoundException("Database file not found/does not exists.");

           
        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================
        /// <summary>
        /// Closes the database connection.
        /// </summary>
        /// <example>
        /// <code>
        /// existingDatabase("C:\\Users\\usedDb.db");
        /// CloseDatabaseAndReleaseFile();
        /// </code>
        /// </example>
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();
                

                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }

}
