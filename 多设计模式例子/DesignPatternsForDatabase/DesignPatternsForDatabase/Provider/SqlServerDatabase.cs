namespace DesignPatternsForDatabase.Provider
{
    public class SqlServerDatabase:Database
    {
        public SqlServerDatabase(string name):base(name)
        {

        }
        protected override string ParameterPrefix => "@";
    }
}
