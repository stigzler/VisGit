using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.ViewModels;

namespace VisGitCore.Messages
{
    public class LabelDeletedMessage : ValueChangedMessage<LabelViewModel>
    {
        public LabelDeletedMessage(LabelViewModel value) : base(value)
        {
        }
    }
}