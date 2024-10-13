using System;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Animations;

namespace SonicRemake.Components;

public record struct Transform(Vector2f Position, Vector2f Scale, float Rotation = 0, ushort GroundAngle = 0);

public record struct Velocity(Vector2f Speed, float GroundSpeed);

public record struct Sprite(string SpriteId, params Color[] MaskColors);

public record struct Rectangle(Vector2f Size, Color FillColor, Color OutlineColor, float OutlineThickness);

public record struct SpriteSheet(int X, int Y, int SpriteSize);

public record struct Renderer(Drawable? Drawable, bool FlipX = false, bool FlipY = false);

public record struct SpriteAnimator(AnimationData AnimationData, int SpritesLeft, int FramesLeft, int LoopsLeft);

public record struct SpriteAnimation(string Animation = "idle", int FramesPerSprite = 6, bool Loop = true);

public record struct Sonic(SonicState State, bool IsOnGround, float SpinRef, Facing Facing, int BoredCount
);

public record struct Sensor();

public record struct Camera(float Zoom = 4f);

public enum SonicState
{
  Idle,
  Running,
  Skidding,
  Jumping,
  Falling,
  Charging,
  SpinRoll,
  Crouching
}

public enum Facing
{
  Right,
  Left
}
