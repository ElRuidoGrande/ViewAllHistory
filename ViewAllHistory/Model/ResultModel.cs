using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSExtensions.ViewAllHistory.Model
{
    public class ResultModel
    {
        public string Branch { get; set; }

        public int ChangesetId { get; set; }

        public string Note { get; set; }

        public DateTime ChangesetDate { get; set; }

        public string Committer { get; set; }

    }
}
