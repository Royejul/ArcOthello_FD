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

        List<int> listLastPossible;

        Board board;

        bool turnPlayer1;
        int nbPass;

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
            Label lblUpdate;

            foreach (int pos in listLastPossible)
            {
                lblUpdate = GameBoard.Children[pos] as Label;
            }
            listLastPossible.Clear();

            foreach (int i in board.getPossibleMoves(turnPlayer1))
            {
                listLastPossible.Add(i);

                lblUpdate = GameBoard.Children[i] as Label;
                lblUpdate.Background = Brushes.LightGray;

            }

            int actualWinner=-1;
            int scoreW = board.GetWhiteScore();
            int scoreB = board.GetBlackScore();

            if (scoreW < scoreB)
                actualWinner = 0;
            else if (scoreW > scoreB)
                actualWinner = 1;

            if (actualWinner == 1)
                GameBoard.Background = Brushes.ForestGreen;
            else if (actualWinner==0)
                GameBoard.Background = Brushes.SandyBrown;

            if (board.checkBoardFull())
            {
                MessageBox.Show(getWinnerMsg(actualWinner));
            }else if (listLastPossible.Count==0)
            {
                if(nbPass>0)
                    MessageBox.Show(getWinnerMsg(actualWinner));
                else
                {
                    turnPlayer1 = !turnPlayer1;
                    nbPass = 1;
                    update();
                }
            }else
            {
                nbPass = 0;
            }
        }

        private string getWinnerMsg(int w)
        {
            if (turnPlayer1)
                swPlayer1.Stop();
            else
                swPlayer2.Stop();

            if (w == 1)
                return "Congratulation " + player2.Name + ",\nYou won!";
            else if (w == 0)
                return "Congratulation " + player1.Name + ",\nYou won!";
            else
                return "Congratulation to both,\nIt's a draw!";
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
            lblNamePlayer2.Background = Brushes.White;

            lblScorePlayer1.Content = "Score : " + 2;
            lblScorePlayer2.Content = "Score : " + 2;

            listLastPossible = new List<int>();

            board = new Board(gridColumn, gridRow);

            turnPlayer1 = true;
            nbPass = 0;
            TimerInitialize();
        }

        private void InitializeGameFromLoad()
        {

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
                    lblGrid.MouseLeftButtonDown += MouseButtonDownGrid;

                    if (board.GetBoard()[i, j] == 0)
                    {
                        lblGrid.Background = player1.ImagePawn;
                    }
                    else if (board.GetBoard()[i, j] == 1)
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
            int col = Grid.GetColumn(lblGrid);
            int row = Grid.GetRow(lblGrid);
            int posClick = col * gridRow + row;

            if (listLastPossible.Contains(posClick))
            {
                if (turnPlayer1)
                    lblGrid.Background = player1.ImagePawn;
                else
                    lblGrid.Background = player2.ImagePawn;
            }
        }

        private void MouseLeaveGrid(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label lblGrid = sender as Label;
            int col = Grid.GetColumn(lblGrid);
            int row = Grid.GetRow(lblGrid);
            int posClick = col * gridRow + row;

            if (listLastPossible.Contains(posClick))
            {
                update();
            }
        }

        private void MouseButtonDownGrid(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label lblGrid = sender as Label;

            int col = Grid.GetColumn(lblGrid);
            int row = Grid.GetRow(lblGrid);
            int posClick = col * gridRow + row;

            if (listLastPossible.Contains(posClick))
            {
                board.PlayMove(col, row, turnPlayer1);
                GameBoard.Children.Clear();
                InitializeBoard();

                listLastPossible.Remove(posClick);

                if (PlayerTurn())
                {
                    lblGrid.Background = player2.ImagePawn;
                    board.GetBoard()[col, row] = 1;
                    turnPlayer1 = true;
                }
                else
                {
                    lblGrid.Background = player1.ImagePawn;
                    board.GetBoard()[col, row] = 0;
                    turnPlayer1 = false;
                }

                lblScorePlayer1.Content = "Score : " + board.GetWhiteScore();
                lblScorePlayer2.Content = "Score : " + board.GetBlackScore();
                update();
            }
        }

        private bool PlayerTurn()
        {
            if (!turnPlayer1)
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
            if (sender == timerPlayer1)
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
            MessageBox.Show("Othello C# - HE-ARC \nAuteur : Feuillade Julien et Dubois Jeremy");
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            GameBoard.Children.Clear();
            InitializeGame();
            InitializeBoard();
            update();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            if (turnPlayer1)
                swPlayer1.Stop();
            else
                swPlayer2.Stop();

            string filename = "", strBoard = "";

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Title = "Save the Othello",
                DefaultExt = "xml",
                CheckPathExists = true,
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = saveFileDialog.FileName;
                strBoard = board.ToString();

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

                        writer.WriteElementString("Turn", turnPlayer1.ToString());

                        writer.WriteElementString("Board", strBoard);

                        writer.WriteStartElement("Player1");
                        writer.WriteElementString("Name", player1.Name);
                        writer.WriteElementString("Time", swPlayer1.Elapsed.ToString("mm\\:ss\\:ff"));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Player2");
                        writer.WriteElementString("Name", player2.Name);
                        writer.WriteElementString("Time", swPlayer2.Elapsed.ToString("mm\\:ss\\:ff"));
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                        writer.Flush();
                        writer.Close();

                        if (turnPlayer1)
                            swPlayer1.Start();
                        else
                            swPlayer2.Start();
                    }
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Error while writing XML file");
                }

            }
            else
            {
                if (turnPlayer1)
                    swPlayer1.Start();
                else
                    swPlayer2.Start();
            }
        }

        /// <summary>
        /// Load method who reads the XML file to To fill the struct with the read data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {
            if (turnPlayer1)
                swPlayer1.Stop();
            else
                swPlayer2.Stop();

            string strBoard = "";
            string[] strTabBoard;


            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                FilterIndex = 0,
                DefaultExt = "xml"
            };


            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!String.Equals(Path.GetExtension(ofd.FileName),
                                   ".xml",
                                   StringComparison.OrdinalIgnoreCase))
                {
                    // Invalid file type selected; display an error.
                    MessageBox.Show("You must select an XML file.",
                                    "Invalid File Type",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                }
                else
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        Async = true
                    };

                    using (XmlReader reader = XmlReader.Create(ofd.FileName, settings))
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                if (reader.IsStartElement())
                                {
                                    switch (reader.Name)
                                    {


                                        case "Board":
                                            if (reader.Read())
                                            {
                                                strBoard = reader.Value.Trim();
                                            }
                                            break;

                                        case "Turn":
                                            if (reader.Read())
                                                turnPlayer1 = Boolean.Parse(reader.Value.Trim());
                                            break;
                                    }
                                }
                            }
                        }
                        catch (XmlException)
                        {
                            MessageBox.Show("Error while reading XML file, please chose another one");
                        }

                        try
                        {

                            strTabBoard = strBoard.Split(',');
                            Console.Write(strTabBoard);
                            int k = 0;
                            for (int i = 0; i < gridColumn; i++)
                            {
                                for (int j = 0; j < gridRow; j++)
                                {
                                    board.GetBoard()[i, j] = int.Parse(strTabBoard[k]);
                                    
                                    k++;
                                }
                            }
                            GameBoard.Children.Clear();
                            InitializeBoard();
                            update();

                        }
                        catch (NullReferenceException)
                        {
                            Debug.WriteLine("NullReference Exception");
                        }
                    }
                }
            }

        }
    }
}
