using System;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Animations;

namespace SonicRemake.Components
{
  public record struct Transform(Vector2f Position, Vector2f Scale, bool IsOnGround = false, float Rotation = 0, ushort GroundAngle = 0);

  public record struct Velocity(Vector2f Speed, float GroundSpeed);

  public record struct Sprite(string SpriteId, params Color[] MaskColors);

  public record struct SpriteSheet(int X, int Y, int SpriteSize);

  public record struct Renderer(Texture? Texture, bool FlipX = false, bool FlipY = false);

  public record struct Animator(AnimationData AnimationData, int SpritesLeft, int FramesLeft, int LoopsLeft);

  public record struct AnimationQueue(string Animation = "idle", int FramesPerSprite = 6);

  public record struct Sonic();
}
