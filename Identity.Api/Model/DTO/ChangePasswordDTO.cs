using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Model.DTO
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=(?:.*\\d){1})(?=(?:.*[A-Z]){1})(?=(?:.*[a-z]){1})(?=(?:.*[@$><#.,?¡\\-_]){1})\\S{8,16}$",
            ErrorMessage = "La contraseña debe tener entre 8-16 caracteres, incluir mayúsculas, minúsculas, números y caracteres especiales")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmNewPassword { get; set; }
    }
}