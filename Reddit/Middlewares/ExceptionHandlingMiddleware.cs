using System.Text.Json;

namespace Reddit.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Invoke the next middleware in the pipeline.
            }
            catch (Exception ex)
            {
                //log the error
                _logger.LogError(ex, "Developer go fix it up");

                // Create the error response model (Class created by me)
                var errorResponseModel = new ErrorResponse
                {
                    Message = "Sorry, an error occurred!",
                    Description = ex.Message
                };

                // Set the response status code and content type
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json"; // Set the response content type to JSON

                // Serialize the error response to JSON
                var jsonResponse = JsonSerializer.Serialize(errorResponseModel);

                // Write the response body
                await context.Response.WriteAsync(jsonResponse);
            }
        }

    }
}
