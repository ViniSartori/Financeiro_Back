﻿using CrudDashboard.Dto;

namespace CrudDashboard.Models
{
    public class ResponseMovimentacoes<T>
    {
        public T? Dados { get; set; }

        public string Mensagem { get; set; } = string.Empty;

        public bool Status { get; set; } = true;
        public List<MovimentacoesDto> Data { get; internal set; }
        public string Message { get; internal set; }
        public bool Success { get; internal set; }
    }
}
