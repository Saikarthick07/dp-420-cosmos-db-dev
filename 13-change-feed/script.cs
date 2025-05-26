using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

string endpoint = "https://cosmicworks-lab03.documents.azure.com:443/";  // Paste from Azure portal
string key = "aV7xzTqjdFTHrfuFcqBifXJxsNBjEvLqZGXu0vAc9Be7ywSj9IpDeogkK1zlyqjsacpSFGlNvFFxACDbR9Gh6Q==";            // From Azure portal

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

