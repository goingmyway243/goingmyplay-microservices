namespace Play.Identity.Settings
{
    public class JwtSettings
    {
        public string PrivateKey { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int ExpiryInMinutes { get; init; }
    }
}
