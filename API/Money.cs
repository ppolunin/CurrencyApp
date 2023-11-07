using System.Globalization;

namespace CurrencyApp.API
{
    internal class Money
    {
        public string currency;
        public decimal value;
        public override string ToString()
        {
            return $"{{ \"currency\": \"{currency}\", \"value\": {value.ToString("N4", CultureInfo.InvariantCulture)} }}";
        }
    }
}