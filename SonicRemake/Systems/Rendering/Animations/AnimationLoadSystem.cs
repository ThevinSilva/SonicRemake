using Arch.Core;
using SonicRemake.Animations;
using SonicRemake.Components;

namespace SonicRemake.Systems.Rendering.Animations;

public class AnimationLoadSystem : GameSystem
{
	private QueryDescription Query = new QueryDescription().WithAll<SpriteAnimation, SpriteAnimator>();

	public override void OnTick(World world, GameContext context)
	{
		world.Query(in Query, (Entity entity, ref SpriteAnimation queue, ref SpriteAnimator animation) =>
		{
			// Check if a new animation was requested
			if (queue.Animation != animation.AnimationData.Name)
			{
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
