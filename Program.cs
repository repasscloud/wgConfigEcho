using System.Net;
using System.Text;

namespace wgCfgEcho;

class Program
{
    static void Main(string[] args)
    {
        // Read the PNG file into a base64 string
        string pngFilePath = "/app/config/peer1/peer1.png";
        string pngBase64 = FileToBase64(pngFilePath);

        // Read the text file into a base64 string
        string textFilePath = "/app/config/peer1/peer1.config";
        string textBase64 = FileToBase64(textFilePath);

        // Start listening on port 8080
        string url = "http://localhost:8080/";
        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine($"Listening for requests at {url}");

            while (true)
            {
                // Wait for a request
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                // Check if the request is a GET request
                if (request.HttpMethod == "GET")
                {
                    // Prepare the JSON response
                    string jsonResponse = $"{{\"png\": \"{pngBase64}\", \"conf\": \"{textBase64}\"}}";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                    // Send the response
                    context.Response.ContentType = "application/json";
                    context.Response.ContentEncoding = Encoding.UTF8;
                    context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                    context.Response.Close();
                }
            }
        }
    }

    static string FileToBase64(string filePath)
    {
        try
        {
            // Read the file content and convert it to base64
            byte[] fileBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return string.Empty;
        }
    }
}

