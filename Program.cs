using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Demo
{
    class Program
    {
        public static string[] str;
       
        static void Main(string[] args)
        {
            StringBuilder oilseed = new StringBuilder();//stringbuuilder for oilseed json
            StringBuilder foodgrain = new StringBuilder();//stringbuilder for foodgrain josn
            StringBuilder commercial = new StringBuilder();//stringbuilder for commercial josn
            StringBuilder states = new StringBuilder();//stringbuilder for state josn
            Dictionary<string, double> oilseedDict = new Dictionary<string, double>();//oilseed dictionary 
            Dictionary<string, double> foodgrainDict = new Dictionary<string, double>();//food grain dictionary
            oilseed.Append("[");
            foodgrain.Append("[");
            commercial.Append("[");
            states.Append("[");
            double[] karnataka = new double[22];
            double[] kerala = new double[22];
            double[] tamilNadu = new double[22];
            double[] andharaPradesh = new double[22];
            double[] counter = new double[22];//array to store aggregate value for each year 
            //csv file to read  
            try
            {
                using (StreamReader read = new StreamReader(new FileStream(@"D:\Demo\Production-Department_of_Agriculture.csv", FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string[] heading = read.ReadLine().Split(',');//reading heading                                      
                    while (!read.EndOfStream)
                    {
                        str = read.ReadLine().Split(',');//reading data line by line
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
                                tamilNadu = StoreValueInStateArray();
                            }
                            else if (str[0].Contains("Karnataka"))
                            {
                                karnataka = StoreValueInStateArray();
                            }
                            else if (str[0].Contains("Andhra Pradesh"))
                            {
                                andharaPradesh = StoreValueInStateArray();
                            }
                            else if (str[0].Contains("Kerala"))
                            {
                                kerala = StoreValueInStateArray();
                            }
                        }
                    }
                }
            }
            catch(IOException e)
            {
                Console.WriteLine("File is already in use");
            }
         
            for (int i=1993; i<=2014; i++)//iteration for aggregate value
            {
                states.Append("{\"Year\":" + "\"" + i + "\"," + "\"Kerala\":" + kerala[i - 1993] + "," + "\"Tamil_Nadu\":" + tamilNadu[i - 1993] + "," + "\"Karnataka\":" + karnataka[i - 1993] + "," + "\"Andhara_Pradesh\":" + andharaPradesh[i - 1993] + "},");
                commercial.Append("{\"Year\":"+i+","+"\"Aggregate_Value\":"+counter[i-1993]+"}"+",");
            }
            var oilseedSortedDict = from oilseedEntry in oilseedDict orderby oilseedEntry.Value descending select oilseedEntry;//descending order oilseed json
            foreach (KeyValuePair<string,double> keyValue in oilseedSortedDict)
            {
                oilseed.Append("{\"CropName\":"+"\""+keyValue.Key+"\","+"\"Year2013\":"+keyValue.Value+"},");
            }
            var foodgrainSortedDict = from foodgrainEntry in foodgrainDict orderby foodgrainEntry.Value descending select foodgrainEntry;//descending order foodgrain json
            foreach(KeyValuePair<string,double> keyValue in foodgrainSortedDict)
            {
                foodgrain.Append("{\"CropName\":" + "\"" + keyValue.Key + "\"," + "\"Year2013\":" + keyValue.Value + "},");
            }
            states.Length--;
            states.Append("]");
            commercial.Length--;
            commercial.Append("]");
            oilseed.Length--;
            oilseed.Append("]");
            foodgrain.Length--;
            foodgrain.Append("]");
            //oilseed file to write oilseed json
            using (StreamWriter oilseedWrite = new StreamWriter(new FileStream(@"D:\Demo\Oilseed.json", FileMode.OpenOrCreate, FileAccess.Write)))
            {
                oilseedWrite.Write(oilseed);
            }
            //foograin file to write foodgrain json
            using (StreamWriter foodgrainsWrite = new StreamWriter(new FileStream(@"D:\Demo\Foodgrains.json", FileMode.OpenOrCreate, FileAccess.Write)))
            {
                foodgrainsWrite.Write(foodgrain);
            }
            //comercial file to write commercial json
            using (StreamWriter commercialWrite = new StreamWriter(new FileStream(@"D:\Demo\Commercial.json", FileMode.OpenOrCreate, FileAccess.Write)))
            {
                commercialWrite.Write(commercial);
            }
            //south state file to write state json
            using (StreamWriter southstateWrite = new StreamWriter(new FileStream(@"D:\Demo\SouthState.json", FileMode.OpenOrCreate, FileAccess.Write)))
            {
                southstateWrite.Write(states);
            } 
        }
        public static double[] StoreValueInStateArray()
        {
            double[] arr = new double[22];
            for (int i = 4; i <= 25; i++)
            {              
                arr[i - 4] = (str[i] == "NA") ? double.Parse("0") : double.Parse(str[i]);
            }
            return arr;
        }
    }
}
