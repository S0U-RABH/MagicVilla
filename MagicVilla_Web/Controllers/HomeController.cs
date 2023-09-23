using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MagicVilla_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVillaServicecs _VillaServicec;
        private readonly IMapper _mapper;

        public HomeController(IVillaServicecs villaServicec, IMapper mapper)
        {
            _VillaServicec = villaServicec;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<VillaDTO> list = new List<VillaDTO>();
            var responce = await _VillaServicec.GetAllAsync<APIResponse>();
            if (responce != null && responce.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responce.Result));
            }
            return View(list);
        }
    }
}