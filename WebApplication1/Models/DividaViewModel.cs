using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Models.ViewModels
{
    public class DividaViewModel
    {
        public Divida Divida { get; set; }

        [Display(Name = "Devedor")]
        [Required(ErrorMessage = "Selecione um devedor")]
        public int DevedorId { get; set; }

        [Display(Name = "Empresa")]
        [Required(ErrorMessage = "Selecione uma empresa")]
        public int EmpresaId { get; set; }

        public List<SelectListItem> Devedores { get; set; }
        public List<SelectListItem> Empresas { get; set; }
    }
}
