using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ProductsFeedImporter
{
    public class Program
    {
        public static string outputResultFormat { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                SetUpConfig();

                Console.WriteLine("Please enter import followed by the format \n " + 
                    "1. softwareadvice \n 2. capterra \n" +
                    "and then followed by  the path of the file from where you want to import the feeds.\n" +
                    "For eg: import softwareadvice feed-products/softwareadvice.json");

                string inputFromUser = Console.ReadLine();
                ProductsFeedImporter productFeedImporter = new ProductsFeedImporter(outputResultFormat);
                var feeds = productFeedImporter.ImportProductDetails(inputFromUser);
                foreach (var feed in feeds)
                {
                    Console.WriteLine(feed);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit from the console!");
            Console.ReadKey();
        }

        /// <summary>
        /// This method configures settings to read appSettings.json file which has been used to set final output message format.  
        /// </summary>
        private static void SetUpConfig()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            outputResultFormat = configuration["outputResultFormat"];
        }
    }
}
