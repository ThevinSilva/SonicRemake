using Arch.Core;
using Arch.Core.Extensions;
using SFML.Graphics;
using SonicRemake.Components;
using Sprite = SonicRemake.Components.Sprite;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems;

public class RenderSystem : GameSystem
{
	private static QueryDescription Query = new QueryDescription().WithAll<Renderer, Sprite, Transform>();

	public override void OnRender(World world, RenderWindow window, GameContext context)
	{
		world.Query(in Query, (Entity entity, ref Renderer renderer, ref Sprite sprite, ref Transform transform) =>
		{
			// Load the sprite texture if it hasn't been loaded yet
			renderer.Texture ??= new Texture($"Assets/Sprites/{sprite.SpriteId}");

			// If the entity has a SpriteSheet component, calculate the texture rect mask
			var textureRect = new IntRect();
			if (entity.Has<SpriteSheet>())
			{
				ref var spriteSheet = ref entity.Get<SpriteSheet>();
				textureRect = CalculateSpriteInSheet(spriteSheet.SpriteIndex, spriteSheet.SpriteWidth, spriteSheet.SpritesPerRow);
				spriteSheet.SpriteIndex = 0;
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

	/// <summary>
	/// Calculate the texture rect mask for a sprite in a sprite sheet, 
	/// given the sprite's index, width and amount of sprites per row in the sheet.
	/// </summary>
	private static IntRect CalculateSpriteInSheet(int spriteIndex, int spriteWidth, int spritesPerRow)
	{
		var x = spriteIndex % spritesPerRow;
		var y = spriteIndex / spritesPerRow;

		return new IntRect(x * spriteWidth, y * spriteWidth, spriteWidth, spriteWidth);
	}

}
