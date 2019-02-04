﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Api.Token;
using Entidades.Entidades.Gerencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Persistencia;
using Persistencia.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        [Authorize("Bearer")]
        [HttpPost("empresa/{empresa}")]
        public IActionResult DefinirEmpresa([FromServices]IHttpContextAccessor httpContextAccessor, string empresa)
        {
            if (!string.IsNullOrEmpty(empresa))
            {
                ConnectionString.Database = empresa;
                string email = httpContextAccessor.HttpContext.User.Identity.Name;
                Session.AdicionarSessaoUsuario(email, empresa);
                return Ok();
            }
            return BadRequest("nome de empresa inválido");
        }

        [HttpPost]
        [AllowAnonymous]
        public object Post(
            [FromBody]Usuario usuario,
            [FromServices] IAutenticacaoService autenticacaoService,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            bool credenciaisValidas = false;
            Usuario usuarioBase = null;
            if (usuario != null && !String.IsNullOrWhiteSpace(usuario.Email))
            {
                usuarioBase = autenticacaoService.Entity().Where(user => user.Email == usuario.Email).Single();
                credenciaisValidas = (usuarioBase != null &&
                    usuario.Email == usuarioBase.Email &&
                    usuario.Senha == usuarioBase.Senha);
            }

            if (credenciaisValidas)
            {
                if (Session.IsUsuarioLogado(usuario.Email))
                {
                    return new
                    {
                        authenticated = false,
                        message = "Usuário logado"
                    };
                }

                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(usuario.Email, "Email"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });
                var token = handler.WriteToken(securityToken);

                return new
                {
                    authenticated = true,
                    created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    message = "OK",
                    IdUsuario = usuarioBase.Id
                };
            }
            else
            {
                return new
                {
                    authenticated = false,
                    message = "Email ou senha inválidos"
                };
            }
        }
    }
}
