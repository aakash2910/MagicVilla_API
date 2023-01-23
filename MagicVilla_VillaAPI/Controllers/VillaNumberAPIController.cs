using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {   
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _dbVillaNumber;
        protected APIResponse _response;

        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            this._response = new();
        }

        // Get all the villas

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
            // Old without mapper
            //return Ok(await _appDbContext.Villas.ToListAsync());
        }

        // Get Villa by id

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaNumberDTO>> GetVillaNumber(int id) 
        {
            if(id == 0)
            {
                return BadRequest();
            }
            
            var villaNumber = await _dbVillaNumber.GetAsync(villa => villa.VillaNum == id);

            if(villaNumber == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaNumberDTO>(villaNumber));
        }


        // Create Villa

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if(await _dbVillaNumber.GetAsync(x => x.VillaNum == villaDTO.VillaNum) != null)
            {
                ModelState.AddModelError("Custom Error", "Villa Number already exists");
                return BadRequest(ModelState);
            }

            /*if(villaDTO.Id > 0)
            {  
                return StatusCode(StatusCodes.Status500InternalServerError);
            }*/

            // Mapping of VillaDTO to Villa
            /*Villa villa = new Villa()
            {
                Amenity = villaDTO.Amenity,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Rate = villaDTO.Rate,
                Details = villaDTO.Details,
                //Id= villaDTO.Id,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft
            };*/

            VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaDTO);

            

            await _dbVillaNumber.CreateAsync(villaNumber);

            return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNum} , villaNumber);
            //return Ok(villaDTO);
        }


        // Delete Villa

        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVillaNumber(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            
            var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNum == id);
            
            if(villaNumber == null) 
            {
                return NotFound();
            }

            await _dbVillaNumber.RemoveAsync(villaNumber);
            
            return NoContent();
        }

        // Update Villa

        [HttpPut("{id:int}",Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO villaNumberDTO)
        {
            if(villaNumberDTO == null || id != villaNumberDTO.VillaNum)
            {
                return BadRequest();
            }

            // Mapping VillaUpdateDTO to Villa
            /*Villa villa = new Villa()
            {
                Amenity = villaDTO.Amenity,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Rate = villaDTO.Rate,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft
            };*/

            VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberDTO);
            await _dbVillaNumber.UpdateAsync(villaNumber);
            
            return NoContent();
        }        
    }
}
