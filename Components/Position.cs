using System;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Animations;

namespace SonicRemake.Components
{
  public record struct Transform(Vector2f Position, Vector2f Scale, float Rotation);

  public record struct Sprite(string SpriteId, Color? MaskColor = null);

  public record struct SpriteSheet(int X, int Y, int SpriteSize);

  public record struct Renderer(Texture? Texture);

  public record struct Animator(AnimationData AnimationData, int SpritesLeft, int FramesLeft, int LoopsLeft);

  public record struct AnimationQueue(Queue<string> Animations);

  public record struct Sonic();
}
