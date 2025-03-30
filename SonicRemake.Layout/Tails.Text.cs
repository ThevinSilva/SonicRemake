using SFML.Graphics;
using SonicRemake.Layout.Engine;
using SonicRemake.Layout.Helpers;

namespace SonicRemake.Layout;

public static partial class Tails
{
    public static void Text(string content)
    {
        content = content.ToUpper();

        var textWrapper = new Node(content);

        foreach (char character in content)
        {
            var c = character.ToString();
            (var width, var height) = TextTextureHelper.GetCharacterSize(c);
            var texture = TextTextureHelper.GetCharacterTexture(c);
            var charNode = new Node(c.ToString())
                .Size(width, height)
                .Background(texture);
            textWrapper.Children(charNode);
        }

        UI.Open(textWrapper);
        UI.Close();
    }
}
