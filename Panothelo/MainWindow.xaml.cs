using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using System.Xml;

namespace Panothelo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int gridColumn = 9;
        int gridRow = 7;

        public string TimePlayer1 { get; set; }
        public string TimePlayer2 { get; set; }

        public string ScorePlayer1 { get; set; }
        public string ScorePlayer2 { get; set; }

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

        int[] timerAddPlayer1;
        int[] timerAddPlayer2;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public MainWindow()
        {
            this.DataContext = this;

            InitializeComponent();
            InitGame();
            InitBoard();
            UpdateDisplay();
        }

        /// <summary>
        /// Update the grid display
        /// </summary>
        private void UpdateDisplay()
        {
            Label lblUpdate;

            foreach (int pos in listLastPossible)
            {
                lblUpdate = GameBoard.Children[pos] as Label;
            }
            listLastPossible.Clear();

            foreach (int i in board.GetPossibleMoves(turnPlayer1))
            {
                listLastPossible.Add(i);

                lblUpdate = GameBoard.Children[i] as Label;
                lblUpdate.Background = Brushes.LightGray;

            }

            int actualWinner = -1;
            int scoreW = board.GetWhiteScore();
            int scoreB = board.GetBlackScore();

            if (scoreW < scoreB)
                actualWinner = 0;
            else if (scoreW > scoreB)
                actualWinner = 1;

            if (actualWinner == 1)
                GameBoard.Background = Brushes.ForestGreen;
            else if (actualWinner == 0)
                GameBoard.Background = Brushes.SaddleBrown;

            if (board.CheckBoardFull())
            {
                MessageBox.Show(GetWinnerMessage(actualWinner));
            }
            else if (listLastPossible.Count == 0)
            {
                if (nbPass > 0)
                    MessageBox.Show(GetWinnerMessage(actualWinner));
                else
                {
                    turnPlayer1 = !turnPlayer1;
                    nbPass = 1;
                    UpdateDisplay();
                }
            }
            else
            {
                nbPass = 0;
            }
        }

        /// <summary>
        /// Update the score
        /// </summary>
        private void UpdateScore()
        {
            ScorePlayer1 = "Score : " + board.GetWhiteScore().ToString();
            OnPropertyChanged("ScorePlayer1");
            ScorePlayer2 = "Score : " + board.GetBlackScore().ToString();
            OnPropertyChanged("ScorePlayer2");
        }

        /// <summary>
        /// Winner message
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        private string GetWinnerMessage(int w)
        {
            if (turnPlayer1)
                swPlayer1.Stop();
            else
                swPlayer2.Stop();

            if (w == 1)
                return "Congratulation " + player1.Name + ",\nYou won!";
            else if (w == 0)
                return "Congratulation " + player2.Name + ",\nYou won!";
            else
                return "Congratulation to both,\nIt's a draw!";
        }

        /// <summary>
        /// Init data for the game
        /// </summary>
        private void InitGame()
        {
            blackPawn = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Stump.png")));
            whitePawn = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Leaf.png")));
            lblPlayerImage.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Panoramix.png")));

            player1 = new Player("Player1", whitePawn);
            player2 = new Player("Player2", blackPawn);

            lblNamePlayer1.Content = player1.Name;
            lblNamePlayer1.Background = Brushes.Green;

            lblNamePlayer2.Content = player2.Name;
            lblNamePlayer2.Background = Brushes.White;

            listLastPossible = new List<int>();

            board = new Board(gridColumn, gridRow);

            UpdateScore();

            timerAddPlayer1 = new int[2];
            timerAddPlayer2 = new int[2];

            turnPlayer1 = true;
            nbPass = 0;
            TimerInit();
        }

        /// <summary>
        /// Init data for the game from a file
        /// </summary>
        /// <param name="namePlayer1"></param>
        /// <param name="namePlayer2"></param>
        private void InitGameFromLoadFile(String namePlayer1, String namePlayer2)
        {
            player1 = new Player(namePlayer1, whitePawn);
            player2 = new Player(namePlayer2, blackPawn);

            lblNamePlayer1.Content = player1.Name;
            lblNamePlayer2.Content = player2.Name;
            if (turnPlayer1)
            {
                lblNamePlayer1.Background = Brushes.Green;
                lblNamePlayer2.Background = Brushes.White;
                lblPlayerImage.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Panoramix.png")));
            }
            else
            {
                lblNamePlayer2.Background = Brushes.Green;
                lblNamePlayer1.Background = Brushes.White;
                lblPlayerImage.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Panothelo;component/PawnImage/Romain.png")));
            }

            UpdateScore();
            TimerInit();
        }

        /// <summary>
        /// Init all label on the grid
        /// </summary>
        private void InitBoard()
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
            if (turnPlayer1)
                swPlayer1.Start();
            else
                swPlayer2.Start();
        }

        /// <summary>
        /// Init timer
        /// </summary>
        private void TimerInit()
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

        /// <summary>
        /// Timer tick every second
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object sender, EventArgs e)
        {
            if (sender == timerPlayer1)
            {
                TimePlayer1 = String.Format("{0:00}:{1:00}", swPlayer1.Elapsed.Minutes + timerAddPlayer1[0], swPlayer1.Elapsed.Seconds + timerAddPlayer1[1]);
                OnPropertyChanged("TimePlayer1");
            }
            else
            {
                TimePlayer2 = String.Format("{0:00}:{1:00}", swPlayer2.Elapsed.Minutes + timerAddPlayer2[0], swPlayer2.Elapsed.Seconds + timerAddPlayer2[1]);
                OnPropertyChanged("TimePlayer2");
            }
        }

        /// <summary>
        /// Witch player turn
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Mouse enter on grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Mouse leave on grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeaveGrid(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Label lblGrid = sender as Label;
            int col = Grid.GetColumn(lblGrid);
            int row = Grid.GetRow(lblGrid);
            int posClick = col * gridRow + row;

            if (listLastPossible.Contains(posClick))
            {
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Mouse down on grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                InitBoard();

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

                UpdateScore();
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Menu who exit the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuQuit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Menu for about
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Othello C# - HE-ARC \nAuteur : Feuillade Julien et Dubois Jeremy");
        }

        /// <summary>
        /// Menu for a new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuNew(object sender, RoutedEventArgs e)
        {
            GameBoard.Children.Clear();
            InitGame();
            InitBoard();
            UpdateDisplay();
        }

        /// <summary>
        /// Menu save the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuSave(object sender, RoutedEventArgs e)
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

                try
                {
                    using (XmlWriter writer = XmlWriter.Create(filename, settings))
                    {
                        writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");

                        writer.WriteStartElement("OthelloGame");

                        writer.WriteElementString("Turn", turnPlayer1.ToString());
                        writer.WriteElementString("Board", strBoard);

                        writer.WriteElementString("NamePlayer1", player1.Name);
                        writer.WriteElementString("TimePlayer1", swPlayer1.Elapsed.ToString("mm\\:ss"));

                        writer.WriteElementString("NamePlayer2", player2.Name);
                        writer.WriteElementString("TimePlayer2", swPlayer2.Elapsed.ToString("mm\\:ss"));

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
        /// Menu load data from a XML file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuLoad(object sender, RoutedEventArgs e)
        {
            if (turnPlayer1)
                swPlayer1.Stop();
            else
                swPlayer2.Stop();

            string strBoard = "";
            string[] strTabBoard;

            string namePlayer1 = "";
            string namePlayer2 = "";

            string timePlayer1 = "";
            string timePlayer2 = "";


            System.Windows.Forms.OpenFileDialog fileDiag = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                FilterIndex = 0,
                DefaultExt = "xml"
            };


            if (fileDiag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!String.Equals(Path.GetExtension(fileDiag.FileName),
                                   ".xml",
                                   StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Select an XML file.",
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

                    using (XmlReader reader = XmlReader.Create(fileDiag.FileName, settings))
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                if (reader.IsStartElement())
                                {
                                    switch (reader.Name)
                                    {
                                        case "NamePlayer1":
                                            if (reader.Read())
                                            {
                                                namePlayer1 = reader.Value.Trim();
                                            }
                                            break;

                                        case "NamePlayer2":
                                            if (reader.Read())
                                            {
                                                namePlayer2 = reader.Value.Trim();
                                            }
                                            break;

                                        case "TimePlayer1":
                                            if (reader.Read())
                                            {
                                                timePlayer1 = reader.Value.Trim();
                                            }
                                            break;

                                        case "TimePlayer2":
                                            if (reader.Read())
                                            {
                                                timePlayer2 = reader.Value.Trim();
                                            }
                                            break;

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
                            timerAddPlayer1[0] = int.Parse(timePlayer1.Split(':')[0]);
                            timerAddPlayer1[1] = int.Parse(timePlayer1.Split(':')[1]);

                            timerAddPlayer2[0] = int.Parse(timePlayer2.Split(':')[0]);
                            timerAddPlayer2[1] = int.Parse(timePlayer2.Split(':')[1]);

                            strTabBoard = strBoard.Split(',');
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
                            InitGameFromLoadFile(namePlayer1, namePlayer2);
                            InitBoard();
                            UpdateDisplay();

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
