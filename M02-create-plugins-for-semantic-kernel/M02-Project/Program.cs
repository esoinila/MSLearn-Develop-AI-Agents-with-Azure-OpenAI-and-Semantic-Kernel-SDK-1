using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;


string filePath = Path.GetFullPath("../../appsettings.json");
var config = new ConfigurationBuilder()
    .AddJsonFile(filePath)
    .Build();

// Set your values in appsettings.json
string modelId = config["modelId"]!;
string endpoint = config["endpoint"]!;
string apiKey = config["apiKey"]!;

//
// Add your code
//
// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

// Build the kernel
Kernel kernel = builder.Build();

// Get chat completion service.
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Create a chat history object
ChatHistory chatHistory = [];


void AddMessage(string msg) {
    Console.WriteLine(msg);
    chatHistory.AddAssistantMessage(msg);
}

void GetInput() {
    string input = Console.ReadLine()!;
    chatHistory.AddUserMessage(input);
}

async Task GetReply() {
    ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
        chatHistory,
        kernel: kernel
    );
    Console.WriteLine(reply.ToString());
    chatHistory.AddAssistantMessage(reply.ToString());
}

// Prompt the LLM
chatHistory.AddSystemMessage("You are a helpful travel assistant.");
chatHistory.AddSystemMessage("Recommend a destination to the traveler based on their background and preferences.");

// Get information about the user's plans
AddMessage("Tell me about your travel plans.");
GetInput();
await GetReply();


// Offer recommendations
AddMessage("Would you like some activity recommendations?");
GetInput();
await GetReply();

// Offer language tips
AddMessage("Would you like some helpful phrases in the local language?");
GetInput();
await GetReply();

Console.WriteLine("Chat Ended.\n");
Console.WriteLine("Chat History:");

for (int i = 0; i < chatHistory.Count; i++)
{
    Console.WriteLine($"{chatHistory[i].Role}: {chatHistory[i]}");
}













// Create a kernel with Azure OpenAI chat completion
// var builder = Kernel.CreateBuilder();
// builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

// // Build the kernel
// Kernel kernel = builder.Build();

// string prompt = """
//     <message role="system">Instructions: Identify the from and to destinations 
//     and dates from the user's request</message>

//     <message role="user">Can you give me a list of flights from Seattle to Tokyo? 
//     I want to travel from March 11 to March 18.</message>

//     <message role="assistant">
//     Origin: Seattle
//     Destination: Tokyo
//     Depart: 03/11/2025 
//     Return: 03/18/2025
//     </message>

//     <message role="user">{{input}}</message>
//     """;
    
// string input = "I want to travel from June 1 to July 22. I want to go to Greece. I live in Chicago.";

// // Create the kernel arguments
// var arguments = new KernelArguments { ["input"] = input };

// // Create the prompt template config using handlebars format
// var templateFactory = new HandlebarsPromptTemplateFactory();
// var promptTemplateConfig = new PromptTemplateConfig()
// {
//     Template = prompt,
//     TemplateFormat = "handlebars",
//     Name = "FlightPrompt",
// };

// // Invoke the prompt function
// var function = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
// var response = await kernel.InvokeAsync(function, arguments);
// Console.WriteLine(response);



// Create a kernel builder with Azure OpenAI chat completion
// var builder = Kernel.CreateBuilder();
// builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

// // Build the kernel
// var kernel = builder.Build();


// string prompt = """
//     You are a helpful travel guide. 
//     I'm visiting {{$city}}. {{$background}}. What are some activities I should do today?
//     """;
// string city = "Barcelona";
// string background = "I really enjoy art and dance.";

// // Create the kernel function from the prompt
// var activitiesFunction = kernel.CreateFunctionFromPrompt(prompt);

// // Create the kernel arguments
// var arguments = new KernelArguments { ["city"] = city, ["background"] = background };

// // InvokeAsync on the kernel object
// var result = await kernel.InvokeAsync(activitiesFunction, arguments);
// Console.WriteLine(result);

