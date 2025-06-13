namespace CrudDashboard.Dto
{
    public class MovimentacoesConferidoDto
    {
        public int Id { get; set; }
        //public string NumeroConta { get; set; }
        public DateTime Data { get; set; }
        public string Historico { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public bool Conferido { get; set; } = false;
    }
}
