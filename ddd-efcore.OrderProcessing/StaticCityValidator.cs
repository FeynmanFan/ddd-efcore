using System.Text.RegularExpressions;

namespace ddd_efcore.OrderProcessing
{
    public class StaticCityValidator : ICityValidator
    {
        private const string StateRegexPattern = @"^(AL|AK|AZ|AR|CA|CO|CT|DE|FL|GA|HI|ID|IL|IN|IA|KS|KY|LA|ME|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|OH|OK|OR|PA|RI|SC|SD|TN|TX|UT|VT|VA|WA|WV|WI|WY)$";
        private const string ZipCodeRegexPattern = @"^\d{5}$";

        private static readonly Dictionary<string, (string City, string State)> ZipCodeLookup = new()
        {
            { "90210", ("Beverly Hills", "CA") },
            { "10001", ("New York", "NY") },
            { "76103", ("Fort Worth", "TX") },
            { "00501", ("Holtsville", "NY") }
        };

        public bool IsValidCity(string zipCode, string cityName, string state)
        {
            var validationErrors = new List<string>();

            // Validate ZipCode
            if (string.IsNullOrWhiteSpace(zipCode))
                validationErrors.Add("Zip code is required.");
            else if (!Regex.IsMatch(zipCode, ZipCodeRegexPattern))
                validationErrors.Add("Zip code must be a five-digit number.");

            // Validate CityName
            if (string.IsNullOrWhiteSpace(cityName))
                validationErrors.Add("City name is required.");
            else if (cityName.Length < 1)
                validationErrors.Add("City name cannot be empty.");

            // Validate State
            if (string.IsNullOrWhiteSpace(state))
                validationErrors.Add("State is required.");
            else if (!Regex.IsMatch(state, StateRegexPattern))
                validationErrors.Add("State must be a valid U.S. state code (e.g., CA, NY).");

            // Validate ZipCode alignment
            if (ZipCodeLookup.TryGetValue(zipCode, out var expected))
            {
                if (expected.City.ToLower() != cityName.ToLower() || expected.State.ToLower() != state.ToLower())
                    validationErrors.Add($"Zip code {zipCode} does not match city {cityName} or state {state}.");
            }
            else
            {
                validationErrors.Add($"Invalid zip code: {zipCode}.");
            }

            return !validationErrors.Any();
        }
    }
}