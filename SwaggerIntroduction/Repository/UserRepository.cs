using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwaggerIntroduction.Models.DataModels;

namespace SwaggerIntroduction.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public UserMaster GetUserMaster(int userId)
        {
            using (_context)
            {
                return _context.UserMaster.Find(userId);
            }
        }

        public async Task<T> AddDataToDataSet<T>(T data) where T : class
        {
            var dataSet = _context.GetDbSet<T>();
            var result = await dataSet.AddAsync(data);
            if (result.State != EntityState.Added)
            {
                throw new DbUpdateException("Couldn't add to data set", new Exception($"{result.Entity}"));
            }

            return data;
        }

        public async Task<int> SaveDataAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveData()
        {
            return _context.SaveChanges();
        }
    }

    public interface IUserRepository
    {
        UserMaster GetUserMaster(int userId);

        Task<T> AddDataToDataSet<T>(T data) where T : class;

        Task<int> SaveDataAsync();

        int SaveData();
    }
}
