namespace Api.Shared;

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
