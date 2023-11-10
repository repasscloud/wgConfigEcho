using System.Net;
using System.Text;
using System.Diagnostics;

namespace wgConfigEcho
{
    class Program
    {
        static void Main(string[] args)
        {
            // Obtain the public IP address
            string publicIp = GetPublicIpAddress();
            string url = $"http://{publicIp}:8080/";
            Console.WriteLine($"Server URL: {url}");

            // Read the PNG file into a base64 string
            string pngFilePath = "/app/config/peer1/peer1.png";
            string pngBase64 = FileToBase64(pngFilePath);

            // Read the text file into a base64 string
            string textFilePath = "/app/config/peer1/peer1.config";
            string textBase64 = FileToBase64(textFilePath);

            // Start listening on port 8080
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

        static string GetPublicIpAddress()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sh",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            // Run a command to get the public IP address
            process.StandardInput.WriteLine("curl -s ifconfig.me");

            // Read the output of the command
            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            // Trim any leading or trailing whitespaces
            return output.Trim();
        }
    }
}
