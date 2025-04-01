using Arch.Core;
using SonicRemake.Animations;
using SonicRemake.Components;

namespace SonicRemake.Systems.Rendering.Animations
{
    class AnimationSequenceSystem : GameSystem
    {
        private static readonly Log _log = new(typeof(AnimationSequenceSystem));
        private QueryDescription Query = new QueryDescription().WithAll<AnimationSequence, SpriteAnimation>();

        public override void OnTick(World world, GameContext context)
        {
            world.Query(in Query, (Entity entity, ref AnimationSequence sequence, ref SpriteAnimation queue) =>
            {
                var current = sequence.CurrentTime;

                if (sequence.Names.Length == 0)
                    return;

                int[] timeStamps = [0, ..sequence.Names
                .Select(name =>
                    AnimationHelper.Animations[name].NumberOfSprites
                    * AnimationHelper.Animations[name].FramesPerSprite
          * AnimationHelper.Animations[name].Loops)
                .Take(sequence.Names.Length -1)
                ];

                if (timeStamps.Contains(current))
                {
                    queue.Animation = sequence.Names[Array.FindIndex(timeStamps, x => x == current)];
                    queue.Loop = sequence.Loop;
                }

                sequence.CurrentTime++;
            });
        }
    }
}
