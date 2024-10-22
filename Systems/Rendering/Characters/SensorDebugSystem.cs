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

		private const float _tileThickness = 1;
		private const float _lineThickness = 1;

		private Color _lowerLeftColor = new(0, 240, 0, 150);
		private Color _lowerRightColor = new(50, 255, 162, 150);
		private Color _upperLeftColor = new(0, 174, 239, 150);
		private Color _upperRightColor = new(255, 242, 56, 150);
		private Color _horizontalLeftColor = new(255, 56, 255, 150);
		private Color _horizontalRightColor = new(255, 84, 84, 150);

		private Entity _lowerLeftLine, _lowerLeftTile;
		private Entity _lowerRightLine, _lowerRightTile;
		private Entity _upperLeftLine, _upperLeftTile;
		private Entity _upperRightLine, _upperRightTile;
		private Entity _horizontalLeftLine, _horizontalLeftTile;
		private Entity _horizontalRightLine, _horizontalRightTile;

		public override void OnStart(World world)
		{
			_lowerLeftLine = world.Create(
				new Transform(),
				new Line(new(0, 0), new(0, 0), _lowerLeftColor, _lineThickness),
				new Renderer(Layer.Debug));
			_lowerLeftTile = world.Create(
				new Transform(),
				new Rectangle(new(16, 16), new(0), _lowerLeftColor, _tileThickness),
				new Renderer(Layer.Debug));

			_lowerRightLine = world.Create(
				new Transform(),
				new Line(new(0, 0), new(0, 0), _lowerRightColor, _lineThickness),
				new Renderer(Layer.Debug));
			_lowerRightTile = world.Create(
				new Transform(),
				new Rectangle(new(16, 16), new(0), _lowerRightColor, _tileThickness),
				new Renderer(Layer.Debug));

			_upperLeftLine = world.Create(
				new Transform(),
				new Line(new(0, 0), new(0, 0), _upperLeftColor, _lineThickness),
				new Renderer(Layer.Debug));
			_upperLeftTile = world.Create(
				new Transform(),
				new Rectangle(new(16, 16), new(0), _upperLeftColor, _tileThickness),
				new Renderer(Layer.Debug));

			_upperRightLine = world.Create(
				new Transform(),
				new Line(new(0, 0), new(0, 0), _upperRightColor, _lineThickness),
				new Renderer(Layer.Debug));
			_upperRightTile = world.Create(
				new Transform(),
				new Rectangle(new(16, 16), new(0), _upperRightColor, _tileThickness),
				new Renderer(Layer.Debug));

			_horizontalLeftLine = world.Create(
				new Transform(),
				new Line(new(0, 0), new(0, 0), _horizontalLeftColor, _lineThickness),
				new Renderer(Layer.Debug));
			_horizontalLeftTile = world.Create(
				new Transform(),
				new Rectangle(new(16, 16), new(0), _horizontalLeftColor, _tileThickness),
				new Renderer(Layer.Debug));

			_horizontalRightLine = world.Create(
				new Transform(),
				new Line(new(0, 0), new(0, 0), _horizontalRightColor, _lineThickness),
				new Renderer(Layer.Debug));
			_horizontalRightTile = world.Create(
				new Transform(),
				new Rectangle(new(16, 16), new(0), _horizontalRightColor, _tileThickness),
				new Renderer(Layer.Debug));
		}

		public override void OnRender(World world, RenderWindow window, GameContext context)
		{
			world.Query(in _sonicQuery, (Entity entity, ref Sonic sonic, ref Transform transform, ref SpriteSheet sheet, ref Components.Sensors sensors) =>
			{
				static void updateLine(ref Line line, SensorData sensor)
				{
					line.Start = sensor.Position;
					line.End = sensor.Intersection ?? sensor.Position;
				}

				updateLine(ref world.Get<Line>(_horizontalLeftLine), sensors.HorizontalLeft);
				updateLine(ref world.Get<Line>(_horizontalRightLine), sensors.HorizontalRight);
				updateLine(ref world.Get<Line>(_upperLeftLine), sensors.UpperLeft);
				updateLine(ref world.Get<Line>(_upperRightLine), sensors.UpperRight);
				updateLine(ref world.Get<Line>(_lowerLeftLine), sensors.LowerLeft);
				updateLine(ref world.Get<Line>(_lowerRightLine), sensors.LowerRight);

				static void updateRectangle(ref Transform transform, SensorData sensor)
				{
					if (sensor.DetectedTile.HasValue)
					{
						transform.Position = new(sensor.DetectedTile.Value.X * 16, sensor.DetectedTile.Value.Y * 16);
						transform.Scale = new(1, 1);
					}
					else
						transform.Scale = new(0, 0);
				}

				updateRectangle(ref world.Get<Transform>(_horizontalLeftTile), sensors.HorizontalLeft);
				updateRectangle(ref world.Get<Transform>(_horizontalRightTile), sensors.HorizontalRight);
				updateRectangle(ref world.Get<Transform>(_upperLeftTile), sensors.UpperLeft);
				updateRectangle(ref world.Get<Transform>(_upperRightTile), sensors.UpperRight);
				updateRectangle(ref world.Get<Transform>(_lowerLeftTile), sensors.LowerLeft);
				updateRectangle(ref world.Get<Transform>(_lowerRightTile), sensors.LowerRight);
			});
		}
	}
}
