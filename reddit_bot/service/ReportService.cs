using Reddit.Things;
using System;
using System.Collections.Generic;
using System.IO;

namespace reddit_bor.service
{
    public class ReportService
    {
        private const string _dirPath = "./data/reports/";

        public void CreateReport(List<Reddit.Things.PostResultShortContainer> results)
        {
            string currentDate = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
            string filePath = Path.Combine(_dirPath, $"{currentDate}.txt");
            
            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                }
            }
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < results.Count; i++)
                {
                    PostResultShortContainer result = results[i];
                    string link = $"{i + 1}) {result.JSON.Data.URL}";
                    writer.WriteLine(link);
                }
            }
        }
    }
}
