using TicketAppApi.Model;

namespace TicketAppApi.Extension
{
    public static class HelloWorld
    {
        public static void Hellomsg(this WebApplication app)
        {
            //Console.WriteLine(typeof(FinancialStaff).Name);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("TicketServiceApi Run Successfully!--------------"+DateTime.Now);
            LogHelper.LogInfo(0, "TicketServiceApi Run Successfully!--------------"+DateTime.Now);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
