using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SimuladorCredito.DTO.Requests
{
    public class RequisicaoSimulacao : IValidatableObject
    {
        [SwaggerSchema(Nullable = true)]
        [Range(200.00, double.MaxValue, ErrorMessage = "O valor desejado deve ser maior que R$ 200,00.")]
        public decimal valorDesejado { get; set; }

        [SwaggerSchema(Nullable = true)]
        [Range(1, int.MaxValue, ErrorMessage = "O prazo deve ser maior que zero.")]
        public int prazo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
