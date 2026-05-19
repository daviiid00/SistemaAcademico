namespace SistemaAcademico.Domain.Models
{
    /// <summary>
    /// Representa un estudio previo (pregrado/especialización) de un EstudiantePosgrado.
    /// Relación: EstudiantePosgrado <>--- EstudioPrevio (composición, lista).
    /// </summary>
    public class EstudioPrevio
    {
        public string Institucion    { get; private set; }
        public string TituloObtenido { get; private set; }
        public int    AñoGraduacion  { get; private set; }

        public EstudioPrevio(string institucion, string tituloObtenido, int añoGraduacion)
        {
            if (string.IsNullOrWhiteSpace(institucion))
                throw new ArgumentException("La institución es requerida.", nameof(institucion));

            if (string.IsNullOrWhiteSpace(tituloObtenido))
                throw new ArgumentException("El título obtenido es requerido.", nameof(tituloObtenido));

            if (añoGraduacion < 1950 || añoGraduacion > DateTime.Now.Year)
                throw new ArgumentOutOfRangeException(nameof(añoGraduacion),
                    $"El año de graduación debe estar entre 1950 y {DateTime.Now.Year}.");

            Institucion    = institucion.Trim();
            TituloObtenido = tituloObtenido.Trim();
            AñoGraduacion  = añoGraduacion;
        }

        public override string ToString()
            => $"Institución: {Institucion} | Título: {TituloObtenido} | Año: {AñoGraduacion}";
    }
}
