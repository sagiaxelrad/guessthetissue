using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace gtt.Controllers
{
    public class QuizController : Controller
    {
        private static List<Question> questions;
        private static Random rnd = new Random();

        static QuizController()
        {
            Parser parser = new Parser("questions.json");
            questions = parser.Questions;
        }

        public IActionResult Question(int id = 1)
        {
            if (questions.Count == 0)
                return Content("No questions loaded.");

            // convert human number → zero-based index
            id = id - 1;

            if (id < 0)
                id = 0;

            if (id >= questions.Count)
                id = questions.Count - 1;

            var q = questions[id];

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
            ViewBag.Id = id;
            ViewBag.Total = questions.Count;

            return View();
        }

        [HttpPost]
        public IActionResult Answer(string selectedAnswer, string correctAnswer, int id)
        {
            bool correct = selectedAnswer == correctAnswer;

            ViewBag.Correct = correct;
            ViewBag.Selected = selectedAnswer;
            ViewBag.CorrectAnswer = correctAnswer;
            ViewBag.NextId = id + 2;

            return View();
        }
    }
}
