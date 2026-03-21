using Database_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace Database_Project.Controllers
{



    public class StudentsController : Controller
    {
        private readonly string? _connectionString;

        public StudentsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult Index()
        {
            List<Student> students = GetStudents();
            return View(students);
        }

        public List<Student> GetStudents()
        {
            List<Student> students = new List<Student>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT t1.persoonID, t1.voornaam, t1.achternaam, t1.telefoonnummer, t2.klas FROM persoon t1 JOIN student t2 ON t1.persoonID = t2.persoonID  ";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new Student(
                        reader.GetInt32(0),
                        reader.GetString(4),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3)
                    ));
                }
            }
            return students;
        }

        [HttpPost]
        public IActionResult DeleteStudent(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "DELETE FROM persoon WHERE persoonID = @id";
                SqlCommand cmd2 = new SqlCommand(query, connection);
                cmd2.Parameters.AddWithValue("@id", id);
                cmd2.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddStudent(string Klas, string Naam, string Achternaam, string Telefoon)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

       
                string queryPersoon = @"
            INSERT INTO persoon (voornaam, achternaam, telefoonnummer)
            VALUES (@voornaam, @achternaam, @telefoonnummer);
            SELECT SCOPE_IDENTITY();
        ";

                SqlCommand cmdPersoon = new SqlCommand(queryPersoon, connection);
                cmdPersoon.Parameters.AddWithValue("@voornaam", Naam);
                cmdPersoon.Parameters.AddWithValue("@achternaam", Achternaam);
                cmdPersoon.Parameters.AddWithValue("@telefoonnummer", Telefoon);

                int persoonId = (int)(decimal)cmdPersoon.ExecuteScalar();

                string queryStudent = @"
            INSERT INTO student (persoonID, klas)
            VALUES (@persoonID, @klas);
        ";

                SqlCommand cmdStudent = new SqlCommand(queryStudent, connection);
                cmdStudent.Parameters.AddWithValue("@persoonID", persoonId);
                cmdStudent.Parameters.AddWithValue("@klas", Klas);

                cmdStudent.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditStudent(int ID, string Klas, string Naam, string Achternaam, string Telefoon)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string updatePersoonQuery = @"
                     UPDATE persoon
                     SET voornaam = @voornaam,
                     achternaam = @achternaam,
                     telefoonnummer = @telefoonnummer
                     WHERE persoonID = @persoonID;
        ";

                SqlCommand cmdPersoon = new SqlCommand(updatePersoonQuery, connection);
                cmdPersoon.Parameters.AddWithValue("@voornaam", Naam);
                cmdPersoon.Parameters.AddWithValue("@achternaam", Achternaam);
                cmdPersoon.Parameters.AddWithValue("@telefoonnummer", Telefoon);
                cmdPersoon.Parameters.AddWithValue("@persoonID", ID);

                cmdPersoon.ExecuteNonQuery();

             
                string updateStudentQuery = @"
                     UPDATE student
                     SET klas = @klas
                     WHERE persoonID = @ID;
        ";

                SqlCommand cmdStudent = new SqlCommand(updateStudentQuery, connection);
                cmdStudent.Parameters.AddWithValue("@klas", Klas);
                cmdStudent.Parameters.AddWithValue("@ID", ID);


                cmdStudent.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
