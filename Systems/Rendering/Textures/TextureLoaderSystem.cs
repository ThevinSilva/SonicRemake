using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;

namespace SonicRemake.Systems.Rendering.Textures
{
  public class TextureLoaderSystem : GameSystem
  {
    private static QueryDescription Query = new QueryDescription().WithAll<Renderer, Sprite>();

    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
      world.Query(in Query, (Entity entity, ref Renderer renderer, ref Sprite sprite) =>
      {
        if (renderer.Texture == null)
        {
          var image = new Image($"Assets/Sprites/{sprite.SpriteId}");

          foreach (var color in sprite.MaskColors)
            image.CreateMaskFromColor(color);

          renderer.Texture = new Texture(image);
        }
      });
    }
  }
}
