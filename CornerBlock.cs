namespace RubiksCube
{
/// <summary>
/// Угловой блок
/// </summary>
public class CornerBlock
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
	/// Номер третьих граней 0-47
	/// </summary>
	public int FaceNo3;

	/// <summary>
	/// Цвет первых граней 0-5
	/// </summary>
	public int FaceColor1;

	/// <summary>
	///Цвет вторых граней 0-5
	/// </summary>
	public int FaceColor2;

	/// <summary>
	/// Цвет третьих граней 0-5
	/// </summary>
	public int FaceColor3;

	/// <summary>
	/// Конструктор углового блока
	/// </summary>
	/// <param name="FaceNo1">Грань 1</param>
	/// <param name="FaceNo2">Грань 2</param>
	/// <param name="FaceNo3">Грань 3</param>
	public CornerBlock
			(
			int FaceNo1,
			int FaceNo2,
			int FaceNo3
			)
		{
		this.FaceNo1 = FaceNo1;
		this.FaceNo2 = FaceNo2;
		this.FaceNo3 = FaceNo3;
		FaceColor1 = FaceNo1 / Cube.FaceNoToColor;
		FaceColor2 = FaceNo2 / Cube.FaceNoToColor;
		FaceColor3 = FaceNo3 / Cube.FaceNoToColor;
		return;
		}
	}
}
