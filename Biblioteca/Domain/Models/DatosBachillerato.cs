namespace SistemaAcademico.Domain.Models
{
    /// <summary>
    /// Representa la información de bachillerato de un EstudiantePregrado.
    /// Relación: EstudiantePregrado <>--- DatosBachillerato (composición).
    /// </summary>
    public class DatosBachillerato
    {
        public string Colegio       { get; private set; }
        public int    AñoGraduacion { get; private set; }
        public string TituloDiploma { get; private set; }

        public DatosBachillerato(string colegio, int añoGraduacion, string tituloDiploma)
        {
            if (string.IsNullOrWhiteSpace(colegio))
                throw new ArgumentException("El colegio es requerido.", nameof(colegio));

            if (añoGraduacion < 1950 || añoGraduacion > DateTime.Now.Year)
                throw new ArgumentOutOfRangeException(nameof(añoGraduacion),
                    $"El año de graduación debe estar entre 1950 y {DateTime.Now.Year}.");

            if (string.IsNullOrWhiteSpace(tituloDiploma))
                throw new ArgumentException("El título del diploma es requerido.", nameof(tituloDiploma));

            Colegio       = colegio.Trim();
            AñoGraduacion = añoGraduacion;
            TituloDiploma = tituloDiploma.Trim();
        }

        public override string ToString()
            => $"Colegio: {Colegio} | Año: {AñoGraduacion} | Título: {TituloDiploma}";
    }
}
