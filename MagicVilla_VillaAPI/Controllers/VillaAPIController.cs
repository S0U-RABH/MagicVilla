using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MagicVilla_VillaAPI.Controllers
{
  
    // [Route("api/[controller]")]
    [Route("api/VillaAPI")]

    [ApiController] // Attribute
    public class VillaAPIController : ControllerBase
    {
        // Dependancies Injection
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        //----------------------------------------------------------------------------------------------------

        [HttpGet] //Endpoint
        [ProducesResponseType(200)] // remove Undocumented
        public async Task< ActionResult <IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> VillaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(VillaList));
        }

        [HttpGet("{id:int}",Name ="GetVilla")]
        // [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(200, Type = typeof(VillaDTO))]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            //ActionResult response the success code 200. if api work fine.

            if (id == 0) 
            {
                return BadRequest();
            }

            var Villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            
            if(Villa == null) { return NotFound(); }

            return Ok(_mapper.Map<VillaDTO>(Villa));
        }

        //----------------------------------------------------------------------------------------------------

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO CreateDTO) 
        {
            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == CreateDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                return BadRequest(ModelState);
            }
            if (CreateDTO == null) { return BadRequest(CreateDTO); }

            Villa model = _mapper.Map<Villa>(CreateDTO); // that one line code replace whole code in blow code.

            //Villa model = new()
            //{
            //    Name = CreateDTO.Name,
            //    Details = CreateDTO.Details,
            //    Rate = CreateDTO.Rate,
            //    Sqft = CreateDTO.Sqft,
            //    Amenity = CreateDTO.Amenity,
            //    Occupancy = CreateDTO.Occupancy,
            //    ImageUrl = CreateDTO.ImageUrl
            //};
           await _db.Villas.AddAsync(model);
           await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }

        //----------------------------------------------------------------------------------------------------

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteVilla(int id) 
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var Villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (Villa == null) { return NotFound(); }

            _db.Villas.Remove(Villa);
            await _db.SaveChangesAsync();
            return NoContent();
            //return Ok(Villa);   //if you send the message that is villa is delete.
        }

        //----------------------------------------------------------------------------------------------------

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO UpdateDTO)
        {
            if (UpdateDTO == null || id != UpdateDTO.Id) { return BadRequest(); }

            Villa model = _mapper.Map<Villa>(UpdateDTO); // that one line code replace whole code in blow code.

            //Villa model = new()
            //{
            //    Id = UpdateDTO.Id,
            //    Name = UpdateDTO.Name,
            //    Details = UpdateDTO.Details,
            //    Rate = UpdateDTO.Rate,
            //    Sqft = UpdateDTO.Sqft,
            //    Amenity = UpdateDTO.Amenity,
            //    Occupancy = UpdateDTO.Occupancy,
            //    ImageUrl = UpdateDTO.ImageUrl
            //};

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        //----------------------------------------------------------------------------------------------------

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> PatchDTO)
        {
            if(PatchDTO == null || id == 0) { return  BadRequest(); }

            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa); // that one line code replace whole code in blow code.
            //VillaUpdateDTO villaDTO = new()
            //{
            //    Amenity = villa.Amenity,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //    ImageUrl = villa.ImageUrl,
            //    Name = villa.Name,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft
            //};

            if (villa == null) {  return BadRequest(); }

            PatchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO); // that one line code replace whole code in blow code.
            //Villa model = new()
            //{
            //    Id = villaDTO.Id,
            //    Name = villaDTO.Name,
            //    Details = villaDTO.Details,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //    Amenity = villaDTO.Amenity,
            //    Occupancy = villaDTO.Occupancy,
            //    ImageUrl = villaDTO.ImageUrl
            //};
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            if (!ModelState.IsValid) { return BadRequest(ModelState); } 
            return NoContent();
        }
    }
}
