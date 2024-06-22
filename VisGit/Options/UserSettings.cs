using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGit.Core.Data
{
    internal class UserSettings : VisGit.Core.Interfaces.IUserSettings
    {
        private OptionsPage optionsPage;

        public string PersonalAccessToken { get => optionsPage.PersonalAccessToken; }

        public UserSettings()
        {
            optionsPage = OptionsPage.Instance;
        }
    }
}