using System;
using System.Linq;
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
            return _context.UserMaster.Find(userId);
        }

        public UserMaster GetUserMaster(string emailId)
        {
            return _context.UserMaster.FirstOrDefault(user => string.Equals(user.UserEmail, emailId));
        }

        public UserDetails GetUserDetails(int userId)
        {
            return _context.UserDetails.FirstOrDefault(user => Equals(user.UserId, userId));
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

        public void UpdateMasterInformation(UserMaster data)
        {
            var result = _context.UserMaster.Find(data.UserId);
            if (result == null)
            {
                throw new DbUpdateException($"Couldnt get data for userId {data.UserId}", new Exception());
            }

            result.UserPassword = data.UserPassword;
            result.Salt = data.Salt;
        }

        public void UnMarkUserAddressNotDefault(int userId)
        {
            var result = _context.UserAddress.Where(query => Equals(query.UserId, userId));
            if (!result.Any())
            {
                return;
            }

            foreach (var userAddress in result)
            {
                userAddress.IsDefaultAddress = false;
            }
        }

        public void UpdateParticularAddress(int addressId, UserAddress address)
        {
            var result = _context.UserAddress.Find(addressId);
            if (result == null)
            {
                throw new DbUpdateException($"Couldnt get data for addressId {addressId}", new Exception());
            }

            if (address.IsDefaultAddress != null)
            {
                result.IsDefaultAddress = address.IsDefaultAddress;
            }

            if (address.AddressLineOne != null)
            {
                result.AddressLineOne = address.AddressLineOne;
            }

            if (address.AddressLineTwo != null)
            {
                result.AddressLineTwo = address.AddressLineTwo;
            }

            if (address.City != null)
            {
                result.City = address.City;
            }

            if (address.State != null)
            {
                result.State = address.State;
            }

            if (address.Country != null)
            {
                result.Country = address.Country;
            }

            if (address.Postcode != null)
            {
                result.Postcode = address.Postcode;
            }

            if (address.PhoneNumber != null)
            {
                result.PhoneNumber = address.PhoneNumber;
            }
        }

        public int SaveData()
        {
            return _context.SaveChanges();
        }
    }

    public interface IUserRepository
    {
        UserMaster GetUserMaster(int userId);

        UserMaster GetUserMaster(string emailId);

        UserDetails GetUserDetails(int userId);

        Task<T> AddDataToDataSet<T>(T data) where T : class;

        Task<int> SaveDataAsync();

        void UpdateMasterInformation(UserMaster data);

        void UnMarkUserAddressNotDefault(int userId);

        int SaveData();

        void UpdateParticularAddress(int addressId, UserAddress address);
    }
}
