using Data.Interfaces;
using Data.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnection sql;
        public UserRepository(ConnectionStrings connection)
        {
            sql = new SqlConnection(connection.Value);
        }

        public Task<IEnumerable<Role>> GetRoles()
        {
            var roles = sql.GetAllAsync<Role>();
            return roles;
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            var users = sql.GetAllAsync<User>();
            return users;
        }
    }
}
