using Newtonsoft.Json;
using ProductsFeedImporter.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace ProductsFeedImporter
{
    public class ProductsFeedImporter : IProductsFeedImporter
    {
        public string outputMessage { get; set; }
        public ProductsFeedImporter(string outputMessage)
        {
            this.outputMessage = outputMessage;
        }

        /// <summary>
        /// This method reads product details from external source and returns array of Product feeds. 
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public List<string> ImportProductDetails(string userInput)
        {
            string[] inputArr = userInput.Split(" ");
            if (inputArr.Length != 3)
            {
                throw new InvalidOperationException(message: "Invalid input! Valid Syntax: <import> <capterra/softwareadvice/..> <file full path>");
            }

            List<string> ProductFeeds;
            string filePath = inputArr[2];
            var fileContent = string.Empty;
            switch (inputArr[1].ToLower())
            {
                case "capterra":
                    ProductFeeds = ImportCapterraFeeds(filePath);
                    break;
                case "softwareadvice":
                    ProductFeeds = ImportSoftwareAdviceFeeds(filePath);
                    break;
                default:
                    throw new InvalidOperationException(message: "Provider not supported!");
            }
            return ProductFeeds;
        }

        /// <summary>
        /// This method uses Newtonsoft json nuget package to convert Json text into c# object.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<string> ImportSoftwareAdviceFeeds(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            string fileContent = File.ReadAllText(filePath);
            List<string> ProductFeeds = new List<string>();
            var softwareAdviceFeeds = JsonConvert.DeserializeObject<SoftwareAdviceProductModel>(fileContent);
            foreach (var feed in softwareAdviceFeeds.products)
            {
                ProductFeeds.Add(string.Format(outputMessage, feed.title, string.Join(',', feed.categories), ProcessTwitterText(feed.twitter)));
            }
            return ProductFeeds;
        }

        /// <summary>
        /// This method uses YamlDotNet to convert Yaml file text into c# object.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<string> ImportCapterraFeeds(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            string fileContent = File.ReadAllText(filePath);
            List<string> ProductFeeds = new List<string>();
            Deserializer deserializer = new Deserializer();
            var capterraFeeds = deserializer.Deserialize<List<CapterraFeed>>(fileContent);
            foreach (var feed in capterraFeeds)
            {
                ProductFeeds.Add(string.Format(outputMessage, feed.name, feed.tags, ProcessTwitterText(feed.twitter)));
            }
            return ProductFeeds;
        }

        /// <summary>
        /// This method handles emtpy values and extra @ symbols. 
        /// </summary>
        /// <param name="twitterText"></param>
        /// <returns></returns>
        private static string ProcessTwitterText(string twitterText)
        {
            return !string.IsNullOrWhiteSpace(twitterText) ? "@" + twitterText.TrimStart('@') : "";
        }
    }
}
