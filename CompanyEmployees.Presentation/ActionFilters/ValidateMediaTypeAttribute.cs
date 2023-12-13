using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace CompanyEmployees.Presentation.ActionFilters;

/**
 * Now,since we've implemented custom media types, we want our Accept header
 *  to be present in our request so we can detect when the user requested the HATEOS-enriched response.
 */
public class ValidateMediaTypeAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var acceptHeaderPresent = context.HttpContext
            .Request.Headers.ContainsKey("Accept");

        // We check for the existence of the Accept Header first
        if (!acceptHeaderPresent)
        {
            context.Result = new BadRequestObjectResult($"Accept header is missing.");

            return;
        }

        // We parse the mediaType
        var mediaType = context.HttpContext
            .Request.Headers["Accept"].FirstOrDefault();

        if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue?outMediaType))
        {
            // If mediaType is not valide
            context.Result =
                new BadRequestObjectResult(
                    $"Media type not present. Please add Accept header with the required media type.");
            return;
        }

        // We pass the parsed mediaType to the HttpContext of the controller.
        context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
    }

    public void OnActionExecuted(ActionExecutedContext context) {}
}