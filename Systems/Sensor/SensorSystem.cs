using System.ComponentModel;
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

	private QueryDescription Query = new QueryDescription().WithAll<Sonic, Transform, SpriteSheet, SolidTiles, Components.Sensors>();

	public override void OnTick(World world, GameContext context)
	{
		world.Query(in Query, (
		Entity entity,
		ref Sonic sonic,
		ref Transform transform,
		ref SolidTiles map,
		ref Components.Sensors sensors
		) =>
		{
			sonic.Origin = new Vector2f(transform.Position.X - 3, transform.Position.Y + 4);

			if (sonic.State == SonicState.Jumping || sonic.State == SonicState.SpinRoll)
			{
				sonic.WidthRadius = 7;
				sonic.HeightRadius = 14;
			}
			else if (sonic.State == SonicState.Crouching || sonic.State == SonicState.Charging)	
			{
				sonic.WidthRadius = 9;
				sonic.HeightRadius = 12;
				sonic.Origin = new Vector2f(sonic.Origin.X, transform.Position.Y + 11);
			}
			else
			{
				sonic.WidthRadius = 9;
				sonic.HeightRadius = 19;
			}

			sensors.HorizontalRight = new Vector2f(sonic.Origin.X + sonic.WidthRadius, sonic.Origin.Y);
			sensors.HorizontalLeft = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y);
			sensors.UpperRight = new Vector2f(sonic.Origin.X + sonic.WidthRadius, sonic.Origin.Y + sonic.HeightRadius);
			sensors.UpperLeft = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y + sonic.HeightRadius);
			sensors.LowerRight = new Vector2f(sonic.Origin.X + sonic.WidthRadius, sonic.Origin.Y - sonic.HeightRadius);
			sensors.LowerLeft = new Vector2f(sonic.Origin.X - sonic.WidthRadius, sonic.Origin.Y - sonic.HeightRadius);

			if (sensors.HorizontalRight.X >= 0)
				_log.Debug(FindTileIndex(sensors.HorizontalRight, map.TileMap, Dimension.Right));

		});
	}

	private static Vector2i GetIndex(Vector2f sensor) => new((int)Math.Floor(sensor.X / 16), (int)Math.Floor(sensor.Y / 16));




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
