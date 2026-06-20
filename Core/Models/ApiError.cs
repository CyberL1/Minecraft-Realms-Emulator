namespace Core.Models;

public class ApiError
{
    private ApiError(int errorCode, string errorMsg)
    {
        ErrorCode = errorCode;
        ErrorMsg = errorMsg;
    }

    public int ErrorCode { get; init; }
    public string ErrorMsg { get; init; }

    public static ApiError WorldNotFound => new(404, "World not found");
    public static ApiError NotAWorldMember => new(403, "Not a world member"); // TODO: Check if this is correct
    public static ApiError NotOwner => new(403, "Not owner");
    public static ApiError WorldAlreadyInitialized => new(409, "World already initialized");
}
