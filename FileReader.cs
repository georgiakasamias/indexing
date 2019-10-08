using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;




namespace SearchIndexApp
{
    public class FileReader
    {


        //reads files and passes to indexer
        public List<string> URLRetriever()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, "urls.txt");

            try
            {
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    List<string> urlList = new List<string>();

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        urlList.Add(line);

                    }



                    return urlList;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error retrieving stop words.");
                Console.WriteLine(e.Message);

                return null; //need to check if this is null later on
            }

        }


        public List<string> StopWordsRetriever()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, "stopWords.txt");

            try
            {
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    List<string> stopWords = new List<string>();

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        stopWords.Add(line);

                    }



                    return stopWords;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error retrieving stop words.");
                Console.WriteLine(e.Message);

                return null; //need to check if this is null later on
            }

        }



    }
}
