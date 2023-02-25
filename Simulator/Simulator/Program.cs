using System;
using System.Threading;

namespace Simulator
{
    internal class Program
    {
        static Random random;
        static Program program;
        static SharableSpreadSheet spreadSheet;
        static int rows;
        static int cols;
        static int nUsers;
        static int nThreads;
        static int nOperations;
        static int sleep;
        static void Main(string[] args)
        {
            rows = Int32.Parse(args[0]);
            cols = Int32.Parse(args[1]);
            nUsers = Int32.Parse(args[2]);
            program = new Program();
            spreadSheet = new SharableSpreadSheet(rows, cols, nUsers);
            sleep = Int32.Parse(args[4]);

            nThreads = nUsers;
            nOperations = Int32.Parse(args[3]);
            random = new Random();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    spreadSheet.setCell(i, j, "testcell" + i + j);
                }
            }

            for (int i = 0; i < nThreads; i++)
            {
                Thread thread1 = new Thread(() => program.operation());
                thread1.Start();
            }

            //spreadSheet.save("avram");
        }


        private void operation()
        {
            try {
                Thread thread1 = null;
                int prevRandRow = 0;
                int prevRandCol = 0;
                for (int j = 0; j < nOperations; j++)
                {
                    int rand = random.Next(14);
                    prevRandRow = random.Next(rows);
                    prevRandCol = random.Next(rows);
                    int randRow = random.Next(rows);
                    int randCol = random.Next(cols);
                    string rbrb = "";
                    //Console.WriteLine("this is rand: " + rand);
                    if (rand == 0)
                    {
                        rbrb = spreadSheet.getCell(randRow, randCol);
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} Returned String " + rbrb + " from Cell:(" + randRow + "," + randCol + ")");
                    }
                    else if (rand == 1)
                    {
                        spreadSheet.setCell(randRow, randCol, "Changed");
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} String 'Changed' inserted to Cell:(" + randRow + "," + randCol + ")");
                    }
                    else if (rand == 2)
                    {
                        spreadSheet.searchString("Changed");
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} String 'Changed' has been found first in Cell:(" + randRow + "," + randCol + ")");
                    }

                    else if (rand == 3)
                    {
                        spreadSheet.exchangeRows(randRow, randCol);
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} has exchanged between rows:(" + randRow + "," + randCol + ")");
                    }

                    else if (rand == 4)
                    {
                        spreadSheet.exchangeCols(randRow, randCol);
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} has exchanged between columns:(" + randRow + "," + randCol + ")");
                    }

                    else if (rand == 5)
                    {
                        int r = 0;
                        r = spreadSheet.searchInRow(randRow, "Changed");
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} has found String 'Changed' int row:" + r);

                    }

                    else if (rand == 6)
                    {
                        int r = 0;
                        spreadSheet.searchInCol(randCol, "Changed");
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} has found String 'Changed' int row:" + r);
                    }

                    else if (rand == 7)
                    {
                        Tuple<int, int> TupleArray = new Tuple<int, int>(0, 0);
                        TupleArray = spreadSheet.searchInRange(randRow, randCol, prevRandRow, prevRandCol, "Changed");
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} search in range found:" + TupleArray);
                    }

                    else if (rand == 8)
                    {
                        spreadSheet.addRow(randRow);
                        rows++;
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "}added an empty row");

                    }

                    else if (rand == 9)
                    {
                        spreadSheet.addCol(randRow);
                        cols++;
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "}added an empty col");
                    }

                    else if (rand == 10)
                    {
                        spreadSheet.findAll("Changed", true);
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} has finding Strings 'Changed' in the follwing Cells:");
                    }

                    else if (rand == 11)
                    {
                        spreadSheet.setAll("Changed", "falafel", false);
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} set the entire spreatsheet to 'falafel'");

                    }

                    else
                    {
                        Tuple<int, int> TupleArray = new Tuple<int, int>(0, 0);
                        TupleArray = spreadSheet.getSize();
                        //rows = TupleArray.Item1;
                        //cols = TupleArray.Item2;
                        Console.WriteLine("USER: {" + Thread.CurrentThread.ManagedThreadId + "} get spreadsheet size:" + TupleArray.ToString());
                    }

                    Thread.Sleep(sleep);
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}