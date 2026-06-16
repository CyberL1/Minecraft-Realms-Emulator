namespace Core.Models;

public class ApiError
{
    public required int ErrorCode { get; set; }
    public required string ErrorMsg { get; set; }
}
