using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLdap
{
    public class ProgressInfo
    {
        public  DateTime Date { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public  string Note { get; set; }
        public bool Passed { get; set; }
    }
}
