namespace SonicRemake.Animations
{
  public readonly record struct AnimationData
  {
    public string Name { get; init; }
    public int StartFrameX { get; init; }
    public int StartFrameY { get; init; }
    public int NumberOfSprites { get; init; }
    public int FramesPerSprite { get; init; }
    public int Loops { get; init; }
  }
}
