using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace ProcessDelimitedTextFile
{
    class Program
    {
        static void Main(string[] args)
        {
            int inputeFields;
            string fieldCntEqual = @"C:\Temp\FieldsEqualCnt.txt";
            string fieldCntNotEqual = @"C:\Temp\FieldsNotEqualCnt.txt";
            string inputFormat = null;
            string inputLocation = null;
            string outFile = null;

            //Clean directory space if needed
            if (System.IO.File.Exists(fieldCntNotEqual))
                System.IO.File.Delete(fieldCntNotEqual);
            if (System.IO.File.Exists(fieldCntEqual))
                System.IO.File.Delete(fieldCntEqual);

            //Get input filename
            Console.WriteLine("Where is the file located?");
            inputLocation = Console.ReadLine();
            Console.WriteLine();
            
            //Loop until valid filename entered
            while (!File.Exists(inputLocation))
            {
                Console.WriteLine("Filename does not exist! Where is the file located?");
                inputLocation = Console.ReadLine();
                Console.WriteLine();
            }

            //Get file format separator.  If anything else besides tab, CSV is used.
            Console.WriteLine("Choose the file format option:");
            Console.WriteLine("\tc - CSV (comma-separated values) DEFAULT");
            Console.WriteLine("\tt - TSV (tab-separated values)");
            inputFormat = ("t" == Console.ReadLine()) ?  "\t" : ",";
            Console.WriteLine();

            //Get fields count for each record
            Console.WriteLine("How many fields should each record contain?");

            //Loop until valid integer entered
            while (!int.TryParse(Console.ReadLine(), out inputeFields))
            {
                Console.WriteLine();
                Console.WriteLine("Field count is not valid! How many fields should each record contain?");
            }

            //Parse input file.
            using (TextFieldParser parser = new TextFieldParser(inputLocation))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(inputFormat);

                // Skip over header line.
                parser.ReadLine();

                ParseInputFile(inputeFields, fieldCntEqual, fieldCntNotEqual, inputFormat, ref outFile, parser);
            }
        }

        private static void ParseInputFile(int inputeFields, string fieldCntEqual, string fieldCntNotEqual, string inputFormat, ref string outFile, TextFieldParser parser)
        {
            int fieldCnt;
            while (!parser.EndOfData)
            {
                //Count the fields on each row
                fieldCnt = 0;
                string[] fields = parser.ReadFields();
                foreach (string field in fields)
                {
                    fieldCnt++;
                }

                outFile = SetOutputFileNameBasedOnFieldCount(fieldCnt, inputeFields, fieldCntEqual, fieldCntNotEqual, outFile);
                WriteOutputLine(inputFormat, outFile, fields);
            }
        }

        private static string SetOutputFileNameBasedOnFieldCount(int fieldCnt, int inputeFields, string fieldCntEqual, string fieldCntNotEqual, string outFile)
        {
            //Determine which file to write to based on the field count
            if (fieldCnt == inputeFields)
            {
                Console.WriteLine("set write file to count matches");
                outFile = fieldCntEqual;
            }
            else
            {
                Console.WriteLine("set write file to count does not match");
                outFile = fieldCntNotEqual;
            }
            return outFile;
        }

        private static void WriteOutputLine(string inputFormat, string outFile, string[] fields)
        {
            //Write line to determined output file
            using (StreamWriter writer = new System.IO.StreamWriter(outFile, true))
            {
                string line = string.Join(inputFormat, fields);
                writer.WriteLine(line);
            }
        }
    }
}