using Microsoft.AspNetCore.Mvc;
using OnlineTestSystem.DataAccess;
using OnlineTestSystem.Models;

namespace OnlineTestSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public StudentController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Student")
                return RedirectToAction("Login", "Account");
            return View();
        }

        public IActionResult TakeTest()
        {
            var questions = _dbHelper.GetQuestions();
            HttpContext.Session.SetInt32("TotalQuestions", questions.Count);
            HttpContext.Session.SetInt32("CurrentQuestion", 0);
            return View(questions);
        }

        [HttpPost]
        public IActionResult SubmitTest(Dictionary<int, int> answers)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            int marks = 0;
            foreach (var answer in answers)
            {
                var question = _dbHelper.GetQuestionById(answer.Key);
                if (question != null && question.CorrectOption == answer.Value)
                    marks++;
            }

            _dbHelper.SaveTestResult(userId, marks);
            return RedirectToAction("Result", new { marks = marks });
        }

        public IActionResult Result(int marks)
        {
            ViewBag.Marks = marks;
            ViewBag.Total = HttpContext.Session.GetInt32("TotalQuestions");
            return View();
        }
    }
}