using System;
using Arch.Core;
using Newtonsoft.Json;

namespace SonicRemake.Systems.Rendering;

public class LevelTransitionSystem : GameSystem
{
  private TransitionState _transitionState = TransitionState.FadedIn;

  private float _transitionTime = 0.0f;
  private float _transitionDuration = 1.0f;
  private float _transitionAlpha = 0.0f;

  public override void OnTick(World world, GameContext context)
  {
    if (_transitionState == TransitionState.FadingOut)
    {
      _transitionTime += context.DeltaTime;
      _transitionAlpha = Math.Min(_transitionTime / _transitionDuration, 1.0f);

      if (_transitionTime >= _transitionDuration)
        _transitionState = TransitionState.FadedOut;
    }

    if (_transitionState == TransitionState.FadingIn)
    {
      _transitionTime += context.DeltaTime;
      _transitionAlpha = 1.0f - Math.Min(_transitionTime / _transitionDuration, 1.0f);

      if (_transitionTime >= _transitionDuration)
        _transitionState = TransitionState.FadedIn;
    }

    if (_transitionState == TransitionState.FadedOut || _transitionState == TransitionState.FadedIn)
      _transitionTime = 0.0f;


  }

  enum TransitionState
  {
    FadingOut,
    FadedOut,
    FadingIn,
    FadedIn,
  }
}
