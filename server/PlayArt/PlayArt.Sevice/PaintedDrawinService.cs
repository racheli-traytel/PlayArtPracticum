using AutoMapper;
using PlayArt.Core.DTOs;
using PlayArt.Core.entities;
using PlayArt.Core.Entities;
using PlayArt.Core.Interfaces;
using PlayArt.Core.Interfaces.Services_interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayArt.Service
{
    public class PaintedDrawingService : IPaintedDrawingService
    {
        private readonly IRepository<PaintedDrawing> _repository;
        private readonly IMapper _mapper;

        public PaintedDrawingService(IRepository<PaintedDrawing> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<PaintedDrawingDTO> GetList()
        {
            return _mapper.Map<IEnumerable<PaintedDrawingDTO>>(_repository.GetAllData());
        }

        public PaintedDrawingDTO GetById(int id)
        {
            return _mapper.Map<PaintedDrawingDTO>(_repository.GetById(id));
        }

        public async Task<PaintedDrawingDTO> AddPaintedDrawingAsync(PaintedDrawingDTO paintedDrawing)
        {
            if (_repository.GetById(paintedDrawing.Id) == null)
            {
                var result = await _repository.AddAsync(_mapper.Map<PaintedDrawing>(paintedDrawing));
                return _mapper.Map<PaintedDrawingDTO>(result);
            }
            return null;
        }

        public async Task<PaintedDrawingDTO> UpdateAsync(int id, PaintedDrawingDTO paintedDrawing)
        {
            if (id < 0)
                return null;

            var result = await _repository.UpdateAsync(_mapper.Map<PaintedDrawing>(paintedDrawing), id);
            return _mapper.Map<PaintedDrawingDTO>(result);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            if (id < 0) return false;
            return await _repository.DeleteAsync(id);
        }
    }
}
