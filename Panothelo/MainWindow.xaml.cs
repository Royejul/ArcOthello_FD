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

        List<int[]> listPossibility;

        Board board;

        bool turnPlayer1;

        DispatcherTimer timerPlayer1;
        DispatcherTimer timerPlayer2;

        Stopwatch swPlayer1;
        Stopwatch swPlayer2;

        

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
            blackPawn = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Stump.png")));
            whitePawn = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Leaf.png")));
            lblPlayerImage.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Panoramix.png")));

            player1 = new Player("Jeremy", 1, whitePawn);
            player2 = new Player("Julien", 2, blackPawn);

            lblNamePlayer1.Content = player1.Name;
            lblNamePlayer1.Background = Brushes.Green;

            lblNamePlayer2.Content = player2.Name;

            lblScorePlayer1.Content = "Score : " + 2;
            lblScorePlayer2.Content = "Score : " + 2;


            board = new Board(gridColumn, gridRow);

            turnPlayer1 = true;
            TimerInitialize();
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

            if (board.GetBoard()[col, row] == -1)
            {
                if (PlayerTurn())
                {
                    lblGrid.Background = player2.ImagePawn;
                    board.GetBoard()[col, row] = 1;
                    lblScorePlayer2.Content = board.GetBlackScore();
                    player2.Score = board.GetBlackScore();
                    turnPlayer1 = true; 
                }
                else
                {
                    lblGrid.Background = player1.ImagePawn;
                    board.GetBoard()[col, row] = 0;
                    lblScorePlayer1.Content = "Score : " + board.GetWhiteScore();
                    player1.Score = board.GetWhiteScore();
                    turnPlayer1 = false;
                }
                update();
            }
        }

        private bool PlayerTurn()
        {
            if(!turnPlayer1)
            {
                lblNamePlayer1.Background = Brushes.Green;
                lblNamePlayer2.Background = Brushes.White;

                lblPlayerImage.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Panoramix.png")));

                swPlayer2.Stop();
                swPlayer1.Start();

                return true;
            }
            else
            {
                lblNamePlayer1.Background = Brushes.White;
                lblNamePlayer2.Background = Brushes.Green;

                lblPlayerImage.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Romain.png")));

                swPlayer1.Stop();
                swPlayer2.Start();

                return false;
            }
        }

        private void TimerInitialize()
        {
            timerPlayer1 = new DispatcherTimer();
            timerPlayer1.Tick += new EventHandler(TimerTick);
            timerPlayer1.Start();

            timerPlayer2 = new DispatcherTimer();
            timerPlayer2.Tick += new EventHandler(TimerTick);
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

        private void MenuQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("About : Feuillade Julien and Dubois Jeremy.\n Othello C# 2018-2019.\n HE-ARC");
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            GameBoard.Children.Clear();
            InitializeGame();
            InitializeBoard();
        }

        /// <summary>
        /// Save method who writes all parameter to an extern XML file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
           // if(turnPlayer % 2 == 0)
                

            string filename = "", strBoard = "";

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Title = "Save the game",
                DefaultExt = "xml",
                CheckPathExists = true,
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = saveFileDialog.FileName;

                strBoard = board.GetBoard().ToString();

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = ("\t"),
                };

                //Write data on an external XML file
                try
                {
                    using (XmlWriter writer = XmlWriter.Create(filename, settings))
                    {

                        writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");

                        writer.WriteStartElement("OthelloGame");

                        writer.WriteStartElement("Player1");
                        writer.WriteElementString("Name", player1.Name);
                        writer.WriteElementString("Time", swPlayer1.Elapsed.ToString("mm\\:ss\\:ff"));
                        writer.WriteElementString("Score", player1.Score.ToString());
                        writer.WriteEndElement();


                        writer.WriteStartElement("Player2");
                        writer.WriteElementString("Name", player2.Name);
                        writer.WriteElementString("Time", swPlayer2.Elapsed.ToString("mm\\:ss\\:ff"));
                        writer.WriteElementString("Score", player2.Score.ToString());
                        writer.WriteEndElement();

                        writer.WriteElementString("Turn", turnPlayer1.ToString());


                        writer.WriteElementString("Board", strBoard);

                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                        writer.Flush();
                        writer.Close();

                    }
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Error while writing XML file");
                }

            }


        }

        /// <summary>
        /// Load method who reads the XML file to To fill the struct with the read data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
