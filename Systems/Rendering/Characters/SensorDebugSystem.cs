using System;
using System.Drawing;
using Arch.Core;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Components;
using SonicRemake.Inputs;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering.Characters;

public class SensorDebug : GameSystem
{
	private static Log _log = new(typeof(SonicAnimationSystem));

	private QueryDescription Query = new QueryDescription().WithAll<Sonic, Transform, SpriteSheet>();

	public override void OnRender(World world, RenderWindow window, GameContext context)
	{
		world.Query(in Query, (
		Entity entity,
		ref Sonic sonic,
		ref Transform transform,
		ref SpriteSheet sheet
		) =>
		{
			var origin = transform.Position;
			var widthRadius = sheet.SpriteSize / 2f;
			var heightRadius = sheet.SpriteSize / 2f;

			_log.Debug(origin);
			_log.Debug(widthRadius);


			window.Draw(new RectangleShape()
			{
				FillColor = new SFML.Graphics.Color(255, 0, 0),
				Size = new Vector2f(widthRadius, 1),
				Position = origin,
			});

		});
	}
}
