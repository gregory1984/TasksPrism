using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using Tasks_Prism.Views.Login;
using Tasks_Prism.Helpers;
using Tasks_Model.Services;
using Tasks_Model.Interfaces;

namespace Tasks_Prism
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Container.RegisterInstance(UnityNames.VersionData, new VersionData(), new ContainerControlledLifetimeManager());
            Container.RegisterInstance(UnityNames.CredentialsSerializer, new CredentialsSerializer<Credentials>());
            Container.RegisterType<IUserService, UserService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDatabaseService, DatabaseService>();
            Container.RegisterType<ITaskService, TaskService>();
            Container.RegisterType<IAdministrationService, AdministrationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPreferencesService, PreferencesService>(new ContainerControlledLifetimeManager());
            return Container.Resolve<LoginWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }
    }
}
