using System.Collections.Generic;

namespace ProductsFeedImporter
{
    public interface IProductsFeedImporter
    {
        string outputMessage { get; set; }

        List<string> ImportProductDetails(string userInput);
    }
}