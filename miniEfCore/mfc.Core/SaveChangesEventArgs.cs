using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class SavingChangesEventArgs : EventArgs
    {
        public bool AcceptAllChangesOnSuccess { get; }

        public SavingChangesEventArgs(bool acceptAllChangesOnSuccess)
        {
            AcceptAllChangesOnSuccess = acceptAllChangesOnSuccess;
        }
    }

    public class SavedChangesEventArgs : EventArgs
    {
        public bool AcceptAllChangesOnSuccess { get; }
        public int AffectedRows { get; }

        public SavedChangesEventArgs(bool acceptAllChangesOnSuccess, int affectedRows)
        {
            AcceptAllChangesOnSuccess = acceptAllChangesOnSuccess;
            AffectedRows = affectedRows;
        }
    }
}
