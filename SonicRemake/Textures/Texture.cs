using SFML.Graphics;

namespace SonicRemake.Textures;

public static class Texture
{
    private static readonly Log _log = new(typeof(Texture));
    private static readonly Dictionary<string, SFML.Graphics.Texture> _textures = [];

    public static SFML.Graphics.Texture FromSprite(string sprite, params Color[] maskColors)
    {
        if (_textures.TryGetValue(sprite, out SFML.Graphics.Texture? t))
            return t;

        var image = new Image($"Assets/Sprites/{sprite}");
        foreach (var color in maskColors)
            image.CreateMaskFromColor(color);

        var texture = new SFML.Graphics.Texture(image);
        _textures.Add(sprite, texture);

        _log.Information($"Loaded texture {sprite}");

        return texture;
    }


}
