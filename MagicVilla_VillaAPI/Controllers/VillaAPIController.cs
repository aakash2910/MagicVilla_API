using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        // Get all the villas

        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.VillaList);
        }

        // Get Villa by id

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id) 
        {
            if(id == 0)
            {
                return BadRequest();
            }
            
            var villa = VillaStore.VillaList.FirstOrDefault(villa => villa.Id == id);

            if(villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }


        // Create Villa

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            /*if (VillaStore.VillaList.FirstOrDefault(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Custom Error", "Villa already exists");
                return BadRequest(ModelState);
            }*/

            
            if(villaDTO.Id > 0)
            {  
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.VillaList.Last<VillaDTO>().Id + 1;
            VillaStore.VillaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id} , villaDTO);
            //return Ok(villaDTO);
        }


        // Delete Villa

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            
            var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == id);
            
            if(villa == null) 
            {
                return NotFound();
            }

            VillaStore.VillaList.Remove(villa);

            return NoContent();
        }

        // Update Villa

        [HttpPut("{id:int}",Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if(villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == id);
            villa.Name = villaDTO.Name;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(id == 0 || patchDTO == null)
            {
                return BadRequest();
            }

            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if(villa == null)
            {
                return BadRequest();  // NotFound()
            }
            patchDTO.ApplyTo(villa,ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
