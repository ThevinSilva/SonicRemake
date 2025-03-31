using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;
using SonicRemake.Layout;
using SonicRemake.Layout.Engine;

namespace SonicRemake.Systems.Rendering.Debugging;


public class SpeedometerDebugSystem : GameSystem
{
	private QueryDescription Query = new QueryDescription().WithAll<Sonic, Velocity>();

	public override void OnRender(World world, RenderWindow window, GameContext context)
	{
		UI.Open(new Node("speed debug")
			 .Flow(Flow.Vertical)
			 .Size(Size.Grow)
			 .Position(Position.Absolute)
			 .Align(Align.End, Align.Start)
			 .Padding(10)
		 );

		world.Query(in Query, (Entity entity, ref Sonic sonic, ref Velocity velocity) =>
	   {
		   // Maginitude of the velocity vector
		   var speed = velocity.Speed.X * velocity.Speed.X + velocity.Speed.Y * velocity.Speed.Y;
		   speed = MathF.Sqrt(speed);
		   speed = MathF.Round(speed, 2);

		   Tails.Text($"{speed.ToString("0.00")}m/s");
	   });


		UI.Close();
	}
}
