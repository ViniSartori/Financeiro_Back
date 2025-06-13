
namespace CrudDashboard.Dto
{
    public class UsuarioListarDto
    {
        public string Nome { get; set; } = string.Empty;

        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string CPF { get; set; }

        public string Situacao { get; set; } // 1- Ativo ; 0 - Inativo

        public static implicit operator UsuarioListarDto(List<UsuarioListarDto> v)
        {
            throw new NotImplementedException();
        }
    }
}
