namespace CrudDashboard.Models
{
    public class Conta
    {
        public int Id { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string NumeroConta { get; set; }
        public bool Ativo { get; set; } = true; // Por padrão, as contas são ativas
    }
}
