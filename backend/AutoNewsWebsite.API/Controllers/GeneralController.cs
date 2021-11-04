﻿using System;
using System.Collections.Generic;
using AutoNewsWebsite.DAL;
using Microsoft.AspNetCore.Mvc;

namespace MedicalVideoMultitier.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneralController<T> : Controller where T : new()
    {
        [HttpGet]
        public List<T> GetAll() => Handler.GetAll<T>();

        [HttpGet("{id}")]
        public T Get(Guid id) => Handler.GetFromIndex<T>(id);

        [HttpPost]
        public void Create([FromBody] T item) => item.Insert();


        [HttpDelete("{id}")]
        public void Delete(Guid id) => Handler.Delete<T>(id);

        [HttpPut("{id}")]
        public void Update(Guid id, [FromBody] T item) => Handler.Update(id, item);
    }
}