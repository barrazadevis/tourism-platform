public class AuthValidationResult
{
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? Role { get; set; }
    public Guid? TenantId { get; set; }
    public string? Error { get; set; }
}