using FinalProject_MVC.DAL;
using System;
using System.Linq;

namespace FinalProject_MVC.Services
{
    public interface IAuthService
    {
        bool AuthenticateUser(string username, string password, int categoryId);
        int GetUserCategory(string username);
        int GetUserCategoryId(string category);
    }

    public class AuthService : IAuthService
    {
        private readonly FinalProjectContext _dbContext;

        public AuthService(FinalProjectContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int GetUserCategory(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == username);
            return user?.CategoryId ?? 0;
        }

        public bool AuthenticateUser(string username, string password, int categoryId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == username);

            if (user == null)
            {
                return false;
            }

            if (user.CategoryId != categoryId)
            {
                return false;
            }

            return password == user.Password;
        }

        public int GetUserCategoryId(string category)
        {
            switch (category.ToLower())
            {
                case "administrator":
                    return 4;
                case "owner":
                    return 5;
                case "tenant":
                    return 6;
                case "manager":
                    return 7;
                default:
                    throw new ArgumentException("Invalid category.");
            }
        }
    }
}
