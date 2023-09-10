using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/VillaAPI")]

    [ApiController] // Attribute
    public class VillaAPIController : ControllerBase
    {
        [HttpGet] //Endpoint
        [ProducesResponseType(200)] // remove Undocumented
        public ActionResult <IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok( VillaStore.villaList);
        }

        [HttpGet("{id:int}",Name ="GetVilla")]
        // [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(200, Type = typeof(VillaDTO))]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            //ActionResult response the success code 200. if api work fine.

            if (id == 0) { return BadRequest(); }

            var Villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            
            if(Villa == null) { return NotFound(); }

            return Ok(Villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO) 
        {
            if(villaDTO == null) { return BadRequest( villaDTO ); }
            if (villaDTO.Id > 0) { return StatusCode(StatusCodes.Status500InternalServerError) ; }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;
            VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id) 
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var Villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (Villa == null) { return NotFound(); }

            VillaStore.villaList.Remove(Villa);
            return NoContent();
            //return Ok(Villa);   //if you send the message that is villa is delete.
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id) { return BadRequest(); }
            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            
            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> PatchDYO)
        {
            if(PatchDYO == null || id == 0) { return  BadRequest(); }   

            var Villa = VillaStore.villaList.FirstOrDefault(u  => u.Id == id);
            if (Villa == null) {  return BadRequest(); }

            PatchDYO.ApplyTo(Villa, ModelState);
            if(!ModelState.IsValid) { return BadRequest(ModelState); } 

            return NoContent();
        }
    }
}
