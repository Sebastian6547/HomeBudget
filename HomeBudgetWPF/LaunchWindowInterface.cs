using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetWPF
{
    public interface LaunchWindowInterface
    {
        /// <summary>
        /// Gets file path from file inputted by user and returns the path of the file opened
        /// </summary>
        /// <returns>A string of the file path opened. Returns null if canceled</returns>
        string GetExistingFile();
        /// <summary>
        /// Opens the main window and closes the current window (launch window)
        /// </summary>
        void OpenMainCloseLaunch();
        /// <summary>
        /// Outputs given error message to user
        /// </summary>
        /// <param name="message">Given error message</param>
        void ShowError(string message);

    }
}
