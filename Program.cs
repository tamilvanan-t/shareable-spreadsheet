using System;
using System.Collections.Generic;
using System.Threading;


namespace Simulator
{
    class Program
    {

        static int rows = 10;
        static int cols = 5;
        static int nThreads = 50;
        static int nOperations = 10;
        SharableSpreadsheet sheet;

        //Write Mutexes
        private static Mutex setCellMutex = new Mutex();
        private static Mutex exchangeRowsMutex = new Mutex();
        private static Mutex exchangeColsMutex = new Mutex();
        private static Mutex addRowMutex = new Mutex();
        private static Mutex addColMutex = new Mutex();

        List<Thread> userThreads = new List<Thread>();

        public Program(SharableSpreadsheet sheet)
        {
            this.sheet = sheet;
        }

        static void Main()
        {
           /* rows = args[0];
            cols = args[1];
            nThreads = args[2];
            nOperations = args[3];*/


            //create a rows* cols spreadsheet
            SharableSpreadsheet sheet = new SharableSpreadsheet(rows, cols);

            //initialize values in Sheet
            InitializeValuesInSheet(sheet);

            //Weave Thread
            Program program = new Program(sheet);
            program.WeaveThread(nThreads);

            foreach (Thread userThread in program.userThreads)
            {
                userThread.Join();
            }

            program.printSheet();
        }

        private void printSheet()
        {
            string[,] sheetData = this.sheet.getSheet();

            for (int i = 1; i <= rows; i++)
            {
                for (int j = 1; j <= cols; j++)
                {
                    String value = sheetData[i - 1, j - 1];
                    Console.Write(value + "\t");
                }
                Console.WriteLine("");
            }
        }

        private void WeaveThread(int nThreads)
        {
            for(int i = 0; i < nThreads; i++)
            {
                Thread t = new Thread(() => dOperations(nOperations));
                t.Name = string.Format("User [{0}]:", i);
                userThreads.Add(t);
                t.Start();
            }
        }

        private void dOperations(int nOperations)
        {
            Dictionary<string, Func<int>> writeFunctions = new Dictionary<string, Func<int>>();
            writeFunctions["setCell"] = this.setCell;
            writeFunctions["exchangeRows"] = this.exchangeRows;
            writeFunctions["exchangeCols"] = this.exchangeCols;
            writeFunctions["addRow"] = this.addRow;
            writeFunctions["addCol"] = this.addCol;

            for (int i = 0; i < nThreads; i++)
            {
                int readOrWrite = GetRandomNumberBetween(1, 10);
                if(readOrWrite <= 5)
                {
                    //Read Operations using Semaphores
                    //Console.WriteLine("Read Operations using Semaphores");
                } else
                {
                    //Write Operations using Mutex
                    Func<int> randomMethod = getRandomMethod(writeFunctions);
                    if(randomMethod != null)
                    {
                        randomMethod();
                    }
                }
            }
        }

        private Func<int> getRandomMethod(Dictionary<string, Func<int>> writeFunctions)
        {
            int count = writeFunctions.Count;
            int randNum = GetRandomNumberBetween(0, count - 1);

            int loopCount = 0;
            foreach (KeyValuePair<string, Func<int>> ele in writeFunctions)
            {
                if(loopCount == randNum)
                {
                    return ele.Value;
                }
                loopCount++;
            }
            return null;
        }

        private int setCell()
        {
            setCellMutex.WaitOne();
            int row = GetRandomNumberBetween(0, rows - 1);
            int col = GetRandomNumberBetween(0, cols - 1);
            String word = GenerateRandomWord(GetRandomNumberBetween(1, 10));
            String threadName = Thread.CurrentThread.Name;

            this.sheet.setCell(row, col, word);
            
            Console.WriteLine(threadName + " String " + word + " set in cell " + row + " " + col);

            setCellMutex.ReleaseMutex();
            return 0;
        }

        private int addRow()
        {
            addRowMutex.WaitOne();
            int row = GetRandomNumberBetween(0, rows - 1);
            rows += 1;
            String threadName = Thread.CurrentThread.Name;

            this.sheet.addRow(row);

            Console.WriteLine(threadName + " Added a new row after row " + row);

            addRowMutex.ReleaseMutex();
            return 0;
        }

        private int addCol()
        {
            addColMutex.WaitOne();
            int col = GetRandomNumberBetween(0, cols - 1);
            rows += 1;
            String threadName = Thread.CurrentThread.Name;

            this.sheet.addCol(col);

            Console.WriteLine(threadName + " Added a new column after column " + col);

            addColMutex.ReleaseMutex();
            return 0;
        }

        private int exchangeRows()
        {
            exchangeRowsMutex.WaitOne();
            int row1 = GetRandomNumberBetween(0, rows - 1);
            int row2 = GetRandomNumberBetween(0, rows - 1);
            String threadName = Thread.CurrentThread.Name;

            this.sheet.exchangeRows(row1, row2);

            Console.WriteLine(threadName + " Exchanged the rows " + row1 + " and " + row2);

            exchangeRowsMutex.ReleaseMutex();
            return 0;
        }

        private int exchangeCols()
        {
            exchangeColsMutex.WaitOne();
            int col1 = GetRandomNumberBetween(0, cols - 1);
            int col2 = GetRandomNumberBetween(0, cols - 1);
            String threadName = Thread.CurrentThread.Name;

            this.sheet.exchangeCols(col1, col2);

            Console.WriteLine(threadName + " Exchanged the rows " + col1 + " and " + col2);

            exchangeColsMutex.ReleaseMutex();
            return 0;
        }

        private int GetRandomNumberBetween(int v1, int v2)
        {
            Random rand = new Random();
            int number = rand.Next(v1, v2);
            return number;
        }

        private static void InitializeValuesInSheet(SharableSpreadsheet sheet)
        {
            String[,] sheetValues = sheet.getSheet();
            for (int i = 1; i <= rows; i++)
            {
                for(int j = 1; j <= cols; j++)
                {
                    String value = "Cell_" + i + "_" + j;
                    sheetValues[i - 1, j - 1] = value;
                }
            }
        }

        private static String GenerateRandomWord(int len)
        {
            char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Random rand = new Random();
            // Make a word.
            string word = "";
            for (int j = 1; j <= len; j++)
            {
                // Pick a random number between 0 and 25
                // to select a letter from the letters array.
                int letter_num = rand.Next(0, letters.Length - 1);

                // Append the letter.
                word += letters[letter_num];
            }
            return word;
        }
    }
}