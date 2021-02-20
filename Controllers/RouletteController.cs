using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRuleta.Models;
using WebApiRuleta.Repo;

namespace WebApiRuleta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        #region Instance
        private readonly IRepoRoulette Instance;
        private readonly IRPClient instacli;
        public RouletteController(IRepoRoulette Instancia, IRPClient Instacli)
        {
            this.Instance = Instancia;
            instacli = Instacli;
        }
        #endregion

        [HttpGet("ListRoulette")]
        public async Task<IActionResult> ListRoulette()
        {
            try
            {
                List<Roulette> roulettes = await Instance.ListRoulette();
                if (roulettes == null)
                {
                    return NotFound("No existe ninguna ruleta");
                }
                return Ok(roulettes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ListBet")]
        public async Task<IActionResult> ListBet()
        {
            try
            {
                List<Bet> bets = await Instance.ListBet();
                if (bets==null)
                {
                    return NotFound("No existe ninguna apuesta");
                }
                return Ok(bets);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ConsultRoulette {IdRoulette}")]
        public async Task<IActionResult> ConsultRoulette(int IdRoulette)
        {
            try
            {
                Roulette consult = await Instance.ConsultRoulette(IdRoulette);
                if (consult != null)
                {

                    return Ok(consult);
                }
                else
                {
                    return NotFound("La Ruleta " + IdRoulette + " no existe.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CreateRoulette {RouletteName}")]
        public async Task<IActionResult> CreateRoulette(string RouletteName)
        {
            try
            {
                int IdRoulette = await Instance.CreateNewRoulette(RouletteName);

                return CreatedAtAction(nameof(CreateRoulette), "La ruleta fue registrado bajo el Id: " + IdRoulette);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("OpenRoulette {IdRoulette}")]
        public async Task<IActionResult> OpenRoulette(int IdRoulette)
        {
            try
            {
                Roulette consult = await Instance.ConsultRoulette(IdRoulette);
                if (consult == null)
                {
                    return NotFound("La Ruleta " + IdRoulette + " no existe.");
                }
                if (consult.RouletteState)
                {
                    return BadRequest("La Ruleta " + IdRoulette + " ya fue activada.");
                }
                bool state = await Instance.OpenRoulette(IdRoulette);
                if (state)
                {
                    return Ok("La Ruleta " + IdRoulette + " fue activada.");
                }
                else
                {
                    return BadRequest("La Ruleta " + IdRoulette + " no puse ser activada.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("CreateNewBet")]
        public async Task<IActionResult> CreateNewBet(Bet newbet)
        {
            try
            {
                Client cli = await instacli.GetClient(newbet.IdClient);
                Roulette consult = await Instance.ConsultRoulette(newbet.IdRoulette);
                if (cli == null)
                {
                    return NotFound("El cliente " + newbet.IdClient + " no existe.");
                }
                if (cli.AmountAvailable == 0 || newbet.BetAmount > cli.AmountAvailable)
                {
                    return NotFound("El cliente " + newbet.IdClient + " no tiene el dinero suficiente para apostar.");
                }
                if (consult == null)
                {
                    return NotFound("El la Ruleta " + newbet.IdRoulette + " no existe.");
                }
                if (!consult.RouletteState)
                {
                    return BadRequest("La Ruleta " + newbet.IdRoulette + " no ha sido activada.");
                }
                if (newbet.BetNumber > 36 || newbet.BetNumber < 1)
                {
                    return BadRequest("El numero al que apuestas no es valido");
                }
                if (newbet.BetAmount > 10000 || newbet.BetAmount < 1)
                {
                    return BadRequest("el rango de la apuesta va desde 1 hasta 10000.");
                }
                int IdBet = await Instance.CreateNewBet(newbet);

                return CreatedAtAction(nameof(CreateNewBet), "La apuesta fue registrado bajo el Id: " + IdBet);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("CloseBet {IdRoulette}")]
        public async Task<IActionResult> CloseBet(int IdRoulette)
        {
            try
            {
                Random rnd = new Random();
                Roulette Roulette = await Instance.ConsultRoulette(IdRoulette);
                List<Bet> Bets = await Instance.ListBet();
                List<Bet> Winners = new List<Bet>();
                Roulette.WiningBets = new List<Bet>();
                int win = 0;
                string colorwing = "Black";
                var Win = rnd.Next(1, 36);
                if ((win % 2) == 0)
                {
                    colorwing = "Red";
                }
                if (Roulette == null)
                {
                    return NotFound("La Ruleta " + IdRoulette + " no existe.");
                }
                if (!Roulette.RouletteState)
                {
                    return BadRequest("La Ruleta " + IdRoulette + " no ha sido activada.");
                }
                Roulette.NumWinner = Win;
                foreach (var bet in Bets)
                {
                    if (bet.BetNumber == Win)
                    {
                        bet.WinNumber = Win;
                        decimal AmountWinner = bet.BetAmount * 5m;
                        await instacli.AddPrize(bet.IdClient, AmountWinner);
                        Roulette.WiningBets.Add(bet);
                        Winners.Add(bet);
                    }
                    else if (bet.BetColor == colorwing)
                    {
                        bet.WinNumber = Win;
                        decimal AmountWinner = bet.BetAmount * 1.8m;
                        await instacli.AddPrize(bet.IdClient, AmountWinner);
                        Roulette.WiningBets.Add(bet);
                        Winners.Add(bet);
                    }
                }
                if (Winners == null)
                {
                    return NotFound("No Hubo Ganador...");
                }
                bool state = await Instance.CloseRoulette(Roulette);
                if (state)
                {
                    return Ok(Winners);
                }
                else
                {
                    return BadRequest("La Ruleta " + IdRoulette + " no puse ser cerrada.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
