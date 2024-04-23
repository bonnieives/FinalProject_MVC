using FinalProject_MVC.Controllers;
using FinalProject_MVC.DAL;
using FinalProject_MVC.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FinalProject_MVC.DI
{
    public class DIContainer : IDependencyResolver
    {
        private readonly Dictionary<string, IAuthService> _authServices = new Dictionary<string, IAuthService>();
        private readonly Dictionary<Type, object> _registeredServices = new Dictionary<Type, object>();
        private readonly FinalProjectContext _dbContext;

        public DIContainer(FinalProjectContext dbContext)
        {
            _dbContext = dbContext;
            RegisterServices();
        }

        private void RegisterServices()
        {
            _authServices["default"] = new AuthService(_dbContext);
            _authServices["administrator"] = new AuthService(_dbContext); // Example for "administrator" category
            _authServices["owner"] = new AuthService(_dbContext); // Example for "owner" category
            _authServices["tenant"] = new AuthService(_dbContext); // Example for "tenant" category
            _authServices["manager"] = new AuthService(_dbContext); // Example for "manager" category

            _registeredServices[typeof(FinalProjectContext)] = _dbContext;

            _registeredServices[typeof(IAuthService)] = _authServices["default"];
        }

        public object GetService(Type serviceType)
        {
            if (_registeredServices.ContainsKey(serviceType))
            {
                return _registeredServices[serviceType];
            }
            else if (serviceType.IsSubclassOf(typeof(Controller)))
            {
                return CreateControllerInstance(serviceType);
            }
            return null;
        }

        private object CreateControllerInstance(Type controllerType)
        {
            if (controllerType == typeof(HomeController))
            {
                // Get a new instance of IAuthService
                var authService = GetAuthService("default");
                // Create a new instance of HomeController with dependencies
                return new HomeController(authService, _dbContext);
            }
            else
            {
                // For other controller types, create a new instance without dependencies
                return Activator.CreateInstance(controllerType);
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }

        public IAuthService GetAuthService(string category)
        {
            if (!_authServices.ContainsKey(category.ToLower()))
            {
                throw new ArgumentException($"No AuthService registered for category '{category}'.");
            }

            return _authServices[category.ToLower()];
        }
    }
}
