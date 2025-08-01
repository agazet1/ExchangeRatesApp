using NLog;

namespace ExchangeRatesApi
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                Logger.Info($"Request: {context.Request.Method} {context.Request.Path} Body: {requestBody}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while reading request");
            }

            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "An error occurred while handling the request");
                    throw;
                }

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                if (context.Response.StatusCode >= 400)
                {
                    Logger.Error($"Response Status: {context.Response.StatusCode} Body: {responseText}");
                }
                else
                {
                    Logger.Info($"Response Status: {context.Response.StatusCode}");
                }

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
