using AutoMapper;
using PlayArt.Core.DTOs;
using PlayArt.Core.entities;
using PlayArt.Core.Entities;
using PlayArt.Core.Interfaces;
using PlayArt.Core.Interfaces.IRepositories;
using PlayArt.Core.Interfaces.Services_interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayArt.Service
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserRoleRepository _userrolerepository;
        private readonly IRoleRepository _rolerepository;
        public UserService(IUserRepository repository, IMapper mapper, IUserRoleRepository userrolerepository, IRoleRepository rolerepository)
        {
            _repository = repository;
            _mapper = mapper;
            _userrolerepository = userrolerepository;
            _rolerepository = rolerepository;
        }

        public IEnumerable<UserDTO> GetList()
        {
            return _mapper.Map<IEnumerable<UserDTO>>(_repository.GetAllData());
        }

        public UserDTO GetById(int id)
        {
            return _mapper.Map<UserDTO>(_repository.GetById(id));
        }

        public List<UserGrowthDTO> GetUserGrowthByMonth()
        {
            var result =  GetList()
                .GroupBy(u => new { Year = u.CreatedAt.Year, Month = u.CreatedAt.Month })
                .Select(g => new UserGrowthDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    UserCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return result;
        }
        public async Task<UserDTO> AddUserAsync(UserDTO user)
        {
            int id = _rolerepository.GetIdByRole("User");
            
            if (_repository.GetById(user.Id) == null)
            {
                var result = await _repository.AddAsync(_mapper.Map<User>(user));
           
                _userrolerepository.AddAsync(new UserRoles() { RoleId = id, UserId = result.Id });

                return _mapper.Map<UserDTO>(result);
            }
            return null;
        }

        public async Task<UserDTO> UpdateAsync(int id, UserDTO user)
        {
            if (id < 0)
                return null;

            var result = await _repository.UpdateAsync(_mapper.Map<User>(user), id);
            return _mapper.Map<UserDTO>(result);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            if (id < 0) return false;
            return await _repository.DeleteAsync(id);
        }

        public UserDTO GetUserByEmail(string email)
        {
            return _mapper.Map<UserDTO>(_repository.GetByUserByEmail(email));
        }

        public string Authenticate(string email, string password)
        {
            User user =  _repository.GetByUserByEmail(email);
            if (user == null || !user.Password.Equals(password))
            {
                return null;
            }
            var userRole =  _userrolerepository.GetByUserId(user.Id);
            if (userRole == null)
                return null;

            return userRole.Role.RoleName;
        }
    }
}
