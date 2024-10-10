using System;
using Arch.Core;
using SFML.Graphics;

namespace SonicRemake.Systems.Rendering.Debugging
{
	public class FpsDebugSystem : GameSystem
	{
		private Font _monocraft = new("Assets/Fonts/Monocraft.ttf");
		private float[] _deltaTimeBuffer = new float[25];
		private int _deltaTimeBufferIndex = 0;
		private float _smoothDeltaTime;

		public FpsDebugSystem()
		{
			// Disable anti-aliasing for Monocraft
			_monocraft.SetSmooth(false);
		}

		public override void OnRender(World world, RenderWindow window, GameContext context)
		{
			window.Draw(new Text($"{MathF.Round(1f / _smoothDeltaTime)}fps Δ{MathF.Round(_smoothDeltaTime * 100, 2)}ms", _monocraft, 20)
			{
				FillColor = Color.Yellow,
				Position = new(0, 0),
			});
		}

		public override void OnPhysics(World world, GameContext context)
		{
			// Smooth delta time
			_deltaTimeBuffer[_deltaTimeBufferIndex] = context.DeltaTime;

			_deltaTimeBufferIndex = (_deltaTimeBufferIndex + 1) % _deltaTimeBuffer.Length;
			_smoothDeltaTime = _deltaTimeBuffer.Average();
		}

	}
}
