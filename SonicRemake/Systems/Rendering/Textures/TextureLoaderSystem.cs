using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;
using SonicRemake.Textures;
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
        renderer.Drawable ??= new SFML.Graphics.Sprite() { Texture = SonicRemake.Textures.Texture.FromSprite(sprite.SpriteId, sprite.MaskColors) };
      });
    }
  }
}
