using System;
using System.Globalization;

namespace Questao5
{
    class ContaBancaria
    {
        public int NumeroConta { get; private set; }
        public string NomeTitular { get; set; }
        private double _saldo;

        public ContaBancaria(int numeroConta, string nomeTitular)
        {
            NumeroConta = numeroConta;
            NomeTitular = nomeTitular;
            _saldo = 0.0;
        }

        public ContaBancaria(int numeroConta, string nomeTitular, double depositoInicial) : this(numeroConta, nomeTitular)
        {
            Depositar(depositoInicial);
        }

        public double Saldo
        {
            get { return _saldo; }
        }

        public void Depositar(double valor)
        {
            _saldo += valor;
        }

        public void Sacar(double valor)
        {
            const double taxa = 3.50;
            _saldo -= (valor + taxa);
        }

        public override string ToString()
        {
            return $"Conta {NumeroConta}, Titular: {NomeTitular}, Saldo: $ {_saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
