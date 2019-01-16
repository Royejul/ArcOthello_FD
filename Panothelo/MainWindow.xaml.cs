using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Timers;
using System.Windows.Threading;
using System.ComponentModel;
using System.Xml;

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

        List<int[]> listPossibility;

        Board board;

        bool turnPlayer1;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            InitializeBoard();
            update();

        }

        private void update()
        {
            listPossibility = board.getPossibleMoves(turnPlayer1);
            //listPossibility.ForEach(el => Console.WriteLine(el[0] +", "+el[1]));
            /*foreach (int[] pos in listPossibility)
            {
                //
            }*/
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

            turnPlayer1 = true;
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
                    GameBoard.Children.Add(lblGrid);
                }
            }
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
                if (turnPlayer1)
                {
                    lblGrid.Background = player1.ImagePawn;
                    board.GetBoard()[col, row] = 0;
                    lblScorePlayer1.Content = board.GetWhiteScore();
                    player1.Score = board.GetWhiteScore();
                    turnPlayer1 = false;
                }
                else
                {
                    lblGrid.Background = player2.ImagePawn;
                    board.GetBoard()[col, row] = 1;
                    lblScorePlayer2.Content = board.GetBlackScore();
                    player2.Score = board.GetBlackScore();
                    turnPlayer1 = true;
                }
                update();
            }

        }

    }

}
