using System.Web.Http;
using Microsoft.Practices.Unity;
using VirtoCommerce.DerivativesModule.Core.Services;
using VirtoCommerce.DerivativesModule.Data.Repositories;
using VirtoCommerce.DerivativesModule.Data.Services;
using VirtoCommerce.DerivativesModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.DerivativesModule.Web
{
    public class Module : ModuleBase
    {
        private static readonly string _connectionString = ConfigurationHelper.GetConnectionStringValue("VirtoCommerce");
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void SetupDatabase()
        {
            using (var db = new DerivativeRepository(_connectionString, _container.Resolve<AuditableInterceptor>()))
            {
                var initializer = new SetupDatabaseInitializer<DerivativeRepository, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(db);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            _container.RegisterType<IDerivativeRepository>(new InjectionFactory(c => new DerivativeRepository(_connectionString, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));
            _container.RegisterType<IDerivativeService, DerivativeService>();
            _container.RegisterType<IDerivativeSearchService, DerivativeSearchService>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            // enable polymorphic types in API controller methods
            var httpConfiguration = _container.Resolve<HttpConfiguration>();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new PolymorphicDerivativesJsonConverter());
        }
    }
}
