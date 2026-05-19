namespace SistemaAcademico.Aspects
{
    using System.Text.RegularExpressions;
    using Domain.Models;

    /// <summary>
    /// Validaciones transversales del dominio académico (aspecto de validación).
    /// Aplicar ANTES de persistir cualquier entidad en un servicio.
    /// </summary>
    public static class ValidadorAcademico
    {
        // ── Constantes de dominio ─────────────────────────────────
        public static readonly double NOTA_MINIMA_APROBATORIA = 3.0;
        public static readonly double NOTA_ESCALA_MINIMA      = 0.0;   // Bug corregido: 0 es la nota mínima válida
        public static readonly double NOTA_ESCALA_MAXIMA      = 5.0;
        public static readonly int    EDAD_MINIMA             = 16;

        // Regex: solo letras (incluye tildes y ñ) y espacios — sin números ni símbolos
        private static readonly Regex _regexNombre =
            new(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$", RegexOptions.Compiled);

        // Regex: una o más letras mayúsculas seguidas de uno o más dígitos — ej. MAT001, DOC1, POS99
        private static readonly Regex _regexFormatoId =
            new(@"^[A-Z]+\d+$", RegexOptions.Compiled);

        // ── Validaciones de formato (sin instancia de dominio) ────

        /// <summary>
        /// Valida que un nombre solo contenga letras y espacios (sin números ni caracteres especiales).
        /// Ej. válido: "Juan Pérez", "María de los Ángeles"
        /// Ej. inválido: "Juan123", "Ana_García"
        /// </summary>
        public static (bool valido, string mensaje) ValidarNombreSoloLetras(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return (false, "El nombre no puede estar vacío.");

            if (!_regexNombre.IsMatch(nombre.Trim()))
                return (false, "El nombre solo puede contener letras y espacios. No se permiten números ni caracteres especiales.");

            return (true, "Nombre válido.");
        }

        /// <summary>
        /// Valida que un ID o matrícula tenga el formato correcto: letras mayúsculas + números.
        /// Ej. válido: MAT001, DOC001, POS001, E001
        /// Ej. inválido: "001MAT", "mat001", "MAT", "001"
        /// </summary>
        public static (bool valido, string mensaje) ValidarFormatoId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return (false, "El ID no puede estar vacío.");

            string idLimpio = id.Trim();

            if (!_regexFormatoId.IsMatch(idLimpio))
                return (false, $"El ID '{idLimpio}' no tiene el formato correcto. Debe comenzar con letras mayúsculas seguidas de números. Ej: MAT001, DOC001, POS001.");

            return (true, "ID con formato válido.");
        }

        /// <summary>
        /// Valida que una nota esté en la escala académica permitida (0.0 a 5.0).
        /// La nota mínima aprobatoria es 3.0, pero cualquier valor entre 0 y 5 es válido para registrar.
        /// </summary>
        public static (bool valido, string mensaje) ValidarNota(double nota)
        {
            if (nota < NOTA_ESCALA_MINIMA || nota > NOTA_ESCALA_MAXIMA)
                return (false, $"La nota {nota:F1} no es válida. Debe estar entre {NOTA_ESCALA_MINIMA:F1} y {NOTA_ESCALA_MAXIMA:F1}.");

            return (true, "Nota válida.");
        }

        /// <summary>
        /// Valida que un año sea razonable para el contexto académico (1900 — año actual).
        /// </summary>
        public static (bool valido, string mensaje) ValidarAño(int año)
        {
            int añoActual = DateTime.Now.Year;
            if (año < 1900 || año > añoActual)
                return (false, $"El año {año} no es válido. Debe estar entre 1900 y {añoActual}.");

            return (true, "Año válido.");
        }

        // ── Validaciones de entidades de dominio ──────────────────

        /// <summary>
        /// Valida un estudiante antes de registrarlo en el sistema.
        /// </summary>
        public static (bool valido, string mensaje) ValidarEstudiante(Estudiante estudiante)
        {
            try
            {
                if (estudiante == null)
                    return (false, "Estudiante no puede ser nulo.");

                if (estudiante.ObtenerEdad() < EDAD_MINIMA)
                    return (false, $"El estudiante debe tener al menos {EDAD_MINIMA} años.");

                var nombreResult = ValidarNombreSoloLetras(estudiante.Nombre);
                if (!nombreResult.valido)
                    return (false, $"Nombre del estudiante inválido: {nombreResult.mensaje}");

                if (string.IsNullOrWhiteSpace(estudiante.Matricula))
                    return (false, "La matrícula del estudiante es requerida.");

                return (true, "Estudiante válido.");
            }
            catch
            {
                return (false, "Error al validar estudiante.");
            }
        }

        /// <summary>
        /// Valida una evaluación antes de registrarla.
        /// CORRECCIÓN: la nota válida es entre 0.0 y 5.0 (no 3.0 a 5.0).
        /// </summary>
        public static (bool valido, string mensaje) ValidarEvaluacion(Evaluacion evaluacion)
        {
            try
            {
                if (evaluacion == null)
                    return (false, "Evaluación no puede ser nula.");

                // CORRECCIÓN: rango válido es 0.0–5.0, no 3.0–5.0
                var notaResult = ValidarNota(evaluacion.NotaFinal);
                if (!notaResult.valido)
                    return (false, notaResult.mensaje);

                if (string.IsNullOrWhiteSpace(evaluacion.Estudiante?.Id))
                    return (false, "El ID del estudiante es requerido.");

                if (string.IsNullOrWhiteSpace(evaluacion.Asignatura?.Codigo))
                    return (false, "El código de asignatura es requerido.");

                return (true, "Evaluación válida.");
            }
            catch
            {
                return (false, "Error al validar evaluación.");
            }
        }

        /// <summary>
        /// Valida un docente antes de registrarlo en el sistema.
        /// </summary>
        public static (bool valido, string mensaje) ValidarDocente(Docente docente)
        {
            try
            {
                if (docente == null)
                    return (false, "Docente no puede ser nulo.");

                if (docente.ObtenerEdad() < EDAD_MINIMA)
                    return (false, $"El docente debe tener al menos {EDAD_MINIMA} años.");

                var nombreResult = ValidarNombreSoloLetras(docente.Nombre);
                if (!nombreResult.valido)
                    return (false, $"Nombre del docente inválido: {nombreResult.mensaje}");

                if (string.IsNullOrWhiteSpace(docente.NumeroEmpleado))
                    return (false, "El número de empleado es requerido.");

                if (docente.Salario <= 0)
                    return (false, "El salario debe ser mayor a cero.");

                return (true, "Docente válido.");
            }
            catch
            {
                return (false, "Error al validar docente.");
            }
        }
    }
}
