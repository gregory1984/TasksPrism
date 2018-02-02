using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Tasks_Model.Entities;
using Tasks_Model.Helpers;
using NHibernate.Tool.hbm2ddl;

namespace Tasks_Model.Database
{
    public static class Hibernate
    {
        private static ISessionFactory sessionFactory;
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    var config = ConfigurationManager.OpenExeConfiguration(Constants.AssemblyName);
                    var section = config.GetSection("connectionStrings");

                    DecryptAppConfigSection(section);

                    sessionFactory = Fluently
                        .Configure()
                        .Database(MySQLConfiguration.Standard.ShowSql().ConnectionString(c => c.FromConnectionStringWithKey("Tasks")))
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<User>())
                        .ExposeConfiguration(c => new SchemaUpdate(c).Execute(false, true))
                        .BuildSessionFactory();

                    EncryptAppConfigSection(section);
                    config.Save(ConfigurationSaveMode.Modified);

                }
                return sessionFactory;
            }
        }

        private static void DecryptAppConfigSection(ConfigurationSection section)
        {
            if (section.SectionInformation.IsProtected)
                section.SectionInformation.UnprotectSection();
        }

        private static void EncryptAppConfigSection(ConfigurationSection section)
        {
            section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            section.SectionInformation.ForceSave = true;
        }

        public static void CheckConnection()
        {
            using (var session = SessionFactory.OpenSession()) { }
        }
    }
}
