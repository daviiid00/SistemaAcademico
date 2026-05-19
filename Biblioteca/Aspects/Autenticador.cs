namespace SistemaAcademico.Aspects
{
    public static class Autenticador
    {
        private static readonly Dictionary<string, string> UsuariosAutorizados = new()
        {
            { "admin", "admin123" },
            { "director", "director123" },
            { "profesor", "profesor123" }
        };

        public static (bool acceso, string mensaje) VerificiarAcceso(string usuario, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
                    return (false, "Usuario y contraseña son requeridos");

                if (!UsuariosAutorizados.ContainsKey(usuario))
                    return (false, "Usuario no autorizado");

                if (UsuariosAutorizados[usuario] != contrasena)
                    return (false, "Contraseña incorrecta");

                return (true, "Acceso concedido");
            }
            catch
            {
                return (false, "Error al verificar acceso");
            }
        }

        public static bool EstaAutenticado(string usuario, string contrasena)
        {
            var (acceso, _) = VerificiarAcceso(usuario, contrasena);
            return acceso;
        }
    }
}
