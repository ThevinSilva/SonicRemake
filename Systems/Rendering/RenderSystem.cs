using Arch.Core;
using Arch.Core.Extensions;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using Transform = SonicRemake.Components.Transform;

// TODO - correct the shift for Tiles

namespace SonicRemake.Systems.Rendering
{
	public class RenderSystem : GameSystem
	{
		private static Log _log = new(typeof(RenderSystem));
		private static QueryDescription Query = new QueryDescription().WithAll<Renderer, Transform>();

		private static QueryDescription CameraQuery = new QueryDescription().WithAll<Components.Camera, Transform>();

		public override void OnRender(World world, RenderWindow window, GameContext context)
		{
			var cameraPosition = new Vector2f();
			var cameraZoom = 1f;

			world.Query(in CameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
			{
				cameraPosition = transform.Position;
				cameraZoom = camera.Zoom;
			});

			world.Query(in Query, (Entity entity, ref Renderer renderer, ref Transform transform) =>
			{
				var origin = new Vector2f(0, 0);

				var textureRect = new IntRect();
				if (entity.Has<SpriteSheet>())
				{
					ref var spriteSheet = ref entity.Get<SpriteSheet>();
					textureRect = CalculateSpriteInSheet(spriteSheet, renderer.FlipX, renderer.FlipY);
					origin = new Vector2f(spriteSheet.SpriteSize / 2, spriteSheet.SpriteSize / 2);
				}
				else if (renderer.Drawable != null && renderer.Drawable is SFML.Graphics.Sprite sprite)
				{
					origin = new Vector2f(sprite.Texture.Size.X / 2, sprite.Texture.Size.Y / 2);
					textureRect = new IntRect(0, 0, (int)sprite.Texture.Size.X, (int)sprite.Texture.Size.Y);
				}
				else if (renderer.Drawable != null && renderer.Drawable is SFML.Graphics.RectangleShape rectangle)
				{
					origin = new Vector2f(rectangle.Size.X / 2, rectangle.Size.Y / 2);
					textureRect = new IntRect(0, 0, (int)rectangle.Size.X, (int)rectangle.Size.Y);
				}

				var position = transform.Position;
				position.X -= cameraPosition.X;
				position.Y -= cameraPosition.Y;

				position.X *= cameraZoom;
				position.Y *= cameraZoom;

				position.X += window.Size.X / 2;
				position.Y += window.Size.Y / 2;

				var scale = transform.Scale * cameraZoom;

				if (scale == new Vector2f(0, 0))
					_log.Warning("Scale is 0, 0, object will not be visible");

				// Draw the sprite
				if (renderer.Drawable is SFML.Graphics.Sprite s)
				{
					s.Origin = origin;
					s.Position = position;
					s.Scale = scale;
					s.TextureRect = textureRect;
					s.Rotation = transform.Rotation;
					window.Draw(s);
				}

				// Draw the rectangle
				// FIXME: Something about the original rectangle makes it not render correctly
				///       Temporarily replaced with a new RectangleShape
				else if (renderer.Drawable is SFML.Graphics.RectangleShape r)
				{
					r.Origin = origin;
					r.Position = position;
					r.Scale = scale;
					r.TextureRect = textureRect;
					r.Rotation = transform.Rotation;
					window.Draw(r);
					// window.Draw(new SFML.Graphics.RectangleShape()
					// {
					// 	Size = r.Size,
					// 	FillColor = r.FillColor,
					// 	OutlineColor = r.OutlineColor,
					// 	OutlineThickness = r.OutlineThickness,
					// 	Position = position,
					// 	Origin = origin,
					// 	Scale = scale,
					// });
				}
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
