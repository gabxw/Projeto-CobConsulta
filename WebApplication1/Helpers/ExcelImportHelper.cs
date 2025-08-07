using OfficeOpenXml;
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
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using var package = new ExcelPackage(excelStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    resultado.Erros.Add("A planilha está vazia ou não foi encontrada.");
                    return resultado;
                }

                int linha = 2;
                while (true)
                {
                    var nome = worksheet.Cells[linha, 1].Text?.Trim();
                    var telefone = worksheet.Cells[linha, 2].Text?.Trim();
                    var email = worksheet.Cells[linha, 3].Text?.Trim();
                    var cpf = worksheet.Cells[linha, 4].Text?.Trim();
                    var titulo = worksheet.Cells[linha, 5].Text?.Trim();
                    var descricao = worksheet.Cells[linha, 6].Text?.Trim();
                    var valorTexto = worksheet.Cells[linha, 7].Text?.Trim();
                    var status = worksheet.Cells[linha, 8].Text?.Trim();
                    var vencimentoTexto = worksheet.Cells[linha, 9].Text?.Trim();

                    // Detectar fim dos dados
                    if (string.IsNullOrWhiteSpace(nome) &&
                        string.IsNullOrWhiteSpace(cpf) &&
                        string.IsNullOrWhiteSpace(titulo))
                        break;

                    // Lista de erros dessa linha
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
                        Senha = "importado" // placeholder
                    };

                    var divida = new Divida
                    {
                        Titulo = titulo,
                        Descricao = descricao,
                        Valor = (int)Math.Round(valor),
                        Status = status,
                        DataCriacao = DateTime.Now,
                        DataVencimento = vencimento,
                        EmpresaID = empresaId,
                        Devedor = devedor
                    };

                    resultado.Dividas.Add(divida);
                    linha++;
                }
            }
            catch (Exception ex)
            {
                // Captura qualquer erro inesperado no processamento do Excel
                resultado.Erros.Add($"Erro inesperado ao ler o Excel: {ex.Message}");
            }

            return resultado;
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private static bool IsValidCpf(string cpf)
        {
            cpf = Regex.Replace(cpf ?? "", "[^0-9]", "");
            return cpf.Length == 11;
        }
    }
}