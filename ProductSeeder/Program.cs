using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Product
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "categoryId")]
    public string CategoryId { get; set; }

    public Product(string id, string name, string categoryId)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
    }
}

class Program
{
    private static readonly string endpoint = "";
    private static readonly string key = "";
    private static readonly string databaseId = "cosmicworks";
    private static readonly string containerId = "products";

    static async Task Main(string[] args)
    {
        CosmosClient client = new CosmosClient(endpoint, key);
        Container container = client.GetContainer(databaseId, containerId);

        var products = new List<Product>
        {
            new Product("p001", "Helmet", "gear"),
            new Product("p002", "Bike Pump", "gear"),
            new Product("p003", "Mountain Bike", "bikes"),
            new Product("p004", "Road Bike", "bikes"),
            new Product("p005", "Water Bottle", "accessories"),
        };

        foreach (var product in products)
        {
            await container.CreateItemAsync(product, new PartitionKey(product.CategoryId));
            Console.WriteLine($"Inserted: {product.Id} - {product.Name}");
        }

        Console.WriteLine("Product seeding completed.");
    }
}
