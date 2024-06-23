using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGit.Core.Interfaces;

namespace VisGit.Core.Data
{
    internal class UserSettings : IUserSettings
    {
        //private OptionsPage optionsPage;

        public string PersonalAccessToken { get; set; }

        public UserSettings()
        {
            //optionsPage = OptionsPage.Instance;
        }
    }
}