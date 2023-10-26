using FluentValidation.Results;

namespace Auth.API.Application.Extensions;

public static class Collections
{
    public static Dictionary<string, string[]> ConvertToDictionnary(this ValidationResult @this)
    {
        var errors = from error in @this.Errors
                     group error by error.PropertyName into grouped
                     select grouped;
        return errors.ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).Distinct().ToArray());
    }
}
