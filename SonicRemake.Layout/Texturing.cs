using SFML.Graphics;
using SonicRemake.Common;

namespace SonicRemake.Layout;

public abstract record Texturing;
public record ColorTexturing(Color Color) : Texturing;
public record SpriteTexturing(TextureHandle TextureHandle) : Texturing;
