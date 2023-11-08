using CurrencyApp.API.Valutes;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyApp.API
{
    internal static class CurrencyCalc
    {
        private static CBValutes valutes;
        private static readonly Stopwatch timer = new Stopwatch();
        private static readonly SemaphoreSlim sem = new SemaphoreSlim(1);
        public static TimeSpan UpdateInterval { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Это все игрушки
        /// </summary>
        private static async Task<CBValutes> TimedGetValutes()
        {
            await sem.WaitAsync();
            try
            {
                if (valutes == null || timer.Elapsed > UpdateInterval)
                {
                    timer.Restart();
                    return (valutes = await CBValutes.Get());
                }

                return valutes;
            } 
            finally
            {
                sem.Release();
            }
        }

        public static async Task<Money> Sum(string resultCurrency, Money[] money)
        {
            var valutes = await TimedGetValutes();
            var resultCurrencyCostRUB = 1.0m / valutes.Valutes[resultCurrency].Rate; // Стоимость одного рубля в валюте
            return new Money
            {
                currency = resultCurrency,
                value = money.Aggregate(0.0m, (result, value) =>
                    result +
                    // Умножаем по курсу в рублях
                    value.value * valutes.Valutes[value.currency].Rate 
                    // Умножаем на стоимость одного рубля в валюте
                    * resultCurrencyCostRUB)
            };
        }
    }
}
