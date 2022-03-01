using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiRuleta.Models;
using WebApiRuleta.Repo;

namespace WebApiRuleta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Instance
        private readonly IRPClient instacli;
        public ClientController(IRPClient Instacli)
        {
            this.instacli = Instacli;
        }
        #endregion

        [HttpGet("ListClient")]
        public async Task<IActionResult> ListClient()
        {
            try
            {
                return Ok(await instacli.ListClients());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cliRet = await instacli.GetClient(id);
            try
            {
                if (cliRet == null)
                {
                    return NotFound("El cliente " + id.ToString() + " no existe.");
                }

                return Ok(cliRet);
            }
            catch (Exception ex)
            {
                //500
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                //400
                //return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddClient")]
        public async Task<IActionResult> AddClient(Client NewClient)
        {
            try
            {
                await instacli.AddClient(NewClient);
                return CreatedAtAction(nameof(AddClient), NewClient);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
