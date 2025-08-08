// /Helpers/ExcelImportHelper.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
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

                int linha = 2; // assume cabeçalho na linha 1
                while (true)
                {
                    var nome = worksheet.Cell(linha, 1).GetString()?.Trim();
                    var telefone = worksheet.Cell(linha, 2).GetString()?.Trim();
                    var email = worksheet.Cell(linha, 3).GetString()?.Trim();
                    var cpf = worksheet.Cell(linha, 4).GetString()?.Trim();
                    var titulo = worksheet.Cell(linha, 5).GetString()?.Trim();
                    var descricao = worksheet.Cell(linha, 6).GetString()?.Trim();
                    var valorTexto = worksheet.Cell(linha, 7).GetString()?.Trim();
                    var status = worksheet.Cell(linha, 8).GetString()?.Trim();
                    var vencimentoTexto = worksheet.Cell(linha, 9).GetString()?.Trim();

                    // fim de dados
                    if (string.IsNullOrWhiteSpace(nome)
                        && string.IsNullOrWhiteSpace(cpf)
                        && string.IsNullOrWhiteSpace(titulo))
                    {
                        break;
                    }

                    var errosLinha = new List<string>();

                    if (string.IsNullOrWhiteSpace(nome)) errosLinha.Add("Nome é obrigatório.");
                    if (string.IsNullOrWhiteSpace(telefone)) errosLinha.Add("Telefone é obrigatório.");
                    if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email)) errosLinha.Add("Email inválido.");
                    if (string.IsNullOrWhiteSpace(cpf) || !IsValidCpf(cpf)) errosLinha.Add("CPF inválido.");
                    if (string.IsNullOrWhiteSpace(titulo)) errosLinha.Add("Título é obrigatório.");
                    if (string.IsNullOrWhiteSpace(descricao)) errosLinha.Add("Descrição é obrigatória.");

                    // tenta parse do valor (pt-BR e invariant)
                    if (!decimal.TryParse(valorTexto, NumberStyles.Any, new CultureInfo("pt-BR"), out var valor) &&
                        !decimal.TryParse(valorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                    {
                        errosLinha.Add("Valor inválido.");
                    }

                    DateTime? vencimento = null;
                    if (!string.IsNullOrWhiteSpace(vencimentoTexto))
                    {
                        if (DateTime.TryParse(vencimentoTexto, new CultureInfo("pt-BR"), DateTimeStyles.None, out var dt1))
                            vencimento = dt1;
                        else if (DateTime.TryParse(vencimentoTexto, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt2))
                            vencimento = dt2;
                        else
                            errosLinha.Add("Data de vencimento inválida.");
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
                resultado.Erros.Add($"Erro inesperado ao ler o Excel: {ex.Message}");
            }

            return resultado;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidCpf(string cpf)
        {
            // validação simples (pode ser incrementada). Aqui, apenas checamos presença de dígitos.
            if (string.IsNullOrWhiteSpace(cpf)) return false;
            var apenasDigitos = new string(cpf.Where(char.IsDigit).ToArray());
            return apenasDigitos.Length >= 8; // ajuste conforme sua regra
        }
    }
}
