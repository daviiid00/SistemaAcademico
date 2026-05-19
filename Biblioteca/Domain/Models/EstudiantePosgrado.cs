namespace SistemaAcademico.Domain.Models
{
    /// <summary>
    /// Estudiante de posgrado. Hereda de Estudiante.
    /// Relaciones de composición:
    ///   EstudiantePosgrado <>--- EstudioPrevio  (lista de estudios previos)
    ///   EstudiantePosgrado <>--- Tesis           (requisito obligatorio de grado)
    /// Nota cualitativa de tesis válida: "Aprobó", "No aprobó", "Laureada".
    /// </summary>
    public class EstudiantePosgrado : Estudiante
    {
        // ── Datos académicos ──────────────────────────────────────
        public string Maestria             { get; private set; }
        public int    SemestresCompletados { get; private set; }
        public bool   TesisPresentada      { get; private set; }

        // ── Datos de la tesis ─────────────────────────────────────
        public string    TituloTesis       { get; private set; }
        public string    DirectorTesis     { get; private set; }
        public string    Evaluador1        { get; private set; }
        public string    Evaluador2        { get; private set; }
        public string    NotaCualitativa   { get; private set; }
        public DateTime? FechaSustentacion { get; private set; }

        // ── Composición: estudios previos ─────────────────────────
        /// <summary>
        /// Lista de estudios previos (pregrado, especializaciones, etc.).
        /// Relación: EstudiantePosgrado <>--- EstudioPrevio.
        /// </summary>
        public List<EstudioPrevio> EstudiosPrevios { get; private set; }

        /// <summary>Valores permitidos para la nota cualitativa de tesis.</summary>
        public static readonly IReadOnlyList<string> NotasCualitativasValidas =
            new List<string> { "Aprobó", "No aprobó", "Laureada" }.AsReadOnly();

        // ── Constructor ───────────────────────────────────────────
        public EstudiantePosgrado(
            string   id,
            string   nombre,
            DateTime fechaNacimiento,
            string   email,
            string   telefono,
            string   matricula,
            DateTime fechaIngreso,
            double   promedio,
            string   estado,
            string   nombreAcudiente,
            string   maestria,
            int      semestresCompletados,
            bool     tesisPresentada)
            : base(id, nombre, fechaNacimiento, email, telefono,
                   matricula, fechaIngreso, promedio, estado, nombreAcudiente)
        {
            Maestria             = maestria;
            SemestresCompletados = semestresCompletados;
            TesisPresentada      = tesisPresentada;
            EstudiosPrevios      = new List<EstudioPrevio>();

            // Valores vacíos por defecto
            TituloTesis     = string.Empty;
            DirectorTesis   = string.Empty;
            Evaluador1      = string.Empty;
            Evaluador2      = string.Empty;
            NotaCualitativa = string.Empty;
        }

        // ── Métodos académicos ────────────────────────────────────

        public void MarcarTesisPresentada()
        {
            TesisPresentada = true;
        }

        public void CompletarSemestre()
        {
            SemestresCompletados++;
        }

        // ── Estudios Previos ──────────────────────────────────────

        /// <summary>
        /// Agrega un estudio previo a la lista del estudiante de posgrado.
        /// Composición: EstudiantePosgrado <>--- EstudioPrevio.
        /// </summary>
        public void AgregarEstudioPrevio(string institucion, string tituloObtenido, int añoGraduacion)
        {
            // Delega la validación al constructor del modelo
            var estudio = new EstudioPrevio(institucion, tituloObtenido, añoGraduacion);
            EstudiosPrevios.Add(estudio);
        }

        // ── Tesis ─────────────────────────────────────────────────

        /// <summary>
        /// Registra la tesis de grado del estudiante de posgrado.
        /// nota: debe ser "Aprobó", "No aprobó" o "Laureada".
        /// </summary>
        public void RegistrarTesis(
            string   titulo,
            string   director,
            string   eval1,
            string   eval2,
            string   nota,
            DateTime fecha)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título de la tesis es requerido.");
            if (string.IsNullOrWhiteSpace(director))
                throw new ArgumentException("El director de tesis es requerido.");
            if (string.IsNullOrWhiteSpace(eval1) || string.IsNullOrWhiteSpace(eval2))
                throw new ArgumentException("Los dos evaluadores son requeridos.");
            if (!NotasCualitativasValidas.Contains(nota))
                throw new ArgumentException(
                    $"La nota cualitativa debe ser: {string.Join(", ", NotasCualitativasValidas)}.");

            TituloTesis     = titulo.Trim();
            DirectorTesis   = director.Trim();
            Evaluador1      = eval1.Trim();
            Evaluador2      = eval2.Trim();
            NotaCualitativa = nota.Trim();
            FechaSustentacion = fecha;

            MarcarTesisPresentada();
        }

        // ── Requisito de Grado ────────────────────────────────────

        /// <summary>
        /// Devuelve texto formateado y legible con los datos de la tesis.
        /// Implementa el método abstracto de Estudiante.
        /// </summary>
        public override string ObtenerRequisitosDeGrado()
        {
            if (!TesisPresentada || string.IsNullOrWhiteSpace(TituloTesis))
                return "⚠ Tesis no registrada — requisito de grado pendiente.";

            string iconoNota = NotaCualitativa switch
            {
                "Aprobó"    => "✔",
                "Laureada"  => "🏆",
                "No aprobó" => "✘",
                _           => "?"
            };

            return $"Título       : {TituloTesis}\n" +
                   $"Director     : {DirectorTesis}\n" +
                   $"Evaluador 1  : {Evaluador1}\n" +
                   $"Evaluador 2  : {Evaluador2}\n" +
                   $"Nota         : {iconoNota} {NotaCualitativa}\n" +
                   $"Sustentación : {FechaSustentacion:dd/MM/yyyy}";
        }
    }
}
