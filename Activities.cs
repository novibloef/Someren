namespace Database_Project.Models
{
    public class Activities
    {
        //properties
        public int ID { get; set; }
        public int PersoonID { get; set; }
        public string Rol { get; set; }
        public string Naamactiviteit { get; set; }
        public DateTime Startdate { get; set; }
        public TimeSpan Starttime { get; set; }
        public string Duration { get; set; }

        public string Voornaam { get; set; }
        public string Achternaam { get; set; }

        //constructor

        public Activities(int id, string name, DateTime date, TimeSpan time, string duur)
        {
            ID = id;
            Naamactiviteit = name;
            Startdate = date;
            Starttime = time;
            Duration = duur;
        }

        public Activities(int id, int persoonID, string rol, string name, DateTime date, TimeSpan time, string duur)
        {
            ID = id;
            PersoonID = persoonID;
            Rol = rol;
            Naamactiviteit = name;
            Startdate = date;
            Starttime = time;
            Duration = duur;
        }

        public Activities(int id, int persoonID, string rol, string name, DateTime date, TimeSpan time, string duur, string voornaam, string achternaam)
        {
            ID = id;
            PersoonID = persoonID;
            Rol = rol;
            Naamactiviteit = name;
            Startdate = date;
            Starttime = time;
            Duration = duur;
            Voornaam = voornaam;
            Achternaam = achternaam;
        }
    }
}
