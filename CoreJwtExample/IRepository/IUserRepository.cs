using CoreJwtExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJwtExample.IRepository
{
    public interface IUserRepository
    {
        Task<User> Save(User obj);
        Task<User> Get(User objId);
        Task<List<User>> Gets();
        Task<User> GetByUsernamePassword(User user);
        Task<User> GetByUsername(User user);
        Task<String> Delete(User obj);
    }
}
