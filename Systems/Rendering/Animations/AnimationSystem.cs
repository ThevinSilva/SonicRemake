using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;

namespace SonicRemake.Systems.Rendering.Animations
{
  public class AnimationSystem : GameSystem
  {
    private QueryDescription Query = new QueryDescription().WithAll<Animator, SpriteSheet>();

    public override void OnAnimation(World world, RenderWindow window, GameContext context)
    {
      world.Query(in Query, (Entity entity, ref Animator animation, ref SpriteSheet spriteSheet) =>
      {
        if (animation.FramesLeft > 0)
        {
          animation.FramesLeft--;
        }

        if (animation.FramesLeft == 0 && animation.SpritesLeft > 0)
        {
          animation.SpritesLeft--;
          animation.FramesLeft = animation.AnimationData.FramesPerSprite;
        }

        if (animation.SpritesLeft == 0 && animation.LoopsLeft > 0)
        {
          animation.LoopsLeft--;
          animation.SpritesLeft = animation.AnimationData.NumberOfSprites;
        }

        spriteSheet.X = animation.AnimationData.StartFrameX + spriteSheet.SpriteSize * (animation.AnimationData.NumberOfSprites - animation.SpritesLeft) + (animation.AnimationData.NumberOfSprites - animation.SpritesLeft);
        spriteSheet.Y = animation.AnimationData.StartFrameY;
      });
    }
  }
}
