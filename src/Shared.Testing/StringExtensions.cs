namespace Shared.Testing;

public static class StringExtensions
{
  public static string Minify(this string str)
    => str.Replace("\r\n", "").Replace(" ", "");
}
