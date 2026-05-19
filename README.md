# Sistema Académico Universitario 🎓

¡Hola! Este es mi proyecto final para la asignatura. Es un **Sistema Académico Universitario** completamente funcional construido con **C# y Blazor**. 

A lo largo de este semestre aprendí muchísimo sobre Paradigmas de Programación y Programación Orientada a Objetos (POO), y quise aplicar todo eso aquí. El objetivo principal de este proyecto es demostrar cómo separar la lógica de negocio de la interfaz gráfica usando buenas prácticas y patrones de diseño, sin hacer una aplicación gigante, pero sí bien estructurada.

---

## 🚀 ¿Qué hace la aplicación?

Es un sistema que simula la gestión de una universidad. Permite:
- **Ver y administrar estudiantes** (de Pregrado y Posgrado).
- **Consultar docentes y asignaturas**.
- **Registrar evaluaciones** a los estudiantes.
- **Cancelar materias** (dejando un registro en el historial).
- **Ejecutar un análisis de permanencia**: el sistema revisa las notas de todos los estudiantes y si alguien tiene un promedio muy bajo, lanza una alerta.
- **Generar reportes estadísticos** (aprobados, perdidos, promedios) en tiempo real usando consultas LINQ.

---

## 🏗️ ¿Cómo está construido? (Arquitectura y Patrones)

Para este proyecto me esforcé por aplicar los conceptos de **Arquitectura Limpia**. Dividí el proyecto en dos partes principales:

1. **`Biblioteca/`**: Aquí vive toda la lógica pura. No sabe nada de interfaces gráficas ni de botones. Está hecha con C# puro.
2. **`Front/`**: Aquí está la interfaz web hecha con **Blazor Server**. Solo se encarga de mostrar los datos y capturar los clics del usuario.

### Conceptos de POO Aplicados:
Como estudiante, me pareció genial poder integrar todos estos conceptos en código real:

*   **Herencia y Polimorfismo**: Tengo una clase base `Persona` de la cual heredan `Estudiante` y `Docente`. Además, `Estudiante` se divide en `EstudiantePregrado` y `EstudiantePosgrado`, cada uno con su propia forma de calcular sus requisitos de grado (usando métodos virtuales y `override`).
*   **Observer Pattern (Eventos)**: Implementé un sistema reactivo. Cuando se registra una mala nota o se cancela una materia, el sistema dispara "Eventos" (`EventoAlertaPermanencia`, `EventoCancelarMateria`) que son capturados y mostrados en un Log en la pantalla de Inicio.
*   **Facade Pattern**: Cree la clase `AcademicoFacadeService` en el Front. Esta clase actúa como la "puerta de entrada" para que la interfaz de usuario se comunique con la lógica compleja de la Universidad, sin acoplar las vistas con la lógica profunda.
*   **Inyección de Dependencias**: Todos mis servicios (`EstudianteService`, `EvaluacionService`, `GestorEventos`) están inyectados de forma limpia usando el contenedor de .NET (`AddSingleton`).
*   **LINQ**: Todo el apartado de "Reportes Académicos" en el Dashboard está calculado dinámicamente haciendo consultas LINQ sobre las listas de evaluaciones para sacar promedios y agrupar asignaturas.

---

## 📂 Estructura de Carpetas

```text
SistemaAcademico/
│
├── Biblioteca/                 # Lógica de Negocio (Backend)
│   ├── Domain/                 # Modelos (Persona, Estudiante, Evaluacion...)
│   ├── Interfaces/             # Contratos de los servicios (IEvaluacionService...)
│   └── Services/               # Implementación de los servicios
│
├── Front/                      # Interfaz Gráfica (Frontend)
│   ├── Components/             # Componentes reutilizables de Blazor (Navbar, Cards)
│   ├── Views/                  # Vistas principales (Inicio, Estudiantes, Evaluaciones...)
│   ├── Services/               # Facade Pattern
│   └── wwwroot/                # CSS, estilos y recursos visuales
│
└── Documentacion/              # Enunciado y documentos requeridos
```

---

## 💻 ¿Cómo correr el proyecto?

Es súper fácil probarlo. Solo necesitas tener instalado el SDK de .NET 8 (o superior).
1. Abre una terminal en la raíz del proyecto.
2. Ejecuta el siguiente comando:
   ```bash
   dotnet run --project Front\Front.csproj
   ```
3. Abre tu navegador en la dirección que te indique la consola (normalmente `http://localhost:5000` o `http://localhost:5050`).

---

## 📝 Reflexión del Estudiante

Hacer este proyecto fue un gran reto. Al principio, entender cómo conectar las clases sin que la interfaz web dependiera directamente de todo fue complicado. Sin embargo, al aplicar patrones como Facade e Inyección de Dependencias, me di cuenta de lo fácil que es mantener el código. 

Añadir el último reporte de LINQ no rompió nada de la lógica que ya estaba funcionando, ¡y creo que esa es la magia de la programación orientada a objetos! Espero que este proyecto refleje todo el esfuerzo de este semestre. 

¡Gracias por leer y evaluar mi trabajo!
