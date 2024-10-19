using System.ComponentModel;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Arch.Core;
using CommunityToolkit.HighPerformance;
using SFML.System;
using SonicRemake.Components;
using SonicRemake.Maps;

namespace SonicRemake.Systems.Sensors;

enum Dimension { Left, Right, Up, Down };

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




	private static (int x, int y) FindTileIndex(Vector2f sensor, int[,] map, Dimension dim)
	{
		int xPos = (int)(sensor.X / 16);
		int yPos = (int)(sensor.Y / 16);

		var slice = (dim == Dimension.Left || dim == Dimension.Right
			? map.GetRow(yPos)
			: map.GetColumn(xPos)).ToArray();

		int x = xPos, y = yPos; // Initialize x and y with their respective positions

		/*
		Filter - 
			- index -> End : Right + Down
			- start -> index : Left 
			- index -> start : (inverted) Up 
			"this is helpful right lucas ?" -thevin
		*/

		switch (dim)
		{
			case Dimension.Right:
				x = Array.FindIndex(slice[xPos..], val => val > 0) + xPos;
				break;
			case Dimension.Down:
				y = Array.FindIndex(slice[yPos..], val => val > 0) + yPos;
				break;
			case Dimension.Left:
				x = Array.FindIndex(slice[..xPos], val => val > 0);
				break;
			case Dimension.Up:
				y = Array.FindIndex(slice.Reverse().ToArray()[yPos..], val => val > 0) + yPos;
				break;
		}

		x = Math.Min(x, xPos + 8);
		y = Math.Min(y, yPos + 8);

		return (x, y); // Return a ValueTuple for better readability
	}


}
