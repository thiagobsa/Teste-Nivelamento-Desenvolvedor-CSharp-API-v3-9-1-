using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using Questao5.Application.Commands;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("movimentacao")]
        public async Task<IActionResult> CreateMovement([FromBody] CreateMovementCommand command)
        {
            try
            {
                var movementId = await _mediator.Send(command);
                return Ok(new { MovementId = movementId });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { Message = ex.Message, Type = ex.Type });
            }
        }

        [HttpGet("saldo/{accountId}")]
        public async Task<IActionResult> GetBalance(Guid accountId)
        {
            try
            {
                var query = new GetBalanceQuery { AccountId = accountId };
                var balance = await _mediator.Send(query);
                return Ok(balance);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { Message = ex.Message, Type = ex.Type });
            }
        }
    }
}
