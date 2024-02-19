using HomeBankingMindHub.Handlers.Interfaces;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;
using System.Text;

namespace HomeBankingMindHub.Handlers.Implementations
{
    public class NumbersHandler
    { 
        public static string GenerateVIN()
        {
            Random rnd = new Random();

            string number = rnd.Next(1,100000000).ToString("D8");
            string result = "VIN-" + number;

            return result;
        }

        public static string GenerateCardNumber()
        {
            Random rnd = new Random();
            string result = "";

            for (int i = 0; i < 16; i++)
            {
                if (i > 0 && i % 4 == 0)
                {
                    result += "-";
                }

                result += rnd.Next(0, 10).ToString();
            }

            return result;
        }

        public static int GenerateCardCVV()
        {
            Random random = new Random();
            string result = random.Next(0, 1000).ToString("D3");
            return int.Parse(result);
        }
    }
}
