using System.Data.Common;

namespace DesignPatternsForDatabase
{
    public class DbEventArgs:EventArgs
    {
        private DbCommand command;
        public virtual DbCommand Command { get => command; set => command = value; }
    }
}
