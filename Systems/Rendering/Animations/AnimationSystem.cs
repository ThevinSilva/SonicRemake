using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;

namespace SonicRemake.Systems.Rendering.Animations
{
  public class AnimationSystem : GameSystem
  {
    private static Log _log = new(typeof(AnimationSystem));

    private QueryDescription Query = new QueryDescription().WithAll<SpriteAnimator, SpriteSheet, SpriteAnimation>();

    public override void OnTick(World world, GameContext context)
    {
      world.Query(in Query, (Entity entity, ref SpriteAnimator animation, ref SpriteSheet spriteSheet, ref SpriteAnimation queue) =>
      {
        // _log.Debug($"Frames left: {animation.FramesLeft}, Sprites left: {animation.SpritesLeft}, Loops left: {animation.LoopsLeft}");

        if (animation.FramesLeft > 0)
        {
          animation.FramesLeft--;
        }

        if (animation.FramesLeft == 0 && animation.SpritesLeft > 0)
        {
          animation.SpritesLeft--;
          animation.FramesLeft = queue.FramesPerSprite;
        }

        if (animation.SpritesLeft == 0 && animation.LoopsLeft == 0)
        {
          if (queue.Loop)
          {
            // Restart the animation
            animation.LoopsLeft = animation.AnimationData.Loops;
            animation.SpritesLeft = animation.AnimationData.NumberOfSprites;
          }
          else
          {
            animation.SpritesLeft = 1;

          }
        }

        // if (animation.SpritesLeft == 0 && animation.LoopsLeft > 0)
        // {
        //   animation.LoopsLeft--;
        //   animation.SpritesLeft = animation.AnimationData.NumberOfSprites;
        // }



        spriteSheet.X = animation.AnimationData.StartFrameX + spriteSheet.SpriteSize * (animation.AnimationData.NumberOfSprites - animation.SpritesLeft) + (animation.AnimationData.NumberOfSprites - animation.SpritesLeft);
        spriteSheet.Y = animation.AnimationData.StartFrameY;
      });
    }
  }
}
