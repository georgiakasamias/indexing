using System;
using static SearchIndexApp.Indexer;
using System.Collections.Generic;



namespace SearchIndexApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FileReader fileReader = new FileReader();
            
            List<string> stopWords = fileReader.StopWordsRetriever();
            List<string> urlList = fileReader.URLRetriever();

           

            //creates scrubbed index to search from, as a Dictionary
            Indexer indexer = new Indexer(stopWords, urlList);

          
            //indexer will ask for search token the first time
            string input = Console.ReadLine();
            input = input.ToLower();
    
            //results of search ---  Dict< college, # of times search word appears>
            Dictionary<string, int> results = indexer.Search(input);


            foreach (KeyValuePair<string, int> result in results)
            {
                Console.WriteLine(result.Value.ToString() + " review(s) for " + result.Key);
            }


            //infinite loop to search
            while (true)
            {
                Console.WriteLine("Please enter search token: ");

                input = Console.ReadLine();
                input = input.ToLower();


                results = indexer.Search(input);


                foreach (KeyValuePair<string, int> result in results)
                {
                    Console.WriteLine(result.Value.ToString() + " review(s) for " + result.Key);
                }
            }
            


        }
    }
}
