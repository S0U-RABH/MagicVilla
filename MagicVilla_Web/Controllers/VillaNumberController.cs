﻿using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberServicecs _villaNumberService;
        private readonly IVillaServicecs _VillaService;
        private readonly IMapper _mapper;

        public VillaNumberController(IVillaNumberServicecs villaNumberService, IMapper mapper, IVillaServicecs villaServicec)
        {
            _villaNumberService = villaNumberService;
            _mapper = mapper;
            _VillaService = villaServicec;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new List<VillaNumberDTO>();
            var responce = await _villaNumberService.GetAllAsync<APIResponse>();
            if (responce != null && responce.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(responce.Result));
            }
            return View(list);
        }

        public async Task<IActionResult> CreateVillaNumber() 
        {
            VillaNumberCreateVM villaNumberVM =new VillaNumberCreateVM();
            var responce = await _VillaService.GetAllAsync<APIResponse>();
            if (responce != null && responce.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(responce.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var responcee = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
                if (responcee != null && responcee.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if(responcee.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages",responcee.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            var responce = await _VillaService.GetAllAsync<APIResponse>();
            if (responce != null && responce.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(responce.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(model);
        }

        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
        {
            VillaNumberUpdateVM villaNumberVM = new VillaNumberUpdateVM();

            var responce = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (responce != null && responce.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(responce.Result));
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
            }

            responce = await _VillaService.GetAllAsync<APIResponse>();
            if (responce != null && responce.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(responce.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(villaNumberVM); 
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            //if (ModelState.IsValid)
            //{
            //    var responcee = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
            //    if (responcee != null && responcee.IsSuccess)
            //    {
            //        return RedirectToAction(nameof(IndexVillaNumber));
            //    }
            //    else
            //    {
            //        if (responcee.ErrorMessages.Count > 0)
            //        {
            //            ModelState.AddModelError("ErrorMessages", responcee.ErrorMessages.FirstOrDefault());
            //        }
            //    }
            //}
            if (ModelState.IsValid)
            {
                var responcee = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if (responcee != null)
                {
                    if (responcee.IsSuccess)
                    {
                        return RedirectToAction(nameof(IndexVillaNumber));
                    }
                    else
                    {
                        if (responcee.ErrorMessages != null && responcee.ErrorMessages.Count > 0)
                        {
                            ModelState.AddModelError("ErrorMessages", responcee.ErrorMessages.FirstOrDefault());
                        }
                        else
                        {
                            ModelState.AddModelError("ErrorMessages", "An error occurred during the update.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("ErrorMessages", "No response from the service.");
                }
            }


            var responce = await _VillaService.GetAllAsync<APIResponse>();
            if (responce != null && responce.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(responce.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
        {
            VillaNumberDeleteVM villaNumberVM = new VillaNumberDeleteVM();

            var responce = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (responce != null && responce.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(responce.Result));
                villaNumberVM.VillaNumber = model;
            }

            responce = await _VillaService.GetAllAsync<APIResponse>();
            if (responce != null && responce.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(responce.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(villaNumberVM);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {

            var responce = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo);
            if (responce != null && responce.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            return View(model);
        }
    }
}
