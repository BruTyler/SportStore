using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using System.Configuration;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Infrastructure.Concrete;
using DataAccess.DbProvider;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory: DefaultControllerFactory
    {
        private readonly IKernel _ninjectKernel;
        public NinjectControllerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController)_ninjectKernel.Get(controllerType);
        }
        private void AddBindings()
        {
            _ninjectKernel.Bind<EFDbContext>().ToSelf().InRequestScope();
            _ninjectKernel.Bind<IProductRepository>().To<EFProductRepository>();


            EmailSettings emailSettings = new EmailSettings
            {
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
            };

            _ninjectKernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);

            _ninjectKernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }
    }
}