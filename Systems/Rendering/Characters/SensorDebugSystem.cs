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
	}
}
