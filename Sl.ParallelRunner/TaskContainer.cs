using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sl.Selenium.Parallel
{
    public class TaskContainer
    {
        public TaskContainer(int TaskID, Task Task)
        {
            this.TaskID = TaskID;
            this.Task = Task;
        }
        public int TaskID { get; private set; }

        public Task Task { get; private set; }
        public int SuccessfulRecordsPassed { get; set; }

        public int FailedRecordsPassed { get; set; }

        public int TotalRecordsPassed
        {
            get
            {
                return SuccessfulRecordsPassed + FailedRecordsPassed;
            }
        } 
    }
}
