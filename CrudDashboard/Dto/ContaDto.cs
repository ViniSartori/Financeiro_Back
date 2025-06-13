namespace CrudDashboard.Dto
{
    public class ContaDto
    {
        public int Id { get; set; }
        public string Banco { get; set; } = string.Empty;
        public string Agencia { get; set; } = string.Empty;
        public string NumeroConta { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
