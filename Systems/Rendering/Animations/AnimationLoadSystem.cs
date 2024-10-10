using Arch.Core;
using SFML.Graphics;
using SonicRemake.Animations;
using SonicRemake.Components;
using Range = System.Range;

namespace SonicRemake.Systems.Rendering.Animations
{
	public class AnimationLoadSystem : GameSystem
	{
		private QueryDescription Query = new QueryDescription().WithAll<AnimationQueue, Animator>();

		public override void OnAnimation(World world, RenderWindow window, GameContext context)
		{
			world.Query(in Query, (Entity entity, ref AnimationQueue queue, ref Animator animation) =>
			{
				if (animation.FramesLeft == 0 && animation.SpritesLeft == 0)
				{
					if (queue.Animations.Count > 0)
					{
						var newAnimationName = queue.Animations.Dequeue();
						var newAnimation = AnimationHelper.Animations[newAnimationName];

						animation.AnimationData = newAnimation;
						animation.LoopsLeft = newAnimation.Loops;
						animation.FramesLeft = newAnimation.FramesPerSprite;
						animation.SpritesLeft = newAnimation.NumberOfSprites;
					}
				}
			});
		}
	}
}
