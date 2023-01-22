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

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {   
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;

        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
        }

        // Get all the villas

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync(); 
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
            
            // Old without mapper
            //return Ok(await _appDbContext.Villas.ToListAsync());
        }

        // Get Villa by id

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id) 
        {
            if(id == 0)
            {
                return BadRequest();
            }
            
            var villa = await _dbVilla.GetAsync(villa => villa.Id == id);

            if(villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }


        // Create Villa

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if(await _dbVilla.GetAsync(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom Error", "Villa already exists");
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

            Villa villa = _mapper.Map<Villa>(villaDTO);

            

            await _dbVilla.CreateAsync(villa);

            return CreatedAtRoute("GetVilla", new { id = villa.Id} , villa);
            //return Ok(villaDTO);
        }


        // Delete Villa

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            
            var villa = await _dbVilla.GetAsync(x => x.Id == id);
            
            if(villa == null) 
            {
                return NotFound();
            }

            await _dbVilla.RemoveAsync(villa);
            
            return NoContent();
        }

        // Update Villa

        [HttpPut("{id:int}",Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaDTO)
        {
            if(villaDTO == null || id != villaDTO.Id)
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

            Villa villa = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.UpdateAsync(villa);
            
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if(id == 0 || patchDTO == null)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id);

            VillaUpdateDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Name = villa.Name,
                ImageUrl = villa.ImageUrl,
                Rate =  villa.Rate,
                Details = villa.Details,
                Id = villa.Id,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft
            };

            if (villa == null)
            {
                return BadRequest();  // NotFound()
            }
            patchDTO.ApplyTo(villaDTO,ModelState);

            Villa updatedVilla = new Villa()
            {
                Amenity = villaDTO.Amenity,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Rate = villaDTO.Rate,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft
            };

            await _dbVilla.UpdateAsync(updatedVilla);    

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
