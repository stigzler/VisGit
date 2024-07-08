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
    internal class LabelNameChangingMessage : ValueChangedMessage<LabelViewModel>
    {
        public string NewName;

        public LabelNameChangingMessage(LabelViewModel value) : base(value)
        {
        }
    }
}