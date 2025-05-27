using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

string endpoint = "";  // Paste from Azure portal
string key = "==";            // From Azure portal

CosmosClient client = new CosmosClient(endpoint, key);

Container sourceContainer = client.GetContainer("cosmicworks", "products");
Container leaseContainer = client.GetContainer("cosmicworks", "productslease");

ChangeFeedProcessor changeFeedProcessor = sourceContainer
    .GetChangeFeedProcessorBuilder<Product>(
        processorName: "productsProcessor",
        onChangesDelegate: async (
            IReadOnlyCollection<Product> changes,
            CancellationToken cancellationToken) =>
        {
            Console.WriteLine($"Batch started. Items: {changes.Count}");
            foreach (Product product in changes)
            {
                Console.WriteLine($"Detected Change: {product.Id} - {product.Name}");
            }
        })
    .WithInstanceName("consoleApp")
    .WithLeaseContainer(leaseContainer)
    .WithPollInterval(TimeSpan.FromSeconds(1))  // Optional
    .Build();

await changeFeedProcessor.StartAsync();

Console.WriteLine("Listening for changes. Press any key to stop...");
Console.ReadKey();

await changeFeedProcessor.StopAsync();

