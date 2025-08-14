using Application.Utilities.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SHFTAPI.Extensions
{
    public static class ValidationExtensions
    {
        public static GenericResponse<T> ToGenericResponse<T>(this ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            return GenericResponse<T>.Fail(errors, "Validation failed", 400);
        }

        public static bool IsValid(this ModelStateDictionary modelState)
        {
            return modelState.IsValid;
        }
    }
}
