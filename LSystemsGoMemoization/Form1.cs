// L-Systems Go with Memoization by Logan Cantin

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace LSystemsGoMemoization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            StartSolution();
        }

        Hashtable rules;

        Hashtable[] memoTables;

        private void StartSolution()
        {
            // VARIABLE USED TO TEST THE DIFFERENCE IN SPEED. Try it with false first to establish an idea of how long it takes,
            // then turn it to true to see the extreme speed boost
            bool USE_MEMO = true;

            // Path of the data file (both test files are in the same folder so just change the last number (Data2x.txt)
            string path = Application.StartupPath + @"\Data21.txt";

            // Getting data
            string[] data = System.IO.File.ReadAllLines(path);

            // Pivot point
            int counter = 0;

            // Iterating through cases
            for (int CaseNum = 0; CaseNum < 10; CaseNum++)
            {
                // Splitting the first line to isolate the important info
                string[] firstLine = data[counter].Split(' ');

                // Getting important info
                int numRules = Convert.ToInt32(firstLine[0]);
                int numIterations = Convert.ToInt32(firstLine[1]);
                string axiom = firstLine[2];

                // Creating a new hashtable for the rules
                rules = new Hashtable();

                // Inputting all rules into the hashtable
                for (int line = 1; line <= numRules; line++)
                {
                    // getting the current line and splitting it
                    string[] currentLine = data[line + counter].Split(' ');

                    // Getting the key and value of the rule
                    char key = Convert.ToChar(currentLine[0]);
                    string value = currentLine[1];

                    // Adding it to the rules hashtable
                    rules.Add(key, value);
                }

                // Populate the memotable array if using it
                if (USE_MEMO) PopulateMemoTables(numIterations);

                // Determining the output information
                ulong length = GetLength(axiom, numIterations, USE_MEMO);
                string firstChar = GetFirstChar(axiom[0], numIterations);
                string lastChar = GetLastChar(axiom[axiom.Length - 1], numIterations);

                // Outputting to console
                Console.WriteLine(firstChar + lastChar + " " + length);

                // Moving pivot point forwards
                counter += 1 + numRules;
            }
        }

        // Populates memoTables array with new Hashtables
        private void PopulateMemoTables(int depth)
        {
            // Create a new array
            memoTables = new Hashtable[depth + 1]; // depth + 1 because depth is 1 based, so it makes it easier to be able to use 1 based indexes on the array too

            // Create new hashtables in each index of the array
            for (int i = 0; i <= depth; i++)
            {
                memoTables[i] = new Hashtable();
            }
        }

        // GetLength with string parameter calls getlength on each character of the string
        private ulong GetLength(string axiom, int numIterations, bool useMemo) // ulong because we need extremely large, 64-bit unsigned intergers (biggest one I've seen is 49739193913882)
        {
            // Sum variable
            ulong sum = 0;

            // Call the memo version if you passed it true, otherwise call the regular one
            if (useMemo)
            {
                foreach (char c in axiom)
                {
                    sum += GetLengthMemo(c, numIterations);
                }
            }
            else
            {
                foreach (char c in axiom)
                {
                    sum += GetLength(c, numIterations);
                }
            }
            
            // return the sum
            return sum;
        }

        // Dynamic Programming version of GetLength
        private ulong GetLengthMemo(char inputChar, int depth)
        {
            // Base Case: depth is 0 
            if (depth == 0)
            {
                return 1; // Return 1 because input char is one character long and we aren't expanding it anymore
            }
            else
            {
                // Sum variable
                ulong sum = 0;

                // Gets the current memoTable from memoTables
                Hashtable currentMemoTable = memoTables[depth];

                // If this value has already been calculated, sum is equal to the precalculated value
                if (currentMemoTable.Contains(inputChar))
                {
                    sum = (ulong)currentMemoTable[inputChar];
                }
                else // Otherwise, calculate the value and add it to the memo table
                {
                    // Expand the current character
                    string newAxiom = (string)rules[inputChar];

                    // Get the length of each character and add it together
                    foreach (char c in newAxiom)
                    {
                        sum += GetLengthMemo(c, depth - 1);
                    }

                    // Add the result to the memotable
                    currentMemoTable.Add(inputChar, sum);
                }

                // Return the sum
                return sum;
            }
        }

        // Same as above, except without memoization
        private ulong GetLength(char inputChar, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }
            else
            {
                ulong sum = 0;

                string newAxiom = (string)rules[inputChar];

                foreach (char c in newAxiom)
                {
                    sum += GetLength(c, depth - 1);
                }

                return sum;
            }
        }

        // Gets first character of the expanded string
        private string GetFirstChar(char first, int depth)
        {
            // Base case: depth is 0
            if (depth == 0)
            {
                return Convert.ToString(first); // first character is the character given to us as an argument
            }
            else
            {
                // Expand this character
                string newAxiom = (string)rules[first];

                // Get first character
                char newFirstChar = newAxiom[0];

                // Find the new first character and return it
                return GetFirstChar(newFirstChar, depth - 1);
            }
        }

        // Gets last character of the expanded string
        private string GetLastChar(char last, int depth)
        {
            // Base case: depth is 0
            if (depth == 0)
            {
                return Convert.ToString(last); // last character passed as argument is the last character
            }
            else
            {
                // Expand this character
                string newAxiom = (string)rules[last];

                // Get the new last character
                char newLastChar = newAxiom[newAxiom.Length - 1];

                // Return the last character of the expanded string
                return GetLastChar(newLastChar, depth - 1);
            }

        }
    }
}
