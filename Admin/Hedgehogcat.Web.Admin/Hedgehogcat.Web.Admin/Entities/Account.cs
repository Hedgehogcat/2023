namespace Hedgehogcat.Web.Admin.Entities;

using System.Text.Json.Serialization;

public class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    [JsonIgnore]
    public List<RefreshToken>? RefreshTokens { get; set; }
}