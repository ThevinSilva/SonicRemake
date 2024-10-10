using Arch.Core;
using Arch.Core.Extensions;
using SFML.Graphics;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems
{
	public class RenderSystem : GameSystem
	{
		private static QueryDescription Query = new QueryDescription().WithAll<Renderer, Transform>();

		public override void OnRender(World world, RenderWindow window, GameContext context)
		{
			world.Query(in Query, (Entity entity, ref Renderer renderer, ref Transform transform) =>
			{
				// If the entity has a SpriteSheet component, calculate the texture rect mask
				var textureRect = new IntRect();
				if (entity.Has<SpriteSheet>())
				{
					ref var spriteSheet = ref entity.Get<SpriteSheet>();
					textureRect = CalculateSpriteInSheet(spriteSheet);
				}

				// Draw the sprite
				window.Draw(new SFML.Graphics.Sprite
				{
					Texture = renderer.Texture,
					Position = transform.Position,
					Scale = transform.Scale,
					Rotation = transform.Rotation,
					TextureRect = textureRect
				});
			});
		}

		private static IntRect CalculateSpriteInSheet(SpriteSheet spriteSheet)
		{
			return new IntRect(spriteSheet.X, spriteSheet.Y, spriteSheet.SpriteSize, spriteSheet.SpriteSize);
		}

	}
}
