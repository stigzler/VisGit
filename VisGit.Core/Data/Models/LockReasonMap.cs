using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGitCore.Data.Models
{
    public class LockReasonMap

    {
        public bool Locked { get; set; } = false;

        public string Name { get; set; }

        public StringEnum<LockReason>? LockReason { get; set; }

        public static List<LockReasonMap> LockReasonItems()
        {
            return new List<LockReasonMap>
            {
                new LockReasonMap() { LockReason = null, Locked = false, Name = "Unlock" },
                new LockReasonMap() { LockReason = Octokit.LockReason.OffTopic, Locked = true, Name = "Lock: Off Topic" },
                new LockReasonMap() { LockReason = Octokit.LockReason.Resolved, Locked = true, Name = "Lock: Resolved" },
                new LockReasonMap() { LockReason = Octokit.LockReason.Spam, Locked = true, Name = "Lock: Spam" },
                new LockReasonMap() { LockReason = Octokit.LockReason.TooHeated, Locked = true, Name = "Lock: Too Heated" }
            };
        }
    }
}