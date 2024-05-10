
namespace Water_Tracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.Create();

            Gui.GetUserInput();
        }
    }
}
