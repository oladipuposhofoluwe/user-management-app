using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (BadRequestException ex)
    {
        await HandleExceptionAsync(context, ex.Message, StatusCodes.Status400BadRequest);
    }
    catch (NotFoundException ex)
    {
        await HandleExceptionAsync(context, ex.Message, StatusCodes.Status404NotFound);
    }
    catch (UnauthorizedException ex)
    {
        await HandleExceptionAsync(context, ex.Message, StatusCodes.Status401Unauthorized);
    }
    catch (ForbiddenException ex)
    {
        await HandleExceptionAsync(context, ex.Message, StatusCodes.Status403Forbidden);
    }
    catch (Exception ex)
    {
        await HandleExceptionAsync(context, "An unexpected error occurred.", StatusCodes.Status500InternalServerError);
    }
}

    private Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            message
        });

        return context.Response.WriteAsync(result);
    }

}
