using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Panothelo
{
    class Board : IPlayable.IPlayable
    {
        //Attributs
        private int[,] matBoard;
        private int nbCol;
        private int nbLin;

        private List<int[]> listTokenP1;
        private List<int[]> listTokenP2;

        List<int> listPossibleMoves;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="col"></param>
        /// <param name="line"></param>
        public Board(int col, int line)
        {
            this.nbCol = col;
            this.nbLin = line;
            listTokenP1 = new List<int[]>();
            listTokenP2 = new List<int[]>();
            listPossibleMoves = new List<int>();
            matBoard = new int[col, line];
            InitBoard();
        }

        /// <summary>
        /// Get the score of the black player
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            int score = 0;
            for (int i = 0; i < nbCol; i++)
            {
                for (int j = 0; j < nbLin; j++)
                {
                    if (matBoard[i, j] == 1)
                        score++;
                }
            }
            return score;
        }

        /// <summary>
        /// Get the board
        /// </summary>
        /// <returns></returns>
        public int[,] GetBoard()
        {
            return matBoard;
        }

        /// <summary>
        /// Get the name of the IA
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "IAFD";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the score of the white player
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        {
            int score = 0;
            for (int i = 0; i < nbCol; i++)
            {
                for (int j = 0; j < nbLin; j++)
                {
                    if (matBoard[i, j] == 0)
                        score++;
                }
            }
            return score;
        }

        /// <summary>
        /// Check if the move is playable
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            if (CheckLeft(column, line, isWhite))
                return true;
            if (CheckRight(column, line, isWhite))
                return true;
            if (CheckTop(column, line, isWhite))
                return true;
            if (CheckBottom(column, line, isWhite))
                return true;
            if (CheckDiagLeftTop(column, line, isWhite))
                return true;
            if (CheckDiagLeftBottom(column, line, isWhite))
                return true;
            if (CheckDiagRightTop(column, line, isWhite))
                return true;
            if (CheckDiagRightBottom(column, line, isWhite))
                return true;
            return false;
        }

        /// <summary>
        /// Play the move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            int tok = 1;
            bool allOK = false;
            if (isWhite)
                tok = 0;
            if (CheckLeft(column, line, isWhite))
            {
                SwapPawn(column - 1, line, -1, 0, tok);
                allOK = true;
            }
            if (CheckRight(column, line, isWhite))
            {
                SwapPawn(column + 1, line, 1, 0, tok);
                allOK = true;
            }
            if (CheckTop(column, line, isWhite))
            {
                SwapPawn(column, line - 1, 0, -1, tok);
                allOK = true;
            }
            if (CheckBottom(column, line, isWhite))
            {
                SwapPawn(column, line + 1, 0, 1, tok);
                allOK = true;
            }
            if (CheckDiagLeftTop(column, line, isWhite))
            {
                SwapPawn(column - 1, line - 1, -1, -1, tok);
                allOK = true;
            }
            if (CheckDiagLeftBottom(column, line, isWhite))
            {
                SwapPawn(column - 1, line + 1, -1, 1, tok);
                allOK = true;
            }
            if (CheckDiagRightTop(column, line, isWhite))
            {
                SwapPawn(column + 1, line - 1, 1, -1, tok);
                allOK = true;
            }
            if (CheckDiagRightBottom(column, line, isWhite))
            {
                SwapPawn(column + 1, line + 1, 1, 1, tok);
                allOK = true;
            }
            if (allOK)
            {
                matBoard[column, line] = tok;
                return allOK;
            }
            return allOK;
        }

        /// <summary>
        /// Get in a list all the possible moves
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public List<int> GetPossibleMoves(bool isWhite)
        {
            listPossibleMoves.Clear();
            for (int i = 0; i < nbCol; i++)
            {
                for (int j = 0; j < nbLin; j++)
                {
                    if (matBoard[i, j] == -1)
                    {
                        if (IsPlayable(i, j, isWhite))
                        {
                            listPossibleMoves.Add(i * nbLin + j);
                        }
                    }
                }
            }
            listPossibleMoves.ForEach(el => Console.WriteLine(el));
            return listPossibleMoves;
        }

        /// <summary>
        /// Check the board if it is full
        /// </summary>
        /// <returns></returns>
        public bool CheckBoardFull()
        {
            for (int i = 0; i < nbCol; i++)
            {
                for (int j = 0; j < nbLin; j++)
                {
                    if (matBoard[i, j] == -1)
                        return false;
                }
            }
            return true;
        }


        // privates methods


        /// <summary>
        /// Init the board
        /// </summary>
        private void InitBoard()
        {
            int condI = nbCol / 2;
            int condJ = nbLin / 2;
            for (int i = 0; i < nbCol; i++)
            {
                for (int j = 0; j < nbLin; j++)
                {
                    if ((i == condI && j == condJ) || (i == condI + 1 && j == condJ + 1))
                    {
                        matBoard[i, j] = 0;
                        listTokenP1.Add(new int[] { i, j });
                    }
                    else if ((i == condI + 1 && j == condJ) || (i == condI && j == condJ + 1))
                    {
                        matBoard[i, j] = 1;
                        listTokenP2.Add(new int[] { i, j });
                    }
                    else
                        matBoard[i, j] = -1;
                }
            }
        }

        /// <summary>
        /// Swap a pawn in the board
        /// </summary>
        /// <param name="col"></param>
        /// <param name="line"></param>
        /// <param name="stepI"></param>
        /// <param name="stepJ"></param>
        /// <param name="stopTok"></param>
        private void SwapPawn(int col, int line, int stepI, int stepJ, int stopTok)
        {
            if (stopTok != matBoard[col, line])
            {
                Console.WriteLine(col + " : " + line);
                matBoard[col, line] = stopTok;
                SwapPawn(col + stepI, line + stepJ, stepI, stepJ, stopTok);
            }
        }

        /// <summary>
        /// Check the left move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckLeft(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            for (int i = column - 1; i > -1; i--)
            {
                if (matBoard[i, line] == -1)
                    return false;
                if (matBoard[i, line] == tokenE && i > column - 2)
                    return false;
                else if (matBoard[i, line] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[i, line] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the right move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckRight(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            for (int i = column + 1; i < nbCol; i++)
            {
                if (matBoard[i, line] == -1)
                    return false;
                if (matBoard[i, line] == tokenE && i < column + 2)
                    return false;
                else if (matBoard[i, line] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[i, line] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the top move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckTop(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            for (int j = line - 1; j > -1; j--)
            {
                if (matBoard[column, j] == -1)
                    return false;
                if (matBoard[column, j] == tokenE && j > line - 2)
                    return false;
                else if (matBoard[column, j] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column, j] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the bottom move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckBottom(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            for (int j = line + 1; j < nbLin; j++)
            {
                if (matBoard[column, j] == -1)
                    return false;
                if (matBoard[column, j] == tokenE && j < line + 2)
                    return false;
                else if (matBoard[column, j] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column, j] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the diagonal left to top move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckDiagLeftTop(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            int it = 0;
            if (line < column)
                it = line;
            else
                it = column;
            for (int s = 1; s < it; s++)
            {
                if (matBoard[column - s, line - s] == -1)
                    return false;
                if (matBoard[column - s, line - s] == tokenE && s < 2)
                    return false;
                else if (matBoard[column - s, line - s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column - s, line - s] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the diagonal left to bottom move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckDiagLeftBottom(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            int it = 0;
            if (nbLin - 1 - line < column)
                it = nbLin - 1 - line;
            else
                it = column;
            for (int s = 1; s < it; s++)
            {
                if (matBoard[column - s, line + s] == -1)
                    return false;
                if (matBoard[column - s, line + s] == tokenE && s < 2)
                    return false;
                else if (matBoard[column - s, line + s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column - s, line + s] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the diagonal right to top move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckDiagRightTop(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            int it = 0;
            if (line < nbCol - 1 - column)
                it = line;
            else
                it = nbCol - 1 - column;
            for (int s = 1; s < it; s++)
            {
                if (matBoard[column + s, line - s] == -1)
                    return false;
                if (matBoard[column + s, line - s] == tokenE && s < 2)
                    return false;
                else if (matBoard[column + s, line - s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column + s, line - s] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the diagonal right to bottom move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private bool CheckDiagRightBottom(int column, int line, bool isWhite)
        {
            int tokenS = 0;
            int tokenE = 1;
            bool okMove = false;
            if (isWhite)
            {
                tokenS = 1;
                tokenE = 0;
            }
            int it = 0;
            if (nbLin - 1 - line < nbCol - 1 - column)
                it = nbLin - 1 - line;
            else
                it = nbCol - 1 - column;
            for (int s = 1; s < it; s++)
            {
                if (matBoard[column + s, line + s] == -1)
                    return false;
                if (matBoard[column + s, line + s] == tokenE && s < 2)
                    return false;
                else if (matBoard[column + s, line + s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column + s, line + s] == tokenE && okMove)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Put a board to string for the save in XML
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string boardToString = "";
            for (int i = 0; i < nbCol; i++)
            {
                for (int j = 0; j < nbLin; j++)
                {
                    boardToString += matBoard[i, j] + ",";
                }
            };
            return boardToString;
        }
    }
}
