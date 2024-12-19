namespace Pingring.AddressNormalization.Api.Services;

public static class StateMap
{
    private static readonly Dictionary<string, string> StateToShortMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "alabama", "AL" },
        { "alaska", "AK" },
        { "arizona", "AZ" },
        { "arkansas", "AR" },
        { "california", "CA" },
        { "colorado", "CO" },
        { "connecticut", "CT" },
        { "delaware", "DE" },
        { "district of columbia", "DC" },
        { "florida", "FL" },
        { "georgia", "GA" },
        { "hawaii", "HI" },
        { "idaho", "ID" },
        { "illinois", "IL" },
        { "indiana", "IN" },
        { "iowa", "IA" },
        { "kansas", "KS" },
        { "kentucky", "KY" },
        { "louisiana", "LA" },
        { "maine", "ME" },
        { "maryland", "MD" },
        { "massachusetts", "MA" },
        { "michigan", "MI" },
        { "minnesota", "MN" },
        { "mississippi", "MS" },
        { "missouri", "MO" },
        { "montana", "MT" },
        { "nebraska", "NE" },
        { "nevada", "NV" },
        { "new hampshire", "NH" },
        { "new jersey", "NJ" },
        { "new mexico", "NM" },
        { "new york", "NY" },
        { "north carolina", "NC" },
        { "north dakota", "ND" },
        { "ohio", "OH" },
        { "oklahoma", "OK" },
        { "oregon", "OR" },
        { "pennsylvania", "PA" },
        { "rhode island", "RI" },
        { "south carolina", "SC" },
        { "south dakota", "SD" },
        { "tennessee", "TN" },
        { "texas", "TX" },
        { "utah", "UT" },
        { "vermont", "VT" },
        { "virginia", "VA" },
        { "washington", "WA" },
        { "west virginia", "WV" },
        { "wisconsin", "WI" },
        { "wyoming", "WY" }
    };

    private static readonly Dictionary<string, string> ShortToStateMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "AL", "Alabama" },
        { "AK", "Alaska" },
        { "AZ", "Arizona" },
        { "AR", "Arkansas" },
        { "CA", "California" },
        { "CO", "Colorado" },
        { "CT", "Connecticut" },
        { "DE", "Delaware" },
        { "DC", "District of Columbia" },
        { "FL", "Florida" },
        { "GA", "Georgia" },
        { "HI", "Hawaii" },
        { "ID", "Idaho" },
        { "IL", "Illinois" },
        { "IN", "Indiana" },
        { "IA", "Iowa" },
        { "KS", "Kansas" },
        { "KY", "Kentucky" },
        { "LA", "Louisiana" },
        { "ME", "Maine" },
        { "MD", "Maryland" },
        { "MA", "Massachusetts" },
        { "MI", "Michigan" },
        { "MN", "Minnesota" },
        { "MS", "Mississippi" },
        { "MO", "Missouri" },
        { "MT", "Montana" },
        { "NE", "Nebraska" },
        { "NV", "Nevada" },
        { "NH", "New Hampshire" },
        { "NJ", "New Jersey" },
        { "NM", "New Mexico" },
        { "NY", "New York" },
        { "NC", "North Carolina" },
        { "ND", "North Dakota" },
        { "OH", "Ohio" },
        { "OK", "Oklahoma" },
        { "OR", "Oregon" },
        { "PA", "Pennsylvania" },
        { "RI", "Rhode Island" },
        { "SC", "South Carolina" },
        { "SD", "South Dakota" },
        { "TN", "Tennessee" },
        { "TX", "Texas" },
        { "UT", "Utah" },
        { "VT", "Vermont" },
        { "VA", "Virginia" },
        { "WA", "Washington" },
        { "WV", "West Virginia" },
        { "WI", "Wisconsin" },
        { "WY", "Wyoming" }
    };

    public static string? FromState(string? input)
    {
        return input is null 
            ? null 
            : StateToShortMap.GetValueOrDefault(input);

    }

    public static string? FromShort(string? input)
    {
        return input is null
            ? null
            : ShortToStateMap.GetValueOrDefault(input);
    }
}