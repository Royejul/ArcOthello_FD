using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        List<int[]> listPossibleMoves;

        public Board(int col, int line)
        {
            this.nbCol = col;
            this.nbLin = line;
            listTokenP1 = new List<int[]>();
            listTokenP2 = new List<int[]>();
            listPossibleMoves = new List<int[]>();
            matBoard = new int[col, line];
            initBoard();
        }

        public int GetBlackScore()
        {
            int score = 0;
            for (int i=0; i<nbCol;i++)
            {
                for(int j=0;j<nbLin;j++)
                {
                    if (matBoard[i, j] == 1)
                        score++;
                }
            }
            return score;
            //return listTokenP2.Count;
        }

        public int[,] GetBoard()
        {
            return matBoard;
        }

        public string GetName()
        {
            return "IAName";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

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
            //return listTokenP1.Count;
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            if (checkLeft(column, line, isWhite))
                return true;
            if (checkRight(column, line, isWhite))
                return true;
            if (checkTop(column, line, isWhite))
                return true;
            if (checkBottom(column, line, isWhite))
                return true;
            if (checkDLT(column, line, isWhite))
                return true;
            if (checkDLB(column, line, isWhite))
                return true;
            if (checkDRT(column, line, isWhite))
                return true;
            if (checkDRB(column, line, isWhite))
                return true;
            return false;
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            int tok = 1;
            bool allOK = false;
            if (isWhite)
                tok = 0;
            if (checkLeft(column, line, isWhite))
            {
                swapToken(column - 1, line, -1, 0, tok);
                allOK = true;
            }
            if (checkRight(column, line, isWhite))
            {
                swapToken(column + 1, line, 1, 0, tok);
                allOK = true;
            }
            if (checkTop(column, line, isWhite))
            {
                swapToken(column, line - 1, 0, -1, tok);
                allOK = true;
            }
            if (checkBottom(column, line, isWhite))
            {
                swapToken(column, line + 1, 0, 1, tok);
                allOK = true;
            }
            if (checkDLT(column, line, isWhite))
            {
                swapToken(column + 1, line - 1, -1, -1, tok);
                allOK = true;
            }
            if (checkDLB(column, line, isWhite))
            {
                swapToken(column - 1, line + 1, -1, 1, tok);
                allOK = true;
            }
            if (checkDRT(column, line, isWhite))
            {
                swapToken(column + 1, line - 1, 1, -1, tok);
                allOK = true;
            }
            if (checkDRB(column, line, isWhite))
            {
                swapToken(column + 1, line + 1, -1, 1, tok);
                allOK = true;
            }
            if(allOK)
            {
                matBoard[column, line] = tok;
                return allOK;
            }
            return allOK;
        }

        public List<int[]> getPossibleMoves(bool isWhite)
        {
            listPossibleMoves.Clear();
            for (int i=0;i<nbCol;i++)
            {
                for(int j=0;j<nbLin;j++)
                {
                    if(matBoard[i,j]==-1)
                    {
                        if (IsPlayable(i, j, isWhite))
                        {
                            listPossibleMoves.Add(new int[] { i,j});
                        }
                    }
                }
            }
            listPossibleMoves.ForEach(el => Console.WriteLine(el[0] + ", " + el[1]));
            return listPossibleMoves;
        }

        // privates methods

        private void initBoard()
        {
            int condI = nbCol / 2;
            int condJ = nbLin / 2;
            for (int i=0; i<nbCol;i++)
            {
                for(int j=0; j<nbLin;j++)
                {
                    if ((i == condI && j == condJ) || (i == condI + 1 && j == condJ + 1))
                    {
                        matBoard[i, j] = 0;
                        listTokenP1.Add(new int[] { i, j });
                    }
                    else if ((i == condI + 1 && j == condJ) || (i == condI && j == condJ + 1))
                    {
                        matBoard[i, j] = 1;
                        listTokenP2.Add(new int[]{ i, j});
                    }
                    else
                        matBoard[i, j] = -1;
                }
            }
            listPossibleMoves.Add(new int[] { condI + 2, condJ });
            listPossibleMoves.Add(new int[] { condI + 1, condJ - 1 });
            listPossibleMoves.Add(new int[] { condI - 1, condJ + 1 });
            listPossibleMoves.Add(new int[] { condI, condJ + 2 });
        }

        private void swapToken(int col, int line, int stepI, int stepJ, int stopTok)
        {
            if (stopTok != matBoard[col, line])
            {
                matBoard[col, line] = stopTok;
                swapToken(col + stepI, line + stepJ, stepI, stepJ, stopTok);
            }
        }

        private bool checkLeft(int column, int line, bool isWhite)
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
                if (matBoard[i, line] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[i, line] == tokenE && okMove)
                    return true;
            }
            return false;
        }
        private bool checkRight(int column, int line, bool isWhite)
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
                if (matBoard[i, line] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[i, line] == tokenE && okMove)
                    return true;
            }
            return false;
        }
        private bool checkTop(int column, int line, bool isWhite)
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
                if (matBoard[column, j] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column, j] == tokenE && okMove)
                    return true;
            }
            return false;
        }
        private bool checkBottom(int column, int line, bool isWhite)
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
                if (matBoard[column, j] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column, j] == tokenE && okMove)
                    return true;
            }
            return false;
        }
        private bool checkDLT(int column, int line, bool isWhite)
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
                if (matBoard[column - s, line - s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column - s, line - s] == tokenE && okMove)
                    return true;
            }
            return false;
        }
        private bool checkDLB(int column, int line, bool isWhite)
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
            if (nbLin-1-line < column)
                it = nbLin - 1 - line;
            else
                it = column;
            for (int s = 1; s < it; s++)
            {
                if (matBoard[column - s, line + s] == -1)
                    return false;
                if (matBoard[column - s, line + s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column - s, line + s] == tokenE && okMove)
                    return true;
            }
            return false;
        }
        private bool checkDRT(int column, int line, bool isWhite)
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
            if (line < nbCol- 1 - column)
                it = line;
            else
                it = nbCol - 1 - column;
            for (int s = 1; s < it; s++)
            {
                if (matBoard[column + s, line - s] == -1)
                    return false;
                if (matBoard[column + s, line - s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column + s, line - s] == tokenE && okMove)
                    return true;
            }
            
            return false;
        }
        private bool checkDRB(int column, int line, bool isWhite)
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
                if (matBoard[column + s, line + s] == tokenS && !okMove)
                {
                    okMove = true;
                }
                else if (matBoard[column + s, line + s] == tokenE && okMove)
                    return true;
            }
            return false;
        }
    }
}
