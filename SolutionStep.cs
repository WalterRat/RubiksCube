namespace RubiksCube
{
/// <summary>
/// Результат шага решения
/// </summary>
public class SolutionStep
	{
	/// <summary>
	/// Solution step code
	/// </summary>
	public StepCode StepCode;

	/// <summary>
	/// Текстовое сообщение, связанное с этим шагом
	/// </summary>
	public string Message;

	/// <summary>
	/// Номер грани, на котором сосредоточено внимание
	/// </summary>
	public int FaceNo;

	/// <summary>
	/// цвет верхней грани
	/// </summary>
	public int UpFaceColor;

	/// <summary>
	///  цвет лицевой грани
	/// </summary>
	public int FrontFaceColor;

	/// <summary>
	/// Шаги решения, выраженные в цвете граней
	/// </summary>
	public int[] Steps;

	/// <summary>
	/// Конуструктор
	/// </summary>
	/// <param name="StepCode">Код шага решения</param>
	/// <param name="Message">Текстовое сообщение</param>
	/// <param name="FaceNo">Перемещаемый номер граней</param>
	/// <param name="UpFaceColor">Вверх куба по цвету граней</param>
	/// <param name="FrontFaceColor">Цвет лицевой стороны куба</param>
	/// <param name="Steps">Шаги решения</param>
	public SolutionStep
			(
			StepCode StepCode,
			string Message,
			int FaceNo,
			int UpFaceColor,
			int FrontFaceColor,
			int[] Steps
			)
		{

		this.StepCode = StepCode;
		this.Message = Message;
		this.FaceNo = FaceNo;
		this.UpFaceColor = UpFaceColor;
		this.FrontFaceColor = FrontFaceColor;
		this.Steps = Steps;
		return;
		}

	/// <summary>
	/// Конструктор для куба решен
	/// </summary>
	public SolutionStep()
		{
		StepCode = StepCode.CubeIsSolved;
		return;
		}
	}
}
