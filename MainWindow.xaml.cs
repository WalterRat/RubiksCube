using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace RubiksCube
{
/// <summary>
/// Тип вращения активный
/// </summary>
public enum RotationActive
	{
	Idle,
	Cube,	// вращение всего куба
	Face,	// поворот одной грани
	}

/// <summary>
/// Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
	{
	private const double ButtonsPanelWidth = 280;
	private const double InfoPanelHeight = 100;

	private Viewport3D CubeViewPort3D;
	private Cube3D RubiksCube3D;
	private Point LastMousePosition;
	private AxisAngleRotation3D FullCubeXRotation;
	private AxisAngleRotation3D FullCubeYRotation;
	private AxisAngleRotation3D FullCubeZRotation;
	private Transform3DGroup RotateTransformGroup;

	private int FrontFaceColor;
	private int TopFaceColor;
	private Button[] FrontFaceButtons;
	private Button[] UpFaceButtons;

	private bool UserCubeActive;
	private int UserCubeSelection;
	private Button[] UserColorButtons;
	public int[] UserColorArray;

	private RotationActive RotationActive;
	private List<BlockFace3D> HitList;
	private bool RotationLock;
	private int RotateCode;
	private List<int> PastMoves;
	private List<int> NextMoves;
	private bool AutoSolve;

	private static DoubleAnimation[] AnimationArray =
		{
		new DoubleAnimation(90.0, new Duration(new TimeSpan(2500000))),
		new DoubleAnimation(180.0, new Duration(new TimeSpan(5000000))),
		new DoubleAnimation(-90.0, new Duration(new TimeSpan(2500000))),
		};

	/// <summary>
	/// Конструктор главного окна
	/// </summary>
	public MainWindow()
		{
		InitializeComponent();
		Title="Rubik's Cube";
		return;
		}

	/// <summary>
	/// Инициализация приложения
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void OnMainGridLoaded(object sender, RoutedEventArgs e)
		{

		// кнопки цвета верхней грани
		UpFaceButtons = new Button[Cube.FaceColors];
		UpFaceButtons[0] = UpFaceButton0;
		UpFaceButtons[1] = UpFaceButton1;
		UpFaceButtons[2] = UpFaceButton2;
		UpFaceButtons[3] = UpFaceButton3;
		UpFaceButtons[4] = UpFaceButton4;
		UpFaceButtons[5] = UpFaceButton5;

		// кнопки цвета лицевой стороны
		FrontFaceButtons = new Button[Cube.FaceColors];
		FrontFaceButtons[0] = FrontFaceButton0;
		FrontFaceButtons[1] = FrontFaceButton1;
		FrontFaceButtons[2] = FrontFaceButton2;
		FrontFaceButtons[3] = FrontFaceButton3;
		FrontFaceButtons[4] = FrontFaceButton4;
		FrontFaceButtons[5] = FrontFaceButton5;

		// кнопки цвета пользовательского куба
		UserColorButtons = new Button[Cube.FaceColors];
		UserColorButtons[0] = SetColorButton0;
		UserColorButtons[1] = SetColorButton1;
		UserColorButtons[2] = SetColorButton2;
		UserColorButtons[3] = SetColorButton3;
		UserColorButtons[4] = SetColorButton4;
		UserColorButtons[5] = SetColorButton5;

		// установить метод завершения анимации
		AnimationArray[0].Completed += AnimationCompleted;
		AnimationArray[1].Completed += AnimationCompleted;
		AnimationArray[2].Completed += AnimationCompleted;
		return;
		}

	/// <summary>
	/// Инициализация сетки куба
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void OnCubeGridLoaded(object sender, RoutedEventArgs e)
		{
		Reset();
		return;
		}

	/// <summary>
	/// Нажата кнопка сброса
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void ResetButtonClick(object sender, RoutedEventArgs e)
		{
		Reset();
		return;
		}

	/// <summary>
	/// Возврат к условиям запуска
	/// </summary>
	private void Reset()
		{

		// создайте Viewport3D и добавьте его в родительскую сетку CubeGrid
		CubeViewPort3D = new Viewport3D()
			{
			//Name = "mainViewport",
			ClipToBounds = true
			};
		CubeGrid.Children.Clear();
		CubeGrid.Children.Add(CubeViewPort3D);

		// создайте ModelVisual3D и добавьте его в Viewport3D
		ModelVisual3D ModelVisual = new ModelVisual3D();
		CubeViewPort3D.Children.Add(ModelVisual);

		// создайте группу Model3DGroup с белым рассеянным светом и прикрепите ее к ModelViual
		Model3DGroup ModelGroup = new Model3DGroup();
		ModelGroup.Children.Add(new AmbientLight(Colors.White));
		ModelVisual.Content = ModelGroup;

		// создайте наш кубик рубика и прикрепите его к экрану просмотра
		RubiksCube3D = new Cube3D();
		CubeViewPort3D.Children.Add(RubiksCube3D);

		// положение камеры относительно куба
		// камера смотрит прямо на куб
		double PosZ = Cube3D.CameraDistance * Math.Sin(Cube3D.CameraUpAngle);
		double Temp = Cube3D.CameraDistance * Math.Cos(Cube3D.CameraUpAngle);
		double PosX = Temp * Math.Sin(Cube3D.CameraRightAngle);
		double PosY = Temp * Math.Cos(Cube3D.CameraRightAngle);

		// создайте камеру и прикрепите ее к экрану просмотра
		CubeViewPort3D.Camera = new PerspectiveCamera(new Point3D(PosX, -PosY, PosZ), 
			new Vector3D(-PosX, PosY, -PosZ), new Vector3D(0, 0, 1), Cube3D.CameraViewAngle);

		// группа преобразования полного движения куба позволяет программе
		// повернуть весь куб в любом направлении
		Transform3DGroup FullCubeMotion = new Transform3DGroup();
		RubiksCube3D.Transform = FullCubeMotion;
		FullCubeZRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
		FullCubeMotion.Children.Add(new RotateTransform3D(FullCubeZRotation));
		FullCubeXRotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
		FullCubeMotion.Children.Add(new RotateTransform3D(FullCubeXRotation));
		FullCubeYRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
		FullCubeMotion.Children.Add(new RotateTransform3D(FullCubeYRotation));

		// список активных шагов решения
		NextMoves = new List<int>();

		// список сохраненных ходов, которые будут использоваться кнопкой отмены
		PastMoves = new List<int>();

		// тип активного вращения
		RotationActive = RotationActive.Idle;

		// список попаданий левой кнопки мыши
		HitList = new List<BlockFace3D>();
		RotationLock = false;

		// текущий цвет верха и лицевой стороны
		TopFaceColor = -1;
		FrontFaceColor = -1;
		SetUpAndFrontFace(true, 2, 0);

		// пользователь устанавливает куб
		UserCubeActive = false;
		UserCubeButton.Content = "Задать куб";
		UserCubeSelection = 0;
		SetColorButtonClick(null, null);

		// очистить labels
		ResetInfoLabels(false);
		return;
		}

	/// <summary>
	/// Передняя лицевая кнопка была нажата
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void FrontFaceButtonClick(object sender, RoutedEventArgs e)
		{
		if(!RotationLock) SetUpAndFrontFace(false, TagTranslator(sender), TopFaceColor);
		return;
		}

	/// <summary>
	/// Верхняя лицевая кнопка была нажата
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void UpFaceButtonClick(object sender, RoutedEventArgs e)
		{
		if(!RotationLock) SetUpAndFrontFace(true, FrontFaceColor, TagTranslator(sender));
		return;
		}

	/// <summary>
	/// Установите цвет передней или верхней грани
	/// </summary>
	/// <param name="TopFaceClick">Верхняя грань, если верно, передняя грань, если ложно</param>
	/// <param name="FrontFaceColor">Цвет лицевой стороны</param>
	/// <param name="TopFaceColor">Top face color</param>
	private void SetUpAndFrontFace
			(
			bool TopFaceClick,
			int FrontFaceColor,
			int TopFaceColor
			)
		{
		// никаких изменений
		if(FrontFaceColor == this.FrontFaceColor && TopFaceColor == this.TopFaceColor) return;

		// лицевая сторона
		if(!TopFaceClick)
			{
			int TopIndex;
			for(TopIndex = 0; TopIndex < 4; TopIndex++)
				{
				if(Cube3D.FullMoveTopColor[FrontFaceColor][TopIndex] == TopFaceColor) break;
				}
			if(TopIndex == 4) TopFaceColor = Cube3D.FullMoveTopColor[FrontFaceColor][0];
			}

		// верхняя грань
		else
			{
			int FrontIndex;
			for(FrontIndex = 0; FrontIndex < 4; FrontIndex++)
				{
				if(Cube3D.FullMoveTopColor[TopFaceColor][FrontIndex] == FrontFaceColor) break;
				}
			if(FrontIndex == 4) FrontFaceColor = Cube3D.FullMoveTopColor[TopFaceColor][0];
			}

		// сохранить
		this.FrontFaceColor = FrontFaceColor;
		this.TopFaceColor = TopFaceColor;

		// установите толстую границу
		for(int Index = 0; Index < Cube.FaceColors; Index++)
			{
			UpFaceButtons[Index].BorderThickness = Index == TopFaceColor ? Cube3D.ThickBorder : Cube3D.ThinBorder;
			FrontFaceButtons[Index].BorderThickness = Index == FrontFaceColor ? Cube3D.ThickBorder : Cube3D.ThinBorder;
			}		

		// угол поворота оси y
		int YIndex;
		for(YIndex = 0; YIndex < 4; YIndex++)
			{
			if(Cube3D.FullMoveTopColor[FrontFaceColor][YIndex] == TopFaceColor) break;
			}

		// поверните полный куб
		FullCubeXRotation.Angle = Cube3D.FullMoveAngle[FrontFaceColor][0];
		FullCubeYRotation.Angle = -Cube3D.RotMoveAngle[YIndex];
		FullCubeZRotation.Angle = Cube3D.FullMoveAngle[FrontFaceColor][2];
		return;
		}

	/// <summary>
	/// Нажата кнопка отмены
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void UndoButtonClick(object sender, RoutedEventArgs e)
		{
		if(!RotationLock && !UserCubeActive && PastMoves.Count != 0) RotateSide(-1);
		return;
		}

	/// <summary>
	/// Поверните одну сторону с помощью мыши
	/// </summary>
	private void RotateByMouse()
		{
		// нам нужно два попадания
		if(HitList.Count != 2) return;

		// получить от и до лицевых чисел
		int FromFaceNo = HitList[0].FaceNo;
		int ToFaceNo = HitList[1].FaceNo;
		if(FromFaceNo == ToFaceNo) return;

		// обе стороны должны быть на одной стороне
		if(FromFaceNo / 8 != ToFaceNo / 8) return;

		// обе грани должны быть либо краями, либо углами
		if((FromFaceNo & 1) != (ToFaceNo & 1)) return;

		// вычислить размер перемещения 1=CW2=CW2 3=CCW
		int FromEdgeNo = (FromFaceNo % 8) / 2;
		int ToEdgeNo = (ToFaceNo % 8) / 2;
		int Delta = ToEdgeNo - FromEdgeNo;
		if(Delta < 0) Delta += 4;

		// преобразовать в код поворота
		RotateSide(3 * (FromFaceNo / 8) + (Delta - 1));
		return;
		}

	/// <summary>
	/// Поверните одну сторону
	/// </summary>
	/// <param name="RotateCode">Код поворота</param>
	private void RotateSide
			(
			int RotateCode
			)
		{
		// Код поворота
		if(RotateCode >= 0)
			{
			PastMoves.Add(RotateCode);
			}
		// отменить-взять код поворота из списка прошлых перемещений
		else
			{

			RotateCode = PastMoves[PastMoves.Count - 1];
			PastMoves.RemoveAt(PastMoves.Count - 1);
			switch(RotateCode % 3)
				{
				case 0:
					RotateCode += 2;
					break;

				case 2:
					RotateCode -= 2;
					break;
				}
			}

		// сохранить код поворота
		this.RotateCode = RotateCode;

		// существует 18 кодов поворота по 3 для каждой грани куба
		//Поверните грань 0= белая, 1=синяя, 2=красная, 3=зеленая, 4=оранжевая, 5=желтая
		int RotateFace = RotateCode / 3;

		// создайте группу преобразований
		RotateTransformGroup = new Transform3DGroup();

		// прикрепите объект transformgroup ко всем 9 блокам грани, которую нужно повернуть
		for(int Index = 0; Index < Cube.BlocksPerFace; Index++)
			{
			RubiksCube3D.CubeFaceBlockArray[RotateFace][Index].Transform = RotateTransformGroup;
			}

		// создайте поворот оси для группы преобразования
		AxisAngleRotation3D AxisRot = new AxisAngleRotation3D(Cube3D.RotationAxis[RotateFace], 0);
		RotateTransformGroup.Children.Add(new RotateTransform3D(AxisRot));

		// запустите анимацию
		// примечание: Массив анимации - это массив из 3 объектов Doubleanimation.
		// Три объекта находятся под углом 90 градусов 0,25 секунды, 180 градусов 0,5 секунды, - 90 градусов 0,25 секунды
		AxisRot.BeginAnimation(AxisAngleRotation3D.AngleProperty, AnimationArray[RotateCode % 3]);

		// установите блокировку вращения
		RotationLock = true;
		return;
		}

	/// <summary>
	/// Анимация завершенного события
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void AnimationCompleted(object sender, EventArgs e)
		{
		// очистите все дочерние блоки той стороны, которая была повернута
		RotateTransformGroup.Children.Clear();

		// поверните текущий массив граней объекта куба
		RubiksCube3D.FullCube.RotateArray(RotateCode);

		// установите цвет всех граней блока куба
		RubiksCube3D.SetColorOfAllFaces();

		// сбросьте поля преобразования для каждой грани, входившей в группу
		for(int Index = 0; Index < Cube.BlocksPerFace; Index++)
			RubiksCube3D.CubeFaceBlockArray[RotateCode / 3][Index].Transform = null;

		// куб находится в идеальном порядке.
		if(RubiksCube3D.FullCube.AllBlocksInPlace)
			{
			// очистить список следующих ходов
			NextMoves.Clear();

			// сбросить флаг автоматического решения
			AutoSolve = false;
			}

		// куб не находится в порядке решения, и список следующих ходов не пуст
		else if(NextMoves.Count != 0)
			{
			// выполните следующий шаг и удалите его из списка
			int NextMove = NextMoves[0];
			NextMoves.RemoveAt(0);
			RotateSide(NextMove);
			}

		// куб не в порядке, и автоматическое решение включено
		else if(AutoSolve)
			{
			// решите следующий шаг
			SolveCube();
			}

		// очистить блокировку поворота
		RotationLock = false;
		return;
		}

	/// <summary>
	/// Пользователь нажал на левую кнопку мыши
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
		// игнорировать, если включена блокировка поворота
		if(RotationLock) return;

		// текущее положение мыши
		Point MousePosition = e.GetPosition(CubeViewPort3D);

		// пользователь не устанавливает цвета
		if(!UserCubeActive)
			{
			// очистить список попаданий, если вращение не выполняется или куб заполнен
			if(RotationActive != RotationActive.Face) HitList.Clear();

			// проверка попадания на текущую позицию
			HitTest(MousePosition);

			// список попаданий пуст
			if(HitList.Count == 0)
				{
				// сохранить позицию
				LastMousePosition = MousePosition;

				// установите активное вращение на полное вращение куба
				Cursor = Cursors.SizeAll;
				RotationActive = RotationActive.Cube;
				}

			// в списке попаданий есть одна запись
			else if(HitList.Count == 1 && RotationActive != RotationActive.Face)
				{
				// установите активное вращение на вращение одной грани
				RotationActive = RotationActive.Face;
				Cursor = Cursors.Hand;
				}

			// поверните одну грань с помощью мыши
			else
				{
				RotationActive = RotationActive.Idle;
				Cursor = Cursors.Arrow;
				RotateByMouse();
				}
			}

		// пользователь рисует куб, чтобы согласиться со своим кубом
		else
			{
			HitList.Clear();
			HitTest(MousePosition);
			if(HitList.Count > 0)
				{
				int FaceNo = HitList[HitList.Count - 1].FaceNo;
				if(FaceNo >= 0 && FaceNo < Cube.MovableFaces)
					{
					// сохранить цвет
					UserColorArray[FaceNo] = UserCubeSelection;

					// изменение цвета грани блока
					RubiksCube3D.MovableFaceArray[FaceNo].ChangeColor(UserCubeSelection);
					}
				}
			}
		return;  
		}

	/// <summary>
	/// Левая кнопка вверх
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
 	private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		// если мы находимся в режиме поворота полного куба, сбросьте вращение
		if(RotationActive == RotationActive.Cube)
			{
			RotationActive = RotationActive.Idle;
			Cursor = Cursors.Arrow;
			}
		return; 
		}

	/// <summary>
	/// Указатель мыши перемещается
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void OnMouseMove(object sender, MouseEventArgs e)
		{
		// если мы находимся в режиме полного вращения куба, поверните куб
		if(RotationActive == RotationActive.Cube) FullCubeRotation(e.GetPosition(CubeViewPort3D));
		return;
		}
 
	/// <summary>
	/// Полный поворот куба
	/// </summary>
	/// <param name="MousePosition">Положение мыши</param>
	private void FullCubeRotation
			(
			Point MousePosition
			)
		{
		const int Step = 3;

		// изменение положения мыши
		double DeltaX = MousePosition.X - LastMousePosition.X;
		double DeltaY = MousePosition.Y - LastMousePosition.Y;

		// сохраните для следующего шага
		LastMousePosition = MousePosition;

		// положительное изменение x
		if(DeltaX > 0)
			{
			double Angle = FullCubeZRotation.Angle + Step;
			if(Angle > 180) Angle -= 360;
			FullCubeZRotation.Angle = Angle;
			}

		// отрицательное изменение x
		else if(DeltaX < 0)
			{
			double Angle = FullCubeZRotation.Angle - Step;
			if(Angle < -180) Angle += 360;
			FullCubeZRotation.Angle = Angle;
			}

		// положительное изменение y
		if(DeltaY > 0)
			{
			double Angle = FullCubeXRotation.Angle + Step;
			if(Angle > 180) Angle -= 360;
			FullCubeXRotation.Angle = Angle;
			}
		// отрицательное изменение y
		else if(DeltaY < 0)
			{
			double Angle = FullCubeXRotation.Angle - Step;
			if(Angle < -180) Angle += 360;
			FullCubeXRotation.Angle = Angle;
			}
		return;
		}

	/// <summary>
	/// Хит-тест
	/// </summary>
	/// <param name="p">Положение мыши</param>
	private void HitTest(Point p)
		{
		// hit test
		VisualTreeHelper.HitTest(CubeViewPort3D, null, new HitTestResultCallback(ResultCallBack), new PointHitTestParameters(p));
		return;
		}

	/// <summary>
	/// Хит-тест вызывающий метод
	/// </summary>
	/// <param name="Result"></param>
	/// <returns></returns>
	private HitTestResultBehavior ResultCallBack(HitTestResult Result)
		{
		// проверка на подвижные грани
		if(Result.VisualHit.GetType() == typeof(ModelVisual3DCube))
			{
			ModelVisual3DCube Model = (ModelVisual3DCube) Result.VisualHit;
			BlockFace3D Hit = Model.BlockFace;
			if(Hit.FaceNo >= 0)
				{
				// сохранить hit и прекратить дальнейшие действия
				HitList.Add(Hit);
				return HitTestResultBehavior.Stop;
				}
			}

		// продолжайте искать подвижные грани
		return HitTestResultBehavior.Continue;
		}

	/// <summary>
	/// Решите один шаг
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void SolveStepButtonClick(object sender, RoutedEventArgs e)
		{
		if(!RotationLock && !UserCubeActive) SolveCube();
		return;
		}

	/// <summary>
	/// Автоматическое решение куба
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void AutoSolveButtonClick(object sender, RoutedEventArgs e)
		{
		if(!RotationLock && !UserCubeActive)
			{
			AutoSolve = true;
			SolveCube();
			}
		return;
		}

	/// <summary>
	/// Решайте куб по одному шагу за раз
	/// </summary>
	private void SolveCube()
		{
		try
			{
			// шагу решения
			SolutionStep SolveStep = RubiksCube3D.FullCube.NextSolutionStep();

			// сделано
			if(SolveStep.StepCode == StepCode.CubeIsSolved)
				{
				ResetInfoLabels(false);
				SolveLabel1.Content = Cube.StepCodeName[(int) SolveStep.StepCode];
				}

			// выполните шаги по решению
			else
				{
				ResetInfoLabels(true);
				SolveLabel1.Content = Cube.StepCodeName[(int) SolveStep.StepCode];
				SolveLabel5.Content = Cube.FaceColorName[SolveStep.UpFaceColor];
				SolveLabel5.Background = Cube3D.FaceColor[SolveStep.UpFaceColor];
				SolveLabel7.Content = Cube.FaceColorName[SolveStep.FrontFaceColor];
				SolveLabel7.Background = Cube3D.FaceColor[SolveStep.FrontFaceColor];
				SolveLabel8.Content = Cube.RelativeCodesToText(SolveStep.UpFaceColor, SolveStep.FrontFaceColor, SolveStep.Steps);

				// добавить шаги в список следующих шагов
				NextMoves.AddRange(SolveStep.Steps);

				// задать ориентацию куба
				SetUpAndFrontFace(true, SolveStep.FrontFaceColor, SolveStep.UpFaceColor);

				// инициировать первый шаг
				int NextMove = NextMoves[0];
				NextMoves.RemoveAt(0);
				RotateSide(NextMove);
				}
			}

		// исключение
		catch (Exception Ex)
			{
			SolveLabel1.Content = "Исключение шага решения";
			}

		return;
		}

	/// <summary>
	/// Сброс лейблов
	/// </summary>
	/// <param name="SolveState">State</param>
	private void ResetInfoLabels
			(
			bool SolveState
			)
		{
		SolveLabel1.Content = string.Empty;
		SolveLabel4.Content = SolveState ? "Вверх" : string.Empty;
		SolveLabel5.Content = string.Empty;
		SolveLabel5.Background = Brushes.LightYellow;
		SolveLabel6.Content = SolveState ? "Перед" : string.Empty;
		SolveLabel7.Content = string.Empty;
		SolveLabel7.Background = Brushes.LightYellow;
		SolveLabel8.Content = string.Empty;
		return;
		}

	/// <summary>
	/// Сохранить решение
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void SaveSolutionButtonClick(object sender, RoutedEventArgs e)
		{
		// игнорировать, включена ли блокировка поворота или пользователь рисует куб
		if(RotationLock || UserCubeActive) return;

		// клонировать куб
		Cube TempCube = new Cube(RubiksCube3D.FullCube);

		// файл шагов решения
		StreamWriter SolutionStepsFile = null;

		// решите куб и сохраните результаты
		try
			{
			// откройте существующий или создайте новый файл трассировки
			string FileName = Path.GetFullPath("SolutionSteps.txt");
			SolutionStepsFile = new StreamWriter(FileName);

			// напишите дату и время
			SolutionStepsFile.WriteLine(string.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}:{0:ss} ", DateTime.Now));

			// заголовок записи
			SolutionStepsFile.WriteLine("Определение кубика Рубика");
			SolutionStepsFile.WriteLine("Перед  Верх");

			int Ptr = 0;
			for(int Face = 0; Face < Cube.FaceColors; Face++)
				{
				StringBuilder Str = new StringBuilder();
				Str.AppendFormat("{0,-15}[{1,-6}", Cube.SaveSolutionHeader[Face], Cube.FaceColorName[TempCube.FaceColor(Ptr++)]);
				for(int Index = 0; Index < 7; Index++) Str.AppendFormat(" {0,-6}", Cube.FaceColorName[TempCube.FaceColor(Ptr++)]);
				Str.Append("]");
				SolutionStepsFile.WriteLine(Str.ToString());
				}
	
			// заголовок записи
			SolutionStepsFile.WriteLine("Этапы решения кубика Рубика");

			// считайте шаги
			int TotalSteps = 0;

			// цикл для шагов решения
			for(;;)
				{
				// сделайте следующий шаг
				SolutionStep SolveStep = TempCube.NextSolutionStep();

				// куб решен
				if(SolveStep.StepCode == StepCode.CubeIsSolved)
					{
					SolutionStepsFile.WriteLine(Cube.StepCodeName[(int) SolveStep.StepCode]);
					SolutionStepsFile.WriteLine("Общее количество шагов: " + TotalSteps.ToString());
					break;
					}

				SolutionStepsFile.WriteLine(Cube.StepCodeName[(int) SolveStep.StepCode] + ". " +
					Cube.GetBlockName(SolveStep.FaceNo) + ". " + SolveStep.Message + ". " +
					Cube.FaceColorName[SolveStep.UpFaceColor] + ", " + Cube.FaceColorName[SolveStep.FrontFaceColor] + ", " +
					Cube.RelativeCodesToText(SolveStep.UpFaceColor, SolveStep.FrontFaceColor, SolveStep.Steps) + ", Цвет: " +
					Cube.ColorCodesToText(SolveStep.Steps));

				// выполните следующие действия
				TempCube.RotateArray(SolveStep.Steps);
				TotalSteps += SolveStep.Steps.Length;
				}

			// закройте файл
			SolutionStepsFile.Close();
			SolutionStepsFile = null;

			// запустить текстовый редактор
			Process.Start(FileName);
			}

		catch (Exception Ex)
			{
			if(SolutionStepsFile != null) SolutionStepsFile.Close();
			MessageBox.Show(Ex.Message);
			}
		return;
		}

	/// <summary>
	/// Пользователь хочет установить куб равным своему собственному реальному кубу
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event Arguments</param>
	private void UserCubeButtonClick(object sender, RoutedEventArgs e)
		{
		// игнорировать, если вращение активно
		if(RotationLock) return;

		// начните рисовать куб, чтобы он соответствовал реальному кубу пользователя
		if(!UserCubeActive)
			{
			ResetInfoLabels(false);
			SolveLabel1.Content = "Задать куб";
			UserCubeButton.Content = "Раскрасить";
			UserCubeActive = true;
			UserColorArray = RubiksCube3D.FullCube.ColorArray;
			}

		// конец рисования
		//проверьте цвета, чтобы убедиться, что это правильный куб
		else
			{
			try
				{
				// Cube.ColorArray проверяет правильность цветов
				// если есть 
				RubiksCube3D.FullCube.ColorArray = UserColorArray;
				}
			catch(ApplicationException AppEx)
				{
				MessageBox.Show(AppEx.Message);
				return;
				}

			ResetInfoLabels(false);
			UserCubeButton.Content = "Задать куб";
			UserCubeActive = false;
			}
		return;
		}

	/// <summary>
	/// Выберите цвет для рисования пользовательского куба
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event arguments</param>
	private void SetColorButtonClick(object sender, RoutedEventArgs e)
		{
		// нет инициализации
		if(sender != null)
			{
			// выбранный цвет
			int Selected = TagTranslator(sender);

			// никаких изменений
			if(Selected == UserCubeSelection) return;

			// сохранить выбранный цвет
			UserCubeSelection = Selected;
			}

		// установите толстую границу вокруг выбранного цвета
		for(int Index = 0; Index < Cube.FaceColors; Index++)
			{
			UserColorButtons[Index].BorderThickness = Index == UserCubeSelection ? Cube3D.ThickBorder : Cube3D.ThinBorder;
			}

		// установите цвет фона заголовка
		SetColorHeading.Background = Cube3D.FaceColor[UserCubeSelection];
		return;
		}

	private static int TagTranslator
			(
			object sender
			)
		{
		return int.Parse((string) ((Button) sender).Tag);
		}

	/// <summary>
	/// Окно изменения размера
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnMainGridSizeChanged(object sender, SizeChangedEventArgs e)
		{
		CubeGrid.Width = RubiksCubeWindow.ActualWidth - ButtonsPanelWidth - 20;
		CubeGrid.Height = RubiksCubeWindow.ActualHeight - InfoPanelHeight - 40;
		return;
		}
	}
}
