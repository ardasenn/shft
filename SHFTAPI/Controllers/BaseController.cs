using Application.Utilities.Response;
using Microsoft.AspNetCore.Mvc;
using SHFTAPI.Extensions;

namespace SHFTAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult CreateResponse<T>(GenericResponse<T> response)
        {
            return response.StatusCode switch
            {
                200 => Ok(response),
                201 => Created(string.Empty, response),
                400 => BadRequest(response),
                401 => Unauthorized(response),
                403 => Forbid(),
                404 => NotFound(response),
                409 => Conflict(response),
                500 => StatusCode(500, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }

        protected IActionResult ValidateModelAndExecute<T>(Func<IActionResult> action)
        {
            if (!ModelState.IsValid())
            {
                var validationResponse = ModelState.ToGenericResponse<T>();
                return CreateResponse(validationResponse);
            }

            return action();
        }

        protected async Task<IActionResult> ValidateModelAndExecuteAsync<T>(Func<Task<IActionResult>> action)
        {
            if (!ModelState.IsValid())
            {
                var validationResponse = ModelState.ToGenericResponse<T>();
                return CreateResponse(validationResponse);
            }

            return await action();
        }

        protected string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        protected string GetCurrentUserRole()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
        }

        protected string GetCurrentUserType()
        {
            return User?.FindFirst("UserType")?.Value ?? string.Empty;
        }

        protected bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }

        protected bool IsDietitian()
        {
            return GetCurrentUserRole() == "Dietitian";
        }

        protected bool IsClient()
        {
            return GetCurrentUserRole() == "Client";
        }
    }
}
