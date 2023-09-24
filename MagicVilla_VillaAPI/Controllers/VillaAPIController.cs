using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MagicVilla_VillaAPI.Controllers
{
  
    // [Route("api/[controller]")]
    [Route("api/VillaAPI")]

    [ApiController] // Attribute
    public class VillaAPIController : ControllerBase
    {
        // Dependancies Injection
        protected APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._response = new APIResponse();
        }

        //----------------------------------------------------------------------------------------------------

        [HttpGet] //Endpoint
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(200)] // remove Undocumented
        public async Task< ActionResult <APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> VillaList = await _dbVilla.GetAllAsync();

                _response.Result = _mapper.Map<List<VillaDTO>>(VillaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            
            }
            return _response;
        }

        //----------------------------------------------------------------------------------------------------

        [HttpGet("{id:int}",Name ="GetVilla")]
        [Authorize(Roles = "admin")]
        // [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(200, Type = typeof(VillaDTO))]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                //ActionResult response the success code 200. if api work fine.

                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var Villa = await _dbVilla.GetAsync(u => u.Id == id);

                if (Villa == null) 
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response); 
                }

                _response.Result = _mapper.Map<VillaDTO>(Villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;

        }

        //----------------------------------------------------------------------------------------------------

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO CreateDTO) 
        {
            try
            {
                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == CreateDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
                    return BadRequest(ModelState);
                }
                if (CreateDTO == null) { return BadRequest(CreateDTO); }

                Villa villa = _mapper.Map<Villa>(CreateDTO); // that one line code replace whole code in blow code.

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
                await _dbVilla.CreateAsync(villa);

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }

        //----------------------------------------------------------------------------------------------------

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "CUSTOM")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id) 
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var Villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (Villa == null) { return NotFound(); }

                await _dbVilla.RemoveAsync(Villa);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }

        //----------------------------------------------------------------------------------------------------

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO UpdateDTO)
        {
            try
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

                await _dbVilla.UpdateAsync(model);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }

        //----------------------------------------------------------------------------------------------------

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> PatchDTO)
        {
            try { 
            if(PatchDTO == null || id == 0) { return  BadRequest(); }

            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked:false);

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
            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }
    }
}
