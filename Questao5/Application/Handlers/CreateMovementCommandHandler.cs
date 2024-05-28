using System.Data;
using System.Runtime.Serialization;
using Dapper;
using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands;

public class CreateMovementCommandHandler : IRequestHandler<CreateMovementCommand, Guid>
{
    private readonly IDbConnection _dbConnection;

    public CreateMovementCommandHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<Guid> Handle(Questao5.Application.Commands.CreateMovementCommand request, CancellationToken cancellationToken)
    {
        // Verificar idempotência
        var existing = await _dbConnection.QuerySingleOrDefaultAsync<string>(
            "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @Key",
            new { Key = request.IdempotencyKey });
        
        if (existing != null)
        {
            return new Guid(existing);
        }

        // Verificar conta
        var account = await _dbConnection.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT ativo FROM contacorrente WHERE idcontacorrente = @AccountId",
            new { AccountId = request.AccountId });
        
        if (account == null)
            throw new BusinessException("INVALID_ACCOUNT", "Conta não encontrada.");
        
        if (account.ativo == 0)
            throw new BusinessException("INACTIVE_ACCOUNT", "Conta inativa.");

        if (request.Amount <= 0)
            throw new BusinessException("INVALID_VALUE", "O valor deve ser positivo.");

        if (request.MovementType != "C" && request.MovementType != "D")
            throw new BusinessException("INVALID_TYPE", "Tipo de movimento inválido.");

        var movementId = Guid.NewGuid();

        // Registrar movimento
        await _dbConnection.ExecuteAsync(
            "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) " +
            "VALUES (@MovementId, @AccountId, @Date, @MovementType, @Amount)",
            new
            {
                MovementId = movementId,
                AccountId = request.AccountId,
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                MovementType = request.MovementType,
                Amount = request.Amount
            });

        // Registrar idempotência
        await _dbConnection.ExecuteAsync(
            "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) " +
            "VALUES (@Key, @Request, @Result)",
            new
            {
                Key = request.IdempotencyKey,
                Request = JsonConvert.SerializeObject(request),
                Result = movementId.ToString()
            });

        return movementId;
    }
}

[Serializable]
internal class BusinessException : Exception
{
    internal object Type;
    private string v1;
    private string v2;

    public BusinessException()
    {
    }

    public BusinessException(string? message) : base(message)
    {
    }

    public BusinessException(string v1, string v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }

    public BusinessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}