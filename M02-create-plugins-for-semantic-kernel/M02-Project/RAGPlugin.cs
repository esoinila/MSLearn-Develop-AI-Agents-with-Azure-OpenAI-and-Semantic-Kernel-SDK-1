using System.ComponentModel;
using Microsoft.SemanticKernel;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Text;

public class RAGPlugin
{
    private readonly SearchClient _searchClient;

    public RAGPlugin(string endpoint, string apiKey, string indexName)
    {
        _searchClient = new SearchClient(
            new Uri(endpoint),
            indexName,
            new AzureKeyCredential(apiKey));
    }

    [KernelFunction("retrieve_documents")]
    [Description("Retrieves relevant documents from Azure AI Search based on a query")]
    public async Task<string> RetrieveDocumentsAsync(string query)
    {
        var searchOptions = new SearchOptions
        {
            Size = 3,
            QueryType = SearchQueryType.Semantic,
            SemanticConfigurationName = "default",
            QueryLanguage = "en-us"
        };

        searchOptions.Select.Add("content");
        searchOptions.Select.Add("title");

        var searchResults = await _searchClient.SearchAsync<SearchDocument>(query, searchOptions);
        
        var contextBuilder = new StringBuilder();
        
        await foreach (var result in searchResults.GetResultsAsync())
        {
            var document = result.Document;
            if (document.TryGetValue("title", out string? title) && 
                document.TryGetValue("content", out string? content))
            {
                contextBuilder.AppendLine($"Title: {title}");
                contextBuilder.AppendLine($"Content: {content}");
                contextBuilder.AppendLine();
            }
        }
        
        return contextBuilder.ToString();
    }
} 