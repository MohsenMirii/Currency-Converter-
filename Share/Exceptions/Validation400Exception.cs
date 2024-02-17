namespace Share.Exceptions;

// [PublicAPI]
// public class Validation400Exception : ExceptionWithLogLevel
// {
//     private readonly Dictionary<string, List<string>> _errors = new();
//
//     public Validation400Exception()
//     {
//         
//     }
//     
//     public Validation400Exception(List<ValidationResult> validationResults)
//     {
//         foreach (var result in validationResults)
//         foreach (var memberName in result.MemberNames)
//             Add(memberName, result.ErrorMessage ?? "[UNKNOWN]");
//     }
//
//     public Validation400Exception(string fieldName, params string[] errors)
//     {
//         Add(fieldName, errors);
//     }
//     
//     public void ThrowIfRequired()
//     {
//         if (_errors.Count > 0)
//             throw this;
//     }
//
//     public Validation400Exception Add(string fieldName, params string[] errors)
//     {
//         if (_errors.TryGetValue(fieldName, out var value))
//             value.AddRange(errors);
//         else
//             _errors.Add(fieldName, errors.ToList());
//
//         return this;
//     }
//
//     /// <summary>
//     /// Get all added errors
//     /// </summary>
//     internal Dictionary<string, List<string>> GetErrors()
//     {
//         return _errors;
//     }
// }