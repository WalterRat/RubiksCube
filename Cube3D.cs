using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace RubiksCube
{
	public class Cube3D : ModelVisual3D
	{
	// константы, которые могут быть изменены для внешнего вида

	/// <summary>
	/// Ширина одного блока высота и глубина
	/// </summary>
	public const double BlockWidth = 1.0;

	/// <summary>
	/// Расстояние между блоками
	/// </summary>
	public const double BlockSpacing = 0.05;

	/// <summary>
	/// Ширина куба высота и глубина
	/// </summary>
	public const double CubeWidth = 3 * BlockWidth + 2 * BlockSpacing;

	/// <summary>
	/// Ширина половины куба
	/// </summary>
	public const double HalfCubeWidth = 0.5 * CubeWidth;

	/// <summary>
	/// Расстояние камеры от куба
	/// </summary>
	public const double CameraDistance = 4.0 * CubeWidth;

	/// <summary>
	/// Угол обзора камеры над горизонтальной плоскостью 
	/// </summary>
	public const double CameraUpAngle = 25.0 * Math.PI / 180.0;

	/// <summary>
	/// Угол наклона камеры вправо
	/// </summary>
	public const double CameraRightAngle = 25.0 * Math.PI / 180.0;

	/// <summary>
	/// Угол обзора камеры в градусах
	/// </summary>
	public const double CameraViewAngle = 40;

	/// <summary>
	/// Тонкая граница
	/// </summary>
	public static Thickness ThinBorder = new Thickness(1);

	/// <summary>
	/// Толстая граница
	/// </summary>
	public static Thickness ThickBorder = new Thickness(5);

	/// <summary>
	/// Массив цветов лица
	/// </summary>
	public static Brush[] FaceColor = new Brush[]
		{
		Brushes.White,
		Brushes.Blue,
		Brushes.Red,
		Brushes.Green,
		Brushes.Orange,
		Brushes.Yellow,
		Brushes.Black,
		};

	/// <summary>
	/// Диффузный материал граньевой стороны блока
	/// </summary>
	public static DiffuseMaterial[] Material = new DiffuseMaterial[]
		{
		new DiffuseMaterial(Brushes.White),
		new DiffuseMaterial(Brushes.Blue),
		new DiffuseMaterial(Brushes.Red),
		new DiffuseMaterial(Brushes.Green),
		new DiffuseMaterial(Brushes.Orange),
		new DiffuseMaterial(Brushes.Yellow),
		new DiffuseMaterial(Brushes.Black)
		};

	/// <summary>
	/// Ось вращения граней куба
	/// </summary>
	public static Vector3D[] RotationAxis = new Vector3D[]
		{
		new Vector3D(0, 0, 1),
		new Vector3D(1, 0, 0),
		new Vector3D(0, 1, 0),
		new Vector3D(-1, 0, 0),
		new Vector3D(0, -1, 0),
		new Vector3D(0, 0, -1),
		};

	/// <summary>
	/// Имена вращений
	/// </summary>
	public static string[] RotMoveName = new string[]
		{
		"CW",
		"CW2",
		"CCW",
		};

	/// <summary>
	/// полное движение куба
	/// </summary>
	public static int[][] FullMoveAngle = new int [][]
		{
		new int[] {-90, 0, 0},		// white
		new int[] {0, 0, 90},		// blue
		new int[] {0, 0, 0},		// red
		new int[] {0, 0, -90},		// green
		new int[] {0, 0, 180},		// orange
		new int[] {90, 0, 0},		// yellow
		};

	/// <summary>
	/// Углы поворота анимации сторон куба (без поворота, CW, CW2 и CCW
	/// </summary>
	public static int[] RotMoveAngle = new int[]
		{
		0,
		90,
		180,
		-90,
		};

	/// <summary>
	/// для данного верхнего цвета каковы доступные цвета для лицевой стороны
	/// </summary>
	public static int[][] FullMoveTopColor = new int [][]
		{
		new int[] {2, 3, 4, 1},		// white
		new int[] {5, 2, 0, 4},		// blue
		new int[] {5, 3, 0, 1},		// red
		new int[] {5, 4, 0, 2},		// green
		new int[] {5, 1, 0, 3},		// orange
		new int[] {4, 3, 2, 1},		// yellow
		};

	/// <summary>
	/// номер блока по цветовому коду и положение блока на грани куба
	/// </summary>
	public static int[,] BlockNoOfOneFace = new int[,]
		{
		{  0,  1,  2,  3,  4,  5,  6,  7,  8},		// white
		{  0,  3,  6,  9, 12, 15, 18, 21, 24},		// blue
		{  0,  1,  2,  9, 10, 11, 18, 19, 20},		// red
		{  2,  5,  8, 11, 14, 17, 20, 23, 26},		// green
		{  6,  7,  8, 15, 16, 17, 24, 25, 26},		// orange
		{ 18, 19, 20, 21, 22, 23, 24, 25, 26},		// yellow
		};

	/// <summary>
	/// Объект куба
	/// </summary>
	public Cube FullCube;

	/// <summary>
	/// Все блоки каждой стороны куба
	/// </summary>
	public Block3D[][] CubeFaceBlockArray;

	/// <summary>
	/// Массив граней анимационного блока с одной стороной куба
	/// </summary>
	public BlockFace3D[] MovableFaceArray;

	/// <summary>
	/// Конструктор
	/// </summary>
	public Cube3D()
		{
		// инициализировать объект куба
		FullCube = new Cube();

		MovableFaceArray = new BlockFace3D[Cube.MovableFaces];
		for(int BlockNo = 0; BlockNo < Cube.BlocksPerCube; BlockNo++)
			{
			// создать блок
			Block3D Block = new Block3D(BlockNo);

			// добавить куб в Cube3D VisualMedia3D
			Children.Add(Block);

			// центральный блок (блок № 13)
			if(Block.BlockFaceArray == null) continue;

			// цикл для всех 6 граней каждого блока
			foreach(BlockFace3D Face in Block.BlockFaceArray)
				{
				// сохранить массив подвижных граней
				if(Face.FaceNo >= 0 && Face.FaceNo < Cube.MovableFaces) MovableFaceArray[Face.FaceNo] = Face;
				}
			}

		// создайте двойной массив индексов всех 6 граней и для каждой грани все блоки
		CubeFaceBlockArray = new Block3D[Cube.FaceColors][];

		// цикл для всех цветов
		for(int ColorIndex = 0; ColorIndex < Cube.FaceColors; ColorIndex++)
			{
			// для каждого цвета есть 9 блоков
			CubeFaceBlockArray[ColorIndex] = new Block3D[Cube.BlocksPerFace];

			// цикл для всех блоков одного цвета лица
			for(int BlockIndex = 0; BlockIndex < Cube.BlocksPerFace; BlockIndex++)
				{
				CubeFaceBlockArray[ColorIndex][BlockIndex] = (Block3D) Children[BlockNoOfOneFace[ColorIndex, BlockIndex]];
				}
			}
		return;
		}

	/// <summary>
	/// Установите цвет всех граней в соответствии с Cube.FaceArray
	/// </summary>
	public void SetColorOfAllFaces()
		{
		for(int FaceNo = 0; FaceNo < Cube.MovableFaces; FaceNo++)
			{
			// изменить цвет граней с помощью цвета pos граней
			int FaceColor = FullCube.FaceColor(FaceNo);
			if(MovableFaceArray[FaceNo].CurrentColor != FaceColor) MovableFaceArray[FaceNo].ChangeColor(FaceColor);
			}
		return;
		}
	}
}
