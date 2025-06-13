namespace CrudDashboard.Dto
{
    public class MovimentacoesCadastrarDto
    {
       
        public string NumeroConta { get; set; }
        public DateTime Data { get; set; }
        public string Historico { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
