using System.Windows.Media.Media3D;

namespace RubiksCube
{
/// <summary>
/// Класс-оболочка для сохранения граней блока
/// </summary>
public class ModelVisual3DCube : ModelVisual3D
	{
	/// <summary>
	/// блок граней
	/// </summary>
	public BlockFace3D BlockFace;

	/// <summary>
	/// конструктор
	/// </summary>
	/// <param name="BlockFace">блок граней</param>
	/// <param name="GeometryModel">геометрическая модель</param>
	public ModelVisual3DCube
			(
			BlockFace3D BlockFace,
			GeometryModel3D GeometryModel
			)
		{
		this.BlockFace = BlockFace;
		Content = GeometryModel;
		return;
		}
	}
}
