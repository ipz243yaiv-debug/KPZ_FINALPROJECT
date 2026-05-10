namespace Server.DAL
{
    public class DatabaseConfig
    {
        public string DatabaseName { get; set; }
        public int TimeoutSeconds { get; set; }
        public bool UseForeignKeys { get; set; }
    }
}