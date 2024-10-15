using Arch.Core;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SFML.System;
using SonicRemake.Components;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering.Characters
{
	public class SensorDebug : GameSystem
	{
		private static readonly Log _log = new(typeof(SensorDebug));

		private readonly QueryDescription _sonicQuery = new QueryDescription()
			.WithAll<Sonic, Transform, SpriteSheet>();

		private readonly QueryDescription _cameraQuery = new QueryDescription()
			.WithAll<Components.Camera, Transform>();

		public override void OnRender(World world, RenderWindow window, GameContext context)
		{
			world.Query(in _sonicQuery, (Entity entity, ref Sonic sonic, ref Transform transform, ref SpriteSheet sheet) =>
			{
				UpdateSonicSensorData(ref sonic, transform.Position, sheet.SpriteSize);
				DrawSonicSensors(world, window, sonic);
			});
		}

		private void UpdateSonicSensorData(ref Sonic sonic, Vector2f origin, float spriteSize)
		{
			sonic.Origin = new Vector2f(origin.X, origin.Y + 4);

			if (sonic.State == SonicState.Jumping || sonic.State == SonicState.SpinRoll)
			{
				sonic.WidthRadius = 7;
				sonic.HeightRadius = 14;
			}
			else
			{
				sonic.WidthRadius = 9;
				sonic.HeightRadius = 19;
			}
		}

		private void DrawSonicSensors(World world, RenderWindow window, Sonic sonic)
		{
			var scale = new Vector2f(1, 1) * GetCameraZoom(world);

			// Draw left vertical sensor
			sonic.Origin = sonic.Origin;
			window.Draw(CreateRectangleShape(CameraFix(world, window, sonic.Origin), new Vector2f(1, 1), new Color(255, 255, 255), scale));

			// // Draw left vertical sensor
			var verticalLeftSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y);
			window.Draw(CreateRectangleShape(CameraFix(world, window, verticalLeftSensorPosition), new Vector2f(sonic.WidthRadius, 1), new Color(255, 0, 255), scale));

			// Draw right vertical sensor
			var verticalRightSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius + 1, sonic.Origin.Y);
			window.Draw(CreateRectangleShape(CameraFix(world, window, verticalRightSensorPosition), new Vector2f(-sonic.WidthRadius, 1), new Color(255, 0, 0), scale));

			var upperRightSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y + sonic.HeightRadius);
			window.Draw(CreateRectangleShape(CameraFix(world, window, upperRightSensorPosition), new Vector2f(1, -sonic.HeightRadius - 10), new Color(0, 255, 0), scale));

			var upperLeftSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius, upperRightSensorPosition.Y);
			window.Draw(CreateRectangleShape(CameraFix(world, window, upperLeftSensorPosition), new Vector2f(1, -sonic.HeightRadius), new Color(0, 255, 255), scale));


			var lowerRightSensorPosition = new Vector2f(upperRightSensorPosition.X, sonic.Origin.Y - sonic.HeightRadius);
			window.Draw(CreateRectangleShape(CameraFix(world, window, lowerRightSensorPosition), new Vector2f(1, sonic.HeightRadius), new Color(0, 0, 255), scale));

			var lowerLeftSensorPosition = new Vector2f(upperLeftSensorPosition.X, lowerRightSensorPosition.Y);
			window.Draw(CreateRectangleShape(CameraFix(world, window, lowerLeftSensorPosition), new Vector2f(1, sonic.HeightRadius), new Color(255, 255, 0), scale));

		}

		private RectangleShape CreateRectangleShape(Vector2f position, Vector2f size, Color color, Vector2f scale)
		{
			return new RectangleShape
			{
				FillColor = color,
				Size = size,
				Position = position,
				Scale = scale
			};
		}

		private float GetCameraZoom(World world)
		{
			float cameraZoom = 1f;

			world.Query(in _cameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
			{
				cameraZoom = camera.Zoom; // Update camera zoom from the query
			});

			return cameraZoom;
		}

		public Vector2f CameraFix(World world, RenderWindow window, Vector2f vector)
		{
			var cameraPosition = new Vector2f();
			var cameraZoom = GetCameraZoom(world);

			world.Query(in _cameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
			{
				cameraPosition = transform.Position;
			});

			// Adjust vector based on camera position and zoom
			vector.X = (vector.X - cameraPosition.X) * cameraZoom + window.Size.X / 2;
			vector.Y = (vector.Y - cameraPosition.Y) * cameraZoom + window.Size.Y / 2;

			return vector;
		}
	}
}
