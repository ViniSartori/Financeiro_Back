namespace CrudDashboard.Models
{
    public class Usuario
    {
        public string Nome { get; set; }

        public int Id { get; set; }

        public string Email { get; set; }

        public string CPF { get; set; }

        public string Situacao { get; set; } // 1- Ativo ; 0 - Inativo
    }
}
