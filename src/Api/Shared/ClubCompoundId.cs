namespace Api.Shared;

/// <summary>
///   A compound identifier for any object that belongs to a club.
///   We need to know from which club (tenant) to fetch the data.
/// </summary>
public readonly record struct ClubCompoundId(
  Guid ClubId,
  Guid Id
)
{
  public override string ToString() => $"{ClubId}:{Id}";

  public static ClubCompoundId Parse(string value)
  {
    var parts = value.Split(':');

    return new ClubCompoundId(Guid.Parse(parts[0]), Guid.Parse(parts[1]));
  }
}
