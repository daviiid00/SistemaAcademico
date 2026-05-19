namespace SistemaAcademico.Proxy
{
    using Aspects;
    using Interfaces;
    using System.Reflection;

    public class UniversidadProxy<T> : DispatchProxy where T : class
    {
        private T _target;
        private string _usuario;
        private string _contrasena;

        public static T Create(T target, string usuario, string contrasena)
        {
            try
            {
                object proxy = Create<T, UniversidadProxy<T>>();
                ((UniversidadProxy<T>)(object)proxy).SetTarget(target, usuario, contrasena);
                return (T)proxy;
            }
            catch
            {
                return null;
            }
        }

        private void SetTarget(T target, string usuario, string contrasena)
        {
            _target = target;
            _usuario = usuario;
            _contrasena = contrasena;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                var (acceso, _) = Autenticador.VerificiarAcceso(_usuario, _contrasena);
                if (!acceso)
                {
                    throw new UnauthorizedAccessException($"Acceso denegado para usuario {_usuario}");
                }

                return targetMethod.Invoke(_target, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
