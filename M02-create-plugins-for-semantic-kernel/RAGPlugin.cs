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
            Size = 3
        };

        // Add select fields if your index has specific fields
        searchOptions.Select.Add("content");
        searchOptions.Select.Add("title");

        var searchResult = await _searchClient.SearchAsync<SearchDocument>(query, searchOptions);
        
        var contextBuilder = new StringBuilder();
        
        var results = searchResult.Value.GetResults();
        
        foreach (var result in results)
        {
            var document = result.Document;
            string title = "";
            string content = "";
            
            if (document.TryGetValue("title", out object? titleObj))
            {
                title = titleObj?.ToString() ?? "";
            }
            
            if (document.TryGetValue("content", out object? contentObj))
            {
                content = contentObj?.ToString() ?? "";
            }
            
            if (!string.IsNullOrEmpty(title))
            {
                contextBuilder.AppendLine($"Title: {title}");
            }
            
            if (!string.IsNullOrEmpty(content))
            {
                contextBuilder.AppendLine($"Content: {content}");
            }
            
            contextBuilder.AppendLine();
        }
        
        return contextBuilder.ToString();
    }
} 