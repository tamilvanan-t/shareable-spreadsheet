using System;
using System.IO;
using System.Threading;

//new inside simulator
namespace Simulator
{
    class SharableSpreadsheet
    {
        private string[,] a;

        public string[,] getSheet()
        {
            return this.a;
        }

        public SharableSpreadsheet(int nRows, int nCols)
        {
            // construct a nRows*nCols spreadsheet
            this.a = new string[nRows, nCols];
        }

        //*READ*
        public String getCell(int row, int col)
        {
            // return the string at [row,col]
            if (row >= this.a.GetLength(0) || col >= this.a.GetLength(1))
                return null;
            return this.a[row, col];
        }

        //*WRITE*
        public bool setCell(int row, int col, String str)
        {
            lock(this.a)
            {
                if (row >= this.a.GetLength(0) || col >= this.a.GetLength(1))
                    return false;
                // set the string at [row,col]
                this.a[row, col] = str;
            }
            return true;
        }

        //*READ*
        public bool searchString(String str, ref int row, ref int col)
        {
            // search the cell with string str, and return true/false accordingly.
            // stores the location in row,col.
            // return the first cell that contains the string (search from first row to the last row)

            for (int i = 0; i < this.a.GetLength(0); i++)
            {
                for (int j = 0; j < this.a.GetLength(1); j++)
                {
                    if (this.a[i, j].Equals(str))
                    {
                        row = i;
                        col = j;
                        return true;
                    }
                }
            }
            return false;
        }

        //*WRITE*
        public bool exchangeRows(int row1, int row2)
        {
            if ((row1 > this.a.GetLength(0)) || (row2 > this.a.GetLength(0)))
                return false;
            // exchange the content of row1 and row2
            lock(this.a)
            {
                for (int i = 0; i < this.a.GetLength(1); i++)
                {
                    string s = this.a[row1, i];
                    this.a[row1, i] = this.a[row2, i];
                    this.a[row2, i] = s;
                }
            }
            return true;
        }

        //*WRITE*
        public bool exchangeCols(int col1, int col2)
        {
            if ((col1 > this.a.GetLength(1)) || (col2 > this.a.GetLength(1)))
                return false;
            // exchange the content of col1 and col2
            lock (this.a)
            {
                for (int i = 0; i < this.a.GetLength(0); i++)
                {
                    string s = this.a[i, col1];
                    this.a[i, col1] = this.a[i, col2];
                    this.a[i, col2] = s;
                }
            }
            return true;
        }

        //*READ*
        public bool searchInRow(int row, String str, ref int col)
        {
            if ((row > this.a.GetLength(0)))
                return false;
            // perform search in specific row
            for (int i = 0; i < this.a.GetLength(1); i++)
            {
                if (this.a[row, i].Equals(str))
                {
                    col = i;
                    return true;
                }
            }
            return false;
        }


        //*READ*
        public bool searchInCol(int col, String str, ref int row)
        {
            if ((col > this.a.GetLength(1)))
                return false;
            // perform search in specific col
            for (int i = 0; i < this.a.GetLength(0); i++)
            {
                if (this.a[i, col].Equals(str))
                {
                    row = i;
                    return true;
                }
            }
            return false;
        }

        //*READ*
        public bool searchInRange(int col1, int col2, int row1, int row2, String str, ref int row, ref int col)
        {
            // perform search within spesific range: [row1:row2,col1:col2] 
            //includes col1,col2,row1,row2
            if ((row1 > this.a.GetLength(0)) || (row2 > this.a.GetLength(0)) || (col1 > this.a.GetLength(1)) || (col2 > this.a.GetLength(1)))
                return false;
            for (int i = row1; i < row2; i++)
            {
                for (int j = col1; j < col2; j++)
                {
                    if (this.a[i, j].Equals(str))
                    {
                        row = i;
                        col = j;
                        return true;
                    }
                }
            }
            return false;
        }

        //*WRITE*
        public bool addRow(int row1)
        {
            if (row1 > this.a.GetLength(0))
                return false;
            //add a row after row1
            lock (this.a)
            {
                string[,] arr = new string[this.a.GetLength(0) + 1, this.a.GetLength(1)];
                for (int i = 0; i < row1; i++)
                {
                    for (int j = 0; j < this.a.GetLength(1); j++)

                    {
                        arr[i, j] = this.a[i, j];
                    }
                }
                for (int j = 0; j < this.a.GetLength(1); j++)
                {
                    arr[row1, j] = "AddedRow_" + row1 + "_" + j;
                }
                for (int i = row1 + 1; i < this.a.GetLength(0) + 1; i++)
                {
                    for (int j = 0; j < this.a.GetLength(1); j++)
                    {
                        arr[i, j] = this.a[i - 1, j];
                    }
                }
                this.a = arr;
            }
            return true;
        }

        //*WRITE*
        public bool addCol(int col1)
        {
            if (col1 > this.a.GetLength(1))
                return false;

            lock (this.a)
            {
                int rows = this.a.GetLength(0);
                int cols = this.a.GetLength(1) + 1;

                String[,] arr = new String[rows, cols];
                String[,] arrSplit1 = new String[rows, col1];
                String[,] arrSplit2 = new String[rows, this.a.GetLength(1) - col1];

                for (int i = 0; i < rows; i++)
                {
                    int split2Index = 0;
                    for (int j = 0; j < this.a.GetLength(1); j++)
                    {
                        if (j < col1)
                        {
                            arrSplit1[i, j] = this.a[i, j];
                        }
                        else
                        {
                            arrSplit2[i, split2Index] = this.a[i, j];
                            split2Index++;
                        }

                    }
                }

                for (int i = 0; i < rows; i++)
                {
                    int split2Index = 0;
                    for (int j = 0; j < cols; j++)
                    {
                        if (j < col1)
                        {
                            arr[i, j] = arrSplit1[i, j];
                        }
                        else if (j == col1)
                        {
                            arr[i, j] = "Added Column_" + i + "_" + j;
                        }
                        else
                        {
                            arr[i, j] = arrSplit2[i, split2Index];
                            split2Index++;
                        }
                    }
                }

                this.a = arr;
            }
            //add a column after col1
            return true;
        }

        //*READ*
        public void getSize(ref int nRows, ref int nCols)
        {
            // return the size of the spreadsheet in nRows, nCols
            nRows = this.a.GetLength(0);
            nCols = this.a.GetLength(1);
        }
        public bool setConcurrentSearchLimit(int nUsers)
        {
            bool result = Program.UpdateConcurrentSearchThreadCount(nUsers);
            return result;
        }
        public bool save(String fileName)
        {
            lock(this.a)
            {
                int colsCount = this.a.GetLength(0);
                String[] s = new string[colsCount];

                for(int i = 0; i < colsCount; i++)
                {
                    for (int j = 0; j < this.a.GetLength(1); j++)
                    {
                        if(j > 0)
                        {
                            s[i] += ",";
                        }
                        s[i] += this.a[i, j];
                    }
                }
                
                File.WriteAllLines(fileName, s);
            }
            return true;
        }
        public bool load(String fileName)
        {
            // load the spreadsheet from fileName
            // replace the data and size of the current spreadsheet with the loaded data
            return true;
        }


    }


}