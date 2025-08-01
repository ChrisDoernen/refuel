namespace Api;

public class ErrorFilter : IErrorFilter
{
  public IError OnError(IError error) =>
    ErrorBuilder
      .FromError(error)
      .SetMessage(error.Message)
      .Build();
}
