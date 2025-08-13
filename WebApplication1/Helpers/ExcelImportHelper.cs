using MiniExcelLibs;
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
                var rows = MiniExcel.Query(excelStream, useHeaderRow: true).ToList();
                int linha = 2;
                foreach (var row in rows)
                {
                    var dict = (IDictionary<string, object>)row;
                    string nome = dict.ContainsKey("Nome") ? dict["Nome"]?.ToString()?.Trim() ?? "" : "";
                    string telefone = dict.ContainsKey("Telefone") ? dict["Telefone"]?.ToString()?.Trim() ?? "" : "";
                    string email = dict.ContainsKey("Email") ? dict["Email"]?.ToString()?.Trim() ?? "" : "";
                    string cpf = dict.ContainsKey("CPF") ? dict["CPF"]?.ToString()?.Trim() ?? "" : "";
                    string titulo = dict.ContainsKey("Titulo") ? dict["Titulo"]?.ToString()?.Trim() ?? "" : "";
                    string descricao = dict.ContainsKey("Descricao") ? dict["Descricao"]?.ToString()?.Trim() ?? "" : "";
                    string valorTexto = dict.ContainsKey("Valor") ? dict["Valor"]?.ToString()?.Trim() ?? "" : "";
                    string status = dict.ContainsKey("Status") ? dict["Status"]?.ToString()?.Trim() ?? "" : "";
                    string vencimentoTexto = dict.ContainsKey("DataVencimento") ? dict["DataVencimento"]?.ToString()?.Trim() ?? "" : "";
                    string pagamentoTexto = dict.ContainsKey("DataPagamento") ? dict["DataPagamento"]?.ToString()?.Trim() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(nome) && string.IsNullOrWhiteSpace(cpf) && string.IsNullOrWhiteSpace(titulo))
                        break;

                    List<string> errosLinha = new();
                    if (string.IsNullOrWhiteSpace(nome)) errosLinha.Add("Nome é obrigatório.");
                    if (string.IsNullOrWhiteSpace(telefone)) errosLinha.Add("Telefone é obrigatório.");
                    if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email)) errosLinha.Add("Email inválido.");
                    if (string.IsNullOrWhiteSpace(cpf) || !IsValidCpf(cpf)) errosLinha.Add("CPF inválido.");
                    if (string.IsNullOrWhiteSpace(titulo)) errosLinha.Add("Título é obrigatório.");
                    if (string.IsNullOrWhiteSpace(descricao)) errosLinha.Add("Descrição é obrigatória.");
                    if (!decimal.TryParse(valorTexto.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
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
                resultado.Erros.Add($"Erro ao ler o Excel: {ex.Message}");
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
