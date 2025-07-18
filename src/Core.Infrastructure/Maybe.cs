namespace Core.Infrastructure;

public class Maybe<T>
{
  private T? _value;
  public static Maybe<T> None => new();
  public static Maybe<T> ForValue(T? value) => new() { _value = value };

  public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
    => _value is null ? Maybe<TResult>.None : Maybe<TResult>.ForValue(map(_value));

  public Maybe<U> Map<U>(Func<T, Maybe<U>> binder)
    => _value is null ? new Maybe<U>() : binder(_value);

  public T Reduce(T defaultValue) => _value ?? defaultValue;
  public T Reduce(Func<T> getDefaultValue) => _value ?? getDefaultValue();
  public T ReduceThrow(Exception ex) => _value ?? throw ex;
  public T? ReduceNull => _value;
  public bool HasValue => _value is not null;
}
