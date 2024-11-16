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

	public override void OnRender(World world, SFML.Graphics.RenderWindow window, GameContext context)
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

			sensors.HorizontalRight = CalculateSensorData(sonic, s => new Vector2f(s.Origin.X + s.WidthRadius, s.Origin.Y), map, Dimension.Right);
			sensors.HorizontalLeft = CalculateSensorData(sonic, s => new Vector2f(s.Origin.X - s.WidthRadius, s.Origin.Y), map, Dimension.Left);
			sensors.UpperRight = CalculateSensorData(sonic, s => new Vector2f(s.Origin.X + s.WidthRadius, s.Origin.Y - s.HeightRadius), map, Dimension.Up);
			sensors.UpperLeft = CalculateSensorData(sonic, s => new Vector2f(s.Origin.X - s.WidthRadius, s.Origin.Y - s.HeightRadius), map, Dimension.Up);
			sensors.LowerRight = CalculateSensorData(sonic, s => new Vector2f(s.Origin.X + s.WidthRadius, s.Origin.Y + s.HeightRadius), map, Dimension.Down);
			sensors.LowerLeft = CalculateSensorData(sonic, s => new Vector2f(s.Origin.X - s.WidthRadius, s.Origin.Y + s.HeightRadius), map, Dimension.Down);
		});
	}

	// Something that retrives tile data height array and width array
	public Tile GetTile(int id, Tile[] tileset) => tileset[id];



	private static SensorData CalculateSensorData(Sonic sonic, Func<Sonic, Vector2f> positionFunc, SolidTiles map, Dimension dimension)
	{
		var position = positionFunc(sonic);
		var detectedTile = FindTileIndex(position, map.TileMap, dimension);

		return CalculateSensorData(sonic, position, detectedTile, map, dimension);
	}

	private static SensorData CalculateSensorData(Sonic sonic, Vector2f position, Vector2i? detectedTile, SolidTiles map, Dimension dimension)
	{
		Vector2f? intersection = null;

		if (detectedTile.HasValue)
		{
			Tile tile = map.TileSet[map.TileMap[detectedTile.Value.Y, detectedTile.Value.X]];
			var tileX = detectedTile.Value.X * 16;
			var tileY = detectedTile.Value.Y * 16;
			int offsetX;
			int offsetY;
			int index;
			byte[] offsets;

			switch (dimension)
			{
				case Dimension.Right:
					index = Math.Clamp((int)Math.Floor(position.Y - tileY + 8) - 1, 0, 15);
					offsets = tile.Matrix.GetRow(index).ToArray();

					if (detectedTile.HasValue && offsets.All(x => x == 0) && Math.Abs(position.X - tileX) < 8 * 16)
						return CalculateSensorData(sonic, position, detectedTile + new Vector2i(1, 0), map, dimension);

					offsetX = offsets.TakeWhile(x => x == 0).Count();
					intersection = new Vector2f(detectedTile.Value.X * 16 + offsetX - 8, position.Y);

					_log.Value("index", index);
					_log.Value("offsets", string.Join(",", offsets));
					_log.Value("offsetX", offsetX);
					_log.Value("intersection", intersection);
					break;
				case Dimension.Left:
					index = Math.Clamp((int)Math.Floor(position.Y - tileY + 8) - 1, 0, 15);
					offsets = tile.Matrix.GetRow(index).ToArray().Reverse().ToArray();
					offsetX = offsets.TakeWhile(x => x == 0).Count();
					intersection = new Vector2f(detectedTile.Value.X * 16 + 8 - offsetX, position.Y);
					break;

				case Dimension.Up:
					index = Math.Clamp((int)Math.Floor(position.X - tileX + 8), 0, 15);
					offsets = tile.Matrix.GetColumn(index).ToArray().Reverse().ToArray();

					offsetY = offsets.TakeWhile(x => x == 0).Count();
					intersection = new Vector2f(position.X, detectedTile.Value.Y * 16 + offsetY + 8);
					break;

				case Dimension.Down:
					index = Math.Clamp((int)Math.Floor(position.X - tileX + 8), 0, 15);
					offsets = tile.Matrix.GetColumn(index).ToArray();

					if (offsets.All(x => x == 1) && Math.Abs(position.X - tileX) < 8)
						return CalculateSensorData(sonic, position, detectedTile + new Vector2i(0, -1), map, dimension);

					offsetY = offsets.TakeWhile(x => x == 0).Count();
					intersection = new Vector2f(position.X, detectedTile.Value.Y * 16 - 8 + offsetY);
					break;
			}
		}

		float distance = float.MaxValue;

		if (intersection.HasValue)
		{
			distance = MathF.Sqrt(MathF.Pow(position.X - intersection.Value.X, 2) + MathF.Pow(position.Y - intersection.Value.Y, 2));
		}

		return new SensorData
		{
			Position = position,
			DetectedTile = detectedTile,
			Intersection = intersection,
			Distance = distance
		};
	}

	private static Vector2i? FindTileIndex(Vector2f sensor, int[,] map, Dimension dim)
	{
		int xPos = (int)Math.Round(sensor.X / 16);
		int yPos = (int)Math.Round(sensor.Y / 16);

		if (xPos < 0 || yPos < 0 || xPos >= map.GetLength(1) || yPos >= map.GetLength(0)) return null;

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

		return new(x, y);
	}


}
