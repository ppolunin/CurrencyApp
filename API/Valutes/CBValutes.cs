using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CurrencyApp.API.Valutes
{
    public sealed class CBValute
    {
        [XmlAttribute]
        public string ID;
        [XmlElement("NumCode")]
        public ushort Code;
        public string CharCode;
        /// <summary>
        /// Я так понял размер лота торгового, например, 10 Норвежских крон, 1 Доллар США
        /// </summary>
        public uint Nominal;
        public string Name;
        /// <summary>
        /// Номинальная стоимость рубля, например 10 Норвежских крон 83 рубля стоят
        /// </summary>
        [XmlIgnore]
        public decimal Value;
        /// <summary>
        /// Реальная стоимость рубля, например 1 Норвежская крона 8,3 рубля, хотя торгуют по 10 Норвежских крон
        /// </summary>
        [XmlIgnore]
        public decimal Rate;

        [XmlElement("VunitRate")]
        public string TmpVunitRate
        {
            get => throw new NotImplementedException();
            set => Rate = decimal.Parse(value, ru);
        }

        [XmlElement("Value")]
        public string TmpValue
        {
            get => throw new NotImplementedException();
            set => Value = decimal.Parse(value, ru);
        }

        private static readonly CultureInfo ru = CultureInfo.GetCultureInfo("ru");
    }

    [XmlRoot(ElementName = "ValCurs")]
    public sealed class CBValutes
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public DateTime Date;
        [XmlIgnore]
        public IReadOnlyDictionary<string, CBValute> Valutes;

        [XmlElement(ElementName = "Valute", Type = typeof(CBValute))]
        public CBValute[] TmpValutes
        {
            get => throw new NotImplementedException();
            set => Valutes = value.ToDictionary(key => key.CharCode, StringComparer.OrdinalIgnoreCase);
        }

        [XmlAttribute("Date")]
        public string TmpDate
        {
            get => throw new NotImplementedException();
            set => Date = DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        public static async Task<CBValutes> Get()
        {
            using (var client = new HttpClient(new HttpClientHandler()
            {
                SslProtocols = System.Security.Authentication.SslProtocols.None
            }) { BaseAddress = new Uri("https://www.cbr.ru/scripts/") })
            {
                using (var response = await client.GetAsync("XML_daily.asp"))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        return new XmlSerializer(typeof(CBValutes)).Deserialize(stream) as CBValutes;
                    }
                }
            }
        }
    }
}
