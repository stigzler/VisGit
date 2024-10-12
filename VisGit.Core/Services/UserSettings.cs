using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;

namespace VisGitCore.Services
{
    public partial class OptionsProvider
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
            get => Encryption.DpapiToInsecureString(Encryption.DpapiDecryptString(personalAccessToken));
            set => personalAccessToken = Encryption.DpapiEncryptString(Encryption.DpapiToSecureString(value));
        }

        [Category("UI Settings")]
        [DisplayName("Repository DropDown Width")]
        [Description("The width of the Repository selector drop-down. Adjust to suit your layout.")]
        [DefaultValue(100)]
        public int RepositoryDropDownWidth { get; set; } = 100;

        [Category("UI Settings")]
        [DisplayName("Warn on Repository Refresh")]
        [Description("Issues a 'Are you sure?' warning on Repo refresh to help user avoid loosing any unsaved changes.")]
        [DefaultValue(true)]
        public bool WarnOnRepoRefresh { get; set; } = true;
    }
}