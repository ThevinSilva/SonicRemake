using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace SonicRemake.Common;

public static class TextureHelper
{
    private static readonly Log _log = new(typeof(TextureHelper));

    private static readonly Dictionary<int, Texture> _textures = [];
    private static readonly Dictionary<string, int> _sources = [];

    public static TextureHandle CreateHandle(string sprite, params Color[] maskColors)
    {
        if (_sources.TryGetValue(sprite, out int id))
            return new TextureHandle(id);

        var image = new Image($"Assets/Sprites/{sprite}");
        foreach (var color in maskColors)
            image.CreateMaskFromColor(color);

        var texture = new Texture(image);

        _log.Debug($"Loaded texture {sprite} with size {texture.Size.X}x{texture.Size.Y}");

        var handle = CreateHandle(texture);

        _sources.Add(sprite, handle.Id);

        return handle;
    }

    public static Texture FromHandle(TextureHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle);

        if (_textures.TryGetValue(handle.Id, out Texture? t))
            return t;

        throw new Exception($"Texture with handle {handle.Id} not found.");
    }

    public static TextureHandle CreateHandle(TextureHandle spriteSheet, int x, int y, int width, int height)
    {
        var texture = FromHandle(spriteSheet);

        var image = texture.CopyToImage();
        image.Copy(image, 0, 0, new IntRect(x, y, width, height));
        var croppedTexture = new Texture(image, new IntRect(0, 0, width, height));

        return CreateHandle(croppedTexture);
    }

    public static TextureHandle CreateHandle(Texture texture)
    {
        var handle = new TextureHandle(_textures.Count + 1);
        _textures.Add(handle.Id, texture);

        return handle;
    }
}

public record TextureHandle
{
    internal int Id { get; init; }

    internal TextureHandle(int id)
    {
        Id = id;
    }
}
