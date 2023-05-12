using System;

namespace RubiksCube
{
/// <summary>
/// Класс белых углов
/// </summary>
public class WhiteCorner
	{
	/// <summary>
	/// Переместите угол с белой грани на жёлтую грань
	/// </summary>
	public bool MoveToYellow;

	/// <summary>
	/// Требуется поворот желтой грани
	/// </summary>
	public int YellowRotation;

	private int FaceNo;
	private int FacePos;

	/// <summary>
	/// Создать белый угловой объект
	/// </summary>
	/// <param name="FaceArray">Массив граней</param>
	/// <param name="FaceNo">Номера граней</param>
	/// <returns></returns>
	public static WhiteCorner Create
			(
			int[] FaceArray,
			int FaceNo
			)
		{
		// угол находится в нужном положении
		return FaceArray[FaceNo] == FaceNo ? null : new WhiteCorner(FaceArray, FaceNo);
		}

	/// <summary>
	/// приватный конструктор белых углов
	/// </summary>
	/// <param name="FaceArray">Массив граней</param>
	/// <param name="FaceNo">Номера белых граней</param>
	private WhiteCorner
			(
			int[] FaceArray,
			int FaceNo
			)
		{
		// сохранить номер и положение белой грани
		this.FaceNo = FaceNo;
		FacePos = Cube.FindCorner(FaceArray, FaceNo);

		// номер блока позы грани
		int BlockNo = Cube.FaceNoToBlockNo[FacePos];

		// если номер блока от 0 до 8, он находится на белой стороне, но не в правильном положении
		// нам нужен дополнительный ход, чтобы превратить белую грань в желтую
		if(BlockNo < 9)
			{
			MoveToYellow = true;
			return;
			}

		// Номер жёлтых граней с FacePos
		// рассчитайте поворот, чтобы привести угол в соответствие с его положением
		YellowRotation = (46 - Cube.BlockFace[BlockNo, Cube.YellowFace] - FaceNo) / 2;
		if(YellowRotation < 0) YellowRotation += 4;
		return;
		}

	/// <summary>
	/// Создать шаг решения
	/// </summary>
	/// <param name="Message">Текстовое сообщение</param>
	/// <returns>Шаг решения</returns>
	public SolutionStep CreateSolutionStep
			(
			string Message
			)
		{
		// если номер блока от 0 до 8, он находится на белой стороне, но не в правильном положении
		// нам нужен дополнительный ход, чтобы превратить белое лицо в желтое
		if(MoveToYellow)
			{
			// вычислить положение грани на желтой грани, которая будет перемещена
			// в плохой угловой блок, чтобы заставить его выйти
			FacePos = 0;
			switch(FaceNo)
				{
				case 0:
					FacePos = 16;
					break;

				case 2:
					FacePos = 24;
					break;

				case 4:
					FacePos = 32;
					break;

				case 6:
					FacePos = 8;
					break;
				}
			}

		// если вращение не равно нулю, измените положение грани
		else if(YellowRotation != 0) FacePos = Cube.RotMatrix[Cube.YellowCW + YellowRotation - 1][FacePos];

		// получить шаги
		int CtrlIndex = Cube.WhiteCornerIndex[FacePos / 2];
		int Case = CtrlIndex / 4;
		int StepsIndex = CtrlIndex % 4;
		int FrontFace = StepsIndex + 1;
		StepCtrl StepCtrl = Cube.WhiteCornerCases[Case];
		int[] Steps = StepCtrl.Steps(StepsIndex);

		// угол в жёлтой грани
		if(!MoveToYellow)
			{
			// нет вращения
			if(YellowRotation == 0)
				return new SolutionStep(StepCode.WhiteCorners, Message, FaceNo, Cube.WhiteFace, FrontFace, Steps);

			// создайте новый массив цветовых шагов, чтобы включить поворот желтого цвета
			int Len = Steps.Length;
			int[] TempSteps = new int[Len + 1];
			Array.Copy(Steps, 0, TempSteps, 1, Len);
			TempSteps[0] = Cube.YellowCW + YellowRotation - 1;

			// вернуть шаг решения
			return new SolutionStep(StepCode.WhiteCorners, Message, FaceNo, Cube.WhiteFace, FrontFace, TempSteps);
			}
	
		// переместите угол на желтую грань
		// вернуть с шагом решения
		return new SolutionStep(StepCode.WhiteCorners, Message, FaceNo, Cube.WhiteFace, FrontFace, Steps);
		}
	}
}
