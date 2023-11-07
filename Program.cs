using CurrencyApp.API;
using System;
using System.Threading.Tasks;

namespace CurrencyApp
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine(await CurrencyCalc.Sum("BYN", new Money[]
            {
                new Money
                {
                    currency = "EUR",
                    value = 10.0m
                },
                new Money
                {
                    currency = "USD",
                    value = 12.0m
                }
            }));

            Console.ReadKey();
        }
    }
}
