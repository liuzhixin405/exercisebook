namespace DesignPatternsForDatabase.Provider
{
    public class OracleDatabase : Database
    {
        public OracleDatabase(string name):base(name)
        {

        }
        protected override string ParameterPrefix => ":";
    }
}
