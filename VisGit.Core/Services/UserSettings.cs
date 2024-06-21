using System.ComponentModel;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;

namespace VisGit.Core.Services
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.UserSettingsOptions), "VisGit.Services", "UserSettings", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class UserSettingsOptions : BaseOptionPage<UserSettings>
        { }
    }

    public class UserSettings : BaseOptionModel<UserSettings>
    {
        private string personalAccessToken = "{UNSET}";

        [Category("Github Settings")]
        [DisplayName("Personal Access Token")]
        [Description("Your GitHub Personal Access Token. Get one via Github > Settings > Developer Settings > Personal Access Tokens > Fine-grained tokens")]
        [DefaultValue("{UNSET}")]
        [PasswordPropertyText(true)]
        public string PersonalAccessToken
        {
            get => Encyption.DpapiToInsecureString(Encyption.DpapiDecryptString(personalAccessToken));
            set => personalAccessToken = Encyption.DpapiEncryptString(Encyption.DpapiToSecureString(value));
        }

        [Category("Github Settings")]
        [DisplayName("Auto-login")]
        [Description("Logs-in on startup")]
        [DefaultValue(false)]
        public bool AutoLogin { get; set; } = false;
    }
}