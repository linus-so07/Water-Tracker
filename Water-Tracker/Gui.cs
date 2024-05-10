namespace Water_Tracker
{
    internal class Gui
    {
        internal static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View All Records.");
                Console.WriteLine("Type 2 to Insert Record.");
                Console.WriteLine("Type 3 to Delete Record.");
                Console.WriteLine("Type 4 to Update Record.");
                Console.WriteLine("------------------------------------------\n");

                // User Input
                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                        break;

                }
            }
        }

        internal static void GetAllRecords()
        {
            Console.Clear();

            var tableData = Database.GetData();

            Console.WriteLine("------------------------------------------\n");
            foreach (var dw in tableData)
            {
                Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MMM-yyyy")} - Quantity: {dw.Quantity}");
            }
            Console.WriteLine("------------------------------------------\n");

        }

        private static void Insert()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice " +
                "(no decimals allowed)\n\n");

            Database.Add(date, quantity);
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string numberInput = Console.ReadLine();

            if (numberInput == "0") Gui.GetUserInput();

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main manu.\n\n");

            string dateInput = Console.ReadLine();

            if (dateInput == "0") Gui.GetUserInput();

            if (!IsValidDateFormat(dateInput))
            {
                Console.WriteLine("\nInvalid date format. Please use the format: dd-mm-yy");
                return GetDateInput();
            }

            return dateInput;

        }

        // Method to validate the date format
        internal static bool IsValidDateFormat(string dateInput)
        {
            // Check if the input matches the format "dd-mm-yy"
            if (DateTime.TryParseExact(dateInput, "dd-MM-yy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void Delete()
        {
            Console.Clear();
            Gui.GetAllRecords();

            var recordId = GetNumberInput
                ("\n\nPlease type the Id of the record you want to delete or type 0 to return to Menu");

            var rowCount = Database.Remove(recordId);
            if (rowCount == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
                Delete();

            }
            Database.UpdateIds(recordId);
            Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");

            Gui.GetUserInput();
        }

        internal static void Update()
        {
            Gui.GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease type Id of the record would like to update. Type 0 to return to main manu.\n\n");

            var check = Database.CheckID(recordId);

            if (!check)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                Update();
            }

            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");

            Database.Update(date, quantity, recordId);


        }
    }
}
