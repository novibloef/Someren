namespace Database_Project.Models
{
    public class Student
    {
        //properties
        public int ID { get; set; }
        public string Klas { get; set; }
        public string Naam { get; set; }
        public string Achternaam { get; set; }
        public string Telefoon { get; set; }

        //constructor

        public Student(int id,  string klas, string name, string achternaam, string telefoon)
        {
            ID = id;
            Klas = klas;
            Naam = name;
            Achternaam = achternaam;
            Telefoon = telefoon;
        }
    }
}
