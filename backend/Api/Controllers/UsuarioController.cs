﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entidades.Entidades.Gerencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistencia.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Authorize("Bearer")]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private IUsuarioService usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            this.usuarioService = usuarioService;
        }

        // GET: api/<controller>
        [HttpGet]
        public string Get()
        {
            return "BATATAAAAAAAAA";
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Usuario> Get(long id, [FromHeader]object empresa)
        {
            return await this.usuarioService.BuscarAsync(id);
        }

        [HttpGet("{id}/empresa")]
        public IActionResult GetEmpresasUsuario(long id)
        {
            List<Empresa> empresas = this.usuarioService.BuscarEmpresas(id);
            return Ok(empresas);
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]Usuario usuario)
        {
            try
            {
                this.usuarioService.Inserir(usuario);
                return Ok("Criado com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<controller>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Usuario usuario)
        {
            try
            {
                await this.usuarioService.AtualizarAsync(usuario);
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await this.usuarioService.DeletarAsync(id);
            return Ok();
        }
    }
}
