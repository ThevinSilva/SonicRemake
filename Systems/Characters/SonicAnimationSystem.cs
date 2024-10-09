using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;
using Range = System.Range;

namespace SonicRemake.Systems.Characters;

public class SonicAnimationSystem : GameSystem
{
	private QueryDescription Query = new QueryDescription().WithAll<Sonic, SpriteSheet>();

	private readonly Queue<(Range frameRange, int frameDuration, bool loopIfLast)> _animationQueue = new();
	private (Range frameRange, int currentFrameIndex, bool loopIfLast, int frameDuration, int framesLeft)? _currentAnimation = null;

	// Impatience, Sonic will start tapping his foot if he's not moving
	private const int _impatienceThreshold = 180;
	private int _impatienceCounter = 0;

	public override void OnAnimation(World world, RenderWindow window, GameContext context)
	{
		// Dequeue the next animation if the current one is done
		if (_currentAnimation is null && _animationQueue.TryDequeue(out var requestedAnimation))
			_currentAnimation = (requestedAnimation.frameRange,
				requestedAnimation.frameRange.Start.Value,
				requestedAnimation.loopIfLast,
				requestedAnimation.frameDuration,
				requestedAnimation.frameDuration
			);

		world.Query(in Query, (Entity entity, ref Sonic sonic, ref SpriteSheet spriteSheet) =>
		{
			// Impatience
			_impatienceCounter++;

			if (_impatienceCounter == _impatienceThreshold)
			{
				_animationQueue.Enqueue((5..5, 6, false));   // Blink
				_animationQueue.Enqueue((6..6, 30, false));  // Eyes wide open
				_animationQueue.Enqueue((7..8, 18, false));  // Tap foot
				_animationQueue.Enqueue((7..8, 18, false));  // Tap foot
				_animationQueue.Enqueue((7..8, 18, false));  // Tap foot
				_animationQueue.Enqueue((9..9, 60, false));  // Look at watch
				_animationQueue.Enqueue((7..8, 18, false));  // Tap foot
				_animationQueue.Enqueue((7..8, 18, false));  // Tap foot
				_animationQueue.Enqueue((7..8, 18, false));  // Tap foot
				_animationQueue.Enqueue((9..10, 3, false));  // Lay down on floor
				_animationQueue.Enqueue((11..12, 18, true)); // Tap shoe
			}

			// Set the current frame
			if (_currentAnimation != null)
			{
				spriteSheet.SpriteIndex = _currentAnimation?.currentFrameIndex ?? 0;

				var (frameRange, currentFrameIndex, loopIfLast, frameDuration, framesLeft) = _currentAnimation!.Value;

				// There are still frames left in the current animation
				if (framesLeft > 0)
				{
					framesLeft--;
					_currentAnimation = (frameRange, currentFrameIndex, loopIfLast, frameDuration, framesLeft);
				}
				else
				{
					// The animation is not done, move to the next frame
					if (frameRange.End.Value != currentFrameIndex)
					{
						currentFrameIndex++;
						framesLeft = frameDuration;
						_currentAnimation = (frameRange, currentFrameIndex, loopIfLast, frameDuration, framesLeft);
					}
					// The animation is done and it's a loop, start over
					else if (loopIfLast && _animationQueue.Count == 0)
					{
						currentFrameIndex = frameRange.Start.Value;
						framesLeft = frameDuration;
						_currentAnimation = (frameRange, currentFrameIndex, loopIfLast, frameDuration, framesLeft);
					}
					// The animation is done
					else
						_currentAnimation = null;
				}
			}
		});
	}
}
