using Arch.Core;
using SFML.Graphics;
using SonicRemake.Layout;
using SonicRemake.Layout.Engine;

namespace SonicRemake.Systems.Rendering.Debugging;

public class FpsDebugSystem : GameSystem
{
	private float[] _deltaTimeBuffer = new float[25];
	private int _deltaTimeBufferIndex = 0;
	private float _smoothDeltaTime;

	public override void OnRender(World world, RenderWindow window, GameContext context)
	{
		UI.Open(new Node("fps debug")
			.Size(200, 50)
			.Position(Position.Absolute)
			.Flow(Flow.Vertical)
			.Padding(10)
		);

		Tails.Text($"{MathF.Round(1f / _smoothDeltaTime)}fps, {MathF.Round(_smoothDeltaTime * 100, 2)}ms");

		UI.Close();
	}

	public override void OnTick(World world, GameContext context)
	{
		// Smooth delta time
		_deltaTimeBuffer[_deltaTimeBufferIndex] = context.DeltaTime;

		_deltaTimeBufferIndex = (_deltaTimeBufferIndex + 1) % _deltaTimeBuffer.Length;
		_smoothDeltaTime = _deltaTimeBuffer.Average();
	}
}
