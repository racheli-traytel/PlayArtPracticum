using Microsoft.EntityFrameworkCore;
using PlayArt.Core.entities;
using PlayArt.Core.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayArt.Data.Repositories
{
   public class RoleRepository:IRoleRepository
    {
       private readonly DataContext _dataContext;
        public RoleRepository(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<Roles> AddAsync(Roles role)
        {
            await _dataContext.AddAsync(role);
            role.CreatedAt = DateTime.UtcNow;
            await _dataContext.SaveChangesAsync();

            return role;
        }

        public async Task DeleteAsync(int id)
        {
            var role = GetRoleById(id);
            _dataContext.Roles.Remove(role);
          await  _dataContext.SaveChangesAsync();
        }

        public IEnumerable<Roles> GetAll()
        {
            return  _dataContext.Roles.ToList();
        }

        public Roles GetRoleById(int id)
        {
            return  _dataContext.Roles.Find(id);
        }

 
        public  int GetIdByRole(string role)
        {
            var r =  _dataContext.Roles.FirstOrDefault(r => r.RoleName == role);
            return r.Id;
        }

        public async Task<bool> UpdateAsync(int id, Roles role)
        {
            Roles existingRole =GetRoleById(id);
            if (existingRole != null)
            {
                existingRole.RoleName = role.RoleName;
                existingRole.Description = role.Description;
                existingRole.UpdatedAt = DateTime.UtcNow;
               await _dataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
