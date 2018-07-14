﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5Course.Controllers
{
    public class MBController : BaseController
    {
        public ActionResult Index()
        {
            var data = "Hello World";
            ViewData.Model = data;
            return View();
        }

        public ActionResult ViewBagDemo()
        {
            ViewBag.text = "Hi";
            return View();
        }

        public ActionResult ViewDatademo()
        {
            ViewData["text"] = "Hi";
            return View("ViewBagDemo");
        }
    }
}