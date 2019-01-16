using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Timers;
using System.Windows.Threading;
using System.ComponentModel;
using System.Xml;
using System.Threading.Tasks;

namespace Panothelo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        int gridColumn = 9;
        int gridRow = 7;

        ImageBrush blackPawn;
        ImageBrush whitePawn;

        Player player1;
        Player player2;

        Board board;

        DispatcherTimer timerPlayer1;
        DispatcherTimer timerPlayer2;

        Stopwatch swPlayer1;
        Stopwatch swPlayer2;

        int turnPlayer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            InitializeBoard();


        }

        private void InitializeGame()
        {
            blackPawn = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/BlackPawn.png")));
            whitePawn = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/WhitePawn.png")));

            player1 = new Player("Jeremy", 1, whitePawn);
            player2 = new Player("Julien", 2, blackPawn);

            lblNamePlayer1.Content = player1.Name;
            lblNamePlayer2.Content = player2.Name;

            board = new Board(gridColumn, gridRow);

            TimerInitialize();

            turnPlayer = 1;



        }

        private void InitializeBoard()
        {
            for (int i = 0; i < gridColumn; i++)
            {
                for (int j = 0; j < gridRow; j++)
                {
                    Label lblGrid = new Label
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(2)
                    };

                    lblGrid.MouseEnter += MouseEnterGrid;
                    lblGrid.MouseLeave += MouseLeaveGrid;
                    lblGrid.MouseLeftButtonUp += MouseButtonUpGrid;

                    if(board.GetBoard()[i,j] == 0)
                    {
                        lblGrid.Background = player1.ImagePawn ;
                    }
                    else if(board.GetBoard()[i, j] == 1)
                    {
                        lblGrid.Background = player2.ImagePawn;
                    }

                    Grid.SetRow(lblGrid, j);
                    Grid.SetColumn(lblGrid, i);
                    Board.Children.Add(lblGrid);

                    
                }
            }

            swPlayer1.Start();
        }


        private void MouseEnterGrid(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label lblGrid = sender as Label;
        }

        private void MouseLeaveGrid(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label lblGrid = sender as Label;
        }

        private void MouseButtonUpGrid(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label lblGrid = sender as Label;
            int col = Grid.GetColumn(lblGrid);
            int row = Grid.GetRow(lblGrid);

            //MessageBox.Show(col.ToString() + " - " + row.ToString());

            if (board.GetBoard()[col, row] == -1)
            {
                
                if (turnPlayer % 2 == 0)
                {
                    lblGrid.Background = player2.ImagePawn;
                    board.GetBoard()[col, row] = 1;
                    lblScorePlayer2.Content = "Score : " + board.GetBlackScore();
                    player2.Score = board.GetBlackScore();

                    swPlayer2.Stop();
                    swPlayer1.Start();

                }
                else
                {
                    lblGrid.Background = player1.ImagePawn;
                    board.GetBoard()[col, row] = 0;
                    lblScorePlayer1.Content = "Score : " + board.GetWhiteScore();
                    player1.Score = board.GetWhiteScore();

                    swPlayer1.Stop();
                    swPlayer2.Start();
                }
                turnPlayer++;
            }
        }

        private void TimerInitialize()
        {
            timerPlayer1 = new DispatcherTimer();
            timerPlayer1.Tick += new EventHandler(TimerTick);
            timerPlayer1.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timerPlayer1.Start();

            timerPlayer2 = new DispatcherTimer();
            timerPlayer2.Tick += new EventHandler(TimerTick);
            timerPlayer2.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timerPlayer2.Start();

            swPlayer1 = new Stopwatch();
            swPlayer2 = new Stopwatch();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if(sender == timerPlayer1)
            {
                lblTimerPlayer1.Content = String.Format("{0:00}:{1:00}", swPlayer1.Elapsed.Minutes, swPlayer1.Elapsed.Seconds);
            }
            else
            {
                lblTimerPlayer2.Content = String.Format("{0:00}:{1:00}", swPlayer2.Elapsed.Minutes, swPlayer2.Elapsed.Seconds);
            }
        }
    }
}
