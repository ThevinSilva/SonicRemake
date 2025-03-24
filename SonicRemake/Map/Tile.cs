
using SFML.Graphics;

namespace SonicRemake.Maps;

// Solid Tiles Implementation https://info.sonicretro.org/SPG:Solid_Tiles#Height_Array

public struct Tile
{
	private byte[] _heights;
	private byte[] _widths;
	public byte[,] Matrix { get; set; }

	public float? Angle
	{ get; set; }

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

	public readonly Color[,] GetColorMatrix()
	{
		Color[,] mtrx = new Color[16, 16];

		for (int x = 0; x < 16; x++)
		{
			for (int y = 0; y < 16; y++)
			{
				if (Matrix[y, x] == 1) mtrx[x, y] = Color.Magenta;
				else mtrx[x, y] = Color.Transparent;
			}
		}

		return mtrx;
	}

	public override string ToString()
	{
		return $" Angle - {Angle} \n Heights {string.Join(",", Heights)} \n Widths {string.Join(",", Widths)} ";
	}
}
