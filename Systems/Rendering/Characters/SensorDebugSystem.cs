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
	private QueryDescription CameraQuery = new QueryDescription().WithAll<Components.Camera, Transform>();

	public override void OnRender(World world, RenderWindow window, GameContext context)
	{
		var cameraPosition = new Vector2f();
		var cameraZoom = 1f;

		world.Query(in CameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
		{
			cameraPosition = transform.Position;
			cameraZoom = camera.Zoom;
		});

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

			// FIXME: This functionality is repeated in the RenderSystem, it
			//        should probably be extracted to a helper function somewhere
			origin.X -= cameraPosition.X;
			origin.Y -= cameraPosition.Y;

			origin.X *= cameraZoom;
			origin.Y *= cameraZoom;

			origin.X += window.Size.X / 2;
			origin.Y += window.Size.Y / 2;

			var scale = new Vector2f(1, 1) * cameraZoom;

			window.Draw(new RectangleShape()
			{
				FillColor = new SFML.Graphics.Color(255, 0, 0),
				Size = new Vector2f(widthRadius, 1),
				Position = origin,
				Scale = scale,
			});

		});
	}
}
