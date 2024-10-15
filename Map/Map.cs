using System.Numerics;
using System.Runtime.InteropServices;
using Arch.Core;
using CommunityToolkit.HighPerformance;
using Newtonsoft.Json;
using SFML.System;
using SonicRemake.Components;
using SonicRemake.Systems;

namespace SonicRemake.Maps;

// Solid Tiles Implementation https://info.sonicretro.org/SPG:Solid_Tiles#Height_Array
public struct Tile
{
	private byte[] _heights;
	private byte[] _widths;

	public float? Angle { get; set; }
	public byte[] Heights
	{
		get => _heights;
		set
		{
			Validation(value);
			_heights = value;
		}
	}

	public byte[] Widths
	{
		get => _widths;
		set
		{
			Validation(value);
			_widths = value;
		}
	}

	public Tile(byte[] heights, byte[] widths, ushort angle)
	{
		Heights = heights;
		Widths = widths;
		Angle = angle;
	}

	public static void Validation(byte[] value)
	{
		if (value.Length != 16)
			throw new ArgumentOutOfRangeException("Needs to be size 16");

		if (value.Max() > 16)
			throw new ArgumentException("Values must be less than 16");
	}

	public override string ToString()
	{
		return $" Angle - {Angle} \n Heights {string.Join(",", Heights)} \n Widths {string.Join(",", Widths)} ";
	}
}

public class TileManagementSystem : GameSystem
{
	private static Log _log = new Log(typeof(TileManagementSystem));
	private QueryDescription Query = new QueryDescription().WithAll<SolidTiles>();

	private Tile[] TileSet { get; set; }
	private int[,] TileMap { get; set; }

	public override void OnStart(World world)
	{

		world.Query(in Query, (ref SolidTiles map) =>
		{
			TileSet = LoadTileSet("./Assets/Map/Tiles.json");
			TileMap = LoadTileMap("./Assets/Map/TestStage.csv");
			map.TileMap = TileMap;
			map.TileSet = TileSet;
			CreateDrawableEntities(world);
		});
	}

	private static Tile[] LoadTileSet(string location)
	{
		string text = File.ReadAllText(location);

		Tile[]? tileset = JsonConvert.DeserializeObject<Tile[]>(text);

		if (tileset == null)
			throw new ArgumentNullException();

		_log.Information($"TileSet of size {tileset.Length} loaded in.");

		return tileset;
	}

	private static int[,] LoadTileMap(string location)
	{
		string[] rows = File.ReadAllLines(location);
		int[,] tileMap = new int[rows.Length, rows[0].Split(",").Length]; // please don't judge :3

		for (int i = 0; i < rows.Length; i++)
		{
			string[] row = rows[i].Split(",");
			for (int j = 0; j < row.Length; j++)
			{
				int idx = int.Parse(row[j]);
				if (idx > 0) tileMap[i, j] = idx;
			}
		}

		_log.Information($"Tile Map of size {rows.Length} x {rows[0].Length} loaded in.");

		return tileMap;
	}


	public void CreateDrawableEntities(World world)
	{
		for (int y = 0; y < TileMap.GetLength(0); y++)
		{
			for (int x = 0; x < TileMap.GetLength(1); x++)
			{

				var pos = new Vector2f(x * 16, y * 16);
				var scale = new Vector2f(1, 1);
				var idx = TileMap[y, x];

				if (idx <= 0) continue;

				world.Create(
					new Transform(pos, scale),
					new Renderer(),
					new Sprite(
						$"BaseTileSet/block_{idx}.png",
						new SFML.Graphics.Color(0, 0, 0)
					)
				);
			}
		}
	}


}
