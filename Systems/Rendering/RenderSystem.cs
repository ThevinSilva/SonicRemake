using Arch.Core;
using Arch.Core.Extensions;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering
{
	public class RenderSystem(float scale) : GameSystem
	{
		private static Log _log = new(typeof(RenderSystem));

		private readonly float _scale = scale;
		private static QueryDescription Query = new QueryDescription().WithAll<Renderer, Transform>();

		public override void OnRender(World world, RenderWindow window, GameContext context)
		{
			world.Query(in Query, (Entity entity, ref Renderer renderer, ref Transform transform) =>
			{
				// If the entity has a SpriteSheet component, calculate the texture rect mask
				Vector2f origin;

				var textureRect = new IntRect();
				if (entity.Has<SpriteSheet>())
				{
					ref var spriteSheet = ref entity.Get<SpriteSheet>();
					textureRect = CalculateSpriteInSheet(spriteSheet, renderer.FlipX, renderer.FlipY);
					origin = new Vector2f(spriteSheet.SpriteSize / 2, spriteSheet.SpriteSize / 2);
				}
				else
				{
					origin = new Vector2f(renderer.Texture.Size.X / 2, renderer.Texture.Size.Y / 2);
				}

				_log.Debug($"Position: {transform.Position}, Scale: {transform.Scale}, Rotation: {transform.Rotation}");

				var position = transform.Position;
				position.X *= _scale;
				position.Y *= _scale;

				position.X += window.Size.X / 2;
				position.Y += window.Size.Y / 2;



				var scale = transform.Scale * _scale;



				// Draw the sprite
				window.Draw(new SFML.Graphics.Sprite
				{
					Texture = renderer.Texture,
					Position = position,
					Scale = scale,
					Rotation = transform.Rotation,
					TextureRect = textureRect,
					Origin = origin
				});
			});
		}

		private static IntRect CalculateSpriteInSheet(SpriteSheet spriteSheet, bool flipX, bool flipY)
		{
			var xMultiplier = flipX ? -1 : 1;
			var yMultiplier = flipY ? -1 : 1;

			var xAdjustment = flipX ? spriteSheet.SpriteSize : 0;
			var yAdjustment = flipY ? spriteSheet.SpriteSize : 0;

			return new IntRect(spriteSheet.X + xAdjustment, spriteSheet.Y + yAdjustment, spriteSheet.SpriteSize * xMultiplier, spriteSheet.SpriteSize * yMultiplier);
		}
	}
}
