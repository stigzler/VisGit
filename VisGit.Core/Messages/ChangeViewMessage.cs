using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Enums;

namespace VisGitCore.Messages
{
    internal class ChangeViewMessage : ValueChangedMessage<ViewRequest>
    {
        public ChangeViewMessage(ViewRequest value) : base(value)
        {
        }
    }
}