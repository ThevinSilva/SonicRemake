using Arch.Core;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SFML.System;
using SonicRemake.Components;
using Transform = SonicRemake.Components.Transform;
using System.ComponentModel;
using Arch.Core.Extensions;

namespace SonicRemake.Systems.Rendering.Characters
{
	public class SensorDebug : GameSystem
	{
		private static readonly Log _log = new(typeof(SensorDebug));

		private readonly QueryDescription _sonicQuery = new QueryDescription()
			.WithAll<Sonic, Transform, SpriteSheet, Components.Sensors>();

		private Entity _lowerLeftRect;
		private Entity _lowerRightRect;
		private Entity _upperLeftRect;
		private Entity _upperRightRect;
		private Entity _horizontalLeftRect;
		private Entity _horizontalRightRect;

		public override void OnStart(World world)
		{
			_lowerLeftRect = world.Create(
				new Transform(),
				new Rectangle(new(1, 1), new(0, 240, 0), new(0), 0),
				new Renderer(Layer.Debug));
			_lowerRightRect = world.Create(
				new Transform(),
				new Rectangle(new(1, 1), new Color(50, 255, 162), new(0), 0),
				new Renderer(Layer.Debug));
			_upperLeftRect = world.Create(
				new Transform(),
				new Rectangle(new(1, 1), new Color(0, 174, 239), new(0), 0),
				new Renderer(Layer.Debug));
			_upperRightRect = world.Create(
				new Transform(),
				new Rectangle(new(1, 1), new Color(255, 242, 56), new(0), 0),
				new Renderer(Layer.Debug));
			_horizontalLeftRect = world.Create(
				new Transform(),
				new Rectangle(new(1, 1), new Color(255, 56, 255), new(0), 0),
				new Renderer(Layer.Debug));
			_horizontalRightRect = world.Create(
				new Transform(),
				new Rectangle(new(1, 1), new Color(255, 84, 84), new(0), 0),
				new Renderer(Layer.Debug));
		}

		public override void OnTick(World world, GameContext context)
		{
			world.Query(in _sonicQuery, (Entity entity, ref Sonic sonic, ref Transform transform, ref SpriteSheet sheet, ref Components.Sensors sensors) =>
			{
				world.Get<Transform>(_horizontalLeftRect).Position = sensors.HorizontalLeft;
				world.Get<Transform>(_horizontalRightRect).Position = sensors.HorizontalRight;
				world.Get<Transform>(_upperLeftRect).Position = sensors.UpperLeft;
				world.Get<Transform>(_upperRightRect).Position = sensors.UpperRight;
				world.Get<Transform>(_lowerLeftRect).Position = sensors.LowerLeft;
				world.Get<Transform>(_lowerRightRect).Position = sensors.LowerRight;
			});
		}
<<<<<<< HEAD

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
			var horizontalLeftSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y);

			// Draw right vertical sensor
			var horizontalRightSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius, sonic.Origin.Y);

			var upperRightSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y - sonic.HeightRadius);

			var upperLeftSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius, upperRightSensorPosition.Y);

			var lowerRightSensorPosition = new Vector2f(upperRightSensorPosition.X, sonic.Origin.Y + sonic.HeightRadius);

			var lowerLeftSensorPosition = new Vector2f(upperLeftSensorPosition.X, lowerRightSensorPosition.Y);

			// // Draw left vertical sensor
			window.Draw(CreateRectangleShape(CameraFix(world, window, horizontalLeftSensorPosition), new Vector2f(1, 1), new Color(255, 0, 255), scale));

			// Draw right vertical sensor
			window.Draw(CreateRectangleShape(CameraFix(world, window, horizontalRightSensorPosition), new Vector2f(-1, 1), new Color(255, 0, 0), scale));

			window.Draw(CreateRectangleShape(CameraFix(world, window, upperRightSensorPosition), new Vector2f(1, -1), new Color(0, 255, 0), scale));

			window.Draw(CreateRectangleShape(CameraFix(world, window, upperLeftSensorPosition), new Vector2f(1, -1), new Color(0, 255, 255), scale));


			window.Draw(CreateRectangleShape(CameraFix(world, window, lowerRightSensorPosition), new Vector2f(1, 1), new Color(0, 0, 255), scale));

			window.Draw(CreateRectangleShape(CameraFix(world, window, lowerLeftSensorPosition), new Vector2f(1, 1), new Color(255, 255, 0), scale));

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
=======
>>>>>>> 6a90470ae872f1ddb406c24b32728f7f65881fa2
	}
}
