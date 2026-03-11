using System.Text.Json;
using System.Text.Json.Serialization;

namespace gtt.Controllers
{
    public class Question
    {
        [JsonPropertyName("text")]
        public string question { get; set; }

        public string image { get; set; }
        public string category { get; set; }
        public string answer { get; set; }

        public Question()
        {
        }

        public Question(string question, string image, string category, string answer)
        {
            this.question = question;
            this.image = image;
            this.category = category;
            this.answer = answer;
        }
    }

    public class Parser
    {
        public List<Question> Questions { get; private set; }

        public Parser(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Questions = new List<Question>();
                return;
            }

            string jsonString = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var root = JsonSerializer.Deserialize<
                Dictionary<string, List<Question>>
            >(jsonString, options);

            Questions = root?["questions"] ?? new List<Question>();

            // Fix image paths
            foreach (var q in Questions)
            {
                if (!string.IsNullOrEmpty(q.image))
                {
                    q.image = "/Images/" + q.image;
                }
            }
        }
    }
}