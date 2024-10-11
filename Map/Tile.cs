namespace SonicRemake.Maps;

// Solid Tiles Implementation https://info.sonicretro.org/SPG:Solid_Tiles#Height_Array
public class Tile
{
	private byte[] _heights;
	private byte[] _widths;
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

	public ushort Angle { get; set; }

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
}
