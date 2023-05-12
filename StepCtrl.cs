using System;

namespace RubiksCube
{
/// <summary>
/// Контроль шага решения
/// </summary>
public class StepCtrl
	{
	/// <summary>
	/// Цвет верхней грани куба (белый или желтый)
	/// </summary>
	public int UpFaceColor;

	/// <summary>
	/// шаги решения в цвете для цвета передней части куба: синий, красный, зеленый и оранжевый
	/// </summary>
	public int[][] StepsArray;

	/// <summary>
	/// Шаги решения для цвета шрифта
	/// </summary>
	/// <param name="Index">0=синий, 1=красный, 2=зеленый, 3=оранжевый</param>
	/// <returns>Массив шагов</returns>
	public int[] Steps
			(
			int Index // 0=синий, 1=красный, 2=зеленый, 3=оранжевый
			)
		{
		return StepsArray[Index];
		}

	/// <summary>
	/// Конструктор контроля шагов
	/// </summary>
	/// <param name="UpFaceColor">Цвет верхней грани</param>
	public StepCtrl
			(
			int UpFaceColor,
			params int[] RelativeSteps
			)
		{

		// сохранение аргументов
		this.UpFaceColor = UpFaceColor;

		//проверьте цвет верхней грани
		int UpDown = UpFaceColor == Cube.WhiteFace ? 0 : 4;

		// создайте массивы из 4 шагов по одному для каждого цвета: синий, красный, зеленый и оранжевый
		StepsArray = new int[4][];
		for(int Index = 0; Index < 4; Index++)
			{
			// создайте массив переводов между относительным поворотом (U F R B L D) и цветами граней
			int[] Xlate = Cube.RelativeToColor[UpDown + Index];

			// вращение цвета
			StepsArray[Index] = new int[RelativeSteps.Length];

			// перевести поворот относительно цвета
			for(int Ptr = 0; Ptr < RelativeSteps.Length; Ptr++)
				{
				// один шаг (U, F, R, B, L, D)
				int Step = RelativeSteps[Ptr];

				// добавьте шаг поворота в список
				StepsArray[Index][Ptr] = 3 * Xlate[Step / 3] + (Step % 3);
				}
			}
		return;
		}
	}
}
