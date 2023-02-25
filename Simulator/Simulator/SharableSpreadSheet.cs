using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Simulator
{
    using System;
    using System.IO;
    using System.Threading;

    class SharableSpreadSheet
    {
        public int Rows;
        public int Cols;
        List<List<string>> matrix;
        List<ReaderWriterLockSlim> RowMutex;
        static SemaphoreSlim semaphore;
        public Mutex rowAddMutex;
        public Mutex rowExcMutex;
        int numOfUsers;

        public SharableSpreadSheet(int nRows, int nCols, int nUsers = -1)
        {
            // nUsers used for setConcurrentSearchLimit, -1 mean no limit.
            // construct a nRows*nCols spreadsheet
            // create all the declared data members
            matrix = new List<List<string>>();
            RowMutex = new List<ReaderWriterLockSlim>();
            rowAddMutex = new Mutex();
            rowExcMutex = new Mutex();
            numOfUsers = nUsers;
            setConcurrentSearchLimit(nUsers);
            //semaphore = new SemaphoreSlim(nUsers);
            for (int i = 0; i < nRows; i++)
            {
                matrix.Add(new List<string>());
                RowMutex.Add(new ReaderWriterLockSlim());

                for (int j = 0; j < nCols; j++)
                {
                    matrix[i].Add(new string(""));
                }
            }

        }

        public String getCell(int row, int col)
        {
            // return the string at [row,col]

            semaphore.Wait();
            RowMutex[row].EnterReadLock();
            try
            {
                String returnString = new string(matrix[row][col]);
                return returnString;
            }
            catch (Exception e)
            {
                throw new ArgumentNullException(paramName: nameof(e), message: "Cant get the cell in Func getCell");
            }
            finally
            {
                RowMutex[row].ExitReadLock();
                semaphore.Release();
            }

            return null;
        }

        public void setCell(int row, int col, String str)
        {
            semaphore.Wait();
            RowMutex[row].EnterWriteLock();
            try
            {
                (matrix[row][col]) = str;
            }

            finally
            {
                RowMutex[row].ExitWriteLock();
                semaphore.Release();
            }
            // set the string at [row,col]

        }
        public Tuple<int, int> searchString(String str)
        {
            // return first cell indexes that contains the string (search from first row to the last row)
            semaphore.Wait();
            try
            {
                int row, col;
                for (int i = 0; i < matrix.Count; i++)
                {
                    RowMutex[i].EnterReadLock();
                    try
                    {
                        for (int j = 0; j < matrix[i].Count; j++)
                        {
                            if (str.Equals(matrix[i][j].ToString()))
                            {
                                row = i;
                                col = j;
                                return Tuple.Create(row, col);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentNullException(paramName: nameof(e), message: "Cant search the cell in Func searchString");
                    }
                    finally
                    {
                        RowMutex[i].ExitReadLock();
                    }
                }
                return null;

            }
            finally
            {
                semaphore.Release();
            }
        }



        public void exchangeRows(int row1, int row2)
        {
            int n = 0;
            semaphore.Wait();
            try
            {
                if (row1 == row2)
                {
                    return;
                }

                if (row1 > row2)
                {
                    n = row1;
                    row1 = row2;
                    row2 = n;
                }
                RowMutex[row2].EnterWriteLock();
                RowMutex[row1].EnterWriteLock();

                try
                {

                    List<String> node1 = matrix[row1];
                    List<String> node2 = matrix[row2];

                    matrix[row1] = node2;
                    matrix[row2] = node1;

                }
                catch (Exception e)
                {
                    throw new ArgumentNullException(paramName: nameof(e), message: "Cant exchange Rows in Func exchangeRows");
                }
                finally
                {
                    RowMutex[row2].ExitWriteLock();
                    RowMutex[row1].ExitWriteLock();
                }
            }
            finally
            {
                semaphore.Release();
            }


            // exchange the content of row1 and row2


        }
        public void exchangeCols(int col1, int col2)
        {
            String temp1;
            String temp2;

            int row;
            semaphore.Wait();
            try
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    RowMutex[i].EnterWriteLock();
                    try
                    {
                        temp1 = matrix[i][col1];
                        temp2 = matrix[i][col2];

                        matrix[i][col1] = temp2;
                        matrix[i][col2] = temp1;

                    }
                    catch (Exception e)
                    {
                        throw new ArgumentNullException(paramName: nameof(e), message: "Cant exchange Cols in Func exchangeCols");
                    }
                    finally
                    {
                        RowMutex[i].ExitWriteLock();
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }


        public int searchInRow(int row, String str)
        {
            int col;
            semaphore.Wait();
            try
            {
                RowMutex[row].EnterReadLock();

                try
                {
                    for (int i = 0; i < row; i++)
                    {
                        if (str.Equals(matrix[row][i].ToString()))
                        {
                            col = i;
                            return col;

                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentNullException(paramName: nameof(e), message: "Cant search In Row in Func searchInRow");
                }
                finally
                {
                    RowMutex[row].ExitReadLock();

                }
            }

            finally
            {
                semaphore.Release();
            }
            return -1;
            // perform search in specific row
        }


        public int searchInCol(int col, String str)
        {
            int row;
            semaphore.Wait();
            try
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    RowMutex[i].EnterReadLock();
                    try
                    {
                        if (str.Equals(matrix[i][col].ToString()))
                        {
                            row = i;
                            return row;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentNullException(paramName: nameof(e), message: "Cant search In Col in Func searchInCol");
                    }
                    finally
                    {
                        RowMutex[i].ExitReadLock();
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
            return -1;
            // perform search in specific col
        }



        public Tuple<int, int> searchInRange(int col1, int col2, int row1, int row2, String str)
        {

            semaphore.Wait();
            try
            {
                for (int row = row1; row <= row2; row++)
                {
                    RowMutex[row].EnterReadLock();
                    try
                    {
                        for (int col = col1; col <= col2; col++)
                        {
                            if ((str.Equals(matrix[row][col].ToString())))
                            {
                                return Tuple.Create(row, col);

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentNullException(paramName: nameof(e), message: "Cant search In Range in Func searchInRange");
                    }
                    finally
                    {
                        RowMutex[row].ExitReadLock();

                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
            return null;

            // perform search within spesific range: [row1:row2,col1:col2] 
            //includes col1,col2,row1,row2

        }


        public void addRow(int row1)
        {

            if (row1 == 0)
            {
                return;
            }

            List<string> newRow = new List<string>();
            for (int n = 0; n < matrix[0].Count; n++)
            {
                newRow.Add(new string(""));
            }
            ReaderWriterLockSlim newLock = new ReaderWriterLockSlim();

            rowAddMutex.WaitOne();
            matrix.Add(newRow);
            RowMutex.Add(newLock);
            int j = matrix.Count - 2;
            int i = matrix.Count - 1;
            rowAddMutex.ReleaseMutex();


            while (i != row1)
            {

                RowMutex[i].EnterWriteLock();
                RowMutex[j].EnterWriteLock();
                try
                {
                    List<String> node1 = matrix[j];
                    List<String> node2 = matrix[i];

                    matrix[j] = node2;
                    matrix[i] = node1;

                }
                catch (Exception e)
                {
                    throw new ArgumentNullException(paramName: nameof(e), message: "Cant add Row in Func addRow");
                }
                finally
                {
                    RowMutex[i].ExitWriteLock();
                    RowMutex[j].ExitWriteLock();
                    i--;
                    j--;
                }

            }
            //add a row after row1

        }


        public void addCol(int col1)
        {
            semaphore.Wait();
            try
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    RowMutex[i].EnterWriteLock();
                    try
                    {
                        matrix[i].Insert(col1, new string(""));
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentNullException(paramName: nameof(e), message: "Cant add Col in Func addCol");
                    }
                    finally
                    {
                        RowMutex[i].ExitWriteLock();
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }

            //add a column after col1
        }

        public Tuple<int, int>[] findAll(String str, bool caseSensitive)
        {
            List<Tuple<int, int>> TupleList = new List<Tuple<int, int>>();
            int col;
            semaphore.Wait();
            try
            {
                for (int row = 0; row < matrix.Count; row++)
                {
                    RowMutex[row].EnterReadLock();
                    try
                    {
                        for (int i = 0; i < matrix[row].Count; i++)
                        {
                            if (caseSensitive == false)
                            {
                                if (string.Equals(str, matrix[row][i], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    col = i;
                                    TupleList.Add(Tuple.Create(row, col));
                                }
                            }
                            else
                            {
                                if ((str.Equals(matrix[row][i].ToString())))
                                {
                                    col = i;
                                    TupleList.Add(Tuple.Create(row, col));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentNullException(paramName: nameof(e), message: "Cant find All in Func findAll");
                    }
                    finally
                    {
                        RowMutex[row].ExitReadLock();

                    }
                }
            }
            finally
            {
                semaphore.Release();
            }

            Tuple<int, int>[] TupleArray = new Tuple<int, int>[TupleList.Count];
            for (int i = 0; i < TupleList.Count; i++)
            {
                TupleArray[i] = TupleList[i];
            }

            return TupleArray;
            // perform search in specific row
            // perform search and return all relevant cells according to caseSensitive param

        }





        public void setAll(String oldStr, String newStr, bool caseSensitive)
        {
            //the semaphore alredy Updated in the find all fun
            Tuple<int, int>[] tupleArray = findAll(oldStr, caseSensitive);
            for (int i = 0; i < tupleArray.Length; i++)
            {
                RowMutex[tupleArray[i].Item1].EnterWriteLock();
                try
                {
                    matrix[tupleArray[i].Item1][tupleArray[i].Item2] = newStr;
                }
                catch (Exception e)
                {
                    throw new ArgumentNullException(paramName: nameof(e), message: "Cant set All in Func setAll");
                }
                finally
                {
                    RowMutex[tupleArray[i].Item1].ExitWriteLock();
                }
            }
            // replace all oldStr cells with the newStr str according to caseSensitive param
        }


        public Tuple<int, int> getSize()
        {
            semaphore.Wait();
            try
            {
                int nRows;
                int nCols;
                nRows = matrix.Count;
                nCols = matrix[Rows].Count;
                return Tuple.Create(nRows, nCols);
            }
            catch (Exception e)
            {
                throw new ArgumentNullException(paramName: nameof(e), message: "Cant get Size in Func getSize");
            }
            finally
            {
                semaphore.Release();
            }
            return null;
        }

        public void setConcurrentSearchLimit(int nUsers)
        {
            try
            {
                if (nUsers == -1)
                {
                    semaphore = new SemaphoreSlim(1000000000);
                }

                else
                {
                    semaphore = new SemaphoreSlim(nUsers);

                }
            }

            catch (Exception e)
            {
                throw new ArgumentNullException(paramName: nameof(e), message: "Cant set Concurrent Search Limit in Func setConcurrentSearchLimit");
            }
            // this function aims to limit the number of users that can perform the search operations concurrently.
            // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
            // In this case additional search operations will wait for existing search to finish.
            // This function is used just in the creation
        }

        public void print()
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    Console.Write(matrix[i][j]);
                }
                Console.WriteLine();

            }
        }

        public void save(String fileName)
        {
            // save the spreadsheet to a file fileName.
            // you can decide the format you save the data. There are several options.
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter("C:\\Sample.txt");
                // amount of rows and cols and nUsers
                sw.WriteLine(matrix.Count);
                sw.WriteLine(matrix[0].Count);
                sw.WriteLine(numOfUsers);

                // ever row is a cell, end of row is the string -> "next row"
                for (int i = 0; i < matrix.Count; i++)
                {
                    for (int j = 0; j < matrix[0].Count; j++)
                    {
                        sw.WriteLine(matrix[i][j]);
                    }
                    sw.WriteLine("next row");
                }
                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                throw new ArgumentNullException(paramName: nameof(e), message: "Cant save the Sharable Spread Sheet in Func save");
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
        public void load(String fileName)
        {
            // load the spreadsheet from fileName
            // replace the data and size of the current spreadsheet with the loaded data
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("C:\\Sample.txt");
                //Read the first line of text
                int numberOfRows = Int32.Parse(sr.ReadLine());
                int numberOfCols = Int32.Parse(sr.ReadLine());
                line = sr.ReadLine();
                int numberOfnUsers = Int32.Parse(line);
                int row = 0;
                int col = 0;
                List<List<string>> newMatrix = new List<List<string>>();
                for (int i = 0; i < numberOfRows; i++)
                {
                    newMatrix.Add(new List<string>());
                }
                //Continue to read until you reach end of file
                while (line != null)
                {
                    line = sr.ReadLine();
                    if (string.Equals(line, "next row"))
                    {
                        row += 1;
                    }
                    else
                    {
                        newMatrix[row].Add(new string(line));
                        col += 1;
                    }
                }
                //close the file
                sr.Close();
                //Console.ReadLine();
                this.matrix = newMatrix;
            }
            catch (Exception e)
            {
                throw new ArgumentNullException(paramName: nameof(e), message: "Cant load the Sharable Spread Sheet in Func load");
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
    }
}