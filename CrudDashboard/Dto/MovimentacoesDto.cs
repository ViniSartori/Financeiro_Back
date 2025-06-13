namespace CrudDashboard.Dto
{
    public class MovimentacoesDto
    {
        public int Id { get; set; }
        public string NumeroConta { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Historico { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public bool Conferido { get; set; }
    }
}
