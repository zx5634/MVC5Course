﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5Course.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index(string msg)
        {
            ViewBag.msg = msg + "');\r\n console.log('aa";
            return View();
        }
    }
}