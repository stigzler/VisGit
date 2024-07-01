using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGitCore.Data.Models
{
    public enum SortType
    {
        // Issue, Milestone, Label ---------------------------------------
        None,

        Alphabetially,

        // Issue, Milestone
        Open,

        RecentlyUpdated,

        // Milestone, Label ---------------------------------------
        /// <summary>
        /// Number of open Issues
        /// </summary>
        OpenIssues,

        // Milestone Only ---------------------------------------
        DueDate,

        // Issue Only ---------------------------------------
        RecentlyAdded,
    }
}