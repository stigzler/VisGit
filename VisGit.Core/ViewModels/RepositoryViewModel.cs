using CommunityToolkit.Mvvm.ComponentModel;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace VisGit.Core.ViewModels
{
    public partial class RepositoryViewModel : BaseViewModel
    {
        #region Properties =========================================================================================

        // Fields
        [ObservableProperty]
        private Repository _gitRepository;

        [ObservableProperty]
        private string _name;

        // Public Members
        //public Repository GitRepository { get => _gitRepository; set => SetProperty(ref _gitRepository, value); }

        // public string Name { get => _name; set => SetProperty(ref _name, value); }
        //public string Id { get => _gitRepository.Id.ToString(); }
        //public string CreatedAt { get => _gitRepository.CreatedAt.ToString(); }

        #endregion Properties =========================================================================================

        #region Constructor =========================================================================================

        public RepositoryViewModel()
        {
        }

        public RepositoryViewModel(Repository repository)
        {
            UpdateProperties(repository);
        }

        #endregion Constructor =========================================================================================

        #region Methods: Private =========================================================================================

        private void UpdateProperties(Repository repository)
        {
            GitRepository = repository;
            Name = repository.Name;
        }

        #endregion Methods: Private =========================================================================================
    }
}