using System.Diagnostics;
using System.Net;
using System.Text;

namespace wgConfigEcho
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide the public IP address as a command-line argument.");
                return;
            }

            string publicIp = args[0];
            string url = $"http://{publicIp}:8080/";
            Console.WriteLine($"Server URL: {url}");

            // Read the PNG file into a base64 string
            string pngFilePath = "/app/html/peer1.png";
            string pngBase64 = FileToBase64(pngFilePath);

            // Read the text file into a base64 string
            string textFilePath = "/app/html/peer1.config";
            string textBase64 = FileToBase64(textFilePath);

            // Start listening on port 8080 in the background
            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add(url);
                listener.Start();
                Console.WriteLine($"Listening for requests at {url}");

                // Print the process ID
                Console.WriteLine($"PID: {Process.GetCurrentProcess().Id}");

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
}
