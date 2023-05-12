using System.Windows.Media.Media3D;

namespace RubiksCube
{
/// <summary>
/// Блок класса кубика рубика
/// </summary>
public class Block3D : ModelVisual3D
	{
	/// <summary>
	/// Массив из 6 граней сторон
	/// </summary>
	public BlockFace3D[] BlockFaceArray;

	/// <summary>
	/// Позиция блока X
	/// </summary>
	public double OrigX;

	/// <summary>
	/// Позиция блока Y
	/// </summary>
	public double OrigY;

	/// <summary>
	/// Позиция блока Z
	/// </summary>
	public double OrigZ;

	/// <summary>
	/// Конструктор блока
	/// </summary>
	/// <param name="BlockNo">Блок цифр (0 до 26)</param>
	public Block3D
			(
			int BlockNo
			)
		{
		// Для скрытого центрального блока больше нет инициализации
		if(BlockNo == 13) return;

		// Происхождение блока
		OrigX = -Cube3D.HalfCubeWidth + (BlockNo % 3) * (Cube3D.BlockWidth + Cube3D.BlockSpacing);
		OrigY = -Cube3D.HalfCubeWidth + ((BlockNo / 3) % 3) * (Cube3D.BlockWidth + Cube3D.BlockSpacing);
		OrigZ = -Cube3D.HalfCubeWidth + (BlockNo / 9) * (Cube3D.BlockWidth + Cube3D.BlockSpacing);

		// Массив из 6 граней этого блока
		BlockFaceArray = new BlockFace3D[6];

		// Цикл для всех 6 граней 
		for(int FaceColor = 0; FaceColor < Cube.FaceColors; FaceColor++)
			{
			// Создать 6 граней для каждого блока
			// Перевести номер блока и номер граней к грани
			BlockFaceArray[FaceColor] = new BlockFace3D(this, Cube.BlockFace[BlockNo, FaceColor], FaceColor);
			}
		return;
		}
	}
}
