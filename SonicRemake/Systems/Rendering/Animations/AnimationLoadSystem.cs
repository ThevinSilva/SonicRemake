using Arch.Core;
using SonicRemake.Animations;
using SonicRemake.Components;

namespace SonicRemake.Systems.Rendering.Animations;

public class AnimationLoadSystem : GameSystem
{
	private QueryDescription Query = new QueryDescription().WithAll<SpriteAnimation, SpriteAnimator, AnimationSequence>();

	public override void OnTick(World world, GameContext context)
	{
		world.Query(in Query, (Entity entity, ref SpriteAnimation queue, ref SpriteAnimator animation, ref AnimationSequence sequence) =>
		{
			// Check if a new animation was requested
			if (queue.Animation != animation.AnimationData.Name)
			{
				// Clean up Animation sequence
				if (!sequence.Names.Contains(queue.Animation))
				{
					sequence.Names = [];
					sequence.CurrentTime = 0;
				}

				// if another animation was queued when a sequence was going on
				var newAnimation = AnimationHelper.Animations[queue.Animation];
				animation.AnimationData = newAnimation;
				animation.LoopsLeft = newAnimation.Loops;
				animation.FramesLeft = queue.FramesPerSprite;
				animation.SpritesLeft = newAnimation.NumberOfSprites;
			}

			if (queue.FramesPerSprite != animation.AnimationData.FramesPerSprite)
			{
				animation.AnimationData = animation.AnimationData with { FramesPerSprite = queue.FramesPerSprite };
				animation.FramesLeft = queue.FramesPerSprite;
			}
		});
	}
}
