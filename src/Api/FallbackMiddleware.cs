namespace Api;

public class FallbackMiddleware(
  RequestDelegate next
)
{
  public async Task InvokeAsync(HttpContext context)
  {
    if (
      !context.Request.Path.StartsWithSegments("/graphql")
      && !context.Request.Path.StartsWithSegments("/api")
      && !System.IO.Path.HasExtension(context.Request.Path.Value)
    )
    {
      context.Response.ContentType = "text/html";
      await context.Response.SendFileAsync("wwwroot/index.html");

      return;
    }

    await next(context);
  }
}
