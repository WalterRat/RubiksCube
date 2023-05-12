using System;
using System.Text;

namespace RubiksCube
{
/// <summary>
/// Код шагов решения
/// </summary>
public enum StepCode
	{
	WhiteEdges,
	WhiteCorners,
	MidLayer,
	YellowCross,
	YellowCornersPos,
	YellowCorners,
	YellowEdges,
	CubeIsSolved,
	}

/// <summary>
/// Класс кубика рубика
/// </summary>
public class Cube
	{
	// константы, которые нельзя изменить
	public const int BlocksPerCube = 27;
	public const int BlocksPerFace = 9;
	public const int FaceNoToColor = 8;
	public const int MovableFaces = 48;

	// цветовой код граней
	public const int WhiteFace = 0;
	public const int BlueFace = 1;
	public const int RedFace = 2;
	public const int GreenFace = 3;
	public const int OrangeFace = 4;
	public const int YellowFace = 5;
	public const int FaceColors = 6;
	
	// коды поворота
	public const int UpCW = 0;
	public const int UpCW2 = 1;
	public const int UpCCW = 2;
	public const int FrontCW = 3;
	public const int FrontCW2 = 4;
	public const int FrontCCW = 5;
	public const int RightCW = 6;
	public const int RightCW2 = 7;
	public const int RightCCW = 8;
	public const int BackCW = 9;
	public const int BackCW2 = 10;
	public const int BackCCW = 11;
	public const int LeftCW = 12;
	public const int LeftCW2 = 13;
	public const int LeftCCW = 14;
	public const int DownCW = 15;
	public const int DownCW2 = 16;
	public const int DownCCW = 17;

	// коды поворота
	public const int WhiteCW = 0;
	public const int WhiteCW2 = 1;
	public const int WhiteCCW = 2;
	public const int BlueCW = 3;
	public const int BlueCW2 = 4;
	public const int BlueCCW = 5;
	public const int RedCW = 6;
	public const int RedCW2 = 7;
	public const int RedCCW = 8;
	public const int GreenCW = 9;
	public const int GreenCW2 = 10;
	public const int GreenCCW = 11;
	public const int OrangeCW = 12;
	public const int OrangeCW2 = 13;
	public const int OrangeCCW = 14;
	public const int YellowCW = 15;
	public const int YellowCW2 = 16;
	public const int YellowCCW = 17;
	public const int RotationCodes = 18;
	public const int RotMovesPerColor = 3;

	/// <summary>
	/// имена цветов граней
	/// </summary>
	public static readonly string[] FaceColorName = new string[]
		{
		"Белый",
		"Синий",
		"Красный",
		"Зелёный",
		"Оранжевый",
		"Жёлтый",
		"Чёрный"
		};

	/// <summary>
	/// сохранение заголовков решений
	/// </summary>
	public static readonly string[] SaveSolutionHeader = new string[]
		{
		"Белый  Красный",
		"Синий   Жёлтый",
		"Красный    Жёлтый",
		"Зелёный  Жёлтый",
		"Оранжевый Жёлтый",
		"Жёлтый Оранжевый",
		};

	/// <summary>
	/// Имена относительных вращений
	/// </summary>
	public static readonly string[] RelativeRotationName = new string[]
		{
		"U",
		"U2",
		"U'",

		"F",
		"F2",
		"F'",

		"R",
		"R2",
		"R'",

		"B",
		"B2",
		"B'",

		"L",
		"L2",
		"L'",

		"D",
		"D2",
		"D'",
		};


	/// <summary>
	/// номер граней стороны блока как функция номера блока и цветового кода
	/// подвижные грани кодируются от 0 до 47
	/// невидимые лица кодируются как -1
	/// фиксированные грани кодируются 48 и выше
	/// </summary>
	public static readonly int[,] BlockFace = new int[,]
		{
		// W   B   R   G   O   Y            Z   Y   X
		{  0, 12, 22, -1, -1, -1}, // 0		0	0	0	W0	B4	R6	--	--	--
		{  1, -1, 21, -1, -1, -1}, // 1		0	0	1	W1	--	R5	--	--	--
		{  2, -1, 20, 30, -1, -1}, // 2		0	0	2	W2	--	R4	G6	--	--
		{  7, 13, -1, -1, -1, -1}, // 3		0	1	0	W7	B5	--	--	--	--
		{ 48, -1, -1, -1, -1, -1}, // 4		0	1	1	White					
		{  3, -1, -1, 29, -1, -1}, // 5		0	1	2	W3	--	--	G5	--	--
		{  6, 14, -1, -1, 36, -1}, // 6		0	2	0	W6	B6	--	--	O4	--
		{  5, -1, -1, -1, 37, -1}, // 7		0	2	1	W5	--	--	--	O5	--
		{  4, -1, -1, 28, 38, -1}, // 8		0	2	2	W4	--	--	G4	O6	--
		{ -1, 11, 23, -1, -1, -1}, // 9		1	0	0	--	B3	R7	--	--	--
		{ -1, -1, 50, -1, -1, -1}, // 10	1	0	1	--	--	Red	--	--	--
		{ -1, -1, 19, 31, -1, -1}, // 11	1	0	2	--	--	R3	G7	--	--
		{ -1, 49, -1, -1, -1, -1}, // 12	1	1	0	--	Blue--	--	--	--	
		{ -1, -1, -1, -1, -1, -1}, // 13	1	1	1						
		{ -1, -1, -1, 51, -1, -1}, // 14	1	1	2	--	--	--	Green-	--	
		{ -1, 15, -1, -1, 35, -1}, // 15	1	2	0	--	B7	--	--	O3	--
		{ -1, -1, -1, -1, 52, -1}, // 16	1	2	1	--	--	--	--	Orange	
		{ -1, -1, -1, 27, 39, -1}, // 17	1	2	2	--	--	--	G3	O7	--
		{ -1, 10, 16, -1, -1, 46}, // 18	2	0	0	--	B2	R0	--	--	Y7
		{ -1, -1, 17, -1, -1, 45}, // 19	2	0	1	--	--	R1	--	--	Y6
		{ -1, -1, 18, 24, -1, 44}, // 29	2	0	2	--	--	R2	G0	--	Y4
		{ -1,  9, -1, -1, -1, 47}, // 21	2	1	0	--	B1	--	--	--	Y8
		{ -1, -1, -1, -1, -1, 53}, // 22	2	1	1	--	--	--	--	--	Yellow
		{ -1, -1, -1, 25, -1, 43}, // 23	2	1	2	--	--	--	G1	--	Y3
		{ -1,  8, -1, -1, 34, 40}, // 24	2	2	0	--	B0	--	--	O2	Y0
		{ -1, -1, -1, -1, 33, 41}, // 25	2	2	1	--	--	--	--	O1	Y1
		{ -1, -1, -1, 26, 32, 42}, // 26	2	2	2				G2	O0	Y2
		};

	/// <summary>
	/// вектор поворота для каждого возможного поворота
	/// недостающие векторы вычисляются в статическом конструкторе
	/// </summary>
	public static readonly int[][] RotMatrix = new int[][]
		{
		new int[] {2,3,4,5,6,7,0,1,8,9,10,11,20,21,22,15,16,17,18,19,28,29,30,23,24,25,26,27,36,37,38,31,32,33,34,35,12,13,14,39,40,41,42,43,44,45,46,47},
		null,
		null,
		new int[] {36,1,2,3,4,5,34,35,10,11,12,13,14,15,8,9,0,17,18,19,20,21,6,7,24,25,26,27,28,29,30,31,32,33,46,47,40,37,38,39,16,41,42,43,44,45,22,23},
		null,
		null,
		new int[] {10,11,12,3,4,5,6,7,8,9,44,45,46,13,14,15,18,19,20,21,22,23,16,17,2,25,26,27,28,29,0,1,32,33,34,35,36,37,38,39,40,41,42,43,30,31,24,47},
		null,
		null,
		new int[] {0,1,18,19,20,5,6,7,8,9,10,11,12,13,14,15,16,17,42,43,44,21,22,23,26,27,28,29,30,31,24,25,4,33,34,35,36,37,2,3,40,41,38,39,32,45,46,47},
		null,
		null,
		new int[] {0,1,2,3,26,27,28,7,6,9,10,11,12,13,4,5,16,17,18,19,20,21,22,23,24,25,40,41,42,29,30,31,34,35,36,37,38,39,32,33,14,15,8,43,44,45,46,47},
		null,
		null,
		new int[] {0,1,2,3,4,5,6,7,32,33,34,11,12,13,14,15,8,9,10,19,20,21,22,23,16,17,18,27,28,29,30,31,24,25,26,35,36,37,38,39,42,43,44,45,46,47,40,41},
		null,
		null,
		};

		/// <summary>
		/// перевод между нотациями алгоритмов (U, F, R, B, L и D) до цветов граней
		/// </summary>
		public static readonly int[][] RelativeToColor = new int[][]
		{
		//				up		  front	     right		  back		  left	    down
		new int[] {WhiteFace,   BlueFace, OrangeFace,  GreenFace,    RedFace, YellowFace},
		new int[] {WhiteFace,    RedFace,   BlueFace, OrangeFace,  GreenFace, YellowFace},
		new int[] {WhiteFace,  GreenFace,    RedFace,   BlueFace, OrangeFace, YellowFace},
		new int[] {WhiteFace, OrangeFace,  GreenFace,    RedFace,   BlueFace, YellowFace},
		new int[] {YellowFace,  BlueFace,    RedFace,  GreenFace, OrangeFace, WhiteFace},
		new int[] {YellowFace,   RedFace,  GreenFace, OrangeFace,   BlueFace, WhiteFace},
		new int[] {YellowFace, GreenFace, OrangeFace,   BlueFace,    RedFace, WhiteFace},
		new int[] {YellowFace, OrangeFace,  BlueFace,    RedFace,  GreenFace, WhiteFace},
		};

	/// <summary>
	/// перевод между цветовыми обозначениями в относительные повороты (U, F, R, B, L и D)
	/// </summary>
	public static readonly int[][] ColorToRelative = new int[][]
		{
		//		  White		Blue	    Red			Green		Orange		Yellow
		new int[] {UpCW,	FrontCW,	LeftCW,		BackCW, 	RightCW,	DownCW},
		new int[] {UpCW,	RightCW,	FrontCW,	LeftCW, 	BackCW,		DownCW},
		new int[] {UpCW,	BackCW,		RightCW,	FrontCW, 	LeftCW,		DownCW},
		new int[] {UpCW,	LeftCW, 	BackCW,		RightCW, 	FrontCW,	DownCW},
		new int[] {DownCW,	FrontCW,	RightCW,	BackCW, 	LeftCW,		UpCW},
		new int[] {DownCW,	LeftCW,		FrontCW,	RightCW, 	RightCW,	UpCW},
		new int[] {DownCW,	BackCW,		LeftCW, 	FrontCW, 	RightCW,	UpCW},
		new int[] {DownCW,	RightCW,	BackCW, 	LeftCW, 	FrontCW,	UpCW},
		};

	/// <summary>
	/// текст шагов решения
	/// </summary>
	public static readonly string[] StepCodeName =
		{
		"Белые Рёбра",
		"Белые Углы",
		"Центральные Рёбра в Середине",
		"Жёлтый Крест",
		"Положение Жёлтых Углов",
		"Жёлтые Углы",
		"Жёлтые рёбра",
		"Куб решён"
		};

	/// <summary>
	/// список всех номеров граней для углов -1 на белой грани и >=0 на желтой
	/// </summary>
	public static readonly int[] WhiteCornerIndex = new int[]
		{
		-1, -1, -1, -1,  4,  1, -1, -1,  5,  2, 
		-1, -1,  6,  3, -1, -1,  7,  0, -1, -1, 
		 8, 11, 10,  9, 
		};

	/// <summary>
	/// Случаи решения с белыми углами
	/// </summary>
	public static readonly StepCtrl[] WhiteCornerCases = new StepCtrl[]
		{
		new StepCtrl(WhiteFace, RightCCW, DownCCW, RightCW),
		new StepCtrl(WhiteFace, FrontCW, DownCW, FrontCCW),
		new StepCtrl(WhiteFace, RightCCW, DownCW2, RightCW, DownCW, RightCCW, DownCCW, RightCW),
		};

	/// <summary>
	/// Левая грань среднего слоя
	/// </summary>
	public static readonly StepCtrl MidLayerLeft =
		new StepCtrl(YellowFace, UpCCW, LeftCCW, UpCW, LeftCW, UpCW, FrontCW, UpCCW, FrontCCW);

	/// <summary>
	/// Левая грань правого слоя
	/// </summary>
	public static readonly StepCtrl MidLayerRight =
		new StepCtrl(YellowFace, UpCW, RightCW, UpCCW, RightCCW, UpCCW, FrontCCW, UpCW, FrontCW);

	/// <summary>
	/// Сторона с желтой поперечной линией
	/// </summary>
	public static readonly StepCtrl YellowCrossLineCase =
		new StepCtrl(YellowFace, FrontCW, RightCW, UpCW, RightCCW, UpCCW, FrontCCW);

	/// <summary>
	/// Желтый корпус L-образной формы 
	/// </summary>
	public static readonly StepCtrl YellowCrossLShapeToCrossCase =
		new StepCtrl(YellowFace, FrontCW, UpCW, RightCW, UpCCW, RightCCW, FrontCCW);

	/// <summary>
	/// Желтое угловое положение левой стороны
	/// </summary>
	public static readonly StepCtrl YellowCornerPosLeft =
		new StepCtrl(YellowFace, LeftCW, RightCCW, UpCCW, RightCW, UpCW, LeftCCW, UpCCW, RightCCW, UpCW, RightCW);

	/// <summary>
	/// Желтое угловое положение правой стороны
	/// </summary>
	public static readonly StepCtrl YellowCornerPosRight =
		new StepCtrl(YellowFace, RightCCW, UpCCW, RightCW, UpCW, LeftCW, UpCCW, RightCCW, UpCW, RightCW, LeftCCW);

	/// <summary>
	/// Ориентация желтого угла леовй стороны
	/// </summary>
	public static readonly StepCtrl YellowCornerOrientLeft =
		new StepCtrl(YellowFace, LeftCW, UpCW, LeftCCW, UpCW, LeftCW, UpCW2, LeftCCW, UpCW2);

	/// <summary>
	/// Желтый угол ориентации првой стороны
	/// </summary>
	public static readonly StepCtrl YellowCornerOrientRight =
		new StepCtrl(YellowFace, RightCCW, UpCCW, RightCW, UpCCW, RightCCW, UpCW2, RightCW, UpCW2);

	/// <summary>
	/// Ориентация по желтому краю левой стороны
	/// </summary>
	public static readonly StepCtrl YellowEdgeLeft =
		new StepCtrl(YellowFace, RightCW, UpCCW, RightCW, UpCW, RightCW, UpCW, RightCW, UpCCW, RightCCW, UpCCW, RightCW2);

	/// <summary>
	/// Ориентация по желтому краю правой стороны
	/// </summary>
	public static readonly StepCtrl YellowEdgeRight =
		new StepCtrl(YellowFace, RightCW2, UpCW, RightCW, UpCW, RightCCW, UpCCW, RightCCW, UpCCW, RightCCW, UpCW, RightCCW);

	/// <summary>
	/// Имя блока
	/// </summary>
	public static readonly string[] BlockName;

	/// <summary>
	/// Перевести номер грани в номер блока
	/// </summary>
	public static readonly int[] FaceNoToBlockNo;

	/// <summary>
	/// Массив всех граничных блоков
	/// </summary>
	public static readonly EdgeBlock[] EdgeBlockArray;

	/// <summary>
	/// Массив всех угловых блоков
	/// </summary>
	public static readonly CornerBlock[] CornerBlockArray;

	/// <summary>
	/// массив пар чисел граней на ребро
	/// </summary>
	public static readonly int[] EdgePairArray;

	/// <summary>
	/// Генератор случайных чисел
	/// </summary>
	private static Random RandomMove = new Random();

	/// <summary>
	// Массив граней. Единственное переменное поле класса cube
	/// </summary>
	private int[] FaceArray;

	/// <summary>
	/// Статический конструктор
	/// </summary>
	static Cube()
		{
		// добавление CW2 и CCW в матрицу вращения
		for(int RotateIndex = 0; RotateIndex < RotationCodes; RotateIndex += 3)
			{
			int[] M1 = RotMatrix[RotateIndex];
			int[] M2 = new int[MovableFaces];
			int[] M3 = new int[MovableFaces];
			for(int Index = 0; Index < MovableFaces; Index++) 
				{
				int R1 = M1[Index];
				int R2 = M1[R1];
				int R3 = M1[R2];
				M2[Index] = R2;
				M3[Index] = R3;
				}

			RotMatrix[RotateIndex + 1] = M2;
			RotMatrix[RotateIndex + 2] = M3;
			}

		// от номера граней до номера блока
		FaceNoToBlockNo = new int[MovableFaces];
		for(int Block = 0; Block < BlocksPerCube; Block++)
			for(int Face = 0; Face < FaceColors; Face++)
				{
				int FaceNo = BlockFace[Block, Face];
				if(FaceNo >= 0 && FaceNo < MovableFaces) FaceNoToBlockNo[FaceNo] = Block;
				}

		// имя блока
		BlockName = new string[BlocksPerCube];
		for(int BlockNo = 0; BlockNo < BlocksPerCube; BlockNo++)
			{
			StringBuilder Name = new StringBuilder();
			if(BlockFace[BlockNo, WhiteFace] >= 0 && BlockFace[BlockNo, WhiteFace] < MovableFaces)
				Name.Append(FaceColorName[WhiteFace] + " ");
			if(BlockFace[BlockNo, YellowFace] >= 0 && BlockFace[BlockNo, YellowFace] < MovableFaces)
				Name.Append(FaceColorName[YellowFace] + " ");
			for(int FaceColor = BlueFace; FaceColor < YellowFace; FaceColor++)
				{
				if(BlockFace[BlockNo, FaceColor] >= 0 && BlockFace[BlockNo, FaceColor] < MovableFaces)
					Name.Append(FaceColorName[FaceColor] + " ");
				}
			if(Name.Length > 0)
				{
				Name.Append((BlockNo & 1) == 0 ? "Угол" : "Ребро");
				BlockName[BlockNo] = Name.ToString();
				}
			}

		// Массив граничных блоков
		EdgeBlockArray = new EdgeBlock[12];
		EdgePairArray = new int[24];
		int Ptr = 0;
		for(int BlockNo = 1; BlockNo < BlocksPerCube; BlockNo += 2)
			{
			int FaceNo1 = -1;
			int FaceNo2 = -1;
			for(int FaceColor = WhiteFace; FaceColor <= YellowFace; FaceColor++)
				{
				int FaceNo = BlockFace[BlockNo, FaceColor];
				if(FaceNo >= 0 && FaceNo < MovableFaces)
					{
					if(FaceNo1 < 0) FaceNo1 = FaceNo;
					else FaceNo2 = FaceNo;
					}
				}

			// подвижный блок
			if(FaceNo1 >= 0 && FaceNo2 >= 0)
				{
				EdgeBlockArray[Ptr++] = new EdgeBlock(FaceNo1, FaceNo2);
				EdgePairArray[FaceNo1 / 2] = FaceNo2;
				EdgePairArray[FaceNo2 / 2] = FaceNo1;
				}
			}

		// массив угловых блоков
		CornerBlockArray = new CornerBlock[8];
		Ptr = 0;
		for(int BlockNo = 0; BlockNo < BlocksPerCube; BlockNo += 2)
			{
			int FaceNo1 = -1;
			int FaceNo2 = -1;
			int FaceNo3 = -1;
			for(int FaceColor = WhiteFace; FaceColor <= YellowFace; FaceColor++)
				{
				int FaceNo = BlockFace[BlockNo, FaceColor];
				if(FaceNo >= 0 && FaceNo < MovableFaces)
					{
					if(FaceNo1 < 0) FaceNo1 = FaceNo;
					else if(FaceNo2 < 0) FaceNo2 = FaceNo;
					else FaceNo3 = FaceNo;
					}
				}
			if(FaceNo1 >= 0 && FaceNo2 >= 0 && FaceNo3 >= 0) CornerBlockArray[Ptr++] = new CornerBlock(FaceNo1, FaceNo2, FaceNo3);
			}

		return;
		}

	/// <summary>
	/// Конструктор
	/// </summary>
	public Cube()
		{
		Reset();
		return;
		}

	/// <summary>
	/// Копия конструктора
	/// </summary>
	/// <param name="BaseCube">Куб, который нужно скопировать</param>
	public Cube
			(
			Cube BaseCube
			)
		{
		FaceArray = (int[]) BaseCube.FaceArray.Clone();
		return;
		}

	/// <summary>
	/// Сбросить куб в собранное состояние
	/// </summary>
	public void Reset()
		{
		FaceArray = new int[MovableFaces];
		for(int Index = 0; Index < MovableFaces; Index++) FaceArray[Index] = Index;
		return;
		}

	/// <summary>
	/// Получить цвет одной грани
	/// </summary>
	/// <param name="FaceNo">Номер грани</param>
	/// <returns>Код цвета грани</returns>
	public int FaceColor
			(
			int FaceNo
			)
		{
		return FaceArray[FaceNo] / FaceNoToColor;
		}

	/// <summary>
	/// Сбросить пользовательский цветовой массив в текущий куб
	/// </summary>
	public int[] ColorArray
		{
		get
			{
			int[] ColorArray = new int[MovableFaces];			
			for(int FaceNo = 0; FaceNo < MovableFaces; FaceNo++) ColorArray[FaceNo] = FaceColor(FaceNo);
			return ColorArray;
			}
		set
			{
			FaceArray = TestUserColorArray(value);
			return;
			}
		}

	/// <summary>
	/// Проверка собранности
	/// </summary>
	/// <returns>Решенное состояние</returns>
	public bool AllBlocksInPlace
		{
		get
			{
			for(int Index = 0; Index < MovableFaces; Index++) if(FaceArray[Index] != Index) return false;
			return true;
			}
		}

	/// <summary>
	/// Все белые края находятся в собранном состоянии
	/// </summary>
	public bool AllWhiteEdgesInPlace
		{
		get
			{
			return FaceArray[1] == 1 && FaceArray[3] == 3 && FaceArray[5] == 5 && FaceArray[7] == 7;
			}
		}

	/// <summary>
	/// Все белые углы находятся в собранном состоянии
	/// </summary>
	public bool AllWhiteCornersInPlace
		{
		get
			{
			return FaceArray[0] == 0 && FaceArray[2] == 2 && FaceArray[4] == 4 && FaceArray[6] == 6;
			}
		}

	/// <summary>
	/// Все рёбра среднего слоя находятся в собранном состоянии
	/// </summary>
	public bool AllMidLayerEdgesInPlace
		{
		get
			{
			return FaceArray[11] == 11 && FaceArray[19] == 19 && FaceArray[27] == 27 && FaceArray[35] == 35;
			}
		}

	/// <summary>
	/// Желтые края имеют крестообразную форму
	/// </summary>
	public bool YellowEdgesInCrossShape
		{
		get
			{
			return FaceColor(41) == YellowFace && FaceColor(43) == YellowFace && FaceColor(45) == YellowFace && FaceColor(47) == YellowFace;
			}
		}

	/// <summary>
	/// Все желтые края находятся в собранном состоянии
	/// </summary>
	public bool AllYellowEdgesInPlace
		{
		get
			{
			return FaceArray[41] == 41 && FaceArray[43] == 43 && FaceArray[45] == 45 && FaceArray[47] == 47;
			}
		}

	/// <summary>
	/// Все желтые углы находятся в своем положении, но не обязательно в собранном состоянии
	/// </summary>
	public bool AllYellowCornersInPosition
		{
		get
			{
			return FaceNoToBlockNo[40] == FaceNoToBlockNo[FaceArray[40]] &&
				FaceNoToBlockNo[42] == FaceNoToBlockNo[FaceArray[42]] &&
				FaceNoToBlockNo[44] == FaceNoToBlockNo[FaceArray[44]] &&
				FaceNoToBlockNo[46] == FaceNoToBlockNo[FaceArray[46]];
			}
		}

	/// <summary>
	/// Все желтые углы находятся в собранном состоянии
	/// </summary>
	public bool AllYellowCornersInPlace
		{
		get
			{
			return FaceArray[40] == 40 && FaceArray[42] == 42 && FaceArray[44] == 44 && FaceArray[46] == 46;
			}
		}

	/// <summary>
	/// Массив поворотов
	/// </summary>
	/// <param name="RotationCode">Код поворота цвета</param>
	public void RotateArray
			(
			int RotationCode
			)
		{
		FaceArray = RotateArray(FaceArray, RotationCode);
		return;
		}

	/// <summary>
	/// Массив поворотов
	/// </summary>
	/// <param name="RotationSteps">Массив шагов поворота</param>
	public void RotateArray
			(
			int[] RotationSteps
			)
		{
		foreach(int RotateCode in RotationSteps) RotateArray(RotateCode);
		return;
		}

	/// <summary>
	/// Следующий шаг решения
	/// </summary>
	/// <returns>Результат шага решения</returns>
	public SolutionStep NextSolutionStep()
		{
		if(!AllWhiteEdgesInPlace) return SolveWhiteEdges();
		else if(!AllWhiteCornersInPlace) return SolveWhiteCorners();
		else if(!AllMidLayerEdgesInPlace) return SolveMidLayerEdges();
		else if(!YellowEdgesInCrossShape) return SolveYellowEdgesCrossShape();
		else if(!AllYellowCornersInPosition) return SolveYellowCornersPosition();
		else if(!AllYellowCornersInPlace) return SolveYellowCornersOrientation();
		else if(!AllBlocksInPlace) return SolveYellowEdgesOrientation();
		return new SolutionStep();
		}

	/// <summary>
	/// Решить белые края
	/// </summary>
	/// <returns>Шаг решения</returns>
	private SolutionStep SolveWhiteEdges()
		{
		// проверка, есть ли у нас хотя бы один белый край на белой грани
		int[] TempFaceArray = FaceArray;
		int BestCount = 0;
		int BestRot = 0;
		int BestFaceNo = 0;
		for(int Rotation = 0; Rotation < 4; Rotation++)
			{
			// поверните белую грань CW
			if(Rotation > 0) TempFaceArray = RotateArray(TempFaceArray, WhiteCW);

			// посчитать, сколько белых краев находится в нужном месте
			int SaveFaceNo = 0;
			int Count = 0;
			for(int FaceNo = 1; FaceNo < 9; FaceNo += 2) if(TempFaceArray[FaceNo] == FaceNo)
				{
				Count++;
				SaveFaceNo = FaceNo;
				}

			// сохранить лучшее количество
			if(Count > BestCount)
				{
				BestCount = Count;
				BestRot = Rotation;
				BestFaceNo = SaveFaceNo;
				}
			}

		// у нас есть по крайней мере один белое ребро на белой грани
		// и нам нужно повернуть, чтобы поставить его на место
		// найти первое совпадение
		if(BestCount > 0 && BestRot > 0)
			{
			// другой цвет грани белого ребра
			return new SolutionStep(StepCode.WhiteEdges, "Повернуть в положение", BestFaceNo, WhiteFace,
				OtherEdgeColor(BestFaceNo), new int[] {BestRot - 1});
			}

		// массив белых рёбер на месте
		bool[] WhiteEdges = new bool[4];
		WhiteEdges[0] = FaceArray[1] == 1;
		WhiteEdges[1] = FaceArray[3] == 3;
		WhiteEdges[2] = FaceArray[5] == 5;
		WhiteEdges[3] = FaceArray[7] == 7;

		// попробуй выполнить одно вращение, чтобы получить еще одно ребро на месте
		for(int R1 = BlueCW; R1 < YellowCW; R1++)
			{
			SolutionStep Step = TestWhiteEdges(WhiteEdges, new int[] {R1});
			if(Step != null) return Step;
			}

		// попробуйте все возможные двойные повороты, чтобы получить еще одно ребро на месте
		for(int R1 = 0; R1 < RotationCodes; R1++)
			{
			for(int R2 = 0; R2 < RotationCodes; R2++)
				{
				if(R1 / 3 == R2 / 3) continue;
				SolutionStep Step = TestWhiteEdges(WhiteEdges, new int[] {R1, R2});
				if(Step != null) return Step;
				}
			}

		// попробуйте все возможные три поворота, чтобы получить еще одно ребро на месте
		for(int R1 = 0; R1 < RotationCodes; R1++)
			{
			for(int R2 = 0; R2 < RotationCodes; R2++)
				{
				if(R1 / 3 == R2 / 3) continue;
				for(int R3 = 0; R3 < RotationCodes; R3++)
					{
					if(R2 / 3 == R3 / 3) continue;
					SolutionStep Step = TestWhiteEdges(WhiteEdges, new int[] {R1, R2, R3});
					if(Step != null) return Step;
					}
				}
			}

		// попробуйте все возможные четыре поворота, чтобы получить еще одно ребро на месте
		for(int R1 = 0; R1 < RotationCodes; R1++)
			{
			for(int R2 = 0; R2 < RotationCodes; R2++)
				{
				if(R1 / 3 == R2 / 3) continue;
				for(int R3 = 0; R3 < RotationCodes; R3++)
					{
					if(R2 / 3 == R3 / 3) continue;
					for(int R4 = 0; R4 < RotationCodes; R4++)
						{
						if(R3 / 3 == R4 / 3) continue;
						SolutionStep Step = TestWhiteEdges(WhiteEdges, new int[] {R1, R2, R3, R4});
						if(Step != null) return Step;
						}
					}
				}
			}

		throw new ApplicationException("Решите белые рёбра. Четырех оборотов недостаточно.");
		}

	/// <summary>
	/// Проверьте, есть ли еще один белый край на месте
	/// </summary>
	/// <param name="WhiteEdges">Белые края на месте управляющего массива</param>
	/// <param name="Steps">Шаги вращения</param>
	/// <returns>Шаг решения</returns>
	private SolutionStep TestWhiteEdges
			(
			bool[] WhiteEdges,
			int[] Steps
			)
		{
		// вращать
		int[] TestArray = RotateArray(FaceArray, Steps);

		// убедитесь, что белые края, которые были на месте, все еще на месте
		if(WhiteEdges[0] && TestArray[1] != 1 || WhiteEdges[1] && TestArray[3] != 3 ||
			WhiteEdges[2] && TestArray[5] != 5 || WhiteEdges[3] && TestArray[7] != 7) return null;

		// проверьте, не переместился ли один край, которого не было на месте, на свое место
		for(int FaceNo = 1; FaceNo < 9; FaceNo += 2)
			{
			if(!WhiteEdges[FaceNo / 2] && TestArray[FaceNo] == FaceNo)
				{
				int FrontFace = RedFace + FaceNo / 2;
				if(FrontFace > OrangeFace) FrontFace = BlueFace;
				return new SolutionStep(StepCode.WhiteEdges, "Переместиться на позицию", FaceNo, WhiteFace, FrontFace, Steps);
				}
			}

		// проверка не прошла
		return null;
		}

	/// <summary>
	/// Решите белые углы
	/// </summary>
	/// <returns>шаг решения</returns>
	private SolutionStep SolveWhiteCorners()
		{
		// найдите текущее местоположение 4 белых углов
		WhiteCorner[] CornerArray = new WhiteCorner[4];

		// найдите угол на желтой стороне без поворота
		for(int Index = 0; Index < 4; Index++)
			{
			// создать белый угловой объект
			WhiteCorner Corner = WhiteCorner.Create(FaceArray, 2 * Index);

			// угол на месте
			if(Corner == null) continue;

			// угол находится на желтой грани с нулевым вращением
			if(!Corner.MoveToYellow && Corner.YellowRotation == 0) return Corner.CreateSolutionStep("Переместиться на позицию");

			// сохранить в массив
			CornerArray[Index] = Corner;
			}

		// найдите угол на желтой стороне с поворотом
		foreach(WhiteCorner Corner in CornerArray)
			{
			// угол на месте или на белом лице, но в неправильном положении
			if(Corner != null && !Corner.MoveToYellow)  return Corner.CreateSolutionStep("Поверните и переместите в положение");
			}

		// у нас есть белый угол на белой стороне с неправильным положением или ориентацией
		// нам нужно убрать его с белой грани и переместить на жёлтую грань
		foreach(WhiteCorner Corner in CornerArray)
			{
			// угол на месте или на белой грани, но в неправильном положении
			if(Corner != null)  return Corner.CreateSolutionStep("Перейти к желтым граням");
			}

		// ошибка
		throw new ApplicationException("Решите белые углы. Недопустимый куб.");
		}

	/// <summary>
	/// Решение рёбер среднего слоя
	/// </summary>
	/// <returns>Шаг решения</returns>
	private SolutionStep SolveMidLayerEdges()
		{
		// 4 ребра среднего слоя
		MidLayer[] MidLayerArray = new MidLayer[4];

		// переместите ребро с помощью алгоритма вправо или влево, удаляющего начальное желтое вращение
		for(int Index = 0; Index < 4; Index++)
			{
			// создание объекта ребра среднего слоя
			MidLayer Edge = MidLayer.Create(FaceArray, 11 + 8 * Index);

			// ищите ребро перемещения без начального шага
			if(Edge != null && !Edge.MoveToYellow && Edge.YellowRotation == 0) return Edge.CreateSolutionStep1("Переместитесь в положение (удалите первый шаг)");

			// сохранить объект
			MidLayerArray[Index] = Edge;
			}

		// перемещение ребра с помощью алгоритма вправо или влево с поворотом желтой грани
		foreach(MidLayer Edge in MidLayerArray)
			{
			// ищите край перемещения с жёлтой грани
			if(Edge != null && !Edge.MoveToYellow)
				{
				if(Edge.Rotation == 0) return Edge.CreateSolutionStep2("Переместиться на позицию");
				return Edge.CreateSolutionStep3("Переместиться на позицию (отрегулируйте первый шаг)");
				}
			}

		// переместите ребро из среднего слоя с помощью правильного алгоритма
		// меньше первого движения желтой грани
		foreach(MidLayer Edge in MidLayerArray)
			{
			if(Edge != null) return Edge.CreateSolutionStep4("Переместить ребро на желтую грань");
			}

		// оштбка
		throw new ApplicationException("Решите рёбра среднего слоя. Недопустимый куб.");
		}

	/// <summary>
	/// Решите пересечение желтых рёбер 
	/// </summary>
	/// <returns>Шаг решения</returns>
	private SolutionStep SolveYellowEdgesCrossShape()
		{
		// ищите первую желтую грань края
		int YelEdge1;
		for(YelEdge1 = 41; YelEdge1 < 49 && FaceColor(YelEdge1) != YellowFace; YelEdge1 += 2);

		// ни одно жёлтое ребро не является желтым
		if(YelEdge1 == 49) return new SolutionStep(StepCode.YellowCross, "Переместите первые две желтые грани", 45,
			YellowFace, GreenFace, YellowCrossLineCase.Steps(2));

		// ищите вторую желтую грань края
		int YelEdge2;
		for(YelEdge2 = YelEdge1 + 2; YelEdge2 < 49 && FaceColor(YelEdge2) != YellowFace; YelEdge2 += 2);

		// должно быть два
		if(YelEdge1 == 49) throw new ApplicationException("Решите желтый крест. Недопустимый куб. Две жёлтых грани.");

		// два желтых ребра образуют линию 41-45 (зеленый)
		if(YelEdge1 == 41 && YelEdge2 == 45)
			return new SolutionStep(StepCode.YellowCross, "Переход от линии к кресту", 43,
				YellowFace, GreenFace, YellowCrossLineCase.Steps(2));

		// два желтых ребра образуют линию с 43 по 47 (красный)
		if(YelEdge1 == 43 && YelEdge2 == 47)
			return new SolutionStep(StepCode.YellowCross, "Переход от линии к кресту", 45,
				YellowFace, RedFace, YellowCrossLineCase.Steps(1));

		// у нас есть L-образная форма
		// алгоритм переходит от L-образной формы к перекрестной
		int Index;
		if(YelEdge1 == 41)
			{
			if(YelEdge2 == 43) Index = 0; // 41-43 Синий
			else Index = 1; // 41-47 Красный
			}
		else if(YelEdge1 == 43) Index = 3; // 43-45 Оранжевый
		else Index = 2; // 45-47 Зелёный

		// два желтых края образуют L-образную форму
		return new SolutionStep(StepCode.YellowCross, "Переход от L-образной формы к поперечной", 47 - 2 * Index,
			YellowFace, Index + 1, YellowCrossLShapeToCrossCase.Steps(Index));
		}

	/// <summary>
	/// Решите желтые углы, чтобы исправить положение, но не обязательно правильную ориентацию
	/// </summary>
	/// <returns>Шаг решения</returns>
	private SolutionStep SolveYellowCornersPosition()
		{
		// поворачивайте желтую грань до тех пор, пока в нужном месте не останется только 1 угол или 4 угла
		// обратите внимание, что количество =4 и поворот=0 - все углы находятся в правильном положении
		int[] TempFaceArray = FaceArray;
		int Count;
		int Rotate = 0;
		int Match = 0;
		int Match2 = 0;
		int Rotate2 = 0;
		for(;;)
			{
			Count = 0;
			for(int Index = 40; Index < MovableFaces; Index += 2)
				{
				if(FaceNoToBlockNo[Index] == FaceNoToBlockNo[TempFaceArray[Index]])
					{
					Count++;
					Match = Index;
					if(Count == 2 && Match2 == 0)
						{
						Match2 = Match;
						Rotate2 = Rotate;
						}
					}
				}
			if(Count == 1 || Count == 4 || Rotate == 3) break;
			TempFaceArray = RotateArray(TempFaceArray, YellowCW);
			Rotate++;
			}

		// у нас есть 4 матча, нам нужна просто ротация, чтобы все заняли свои позиции
		if(Count == 4) return new SolutionStep(StepCode.YellowCornersPos, "Поверните желтую грань в положение", 44,
			YellowFace, RedFace, new int[] {YellowCW + Rotate - 1});

		// у нас есть один угловой матч и три угловых не на месте
		if(Count == 1)
			{
			// следующий угловой CW после матча
			int MatchR = Match + 2;
			if(MatchR > 46) MatchR = 40;

			// следующий угловой после матча
			int MatchL = Match - 2;
			if(MatchL < 40) MatchL = 46;

				// используйте левый алгоритм, чтобы переместить угол в MatchR до MatchL 
				// используйте правый алгоритм, чтобы переместить угол в MatchL до MatchR 
				StepCtrl Step = FaceNoToBlockNo[MatchL] == FaceNoToBlockNo[TempFaceArray[MatchR]] ? YellowCornerPosLeft : YellowCornerPosRight;
			
			// вернуть
			return SolveYellowCornersPosition(Step, Match, Rotate, MatchR, "Поверните 3 угла в нужное положение");
			}

		// единственный другой возможный случай - Count== 2
		// если случай 2 не был найден, куб недействителен
		if(Match2 == 0) throw new ApplicationException("Решите положение желтых углов. Недопустимый куб.");

		// алгоритм получения левого, который не включает блок соответствия 2
		StepCtrl Step2 = YellowCornerPosLeft;

			// вернуть
			return SolveYellowCornersPosition(YellowCornerPosLeft, Match2, Rotate, Match2 > 44 ? 40 : Match2 + 2, "Поверните 3 угла, чтобы получить совпадение одного угла");
		// у нас есть два угла, и нам нужно перетасовать углы так, чтобы у нас был один
		}

	/// <summary>
	/// Решить вспомогательный метод определения положения желтого угла
	/// </summary>
	/// <param name="Step">Контрольный шаг</param>
	/// <param name="Match">Номер желтой угловой грани в правильном месте</param>
	/// <param name="Rotate">Поворот, чтобы привести грань в нужное место</param>
	/// <param name="FaceNo">Номер грани</param>
	/// <param name="Message">Сообщение</param>
	/// <returns></returns>
	private SolutionStep SolveYellowCornersPosition
			(
			StepCtrl Step,
			int Match,
			int Rotate,
			int FaceNo,
			string Message
			)
		{
		// вычислить индекс по алгоритму
		int Index = 0;
		switch(Match)
			{
			// зелёный
			case 40:
				Index = 2;
				break;

			// красный
			case 42:
				Index = 1;
				break;

			// синий
			case 44:
				Index = 0;
				break;

			// оранжевый
			case 46:
				Index = 3;
				break;
			}

		// объедините вращение и алгоритм в один шаг решения
		int[] Steps = Step.Steps(Index);
		if(Rotate != 0)
			{
			int[] TempSteps = Steps;
			int Len = TempSteps.Length;
			Steps = new int[Len + 1];
			Steps[0] = YellowCW + Rotate - 1;
			Array.Copy(TempSteps, 0, Steps, 1, Len);
			}

		// вернуть
		return new SolutionStep(StepCode.YellowCornersPos, Message, FaceNo, YellowFace, BlueFace + Index, Steps);
		}

	/// <summary>
	/// Решите проблему ориентации желтых углов
	/// </summary>
	/// <returns>Шаг решения</returns>
	private SolutionStep SolveYellowCornersOrientation()
		{
		// ищите один и только один угол на месте
		SolutionStep Step = LookForOneYellowCornerMatch(FaceArray);
		if(Step != null) return Step;

		// попробуйте все 8 возможных желтых угловых ориентаций, чтобы найти одну с одним совпадением
		for(int Index = 0; Index < 8; Index++)
			{
			StepCtrl StepCtrl = Index < 4 ? YellowCornerOrientLeft : YellowCornerOrientRight;
			int StepsIndex = Index % 4;
			int[] Steps = StepCtrl.Steps(StepsIndex);

			// имитировать поворот трех углов
			int[] TempFaceArray = RotateArray(FaceArray, Steps);

			// проверьте, найдено ли одно совпадение
			SolutionStep Step2 = LookForOneYellowCornerMatch(TempFaceArray);

			// перетасуйте три желтых угла
			if(Step2 != null) return new SolutionStep(StepCode.YellowCorners, "Перетасуйте три желтых угла", Step2.FaceNo,
				YellowFace, BlueFace + StepsIndex, Steps);
			}

		throw new ApplicationException("Решите проблему ориентации желтых углов. Недопустимый куб.");
		}

	/// <summary>
	/// Ищите один желтый угловой Match
	/// </summary>
	/// <param name="FaceArrayArg">Массив граней</param>
	/// <returns>Шаг решения</returns>
	private SolutionStep LookForOneYellowCornerMatch
			(
			int[] FaceArrayArg
			)
		{
		// посчитайте, сколько желтых углов на месте
		int Count = 0;
		int Match = 0;
		for(int FaceNo = 40; FaceNo < MovableFaces; FaceNo += 2) if(FaceArrayArg[FaceNo] == FaceNo)
			{
			Count++;
			Match = FaceNo;
			}

		// одно совпадение не было найдено
		if(Count != 1) return null;

		// алгоритм ориентации указателя на желтый угол
		// вычислить левый индекс по алгоритму
		int Index = 0;
		switch(Match)
			{
			// зелёный
			case 40:
				Index = 2;
				break;

			// красный
			case 42:
				Index = 1;
				break;

			// синий
			case 44:
				Index = 0;
				break;

			// оранжевый
			case 46:
				Index = 3;
				break;
			}

		// проверка на левый или правый алгоритм
		bool LeftAlgo = FaceArrayArg[10 + 8 * Index] / FaceNoToColor == YellowFace;

		// для правильного индекса изменения алгоритма
		if(!LeftAlgo)
			{
			Index--;
			if(Index < 0) Index = 3;
			}

		// загрузите правильное управление шагом
		StepCtrl StepCtrl = LeftAlgo ? YellowCornerOrientLeft : YellowCornerOrientRight;

		// возврат с шагом решения
		return new SolutionStep(StepCode.YellowCorners, "Поверните 3 желтых угла на их место",
			Match, YellowFace, BlueFace + Index, StepCtrl.Steps(Index));
		}

	/// <summary>
	/// Поверните 3 желтых края в нужное место
	/// </summary>
	/// <returns>Шаг Решения</returns>
	private SolutionStep SolveYellowEdgesOrientation()
		{
		// ищите один и только один желтый край на месте
		SolutionStep Step = LookForOneYellowEdgeMatch(FaceArray);
		if(Step != null) return Step;

		// попробуйте все 8 возможных желтых угловых ориентаций, чтобы найти одну с одним совпадением
		for(int Index = 0; Index < 8; Index++)
			{
			StepCtrl StepCtrl = Index < 4 ? YellowEdgeLeft : YellowEdgeRight;
			int StepsIndex = Index % 4;
			int[] Steps = StepCtrl.Steps(StepsIndex);

			// имитировать поворот трех углов
			int[] TempFaceArray = RotateArray(FaceArray, Steps);

			// проверьте, найдено ли одно совпадение
			SolutionStep Step2 = LookForOneYellowEdgeMatch(TempFaceArray);

			// перетасуйте три желтых угла
			if(Step2 != null) return new SolutionStep(StepCode.YellowEdges, "Перетасуйте желтые края, чтобы получить одно совпадение",
				Step2.FaceNo, YellowFace, BlueFace + StepsIndex, Steps);
			}

		throw new ApplicationException("Решите проблему ориентации желтых краев. Недопустимый куб.");
		}

	/// <summary>
	/// Ищите одно совпадение с желтым краем
	/// </summary>
	/// <param name="FaceArrayArg">Массив граней</param>
	/// <returns>Шаг решения</returns>
	private SolutionStep LookForOneYellowEdgeMatch
			(
			int[] FaceArrayArg
			)
		{
		// посчитайте, сколько ребер на месте
		int Count = 0;
		int Match = 0;
		for(int FaceNo = 41; FaceNo < MovableFaces; FaceNo += 2) if(FaceArrayArg[FaceNo] == FaceNo)
			{
			Count++;
			Match = FaceNo;
			}

		// одно совпадение не найдено
		if(Count != 1) return null;

		// следующий край CW после Match
		int MatchR = Match + 2;
		if(MatchR > MovableFaces) MatchR = 41;

		// следующий край CCW после Match
		int MatchL = Match - 2;
		if(MatchL < 41) MatchL = 47;

		// индекс алгоритма
		int Index = 0;
		switch(Match)
			{
			// красный
			case 41:
				Index = 1;
				break;

			// синий
			case 43:
				Index = 0;
				break;

			// оранжевый
			case 45:
				Index = 3;
				break;

			// зелёный
			case 47:
				Index = 2;
				break;
			}

		// получите пошаговый контроль
		StepCtrl StepCtrl = FaceArrayArg[MatchR] == MatchL ? YellowEdgeLeft : YellowEdgeRight;

		// вернуть шаг решения
		return new SolutionStep(StepCode.YellowEdges, "Поверните 3 ребра в нужное положение",
			MatchR, YellowFace, BlueFace + Index, StepCtrl.Steps(Index));
		}

	/// <summary>
	/// Сбросить куб в решённое состояние
	/// </summary>
	public static int[] ResetFaceArray()
		{
		int[] FaceArray = new int[MovableFaces];
		for(int Index = 0; Index < MovableFaces; Index++) FaceArray[Index] = Index;
		return FaceArray;
		}

	/// <summary>
	/// Поворот массива граней по коду поворота
	/// </summary>
	/// <param name="FaceArrayArg">Массив граней</param>
	/// <param name="RotationCode">Код поворота</param>
	/// <returns>Результат массива граней</returns>
	public static int[] RotateArray
			(
			int[] FaceArrayArg,
			int RotationCode
			)
		{
		int[] RotateVector = RotMatrix[RotationCode];
		int[] TempFaceArray = new int[MovableFaces];
		for(int Index = 0; Index < MovableFaces; Index++) TempFaceArray[RotateVector[Index]] = FaceArrayArg[Index];
		return TempFaceArray;
		}

	/// <summary>
	/// Поверните массив граней с помощью серии шагов поворота
	/// </summary>
	/// <param name="FaceArrayArg">Массив граней</param>
	/// <param name="RotationSteps">Код поворота</param>
	/// <returns>Результат массива граней</returns>
	public static int[] RotateArray
			(
			int[] FaceArrayArg,
			int[] RotationSteps
			)
		{
		int[] TempFaceArray = FaceArrayArg;
		foreach(int RotateCode in RotationSteps) TempFaceArray = RotateArray(TempFaceArray, RotateCode);
		return TempFaceArray;
		}

	/// <summary>
	/// Имя блока из номера граней
	/// </summary>
	/// <param name="FaceNo">Номер граней</param>
	/// <returns>Имя блока</returns>
	public static string GetBlockName
			(
			int FaceNo
			)
		{
		return FaceNo >= 0 ? BlockName[FaceNoToBlockNo[FaceNo]] : string.Empty;
		}

	/// <summary>
	/// Найдите положение граней по номеру граней (только по углам)
	/// </summary>
	/// <param name="FaceArrayArg">Массив граней</param>
	/// <param name="FaceNo">Номер граней</param>
	/// <returns>Позиции граней</returns>
	public static int FindCorner
			(
			int[] FaceArrayArg,
			int FaceNo
			)
		{
		for(int FacePos = 0; FacePos < MovableFaces; FacePos += 2) if(FaceNo == FaceArrayArg[FacePos]) return FacePos;
		throw new ApplicationException("Не удалось найти угол");
		}

	/// <summary>
	/// Найти положение грани по номеру грани (только по рёбрам)
	/// </summary>
	/// <param name="FaceArrayArg">Массив граней</param>
	/// <param name="FaceNo">Номер граней</param>
	/// <returns>Позиции граней</returns>
	public static int FindEdge
			(
			int[] FaceArrayArg,
			int FaceNo
			)
		{
		for(int FacePos = 1; FacePos < MovableFaces; FacePos += 2) if(FaceNo == FaceArrayArg[FacePos]) return FacePos;
		throw new ApplicationException("Не удалось найти ребро");
		}

	/// <summary>
	/// Другой цвет ребра
	/// </summary>
	/// <param name="FaceNo">Номер граней</param>
	/// <returns>Другой цвет граней</returns>
	public static int OtherEdgeColor
			(
			int FaceNo
			)
		{
		return EdgePairArray[FaceNo / 2] / FaceNoToColor;
		}

	/// <summary>
	/// Тестовый массив цветов граней пользователя
	/// </summary>
	/// <param name="UserColorArray">Цветовой массив</param>
	/// <returns>Массив граней</returns>
	public static int[] TestUserColorArray
			(
			int[] UserColorArray
			)
		{
		// Проверка углов
		int[] Count = new int[FaceColors];
		for(int Index = 0; Index < MovableFaces; Index += 2)
			{
			// цветовой код в указателе положения
			int ColorCode = UserColorArray[Index];

			// убедитесь, что цветовой код от белого (0) до желтого (5)
			if(ColorCode < WhiteFace || ColorCode > YellowFace) throw new ApplicationException("Элемент угла массива цветов не является допустимым цветом");

			// подсчитайте, сколько раз присутствует каждый цвет
			Count[ColorCode]++;
			}

		// убедитесь, что каждый цвет отображается 4 раза
		for(int Index = 0; Index < FaceColors; Index++) if(Count[Index] != 4)
			{
			throw new ApplicationException(string.Format("Ошибка установки цветов. Здесь очень {0} {1} угловых граней.",
				Count[Index] > 4 ? "много" : "мало", FaceColorName[Index]));
			}

		// проверка рёбер
		Count = new int[FaceColors];
		for(int Index = 1; Index < MovableFaces; Index += 2)
			{
			// цветовой код в указателе положения
			int ColorCode = UserColorArray[Index];

			// убедитесь, что цветовой код от белого (0) до желтого (5)
			if(ColorCode < WhiteFace || ColorCode > YellowFace) throw new ApplicationException("Элемент края массива цветов не является допустимым цветом");

			// count how many times each color is present
			Count[ColorCode]++;
			}

		// убедитесь, что каждый цвет отображается 4 раза
		for(int Index = 0; Index < FaceColors; Index++) if(Count[Index] != 4)
			{
			throw new ApplicationException(string.Format("Ошибка установки цветов. Здесь очень {0} {1} рёберных граней.",
				Count[Index] > 4 ? "много" : "мало", FaceColorName[Index]));
			}

		// построить массив граней в соответствии с пользователем
		int[] UserFaceArray = new int[MovableFaces];

		// углы
		for(int Index = 0; Index < 8; Index++) TestUserCorner(CornerBlockArray[Index], UserColorArray, UserFaceArray);

		// рёбра
		for(int Index = 0; Index < 12; Index++) TestUserEdge(EdgeBlockArray[Index], UserColorArray, UserFaceArray);

		// создать тестовый куб
		Cube TestCube = new Cube()
			{
			FaceArray = (int[]) UserFaceArray.Clone()
			};

		StepCode StepNo = StepCode.WhiteEdges;
		int StepCounter = 0;

		// попробовать решить его
		for(;;)
			{
			// получить следующий шаг
			SolutionStep SolveStep = TestCube.NextSolutionStep();

			// куб решён
			if(SolveStep.StepCode == StepCode.CubeIsSolved) break;

			// проверка процесса
			if(SolveStep.StepCode > StepNo)
				{
				StepNo = SolveStep.StepCode;
				StepCounter = 0;
				}

			// проверка на регрессию
			else if(SolveStep.StepCode < StepNo)
				{
				throw new ApplicationException("Недопустимый куб. Регрессия решения.");
				}

			// проверка на цикл
			else if(StepCounter > 8)
				{
				throw new ApplicationException("Недопустимый куб. Решение находится в цикле.");
				}

			// выполните шаги поворота
			TestCube.RotateArray(SolveStep.Steps);
			StepCounter++;
			}

		// возвращаемый проверенный массив граней
		return UserFaceArray;
		}

	/// <summary>
	/// проверка углов
	/// </summary>
	/// <param name="CornerBlock">Определение углового блока</param>
	/// <param name="UserColorArray">Пользовательский цветовой массив (ввод)</param>
	/// <param name="UserFaceArray">Массив граней пользователя (вывод)</param>
	private static void TestUserCorner
			(
			CornerBlock CornerBlock,
			int[] UserColorArray,
			int[] UserFaceArray
			)
		{
		// три номера граней, связанные со стандартным положением угла
		int StandardFaceNo1 = CornerBlock.FaceNo1;
		int StandardFaceNo2 = CornerBlock.FaceNo2;
		int StandardFaceNo3 = CornerBlock.FaceNo3;

		// три цвета лица после того, как пользователь раскрасит грани
		int UserFaceColor1 = UserColorArray[StandardFaceNo1];
		int UserFaceColor2 = UserColorArray[StandardFaceNo2];
		int UserFaceColor3 = UserColorArray[StandardFaceNo3];

		// убедитесь, что все цвета разные
		if(UserFaceColor1 == UserFaceColor2 || UserFaceColor1 == UserFaceColor3 || UserFaceColor2 == UserFaceColor3)
			throw new ApplicationException(string.Format("Цвета угловых граней {0}, {1} и {2} должеы быть все разные.",
				FaceColorName[UserFaceColor1], FaceColorName[UserFaceColor2], FaceColorName[UserFaceColor3]));

		// найдите угол в стандартном решенном кубе
		foreach(CornerBlock Corner in CornerBlockArray)
			{
			// match color1
			int FaceNo1;
			if(UserFaceColor1 == Corner.FaceColor1) FaceNo1 = Corner.FaceNo1;
			else if(UserFaceColor1 == Corner.FaceColor2) FaceNo1 = Corner.FaceNo2;
			else if(UserFaceColor1 == Corner.FaceColor3) FaceNo1 = Corner.FaceNo3;
			else continue;
				
			// match color2
			int FaceNo2;
			if(UserFaceColor2 == Corner.FaceColor1) FaceNo2 = Corner.FaceNo1;
			else if(UserFaceColor2 == Corner.FaceColor2) FaceNo2 = Corner.FaceNo2;
			else if(UserFaceColor2 == Corner.FaceColor3) FaceNo2 = Corner.FaceNo3;
			else continue;

			// match color3
			int FaceNo3;
			if(UserFaceColor3 == Corner.FaceColor1) FaceNo3 = Corner.FaceNo1;
			else if(UserFaceColor3 == Corner.FaceColor2) FaceNo3 = Corner.FaceNo2;
			else if(UserFaceColor3 == Corner.FaceColor3) FaceNo3 = Corner.FaceNo3;
			else continue;

			// сохраните три номера лиц в массив граней пользователя
			UserFaceArray[StandardFaceNo1] = FaceNo1;
			UserFaceArray[StandardFaceNo2] = FaceNo2;
			UserFaceArray[StandardFaceNo3] = FaceNo3;
			return;
			}

		throw new ApplicationException(string.Format("Цвета угловых граней {0}, {1} и {2} имеют ошибку",
				FaceColorName[UserFaceColor1], FaceColorName[UserFaceColor2], FaceColorName[UserFaceColor3]));
		}

	/// <summary>
	///  проверка рёбер
	/// </summary>
	/// <param name="EdgeBlock">Стандартный рёберный блок</param>
	/// <param name="UserColorArray">Пользовательский цветовой массив</param>
	/// <param name="UserFaceArray">Массив граней пользователей</param>
	private static void TestUserEdge
			(
			EdgeBlock EdgeBlock,
			int[] UserColorArray,
			int[] UserFaceArray
			)
		{
		// два номера граней, связанных с ребром
		int StandardFaceNo1 = EdgeBlock.FaceNo1;
		int StandardFaceNo2 = EdgeBlock.FaceNo2;

		// два цвета лица после того, как пользователь раскрасит грани
		int FaceColor1 = UserColorArray[StandardFaceNo1];
		int FaceColor2 = UserColorArray[StandardFaceNo2];

		// убедитесь, что все цвета разные
		if(FaceColor1 == FaceColor2)
			throw new ApplicationException(string.Format("Цвета граней рёбер {0} и {1} должны быть разными.",
				FaceColorName[FaceColor1], FaceColorName[FaceColor2]));

		// найдите ребро в стандартном решенном кубе
		foreach(EdgeBlock Edge in EdgeBlockArray)
			{
			// match color 1
			int FaceNo1;
			if(FaceColor1 == Edge.FaceColor1) FaceNo1 = Edge.FaceNo1;
			else if(FaceColor1 == Edge.FaceColor2) FaceNo1 = Edge.FaceNo2;
			else continue;
			
			// match color 2
			int FaceNo2;
			if(FaceColor2 == Edge.FaceColor1) FaceNo2 = Edge.FaceNo1;
			else if(FaceColor2 == Edge.FaceColor2) FaceNo2 = Edge.FaceNo2;
			else continue;

			// сохраните два номера лиц в массив граней пользователя
			UserFaceArray[StandardFaceNo1] = FaceNo1;
			UserFaceArray[StandardFaceNo2] = FaceNo2;
			return;
			}

		throw new ApplicationException(string.Format("Цвета граней рёбер {0} и {1} имеют ошибку",
				FaceColorName[FaceColor1], FaceColorName[FaceColor2]));
		}

	/// <summary>
	/// Преобразование цветовых шагов в текстовое сообщение
	/// </summary>
	/// <param name="Steps">Массив цветовых шагов</param>
	/// <returns>Сообщение</returns>
	public static string ColorCodesToText
			(
			int[] Steps
			)
		{
		StringBuilder Text = new StringBuilder();
		for(int Index = 0; Index < Steps.Length; Index++)
			{
			string Separator;
			if(Index == 0) Separator = string.Empty;
			else if((Index % 3) == 0) Separator = " - ";
			else Separator = " ";
			}
		return Text.ToString();
		}

	/// <summary>
	/// Преобразование относительных шагов в текстовое сообщение
	/// </summary>
	/// <param name="UpFaceColor">Цвет верхних граней</param>
	/// <param name="FrontFaceColor">Цвет передних граней</param>
	/// <param name="Steps">Массив относительных шагов</param>
	/// <returns>Сообщение</returns>
	public static string RelativeCodesToText
			(
			int UpFaceColor,
			int FrontFaceColor,
			int[] Steps
			)
		{
		StringBuilder Text = new StringBuilder();

		// преобразование цветовых шагов в относительные шаги
		int[] Xlate = ColorToRelative[(FrontFaceColor - 1) + (UpFaceColor == WhiteFace ? 0 : 4)];
		for(int Index = 0; Index < Steps.Length; Index++)
			{
			int Step = Steps[Index];
			string Separator;
			if(Index == 0) Separator = string.Empty;
			else if((Index % 3) == 0) Separator = " - ";
			else Separator = " ";
			Text.AppendFormat("{0}{1}", Separator, RelativeRotationName[Xlate[Step / 3] + (Step % 3)]);
			}
		return Text.ToString();
		}
	}
}
