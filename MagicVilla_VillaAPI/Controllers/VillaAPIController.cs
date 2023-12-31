﻿using AutoMapper;
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
            _response = new APIResponse();
        }

        //----------------------------------------------------------------------------------------------------

        [HttpGet] //Endpoint
        //[ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(200)] // remove Undocumented
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy,
            [FromQuery] string? search)
        {
            try
            {
                IEnumerable<Villa> VillaList = await _dbVilla.GetAllAsync();

                if (occupancy > 0)
                {
                    VillaList = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy);
                }
                else
                {
                    VillaList = await _dbVilla.GetAllAsync();
                }
                if (!string.IsNullOrEmpty(search))
                {
                    VillaList = VillaList.Where(u => u.Name.ToLower().Contains(search));
                }

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

        [HttpGet("{id:int}", Name = "GetVilla")]
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
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromForm] VillaCreateDTO CreateDTO)
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

                if(CreateDTO.Image != null)
                {
                    string fileName = villa.Id + Path.GetExtension(CreateDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo file = new FileInfo(directoryLocation);

                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        CreateDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    villa.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    villa.ImageLocalPath = filePath;
                }
                else
                {
                    villa.ImageUrl = "https://placehold.co/600x400";
                }

                await _dbVilla.UpdateAsync(villa);
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

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "admin")]
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

                if (!string.IsNullOrEmpty(Villa.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), Villa.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

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
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromForm] VillaUpdateDTO UpdateDTO)
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

                if (UpdateDTO.Image != null)
                {
                    if (!string.IsNullOrEmpty(model.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), model.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);

                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    string fileName = UpdateDTO.Id + Path.GetExtension(UpdateDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        UpdateDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    model.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    model.ImageLocalPath = filePath;
                }
                else
                {
                    model.ImageUrl = "https://placehold.co/600x400";
                }

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
            try
            {
                if (PatchDTO == null || id == 0) { return BadRequest(); }

                var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

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

                if (villa == null) { return BadRequest(); }

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
