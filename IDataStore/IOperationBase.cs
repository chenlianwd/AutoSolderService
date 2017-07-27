using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDataStore
{
    public interface IOperationBase:IOperationBaseR, IOperationBaseW
    {
        bool SettingEventScheduler(string timerange, string tableName);
    }
}
