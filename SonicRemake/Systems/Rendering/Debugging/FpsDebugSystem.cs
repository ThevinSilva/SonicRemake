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
		var wrapper = new Div("fps wrapper")
				.Padding(20)
				.Position(Position.Absolute)
				.Background(new Color(0, 0, 0, 150))
				.Gap(20);

		var text = new Layout.Engine.Text("fps counter")
				   .Content($"{MathF.Round(1f / _smoothDeltaTime)}fps Δ{MathF.Round(_smoothDeltaTime * 100, 2)}ms")
				   .Size(Size.Grow)
				   .Foreground(Color.Yellow);

		wrapper.Children(text);

		UI.Open(wrapper);
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
