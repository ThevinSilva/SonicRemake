using Arch.Core;
using SFML.Graphics;
using SonicRemake.Animations;
using SonicRemake.Components;
using Range = System.Range;

namespace SonicRemake.Systems.Rendering.Animations
{
	public class AnimationLoadSystem : GameSystem
	{
		private static Log _log = new(typeof(AnimationLoadSystem));

		private QueryDescription Query = new QueryDescription().WithAll<SpriteAnimation, SpriteAnimator>();

		public override void OnAnimation(World world, RenderWindow window, GameContext context)
		{
			world.Query(in Query, (Entity entity, ref SpriteAnimation queue, ref SpriteAnimator animation) =>
			{
				// Check if a new animation was requested
				if (queue.Animation != animation.AnimationData.Name)
				{
					_log.Information($"Changing animation from {animation.AnimationData.Name} to {queue.Animation}");

					var newAnimation = AnimationHelper.Animations[queue.Animation];
					animation.AnimationData = newAnimation;
					animation.LoopsLeft = newAnimation.Loops;
					animation.FramesLeft = 6;
					animation.SpritesLeft = newAnimation.NumberOfSprites;
				}

				if (queue.FramesPerSprite != animation.AnimationData.FramesPerSprite)
				{
					_log.Information($"Changing frames per sprite from {animation.AnimationData.FramesPerSprite} to {queue.FramesPerSprite}");
					animation.AnimationData = animation.AnimationData with { FramesPerSprite = queue.FramesPerSprite };
					animation.FramesLeft = queue.FramesPerSprite;
				}
			});
		}
	}
}
