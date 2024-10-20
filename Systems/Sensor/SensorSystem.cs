using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Arch.Core;
using CommunityToolkit.HighPerformance;
using SFML.System;
using SonicRemake.Components;
using SonicRemake.Maps;

namespace SonicRemake.Systems.Sensors;

enum Dimension
{
	Left = -1, Right = 1, Up, Down
};

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
			var horizontalLeftSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y);

			// Draw right vertical sensor
			var horizontalRightSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius, sonic.Origin.Y);

			var upperRightSensorPosition = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y - sonic.HeightRadius);

			var upperLeftSensorPosition = new Vector2f(sonic.Origin.X + sonic.WidthRadius, upperRightSensorPosition.Y);

			var lowerRightSensorPosition = new Vector2f(upperRightSensorPosition.X, sonic.Origin.Y + sonic.HeightRadius);

			var lowerLeftSensorPosition = new Vector2f(upperLeftSensorPosition.X, lowerRightSensorPosition.Y);

			// calculate distance to the right tile 

			var index = FindTileIndex(horizontalRightSensorPosition, map.TileMap, Dimension.Right);



			if (index.HasValue)
			{
				// _log.Information(index.Value.x, index.Value.y);
				float distance = index.Value.x * 16 - MathF.Truncate(horizontalRightSensorPosition.X);
				_log.Information(distance);
			}

			// _log.Information(map.TileMap[y, x]);

		});
	}

	private static (int x, int y)? FindTileIndex(Vector2f sensor, int[,] map, Dimension dim)
	{
		int xPos = (int)((sensor.X + ((int)dim)) * 0.5 / 16);
		int yPos = (int)(sensor.Y / 16);

		var slice = (dim == Dimension.Left || dim == Dimension.Right
			? map.GetRow(yPos)
			: map.GetColumn(xPos)).ToArray();

		int x = xPos, y = yPos;

		/*
		Filter - 
			- index -> End : Right + Down
			- index -> start (inverted): Left Up  
			"this is helpful right lucas ?" -thevin
		*/

		int index;
		switch (dim)
		{
			case Dimension.Right:
				index = Array.FindIndex(slice[xPos..], val => val > 0);
				if (index == -1) return null;
				x = index + xPos;
				break;
			case Dimension.Down:
				index = Array.FindIndex(slice[yPos..], val => val > 0);
				if (index == -1) return null;
				y = index + yPos;
				break;
			case Dimension.Left:
				index = Array.FindIndex(slice[..xPos].Reverse().ToArray(), val => val > 0);
				if (index == -1) return null;
				x = xPos - index - 1;
				break;
			case Dimension.Up:
				index = Array.FindIndex(slice[..yPos].Reverse().ToArray(), val => val > 0);
				if (index == -1) return null;
				y = yPos - index - 1;
				break;
		}

		// validation
		// if (Math.Abs(xPos - x) > 8 || x - xPos < 0) return null;
		// return null if either the array.Find index above returned -1 or if the range between the sensor and the tile is above 8
		if (Math.Abs(xPos - x) > 8) return null;
		if (Math.Abs(yPos - y) > 8) return null;

		return (x, y);
	}


}
