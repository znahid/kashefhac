using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HearingAidWebSite.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class HomeAdminPanelController : Controller
    {
        public IActionResult MENUE()
        {
            return View();
        }
    }
}