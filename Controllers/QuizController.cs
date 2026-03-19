using Microsoft.AspNetCore.Mvc;
namespace gtt.Controllers
{
    public class QuizController : Controller
    {
        private static List<Question> questions;
        private static List<int> shuffledOrder;
        private static Random rnd = new Random();

        static QuizController()
        {
            Parser parser = new Parser("questions.json");
            questions = parser.Questions;
            shuffledOrder = Enumerable.Range(0, questions.Count)
                                      .OrderBy(x => rnd.Next())
                                      .ToList();
        }

        // pos  = 1-based position in the current sequence
        // real = 1-based actual question number (used when toggling or jumping)
        public IActionResult Question(int pos = 1, int real = -1, bool random = false, int orderedPos = -1)
        {
            if (questions.Count == 0)
                return Content("No questions loaded.");

            if (!random && orderedPos >= 1)
            {
                // returning to ordered mode — jump back to where we were
                pos = orderedPos;
            }
            else if (real >= 1 && real <= questions.Count)
            {
                int zeroReal = real - 1;
                if (random)
                    pos = shuffledOrder.IndexOf(zeroReal) + 1;
                else
                    pos = real;
            }

            // wrap around
            if (pos < 1) pos = questions.Count;
            if (pos > questions.Count) pos = 1;

            int zeroPos = pos - 1;
            int index = random ? shuffledOrder[zeroPos] : zeroPos;

            var q = questions[index];
            var wrongAnswers = questions
                .Where(x => x.category == q.category)
                .Select(x => x.answer)
                .Where(a => a != q.answer)
                .Distinct()
                .OrderBy(x => rnd.Next())
                .Take(3)
                .ToList();

            var choices = new List<string>(wrongAnswers);
            choices.Add(q.answer);
            choices = choices.OrderBy(x => rnd.Next()).ToList();

            ViewBag.Question = q;
            ViewBag.Choices = choices;
            ViewBag.Pos = pos;
            ViewBag.RealIndex = index;
            ViewBag.Total = questions.Count;
            ViewBag.RandomMode = random;
            return View();
        }

        [HttpPost]
        public IActionResult Answer(string selectedAnswer, string correctAnswer, int pos, bool random)
        {
            bool correct = selectedAnswer == correctAnswer;
            ViewBag.Correct = correct;
            ViewBag.Selected = selectedAnswer;
            ViewBag.CorrectAnswer = correctAnswer;
            ViewBag.Pos = pos;
            ViewBag.RandomMode = random;
            return View();
        }
    }
}