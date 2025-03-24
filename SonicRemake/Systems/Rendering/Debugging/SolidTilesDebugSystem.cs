using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;

namespace SonicRemake.Systems.Rendering.Textures
{
	public class SolidTilesDebugSystem : GameSystem
	{
		private static QueryDescription Query = new QueryDescription().WithAll<Renderer, SpriteTexture>();

		public override void OnRender(World world, RenderWindow window, GameContext context)
		{
			world.Query(in Query, (Entity entity, ref Renderer renderer, ref SpriteTexture texture) =>
			{
				renderer.Drawable ??= new SFML.Graphics.Sprite() { Texture = texture.Texture };
			});
		}
	}
}
