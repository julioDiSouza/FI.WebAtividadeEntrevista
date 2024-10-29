using System.ComponentModel.DataAnnotations;

namespace FI.WebAtividadeEntrevista.Models
{
    public class BeneficiarioModel
    {
        public long Id { get; set; }

        /// <summary>
        /// CPF
        /// </summary>
        [Required]
        public string CPF { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        public string Nome { get; set; }

        public long IdCliente { get; set; }
    }
}
