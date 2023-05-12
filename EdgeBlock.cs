namespace RubiksCube
{
/// <summary>
/// блок рёбер
/// </summary>
public class EdgeBlock
	{
	/// <summary>
	/// Номер первых граней 0-47
	/// </summary>
	public int FaceNo1;

	/// <summary>
	/// Номер вторых граней 0-47
	/// </summary>
	public int FaceNo2;

	/// <summary>
	/// Первый цвет граней 0-5
	/// </summary>
	public int FaceColor1;

	/// <summary>
	/// Второй цвет граней 0-5
	/// </summary>
	public int FaceColor2;

	/// <summary>
	/// Конструктор блоков рёбер
	/// </summary>
	/// <param name="FaceNo1">грань 1</param>
	/// <param name="FaceNo2">грань 2</param>
	public EdgeBlock
			(
			int FaceNo1,
			int FaceNo2
			)
		{
		this.FaceNo1 = FaceNo1;
		this.FaceNo2 = FaceNo2;
		FaceColor1 = FaceNo1 / Cube.FaceNoToColor;
		FaceColor2 = FaceNo2 / Cube.FaceNoToColor;
		return;
		}
	}
}
