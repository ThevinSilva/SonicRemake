using System.Numerics;
using Arch.Core;
using CommunityToolkit.HighPerformance;
using SFML.System;
using SonicRemake.Components;
using SonicRemake.Maps;

namespace SonicRemake.Systems.Sensors;

public class SensorSystem : GameSystem
{
	private static Log _log = new(typeof(SensorSystem));

	private QueryDescription Query = new QueryDescription().WithAll<Sonic, Transform, SpriteSheet, SolidTiles>();

	public override void OnTick(World world, GameContext context)
	{
		world.Query(in Query, (
		Entity entity,
		ref Sonic sonic,
		ref Transform transform,
		ref SolidTiles map
		) =>
		{

			sonic.Origin = new Vector2f(transform.Position.X - 3, transform.Position.Y + 4);

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


			// // Draw left vertical sensor
			var verticalLeftSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y);

			// Draw right vertical sensor
			var verticalRightSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius + 1, sonic.Origin.Y);

			var upperRightSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y + sonic.HeightRadius);

			var upperLeftSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius, upperRightSensorPosition.Y);

			var lowerRightSensorPosition = new Vector2f(upperRightSensorPosition.X, sonic.Origin.Y - sonic.HeightRadius);

			var lowerLeftSensorPosition = new Vector2f(upperLeftSensorPosition.X, lowerRightSensorPosition.Y);

			_log.Debug(FindRightVerticalTile(map.TileMap, map.TileSet, verticalRightSensorPosition));

		});
	}

	private static Vector2f GetIndex(Vector2f sensor) => new((int)Math.Floor(sensor.X / 16), (int)Math.Floor(sensor.Y / 16));


	public Tile FindRightVerticalTile(int[,] map, Tile[] set, Vector2f sensor)
		=> set[map.GetRow((int)GetIndex(sensor).Y).ToArray()[(int)GetIndex(sensor).X..].FirstOrDefault(val => val > 0)];

	public Tile FindLeftVerticalTile(int[,] map, Tile[] set, Vector2f sensor)
	=> set[map.GetRow((int)GetIndex(sensor).Y).ToArray()[..(int)GetIndex(sensor).X].FirstOrDefault(val => val > 0)];

}
