using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Handlers.Implementations
{
    public class AccountHandler
    { 
        public static string GenerateVIN()
        {
            Random rnd = new Random();

            int number = rnd.Next(1,7);
            string result = "VIN-" + number;

            return result;
        }
    }
}
