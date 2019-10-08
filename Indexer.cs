using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchIndexApp
{
    public class Indexer
    {
        private Dictionary<string, string[]> SearchIndex = new Dictionary<string, string[]>();

       
        public Indexer(List<string> stopWords, List<string> urlList)
        {
            //each new instance of an indexer will create a Search Index of scrubbed reviews
            Main(stopWords, urlList);
       
        }





        //call this method in main Program to search
        public Dictionary<string, int> Search(string searchWord)
        {
            Dictionary<string, int> results = new Dictionary<string, int>();

            // search through and count results per college
            foreach(KeyValuePair<string, string[]> reviews in SearchIndex)
            {
                int counter = 0;

                for(int i = 0; i < reviews.Value.Length; i++)
                {
                    string[] reviewWordsToCompare = reviews.Value[i].Split(" ");

                    for(int j = 0; j < reviewWordsToCompare.Length; j++)
                    {
                        if (reviewWordsToCompare[j] == searchWord)
                        {
                            counter++;
                        }
                    }
                }

                if(counter > 0)
                {
                    results.Add(reviews.Key, counter);
                }

            }

            return results;
        }




        


        static readonly HttpClient client = new HttpClient();

        public async Task Main(List<string> stopWords, List<string> urlList)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();


            //read in all urls
            foreach (string url in urlList)
            {
                List<string> responseStream = new List<string>();

                // Call asynchronous network methods in a try/catch block to handle exceptions.
                try
                {
                   Stream stream = await client.GetStreamAsync(url);
                   try
                   {
                        using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                //add to list to process later
                                string line = streamReader.ReadLine();
                                responseStream.Add(line);
                                
                            }
                        }
                   }
                   catch (Exception e)
                   {
                        Console.WriteLine(e.Message);
                   }

                    Console.WriteLine("Indexing " + url);

                    ///////////////////

                    string[] responseBody = responseStream.ToArray();

                 
                    //1. pull name to add to dictionary as key
                    //2. remove college name from string, pass new array along to Scrubber

                    //1.
                    string collegeName = responseBody[0];
                    //2.
                    string[] reviewsUnprocessed = responseBody.Where((item, index) => index != 0).ToArray();
                    string fullReviewBody = string.Join("", reviewsUnprocessed);


                    //scrubbed array of reviews
                    string[] readyToAdd = Scrubber(fullReviewBody, stopWords);

                    //add to main Dictionary
                    this.SearchIndex.Add(collegeName, readyToAdd);
             


                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }

            }


            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            double timeElapsed = Convert.ToDouble(elapsedMs);
            timeElapsed = (timeElapsed / 60000);
            timeElapsed = Math.Round(timeElapsed, 2);

            //writes after SearchIndex is constructed
            Console.WriteLine("Finished indexing in " + elapsedMs.ToString() + " ms");
            Console.WriteLine("Finished indexing in " + timeElapsed.ToString() + " min");

            Console.WriteLine("Please enter search token: ");

        }




        public string[] Scrubber(string reviewsAsString, List<string> stopWords)
        {

            //use as bucket
            List<string> finalScrubbedReviews = new List<string>();

            string[] test = reviewsAsString.Split('.', '!', '?');

            //REGEX METHOD to keep delimiters
            string[] stringSplit = Regex.Split(reviewsAsString, @"(\.|\!|\?)");
            List<string> stringSplitWithDelims = new List<string>();

            for (int i = 0; i < (stringSplit.Length - 1 ); i+=2)
            {
                    stringSplitWithDelims.Add(stringSplit[i] + stringSplit[i + 1]);
            }


         


            foreach(string review in stringSplitWithDelims)
            {
                //first, split the review [sentence] into another array of words
                string[] splitReview = review.Split(' ');
                List<string> splitReviewList = splitReview.ToList();


                List<string> scrubbed = new List<string>();
                foreach (string word in splitReviewList)
                {
                    if (!stopWords.Contains(word.ToLower()))
                    {
                        scrubbed.Add(word);
                    }
                }


                string reviewScrubbed = string.Join(" ", scrubbed);

                finalScrubbedReviews.Add(reviewScrubbed);

            }




            string[] readyToAdd = finalScrubbedReviews.ToArray();
            return readyToAdd; 

      


        }
    }
}
