using FinalProject_MVC.Services;
using System;
using System.Web.Mvc;

namespace FinalProject_MVC.Authorization
{
    public class CategoryAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly int[] _allowedCategoryIds;

        public CategoryAuthorizeAttribute(params int[] allowedCategoryIds)
        {
            _allowedCategoryIds = allowedCategoryIds;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            var authService = DependencyResolver.Current.GetService<IAuthService>();
            int userCategoryId = authService.GetUserCategory(httpContext.User.Identity.Name);

            return Array.Exists(_allowedCategoryIds, categoryId => categoryId == userCategoryId);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult
            {
                ViewName = "Error",
            };
        }
    }
}
