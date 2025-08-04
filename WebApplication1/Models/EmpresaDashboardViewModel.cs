using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class EmpresaDashboardViewModel
    {
        // Básicos
        public int TotalDevedores { get; set; }
        public int ComPendencia { get; set; }
        public int SemPendencia { get; set; }

        public string NomeEmpresa { get; set; } = "";

        public List<string> NotificacoesRecentes { get; set; } = new();

        public int Pendentes { get; set; }
        public int Atrasadas { get; set; }
        public int Quitadas { get; set; }

        public decimal Faturamento { get; set; }
        public decimal ValorEmAberto { get; set; }

        public List<TopDevedorItem> TopDevedores { get; set; } = new();
        public Dictionary<string, decimal> Aging { get; set; } = new();

        public List<RecentDividaItem> Recentes { get; set; } = new();

        // Extras
        public decimal TaxaRecuperacaoPercent { get; set; } // quitado / (aberto + quitado)
        public decimal TempoMedioPagamentoDias { get; set; } // média das quitadas
        public decimal FaturamentoMesAtual { get; set; }
        public decimal FaturamentoMesAnterior { get; set; }
        public List<DividaAlertItem> Alertas { get; set; } = new(); // ex. vencidas >30 dias

        public class TopDevedorItem
        {
            public Devedor Devedor { get; set; } = null!;
            public decimal TotalDevido { get; set; }
        }

        public class RecentDividaItem
        {
            public string Titulo { get; set; } = "";
            public Devedor? Devedor { get; set; }
            public decimal Valor { get; set; }
            public string Status { get; set; } = "";
            public DateTime? DataVencimento { get; set; }
            public DateTime? DataPagamento { get; set; }
        }

        public class DividaAlertItem
        {
            public string Titulo { get; set; } = "";
            public string DevedorNome { get; set; } = "";
            public decimal Valor { get; set; }
            public int DiasAtraso { get; set; }
        }
    }
}
