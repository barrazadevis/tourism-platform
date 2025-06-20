public class TokenValidationResponse
{
    public bool IsValid { get; set; }
    public UserInfo? User { get; set; }
    public string? Error { get; set; }
}