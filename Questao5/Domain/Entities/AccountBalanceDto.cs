using System.Data;
using Dapper;
using MediatR;

public class GetBalanceQuery : IRequest<AccountBalanceDto>
{
    public Guid AccountId { get; set; }
}

public class AccountBalanceDto
{
    public int AccountNumber { get; set; }
    public string AccountHolderName { get; set; }
    public DateTime QueryDate { get; set; }
    public decimal Balance { get; set; }
}

public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, AccountBalanceDto>
{
    private readonly IDbConnection _dbConnection;

    public GetBalanceQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<AccountBalanceDto> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        // Verificar conta
        var account = await _dbConnection.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT numero, nome, ativo FROM contacorrente WHERE idcontacorrente = @AccountId",
            new { AccountId = request.AccountId });
        
        if (account == null)
            throw new BusinessException("INVALID_ACCOUNT", "Conta não encontrada.");
        
        if (account.ativo == 0)
            throw new BusinessException("INACTIVE_ACCOUNT", "Conta inativa.");

        // Calcular saldo
        var balance = await _dbConnection.QuerySingleAsync<decimal>(
            "SELECT IFNULL(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE -valor END), 0) " +
            "FROM movimento WHERE idcontacorrente = @AccountId",
            new { AccountId = request.AccountId });

        return new AccountBalanceDto
        {
            AccountNumber = account.numero,
            AccountHolderName = account.nome,
            QueryDate = DateTime.Now,
            Balance = balance
        };
    }
}
