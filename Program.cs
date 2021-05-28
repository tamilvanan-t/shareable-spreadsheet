using System;
using System.Collections.Generic;
using System.Threading;


namespace Simulator
{
    class Program
    {

        static int rows;
        static int cols;
        static int nThreads;
        static int nOperations;


        static void Main(int[] args)
        {
            rows = args[0];
            cols = args[1];
            nThreads = args[2];
            nOperations = args[3];


            //create a rows* cols spreadsheet
            SharableSpreadsheet sheet = new SharableSpreadsheet(rows, cols);

            //Filling the spreadsheet with prepared strings
            for (int r = 0; r < rows; r++)//r=0?fixed maybe
            {
                for (int c = 1; c < cols; c++)

                {
                    sheet.setCell(r, c, String.Format("tcell{0}{1}", r + 1, c + 1));
                }

            }
        }
    }
}