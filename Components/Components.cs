using System;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Animations;
using SonicRemake.Maps;

namespace SonicRemake.Components;

public record struct Transform(Vector2f Position, Vector2f Scale, float Rotation = 0, ushort GroundAngle = 0)
{
  // Set scale to (1, 1) with an empty constructor
  public Transform() : this(new Vector2f(0, 0), new Vector2f(1, 1)) { }
}

public record struct Velocity(Vector2f Speed, float GroundSpeed);

public record struct Sprite(string SpriteId, params Color[] MaskColors);

public record struct Rectangle(Vector2f Size, Color FillColor, Color OutlineColor, float OutlineThickness);

public record struct SpriteSheet(int X, int Y, int SpriteSize);

public record struct Renderer(Layer Layer, Drawable? Drawable = null, bool FlipX = false, bool FlipY = false);
public record struct SpriteAnimator(AnimationData AnimationData, int SpritesLeft, int FramesLeft, int LoopsLeft);

public record struct SpriteAnimation(string Animation = "idle", int FramesPerSprite = 6, bool Loop = true);

public record struct Sonic(SonicState State, bool IsOnGround, float SpinRef, Facing Facing, int BoredCount, Vector2f Origin, int WidthRadius, int HeightRadius);

public record struct Sensors(Vector2f UpperLeft, float UpperLeftDistance,
                             Vector2f UpperRight, float UpperRightDistance,
                             Vector2f LowerLeft, float LowerLeftDistance,
                             Vector2f LowerRight, float LowerRightDistance,
                             Vector2f HorizontalLeft, float HorizontalLeftDistance,
                             Vector2f HorizontalRight, float HorizontalRightDistance);

public record struct SolidTiles(int[,] TileMap, Tile[] TileSet);

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

public enum Layer
{
  Debug,
  UI,
  ForegroundTiles,
  Characters,
  BackgroundTiles,
  Objects,
  Background
}
