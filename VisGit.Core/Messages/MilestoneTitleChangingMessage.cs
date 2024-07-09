using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Enums;
using VisGitCore.ViewModels;

namespace VisGitCore.Messages
{
    internal class MilestoneTitleChangingMessage : ValueChangedMessage<MilestoneViewModel>
    {
        public string NewTitle;

        public MilestoneTitleChangingMessage(MilestoneViewModel value) : base(value)
        {
        }
    }
}