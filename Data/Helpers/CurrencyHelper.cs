namespace Data.Helpers
{
    public static class CurrencyHelper
    {
        public static string GetCurrencySymbol(string currencyCode)
        {
            return currencyCode?.ToUpper() switch
            {
                "USD" => "$",
                "EUR" => "€",
                "GEL" => "₾",
                "GBP" => "£",
                "RUB" => "₽",
                "JPY" => "¥",
                "CNY" => "¥",
                _ => currencyCode ?? ""
            };
        }

        public static string FormatAmount(decimal amount, string currencyCode)
        {
            string symbol = GetCurrencySymbol(currencyCode);

            return currencyCode?.ToUpper() switch
            {
                "USD" or "GBP" => $"{symbol}{amount:N2}",
                "EUR" or "GEL" or "RUB" => $"{amount:N2} {symbol}",
                _ => $"{amount:N2} {currencyCode}"
            };
        }
    }
}