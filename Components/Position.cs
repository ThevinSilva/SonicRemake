using System;

namespace SonicRemake.Components;

public record struct Position(float X, float Y);
public record struct Velocity(float X, float Y);
public record struct Sprite(string SpriteId);
