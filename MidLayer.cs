using System;

namespace RubiksCube
{
/// <summary>
/// Класс краев среднего слоя
/// </summary>
public class MidLayer
	{
	/// <summary>
	/// Переместите край со среднего слоя на желтую сторону
	/// </summary>
	public bool MoveToYellow;

	/// <summary>
	/// Дополнительное желтое вращение
	/// </summary>
	public int Rotation;

	/// <summary>
	/// Поворот желтой стороны
	/// </summary>
	public int YellowRotation;

	private int FaceNo;
	private int FacePos;
	private int FrontFace;
	private StepCtrl StepCtrl;
	private int[] Steps;

	/// <summary>
	/// Create mid layer object
	/// </summary>
	/// <param name="FaceArray">Массив граней</param>
	/// <param name="FaceNo">Номера граней</param>
	/// <returns>Объект среднего слоя</returns>
	public static MidLayer Create
			(
			int[] FaceArray,
			int FaceNo
			)
		{
		// угол находится в нужном положении
		return FaceArray[FaceNo] == FaceNo ? null : new MidLayer(FaceArray, FaceNo);
		}

	/// <summary>
	/// приватный конструктор среднего уровня
	/// </summary>
	/// <param name="FaceArray">Массив граней</param>
	/// <param name="FaceNo">Номера граней</param>
	private MidLayer
			(
			int[] FaceArray,
			int FaceNo
			)
		{
		// сохранить номер граней и положение
		// 11 = Синий, 19 = Красный, 27 = Зеленый, 35 = Оранжевый
		this.FaceNo = FaceNo;
		FacePos = Cube.FindEdge(FaceArray, FaceNo);

		// край среднего слоя находится в среднем слое, но в неправильном положении или ориентации
		if(Cube.FaceNoToBlockNo[FacePos] < 18)
			{
			MoveToYellow = true;
			return;
			}

		// цвета граней нет
		int FaceNoColor = FaceNo / Cube.FaceNoToColor;

		// используйте левый алгоритм
		if(FacePos / Cube.FaceNoToColor == Cube.YellowFace)
			{
			// лицевая сторона находится на одну сторону справа
			FrontFace = (FaceNoColor % 4) + 1;

			// контрль шага
			StepCtrl = Cube.MidLayerLeft;

			// получить шаги
			Steps = StepCtrl.Steps(FrontFace - 1);

			// лицо ядовито-желтое лицо
			//переводится в положение лица на лицевой стороне
			//и преобразуется в цветовой код
			Rotation = (33 - 4 * (FacePos - 41)) / 8 - FrontFace + 4;
			}

		// используйте правый алгоритм
		else
			{
			// лицевая сторона
			FrontFace = FaceNoColor;

			// контрль шага
			StepCtrl = Cube.MidLayerRight;

			// получить шаги
			Steps = StepCtrl.Steps(FaceNoColor - 1);

			// требуемая ротация			
			Rotation = FacePos / 8 - FaceNoColor + 4;
			}

		// первым шагом в алгоритме среднего слоя является поворот желтого цвета
		//если потребуется дополнительное желтое вращение, мы объединим его с первым шагом
		YellowRotation = (Steps[0] - Cube.YellowCW + 1 + Rotation) % 4;
		return;
		}

	/// <summary>
	/// Создать состояние решения case 1
	/// </summary>
	/// <param name="Message">Сообщение</param>
	/// <returns>Шаг решения</returns>
	public SolutionStep CreateSolutionStep1
			(
			string Message
			)
		{
		// удалить начальный шаг
		int Len = this.Steps.Length - 1;
		int[] TempSteps = new int[Len];
		Array.Copy(Steps, 1, TempSteps, 0, Len);

		// вернуть с шагом решения
		return new SolutionStep(StepCode.MidLayer, Message, FaceNo, Cube.YellowFace, FrontFace, TempSteps);
		}

	/// <summary>
	/// Создать состояние решения case 2
	/// </summary>
	/// <param name="Message">Сообщение</param>
	/// <returns>Шаг решения</returns>
	public SolutionStep CreateSolutionStep2
			(
			string Message
			)
		{
		// вернуть с шагом решения
		return new SolutionStep(StepCode.MidLayer, Message, FaceNo, Cube.YellowFace, FrontFace, Steps);
		}

	/// <summary>
	/// Создать состояние решения case 3
	/// </summary>
	/// <param name="Message">Сообщение</param>
	/// <returns>Шаг решения</returns>
	public SolutionStep CreateSolutionStep3
			(
			string Message
			)
		{
		// отрегулируйте первый шаг
		int Len = Steps.Length;
		int[] TempSteps = new int[Len];
		Array.Copy(Steps, 0, TempSteps, 0, Len);
		TempSteps[0] = Cube.YellowCW + YellowRotation - 1;

		// вернуть с шагом решения
		return new SolutionStep(StepCode.MidLayer, Message, FaceNo, Cube.YellowFace, FrontFace, TempSteps);
		}

	/// <summary>
	/// Создать состояние решения case 4
	/// </summary>
	/// <param name="Message">Сообщение</param>
	/// <returns>Шаг решения</returns>
	public SolutionStep CreateSolutionStep4
			(
			string Message
			)
		{
		//	текущее положение лица находится в среднем слое, но не в нужном месте.
		//	нам нужно вынести его на желтый слой
		//	мы сделаем правильный алгоритм
		switch(FacePos)
			{
			case 11:
			case 23:
				FrontFace = 1;
				break;

			case 19:
			case 31:
				FrontFace = 2;
				break;

			case 27:
			case 39:
				FrontFace = 3;
				break;

			case 35:
			case 15:
				FrontFace = 4;
				break;
	
			default:
				throw new ApplicationException("Mid layer FacePos");
			}

		// получите правильный контроль шага среднего слоя
		StepCtrl = Cube.MidLayerRight;

		// get steps
		Steps = StepCtrl.Steps(FrontFace - 1);

		// удалить начальный шаг
		int Len = Steps.Length - 1;
		int[] TempSteps = new int[Len];
		Array.Copy(Steps, 1, TempSteps, 0, Len);

		// вернуть с шагом решения
		return new SolutionStep(StepCode.MidLayer, Message, FacePos, Cube.YellowFace, FrontFace, TempSteps);
		}
	}
}
