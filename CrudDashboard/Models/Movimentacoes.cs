namespace CrudDashboard.Models
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public string NumeroConta { get; set; }
        public DateTime Data { get; set; }
        public string Historico { get; set; }
        public decimal Valor { get; set; }
        public bool Conferido { get; set; }
    }
}

