using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace wgConfigEcho
{
    class Program
    {
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        static void Main(string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int numberOfPeers) || numberOfPeers <= 0)
            {
                Console.WriteLine("Please provide a valid number of peers as a command-line argument.");
                return;
            }

            // Create a JSON object to hold the data
            var jsonData = new JsonObject();

            // Loop through each peer and read PNG and CONF files
            for (int i = 1; i <= numberOfPeers; i++)
            {
                string peerKey = $"peer{i}";

                string pngFilePath = $"/app/html/{peerKey}.png";
                string pngBase64 = FileToBase64(pngFilePath);

                string confFilePath = $"/app/html/{peerKey}.conf";
                string confBase64 = FileToBase64(confFilePath);

                // Add peer data to the JSON object
                jsonData.Add(peerKey, new { png = pngBase64, conf = confBase64 });
            }

            // Serialize the JSON object to a string
            string jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions
            {
                WriteIndented = true // for pretty-printing
            });

            // Write the JSON string to values.json
            File.WriteAllText("/app/html/values.json", jsonString);

            Console.WriteLine("values.json has been created.");
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

    // Custom class to represent the JSON structure
    class JsonObject : System.Collections.Generic.Dictionary<string, object> { }
}
