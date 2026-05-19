namespace SistemaAcademico.Domain.Models
{
    /// <summary>
    /// Estudiante de pregrado. Hereda de Estudiante.
    /// Relaciones de composición:
    ///   EstudiantePregrado <>--- DatosBachillerato
    /// Tipos de requisito de grado: Práctica, Pasantía Investigativa, Plan de Negocios.
    /// </summary>
    public class EstudiantePregrado : Estudiante
    {
        // ── Datos académicos ──────────────────────────────────────
        public int    SemestreActual    { get; private set; }
        public int    CreditosCursados  { get; private set; }
        public string ProgramaAcademico { get; private set; }

        // ── Composición: bachillerato ─────────────────────────────
        /// <summary>Puede ser null hasta que se registre la información de bachillerato.</summary>
        public DatosBachillerato? DatosBachillerato { get; private set; }

        // ── Requisito de grado ────────────────────────────────────
        /// <summary>
        /// Tipo de requisito: "Práctica", "Pasantía Investigativa" o "Plan de Negocios".
        /// </summary>
        public string TipoPractica    { get; private set; }
        public string MonitorPractica { get; private set; }
        public double NotaPractica    { get; private set; }

        // ── Constructor ───────────────────────────────────────────
        public EstudiantePregrado(
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
            int      semestreActual,
            int      creditosCursados,
            string   programaAcademico)
            : base(id, nombre, fechaNacimiento, email, telefono,
                   matricula, fechaIngreso, promedio, estado, nombreAcudiente)
        {
            SemestreActual    = semestreActual;
            CreditosCursados  = creditosCursados;
            ProgramaAcademico = programaAcademico;

            // Valores por defecto vacíos (se establecen después con métodos)
            TipoPractica    = string.Empty;
            MonitorPractica = string.Empty;
            NotaPractica    = 0.0;
        }

        // ── Métodos académicos ────────────────────────────────────

        public void AvanzarSemestre()
        {
            SemestreActual++;
        }

        public void AdicionarCreditos(int creditos)
        {
            if (creditos <= 0)
                throw new ArgumentException("Los créditos a adicionar deben ser positivos.");
            CreditosCursados += creditos;
        }

        // ── Bachillerato ──────────────────────────────────────────

        /// <summary>
        /// Establece los datos de bachillerato del estudiante.
        /// Composición: EstudiantePregrado <>--- DatosBachillerato.
        /// </summary>
        public void EstablecerDatosBachillerato(string colegio, int añoGraduacion, string tituloDiploma)
        {
            // Delega la validación al constructor del modelo
            DatosBachillerato = new DatosBachillerato(colegio, añoGraduacion, tituloDiploma);
        }

        // ── Requisito de Grado ────────────────────────────────────

        /// <summary>
        /// Establece el requisito de grado del pregrado.
        /// tipoPractica: "Práctica", "Pasantía Investigativa" o "Plan de Negocios".
        /// nota: entre 0.0 y 5.0.
        /// </summary>
        public void EstablecerRequisitosGrado(string tipoPractica, string monitor, double nota)
        {
            if (string.IsNullOrWhiteSpace(tipoPractica))
                throw new ArgumentException("El tipo de práctica es requerido.");
            if (string.IsNullOrWhiteSpace(monitor))
                throw new ArgumentException("El monitor es requerido.");
            if (nota < 0.0 || nota > 5.0)
                throw new ArgumentOutOfRangeException(nameof(nota), "La nota debe estar entre 0.0 y 5.0.");

            TipoPractica    = tipoPractica.Trim();
            MonitorPractica = monitor.Trim();
            NotaPractica    = nota;
        }

        /// <summary>
        /// Devuelve un texto formateado y legible con los requisitos de grado.
        /// Implementa el método abstracto de Estudiante.
        /// </summary>
        public override string ObtenerRequisitosDeGrado()
        {
            if (string.IsNullOrWhiteSpace(TipoPractica))
                return "⚠ Requisito de grado no configurado.";

            string estadoNota = NotaPractica >= 3.0 ? "✔ Aprobado" : "✘ No aprobado";

            return $"Tipo       : {TipoPractica}\n" +
                   $"Monitor    : {MonitorPractica}\n" +
                   $"Nota       : {NotaPractica:F1} — {estadoNota}";
        }
    }
}
