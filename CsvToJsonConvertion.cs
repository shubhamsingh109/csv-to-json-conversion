using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
[assembly: AssemblyTitle("CsvToJson")]
[assembly: AssemblyVersionAttribute("1.0.0")]
namespace Demo
{
    public class CsvToJsonConvertion
    {
        public readonly static string[] str;
        private readonly static Dictionary<string, double> oilseedDict = new Dictionary<string, double>();//oilseed dictionary 
        private readonly static Dictionary<string, double> foodgrainDict = new Dictionary<string, double>();//food grain dictionary
        private static double[] karnataka = new double[22];
        private static double[] kerala = new double[22];
        private static double[] tamilNadu = new double[22];
        private static double[] andharaPradesh = new double[22];
        private static double[] counter = new double[22];//array to store aggregate value for each year 
        private readonly static StringBuilder oilseedBuilder = new StringBuilder();//stringbuuilder for oilseed json
        private readonly static StringBuilder foodgrainBuilder = new StringBuilder();//stringbuilder for foodgrain josn
        private readonly static StringBuilder commercialBuilder = new StringBuilder();//stringbuilder for commercial josn
        private readonly static StringBuilder statesBuilder = new StringBuilder();//stringbuilder for state josn

        public static void Main(string[] args)
        {         
            oilseedBuilder.Append("[");
            foodgrainBuilder.Append("[");
            commercialBuilder.Append("[");           
            string readingFilePath = "Csv/Production.csv";
            //Reading File 
            fileReading(readingFilePath);
            //Manipuation on read data which stored in dictionary
            manipulationOnData();
            statesBuilder.Length--;
            statesBuilder.Append("]");
            commercialBuilder.Length--;
            commercialBuilder.Append("]");
            oilseedBuilder.Length--;
            oilseedBuilder.Append("]");
            foodgrainBuilder.Length--;
            foodgrainBuilder.Append("]");
            string oilseedJsonPath = "Json/Oilseed.json";
            //writing oilseed json file
            WritingInToFile(oilseedJsonPath,oilseedBuilder);          
            string foodgrainJsonPath = "Json/Foodgrains.json";
            //writing foodgrain json file on path foodgrainJsonPath
            WritingInToFile(foodgrainJsonPath, foodgrainBuilder);                   
            string commercialJsonPath = "Json/Commercial.json";
            //writing commercial crop json file on path commercialJsonPath
            WritingInToFile(commercialJsonPath, commercialBuilder);         
            string southStateJsonPath = "Json/SouthState.json";
            //writing south states json file on path foodgrainJsonPath
            WritingInToFile(southStateJsonPath, statesBuilder);         
        }

        public static double[] storeValueInStateArray(string[] str)
        {
            double[] arr = new double[22];
            for (int i = 4; i <= 25; i++)
            {
                arr[i - 4] = (str[i] == "NA") ? double.Parse("0") : double.Parse(str[i]);
            }
            return arr;
        }

        public static void fileReading(string filePath)
        {
            try
            {
                using (StreamReader read = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string[] heading = read.ReadLine().Split(',');//reading heading 
                    while (!read.EndOfStream)
                    {
                        filter(read.ReadLine().Split(','));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("File not handleed well"+ex);
            }
            
        }

        public static void filter(string[] str)
        {
            if (str[0].Contains("Oilseeds") && (((!str[0].Contains("Yield")) && (!str[0].Contains("Foodgrains Production"))) && (!str[0].Contains("Area"))))//filter for oilseed json
            {
                double value = (str[24] == "NA") ? double.Parse("0") : double.Parse(str[24]);
                oilseedDict.Add(str[0], value);
            }
            else if (str[0].Contains("Agricultural Production Foodgrains") && ((((!str[0].Contains("Area")) && (!str[0].Contains("Foodgrains Production"))) && (!str[0].Contains("Yield"))) && (!str[0].Contains("Volume"))))//filetr for foodgrain json
            {
                double value = (str[24] == "NA") ? double.Parse("0") : double.Parse(str[24]);
                foodgrainDict.Add(str[0], value);
            }
            else if (str[0].Contains("Commercial"))//filter for commercial json
            {
                for (int i = 4; i <= 25; i++)
                {
                    counter[i - 4] += (str[i] == "NA") ? double.Parse("0") : double.Parse(str[i]);
                }
            }
            else if ((((str[0].Contains("Rice Volume Tamil Nadu")) || (str[0].Contains("Rice Volume Andhra Pradesh"))) || (str[0].Contains("Rice Volume Karnataka"))) || (str[0].Contains("Rice Volume Kerala")))//filetr for state json
            {
                if (str[0].Contains("Tamil Nadu"))
                {
                    tamilNadu = storeValueInStateArray(str);
                }
                else if (str[0].Contains("Karnataka"))
                {
                    karnataka = storeValueInStateArray(str);
                }
                else if (str[0].Contains("Andhra Pradesh"))
                {
                    andharaPradesh = storeValueInStateArray(str);
                }
                else if (str[0].Contains("Kerala"))
                {
                    kerala = storeValueInStateArray(str);
                }
            }
        }

        public static void manipulationOnData()
        {
            for (int i = 1993; i <= 2014; i++)//iteration for aggregate value
            {
                statesBuilder.Append("{\"Year\":" + "\"" + i + "\"," + "\"Kerala\":" + kerala[i - 1993] + "," + "\"Tamil_Nadu\":" + tamilNadu[i - 1993] + "," + "\"Karnataka\":" + karnataka[i - 1993] + "," + "\"Andhara_Pradesh\":" + andharaPradesh[i - 1993] + "},");
                commercialBuilder.Append("{\"Year\":" + i + "," + "\"Aggregate_Value\":" + counter[i - 1993] + "}" + ",");
            }
            var oilseedSortedDict = from oilseedEntry in oilseedDict orderby oilseedEntry.Value descending select oilseedEntry;//descending order oilseed json
            foreach (KeyValuePair<string, double> keyValue in oilseedSortedDict)
            {
                oilseedBuilder.Append("{\"CropName\":" + "\"" + keyValue.Key + "\"," + "\"Year2013\":" + keyValue.Value + "},");
            }
            var foodgrainSortedDict = from foodgrainEntry in foodgrainDict orderby foodgrainEntry.Value descending select foodgrainEntry;//descending order foodgrain json
            foreach (KeyValuePair<string, double> keyValue in foodgrainSortedDict)
            {
                foodgrainBuilder.Append("{\"CropName\":" + "\"" + keyValue.Key + "\"," + "\"Year2013\":" + keyValue.Value + "},");
            }
        }
          
        public static void WritingInToFile(string fileWritingPath,StringBuilder stringBuilder)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(fileWritingPath, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                writer.Write(stringBuilder);
            }
        }
    }
}
