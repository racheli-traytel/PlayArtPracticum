using PlayArt.Core.entities;
using PlayArt.Core.Interfaces.IRepositories;
using PlayArt.Core.Interfaces.Services_interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayArt.Sevice
{
   public class UserRoleService:IUserRoleService
    {
        private readonly IUserRoleRepository _userRolesRepository;
        private readonly IRoleRepository _roleRpository;

        public UserRoleService(IUserRoleRepository userRolesRepository, IRoleRepository roleRpository)
        {
            _userRolesRepository = userRolesRepository;
            _roleRpository = roleRpository;

        }
        public async Task<UserRoles> AddAsync(string role, int userId)
        {
            int r = _roleRpository.GetIdByRole(role);
            if (r < 0)
                return null;
            UserRoles u = await _userRolesRepository.AddAsync(new UserRoles { RoleId = r, UserId = userId });
            return u;
        }
    }
}
