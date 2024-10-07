using System;
using SFML.Graphics;
using SFML.System;

namespace SonicRemake.Components;

public record struct Transform(Vector2f Position, Vector2f Scale, float Rotation);
public record struct Velocity(float X, float Y);
public record struct Sprite(string SpriteId);
public record struct Renderer(Texture? Texture);
