namespace Api;

public class ErrorFilter : IErrorFilter
{
  public IError OnError(IError error)
  {
    return ErrorBuilder
      .FromError(error)
      .SetMessage(error.Message)
      .Build();
  }
}
