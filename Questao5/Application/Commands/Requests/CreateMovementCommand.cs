using MediatR;

namespace Questao5.Application.Commands
{
    public class CreateMovementCommand : IRequest<Guid>
    {
        public string IdempotencyKey { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string MovementType { get; set; }
    }
}
