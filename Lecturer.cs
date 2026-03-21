namespace Database_Project.Models
{
    public class Lecturer
    {
        // properties
        public int ID { get; set; }
        public string Naam { get; set; }
        public string Achternaam { get; set; }
        public string Telefoon { get; set; }
        public int Leeftijd { get; set; }

        // constructor
        public Lecturer(int id, string naam, string achternaam, string telefoon, int leeftijd)
        {
            ID = id;
            Naam = naam;
            Achternaam = achternaam;
            Telefoon = telefoon;
            Leeftijd = leeftijd;
        }
    }
}