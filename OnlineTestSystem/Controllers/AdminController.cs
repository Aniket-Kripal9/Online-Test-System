using Microsoft.AspNetCore.Mvc;
using OnlineTestSystem.DataAccess;
using OnlineTestSystem.Models;

namespace OnlineTestSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public AdminController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");
            return View();
        }

        public IActionResult Students()
        {
            var students = _dbHelper.GetStudents();
            return View(students);
        }

        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Role = "Student"; // Explicitly set Role
                    _dbHelper.AddStudent(model);
                    TempData["SuccessMessage"] = "Student added successfully!";
                    return RedirectToAction("Students");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding student: {ex.Message}");
                return View(model);
            }
        }


        public IActionResult DeleteStudent(int id)
        {
            _dbHelper.DeleteStudent(id);
            return RedirectToAction("Students", new { message = "Student Record Deleted!" }
);
        }

        public IActionResult EditStudent(int id)
        {
            var student = _dbHelper.GetStudents().FirstOrDefault(s => s.UserId == id);
            if (student == null)
                return NotFound();
            return View(student);
        }

        [HttpPost]
        public IActionResult EditStudent(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Role = "Student"; // Explicitly set Role
                    _dbHelper.UpdateStudent(model);
                    TempData["SuccessMessage"] = "Student details updated successfully!";
                    return RedirectToAction("Students");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating student: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Marks()
        {
            var results = _dbHelper.GetMarks();
            return View(results);
        }

        public IActionResult Questions()
        {
            try
            {
                var questions = _dbHelper.GetQuestions() ?? new List<Question>();
                return View(questions);
            }
            catch (Exception ex)
            {
                // Log the error (for debugging)
                Console.WriteLine($"Error fetching questions: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading questions. Please try again.";
                return View(new List<Question>());
            }
        }

        public IActionResult AddQuestion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddQuestion(Question model)
        {
            if (ModelState.IsValid)
            {
                _dbHelper.AddQuestion(model);
                return RedirectToAction("Questions");
            }
            return View(model);
        }

        public IActionResult DeleteQuestion(int id)
        {
            _dbHelper.DeleteQuestion(id);
            return RedirectToAction("Questions");
        }
    }
}