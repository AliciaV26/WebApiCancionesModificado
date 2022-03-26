using System.ComponentModel.DataAnnotations;
using WebApiCanciones.Validaciones;

namespace WebApiCanciones.DTOs
{
    public class CancionDTO //Se utiliza para guardar nuevo registro
    {
        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 150, ErrorMessage = "El campo {0} solo puede tener hasta 150 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
    }
}
