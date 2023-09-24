using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaServicecs _VillaServicec;
        private readonly IMapper _mapper;

        public VillaController(IVillaServicecs villaServicec, IMapper mapper)
        {
            _VillaServicec = villaServicec;
            _mapper = mapper;
        }
        public async Task< IActionResult> IndexVilla()
        {
            List<VillaDTO> list = new List<VillaDTO>();
            var responce = await _VillaServicec.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (responce != null && responce.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responce.Result));
            }
            return View(list);
        }
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO  model)
        {
            if(ModelState.IsValid)
            {
                var responce = await _VillaServicec.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (responce != null && responce.IsSuccess)
                {
                   TempData["success"] = "Villa Created Successfully";
                   return RedirectToAction(nameof(IndexVilla));
                }
            }
            TempData["error"] = "Error Encountered.";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            var responce = await _VillaServicec.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
            if (responce != null && responce.IsSuccess)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(responce.Result));
                return View(_mapper.Map<VillaUpdateDTO>(model));
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                TempData["success"] = "Villa Updated Successfully";
                var responce = await _VillaServicec.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (responce != null && responce.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            TempData["error"] = "Error Encountered.";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            var responce = await _VillaServicec.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
            if (responce != null && responce.IsSuccess)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(responce.Result));
                return View(model);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVilla(VillaUpdateDTO model)
        {
            
                var responce = await _VillaServicec.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
                if (responce != null && responce.IsSuccess)
                {
                TempData["success"] = "Villa Deleted Successfully";
                return RedirectToAction(nameof(IndexVilla));
                }

            TempData["error"] = "Error Encountered.";
            return View(model);
        }
    }
}