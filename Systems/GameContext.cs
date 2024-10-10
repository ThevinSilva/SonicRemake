namespace SonicRemake.Systems
{
  public readonly struct GameContext
  {
    public float DeltaTime { get; init; }

    public float PhysicsDeltaTime { get; init; }

    public float AnimationDeltaTime { get; init; }
  }
}
