using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MagicVilla_VillaAPI.Controllers
{

    // [Route("api/[controller]")]
    [Route("api/VillaNumberAPI")]
    [ApiController] // Attribute

    public class VillaNumberAPIController : ControllerBase
    {
        // Dependancies Injection
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            _response = new APIResponse();
            _dbVilla = dbVilla;
        }

        //----------------------------------------------------------------------------------------------------

        //[MapToApiVersion("1.0")]
        [HttpGet] //Endpoint
        [ProducesResponseType(200)] // remove Undocumented
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> VillaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");

                _response.Result = _mapper.Map<List<VillaNumberDTO>>(VillaNumberList);
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

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {

                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var VillaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

                if (VillaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaNumberDTO>(VillaNumber);
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

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO CreateDTO)
        {
            try
            {
                if (await _dbVillaNumber.GetAsync(u => u.VillaNo == CreateDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number already Exists!");
                    return BadRequest(ModelState);
                }
                if (await _dbVilla.GetAsync(u => u.Id == CreateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }
                if (CreateDTO == null) { return BadRequest(CreateDTO); }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(CreateDTO);

                await _dbVillaNumber.CreateAsync(villaNumber);

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }

        //----------------------------------------------------------------------------------------------------

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var VillaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (VillaNumber == null) { return NotFound(); }

                await _dbVillaNumber.RemoveAsync(VillaNumber);

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

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO UpdateDTO)
        {
            try
            {
                if (UpdateDTO == null || id != UpdateDTO.VillaNo) { return BadRequest(); }
                if (await _dbVilla.GetAsync(u => u.Id == UpdateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(UpdateDTO);

                await _dbVillaNumber.UpdateAsync(model);

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

        /*  [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
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
          } */
    }
}
