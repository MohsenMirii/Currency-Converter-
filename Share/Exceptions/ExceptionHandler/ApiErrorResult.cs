

using JetBrains.Annotations;



namespace Share.Exceptions.ExceptionHandler;

[PublicAPI]
public class ApiErrorResult {
    public ApiErrorResult(string[] errors)
    {
        Errors = errors;
    }

    public ApiErrorResult(string[] errors, Exception exception)
    {
        Errors = errors;
        Exception = exception.ToString();
    }

    public ApiErrorResult(string[] errors, ErrorCode code, Exception exception)
    {
        Errors = errors;
        Code = code.Code;
        Exception = exception.ToString();
    }

    public ApiErrorResult(string[] errors, ErrorCode code)
    {
        Errors = errors;
        Code = code.Code;
    }

    // public ApiErrorResult(Dictionary<string, List<string>> validationErrors)
    // {
    //     Validations = validationErrors;
    //     Code = StandardErrorCodes.Validation.Code;
    //     Errors = "بخشی از اطلاعات ارسال شده نادرست است";
    // }

    public string[] Errors { get; set; }

    public string? Code { get; set; }

    // public Dictionary<string, List<string>>? Validations { get; set; }

    public string? Exception { get; set; }
}