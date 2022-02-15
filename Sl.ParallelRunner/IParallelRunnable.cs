using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.ParallelRunner
{
    public interface IParallelRunnable<T>
    {

        bool RunSingleRecord(T Record, int TaskID);


        void AfterSingleRecord(T Record, bool WasSuccess, int TaskID, int SuccessCounter, int FailCounter, int TotalRecordsCount);
    }
}
