﻿using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
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
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public VillaAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        // Get all the villas

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _appDbContext.Villas.ToListAsync();
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
            
            var villa = await _appDbContext.Villas.FirstOrDefaultAsync(villa => villa.Id == id);

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
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if(await _appDbContext.Villas.FirstOrDefaultAsync(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom Error", "Villa already exists");
                return BadRequest(ModelState);
            }

            
            if(villaDTO.Id > 0)
            {  
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa villa = new Villa()
            {
                Amenity= villaDTO.Amenity,
                Name= villaDTO.Name,
                ImageUrl= villaDTO.ImageUrl,
                Rate= villaDTO.Rate,
                Details= villaDTO.Details,
                Id= villaDTO.Id,
                Occupancy= villaDTO.Occupancy,
                Sqft= villaDTO.Sqft
            };

            await _appDbContext.Villas.AddAsync(villa);
            await _appDbContext.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id} , villaDTO);
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
            
            var villa = await _appDbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);
            
            if(villa == null) 
            {
                return NotFound();
            }
               
            _appDbContext.Villas.Remove(villa);
            await _appDbContext.SaveChangesAsync();

            return NoContent();
        }

        // Update Villa

        [HttpPut("{id:int}",Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if(villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }

            Villa villa = new Villa()
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

            _appDbContext.Villas.Update(villa);
            await _appDbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(id == 0 || patchDTO == null)
            {
                return BadRequest();
            }

            var villa = await _appDbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);

            VillaDTO villaDTO = new()
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

            _appDbContext.Villas.Update(updatedVilla);
            await _appDbContext.SaveChangesAsync();    

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
