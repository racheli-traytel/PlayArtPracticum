using PlayArt.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using PlayArt.Core.Interfaces.Services_interfaces;
using PlayArt.Api.Models;
using AutoMapper;
using PlayArt.Core.DTOs;
using PlayArt.Core.entities;

namespace PlayArt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrawingController : ControllerBase
    {
        readonly IDrawingService _drawingService;
        readonly IMapper _mapper;

        public DrawingController(IDrawingService drawingService, IMapper mapper)
        {
            _drawingService = drawingService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_drawingService.GetList());
        }

        [HttpGet("{category}")]
        public ActionResult GetWorksheets([FromQuery] DrawingCategory? category)
        {
            var worksheets = _drawingService.GetDrawingCategory(category);
            return Ok(new { worksheets = worksheets });
        }

        [HttpGet("{id}")]
        public ActionResult<Drawing> GetId(int id)
        {
            if (id < 0) return BadRequest();
            var result = _drawingService.GetById(id);
            if (result == null) { return NotFound(); }
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DrawingPostModel drawing)
        {
            if (drawing == null) return BadRequest();
            var drawingDto = _mapper.Map<DrawingDTO>(drawing);
            var result = await _drawingService.AddDrawingAsync(drawingDto);
            if (result == null)
                return BadRequest("user already exist");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] DrawingPostModel drawing)
        {
            if (id < 0||drawing==null) return BadRequest();
            var success = await _drawingService.UpdateAsync(id, _mapper.Map<DrawingDTO>(drawing));
            if (success == null) return NotFound();
            return Ok(success.Id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id < 0) return BadRequest();
            bool success = await _drawingService.RemoveAsync(id);
            if (!success) return NotFound();
            return Ok(success);
        }
    }
}