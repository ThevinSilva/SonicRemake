using System.Collections.Generic;
using SFML.Graphics;
using SonicRemake.Common;

namespace SonicRemake.Layout.Helpers;

internal static class TextTextureHelper
{
    private static readonly Log log = new(typeof(TextTextureHelper));
    private static readonly TextureHandle _font = TextureHelper.CreateHandle("font.png", new Color(38, 123, 218));
    private static readonly Dictionary<string, TextureHandle> _fontMap = [];

    internal const int FONT_WIDTH = 8;
    internal const int FONT_HEIGHT = 11;

    private const int FONT_START_X = 1;
    private const int FONT_START_Y = 42;

    private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789?:'\".!-,";

    private static readonly Dictionary<string, int> CUSTOM_CHAR_WIDTHS = new()
    {
        { "W", 9 },
        { ":", 5 },
        { "'", 3 },
        { "\"", 6 },
        { ".", 5 },
        { "!", 5 },
        { ",", 3 }
    };

    static TextTextureHelper()
    {
        var x = FONT_START_X;
        var y = FONT_START_Y;
        foreach (var character in CHARS)
        {
            var c = character.ToString();
            (var width, var height) = GetCharacterSize(c);

            var charHandle = TextureHelper.CreateHandle(_font, x, y, width, height);
            _fontMap.Add(c, charHandle);

            // There's a gap between characters
            x += width + 1;
        }

        // Add unknown character
        (var unknownWidth, var unknownHeight) = GetCharacterSize("UNKNOWN");
        _fontMap.Add("UNKNOWN", TextureHelper.CreateHandle(_font, 13, 84, unknownWidth, unknownHeight));

        // Add space
        _fontMap.Add(" ", TextureHelper.CreateHandle(new Texture(FONT_WIDTH, FONT_HEIGHT)));
    }

    public static TextureHandle GetCharacterTexture(string c)
    {
        if (_fontMap.TryGetValue(c, out var charHandle))
            return charHandle;

        log.Warning($"Font texture for character '{c}' not found.");

        return _fontMap["UNKNOWN"];
    }

    public static (int Width, int Height) GetCharacterSize(string character)
    {
        if (CUSTOM_CHAR_WIDTHS.TryGetValue(character, out var customWidth))
            return (customWidth, FONT_HEIGHT);

        return (FONT_WIDTH, FONT_HEIGHT);
    }
}
