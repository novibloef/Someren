using Database_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Database_Project.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly string? _connectionString;

        public ActivitiesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            List<Activities> activities = GetActivities();
            return View(activities);
        }

        public List<Activities> GetActivities()
        {
            List<Activities> activity = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT  pa.persoonID,
                            p.voornaam,
                            p.achternaam,
                            a.activiteitID,
                            a.rol,
                            a.naam,
                            a.datum,
                            a.tijd,
                            a.duur
                    FROM persoon_activiteit pa
                    INNER JOIN persoon p      ON pa.persoonID    = p.persoonID
                    INNER JOIN activiteit a   ON pa.activiteitID = a.activiteitID;";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int persoonId      = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    string voornaam    = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    string achternaam  = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    int activiteitId   = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                    string rol         = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                    string naam        = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                    DateTime datum     = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6);
                    TimeSpan tijd      = reader.IsDBNull(7) ? TimeSpan.Zero : TimeSpan.Parse(reader.GetString(7));
                    string duur        = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);

                    activity.Add(new Activities(
                        activiteitId,
                        persoonId,
                        rol,
                        naam,
                        datum,
                        tijd,
                        duur,
                        voornaam,
                        achternaam
                    ));
                }
            }

            return activity;
        }

        [HttpPost]
        public IActionResult AddActivity(int persoonID, string Naam, DateTime Datum, TimeSpan Tijd, string Duur)
        {
            System.Diagnostics.Debug.WriteLine($"persoonID: {persoonID}, Naam: {Naam}, Datum: {Datum}, Tijd: {Tijd}, Duur: {Duur}");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM persoon WHERE persoonID = @persoonID";
                using SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@persoonID", persoonID);
                int count = (int)checkCmd.ExecuteScalar();

                if (count == 0)
                {
                    ModelState.AddModelError(string.Empty, "The selected person does not exist.");
                    return RedirectToAction("Index");
                }

                string insertActQuery = @"
                    INSERT INTO activiteit (rol, naam, datum, tijd, duur)
                    OUTPUT INSERTED.activiteitID
                    VALUES (NULL, @naam, @datum, @tijd, @duur);";

                int newActiviteitId;

                using (SqlCommand cmd = new SqlCommand(insertActQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@naam", Naam);
                    cmd.Parameters.Add("@datum", SqlDbType.Date).Value = Datum;   
                    cmd.Parameters.Add("@tijd", SqlDbType.Time).Value = Tijd;     
                    cmd.Parameters.AddWithValue("@duur", Duur);                   

                    newActiviteitId = (int)cmd.ExecuteScalar();
                }

                string insertLinkQuery = @"
                    INSERT INTO persoon_activiteit (persoonID, activiteitID)
                    VALUES (@persoonID, @activiteitID);";

                using (SqlCommand linkCmd = new SqlCommand(insertLinkQuery, connection))
                {
                    linkCmd.Parameters.AddWithValue("@persoonID", persoonID);
                    linkCmd.Parameters.AddWithValue("@activiteitID", newActiviteitId);
                    linkCmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditActivity(int ID, string Naam, string Datum, string Tijd, string Duur)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                     UPDATE activiteit
                     SET naam = @naam,
                     datum = @datum,
                     tijd = @tijd,
                     duur = @duur
                     WHERE activiteitID = @activiteitID;
                ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@naam", Naam);
                cmd.Parameters.AddWithValue("@datum", DateTime.Parse(Datum));
                cmd.Parameters.AddWithValue("@tijd", TimeSpan.Parse(Tijd));
                cmd.Parameters.AddWithValue("@duur", Duur);
                cmd.Parameters.AddWithValue("@activiteitID", ID);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteActivity(int id)
        {
            System.Diagnostics.Debug.WriteLine($"Attempting to delete activity with ID: {id}");
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using var tx = connection.BeginTransaction();

                var deleteLink = new SqlCommand(
                    "DELETE FROM persoon_activiteit WHERE activiteitID = @id",
                    connection, tx);
                deleteLink.Parameters.AddWithValue("@id", id);
                deleteLink.ExecuteNonQuery();

                var deleteActivity = new SqlCommand(
                    "DELETE FROM activiteit WHERE activiteitID = @id",
                    connection, tx);
                deleteActivity.Parameters.AddWithValue("@id", id);
                deleteActivity.ExecuteNonQuery();

                tx.Commit();
            }

            return RedirectToAction("Index");
        }
    }
}
