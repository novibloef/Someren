using Database_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Database_Project.Controllers
{
    public class LecturersController : Controller
    {
        private readonly string? _connectionString;

        public LecturersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            List<Lecturer> lecturers = GetLecturers();
            return View(lecturers);
        }

        private List<Lecturer> GetLecturers()
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        p.persoonID,
                        p.voornaam,
                        p.achternaam,
                        p.telefoonnummer,
                        d.leeftijd
                    FROM persoon p
                    JOIN docent d ON p.persoonID = d.persoonID
                ";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lecturers.Add(new Lecturer(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetInt32(4)
                    ));
                }
            }

            return lecturers;
        }

        [HttpPost]
        public IActionResult AddLecturer(string Naam, string Achternaam, string Telefoon, int Leeftijd)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // insert persoon
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

                // insert docent
                string queryDocent = @"
                    INSERT INTO docent (persoonID, leeftijd)
                    VALUES (@persoonID, @leeftijd);
                ";

                SqlCommand cmdDocent = new SqlCommand(queryDocent, connection);
                cmdDocent.Parameters.AddWithValue("@persoonID", persoonId);
                cmdDocent.Parameters.AddWithValue("@leeftijd", Leeftijd);

                cmdDocent.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditLecturer(int ID, string Naam, string Achternaam, string Telefoon, int Leeftijd)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // update persoon
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

                // update docent
                string updateDocentQuery = @"
                    UPDATE docent
                    SET leeftijd = @leeftijd
                    WHERE persoonID = @persoonID;
                ";

                SqlCommand cmdDocent = new SqlCommand(updateDocentQuery, connection);
                cmdDocent.Parameters.AddWithValue("@leeftijd", Leeftijd);
                cmdDocent.Parameters.AddWithValue("@persoonID", ID);

                cmdDocent.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteLecturer(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // docent eerst (FK-veilig)
                string deleteDocent = "DELETE FROM docent WHERE persoonID = @id";
                SqlCommand cmdDocent = new SqlCommand(deleteDocent, connection);
                cmdDocent.Parameters.AddWithValue("@id", id);
                cmdDocent.ExecuteNonQuery();

                // daarna persoon
                string deletePersoon = "DELETE FROM persoon WHERE persoonID = @id";
                SqlCommand cmdPersoon = new SqlCommand(deletePersoon, connection);
                cmdPersoon.Parameters.AddWithValue("@id", id);
                cmdPersoon.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}