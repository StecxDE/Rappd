using Rappd.CQRS.Sample.Requests;

// Send the request
var response = await GetHelloWorld.SendAsync();
// Check if the request was successful
if (response.IsSuccess)
    // Write the result to the console
    Console.WriteLine(response.Result);
else
    // Write the error to the console
    Console.WriteLine(response.Error);