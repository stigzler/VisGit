using Octokit;
using System.Collections.Generic;

namespace VisGitCore.Data.Models
{
    public class ItemStateReasonMap
    {
        public bool Open { get; set; } = true;
        public StringEnum<ItemStateReason>? ItemStateReason { get; set; } = null;

        public string Name { get; set; }

        public static List<ItemStateReasonMap> ItemStateReasons()
        {
            return new List<ItemStateReasonMap>()
            {
                new ItemStateReasonMap() { Name = "Open", Open = false, ItemStateReason = new StringEnum<ItemStateReason>?(Octokit.ItemStateReason.Reopened) },
                new ItemStateReasonMap() { Name = "Close: Completed", Open = true, ItemStateReason = new StringEnum<ItemStateReason>?(Octokit.ItemStateReason.Completed) },
                new ItemStateReasonMap() { Name = "Close: Not Planned" ,Open = true, ItemStateReason = new StringEnum<ItemStateReason>?(Octokit.ItemStateReason.NotPlanned) }
            };
        }
    }
}