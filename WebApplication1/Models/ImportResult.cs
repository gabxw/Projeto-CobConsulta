// /Models/ImportResult.cs
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class ImportResult
    {
        public List<Divida> Dividas { get; set; } = new List<Divida>();
        public List<string> Erros { get; set; } = new List<string>();
    }
}
