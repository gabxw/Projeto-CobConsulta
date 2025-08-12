using OfficeOpenXml;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class ImportResult
    {
        public List<Divida> Dividas { get; set; } = new();
        public List<string> Erros { get; set; } = new();
    }

    public static class ExcelImportHelper
    {
        public static ImportResult ProcessarExcel(Stream excelStream, int empresaId)
        {
            var resultado = new ImportResult();

            try
            {
                using var workbook = new XLWorkbook(excelStream);
                var worksheet = workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    resultado.Erros.Add("A planilha está vazia ou não foi encontrada.");
                    return resultado;
                }

                int linha = 2;
                while (true)
                {
                    var nome = worksheet.Cell(linha, 1).GetString().Trim();
                    var telefone = worksheet.Cell(linha, 2).GetString().Trim();
                    var email = worksheet.Cell(linha, 3).GetString().Trim();
                    var cpf = worksheet.Cell(linha, 4).GetString().Trim();
                    var titulo = worksheet.Cell(linha, 5).GetString().Trim();
                    var descricao = worksheet.Cell(linha, 6).GetString().Trim();
                    var valorTexto = worksheet.Cell(linha, 7).GetString().Trim();
                    var status = worksheet.Cell(linha, 8).GetString().Trim();
                    var vencimentoTexto = worksheet.Cell(linha, 9).GetString().Trim();
                    var pagamentoTexto = worksheet.Cell(linha, 10).GetString().Trim();

                    // Se linha estiver completamente vazia → fim dos dados
                    if (string.IsNullOrWhiteSpace(nome) &&
                        string.IsNullOrWhiteSpace(cpf) &&
                        string.IsNullOrWhiteSpace(titulo))
                        break;

                    // Validação dos campos
                    List<string> errosLinha = new();

                    if (string.IsNullOrWhiteSpace(nome)) errosLinha.Add("Nome é obrigatório.");
                    if (string.IsNullOrWhiteSpace(telefone)) errosLinha.Add("Telefone é obrigatório.");
                    if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email)) errosLinha.Add("Email inválido.");
                    if (string.IsNullOrWhiteSpace(cpf) || !IsValidCpf(cpf)) errosLinha.Add("CPF inválido.");
                    if (string.IsNullOrWhiteSpace(titulo)) errosLinha.Add("Título é obrigatório.");
                    if (string.IsNullOrWhiteSpace(descricao)) errosLinha.Add("Descrição é obrigatória.");
                    if (!decimal.TryParse(valorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
                        errosLinha.Add("Valor inválido.");
                    if (!DateTime.TryParse(vencimentoTexto, out var vencimento))
                        errosLinha.Add("Data de vencimento inválida.");
                    DateTime? dataPagamento = null;
                    if (!string.IsNullOrWhiteSpace(pagamentoTexto))
                    {
                        if (DateTime.TryParse(pagamentoTexto, out var pagamento))
                            dataPagamento = pagamento;
                        else
                            errosLinha.Add("Data de pagamento inválida.");
                    }
                    if (string.IsNullOrWhiteSpace(status)) status = "Pendente";

                    if (errosLinha.Any())
                    {
                        resultado.Erros.Add($"Linha {linha}: {string.Join(" ", errosLinha)}");
                        linha++;
                        continue;
                    }

                    // Criar objeto devedor e dívida
                    var devedor = new Devedor
                    {
                        Name = nome,
                        Email = email,
                        Cpf = cpf,
                        Telefone = telefone,
                        Senha = "importado"
                    };

                    var divida = new Divida
                    {
                        Titulo = titulo,
                        Descricao = descricao,
                        Valor = (int)Math.Round(valor),
                        Status = status,
                        DataCriacao = DateTime.Now,
                        DataVencimento = vencimento,
                        DataPagamento = dataPagamento,
                        EmpresaID = empresaId,
                        Devedor = devedor
                    };

                    resultado.Dividas.Add(divida);
                    linha++;
                }
            }
            catch (Exception ex)
            {
                resultado.Erros.Add($"Erro inesperado ao ler o Excel: {ex.Message}");
            }

            return resultado;
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private static bool IsValidCpf(string cpf)
        {
            cpf = Regex.Replace(cpf ?? "", "[^0-9]", "");
            return cpf.Length == 11;
        }
    }
}
