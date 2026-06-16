# 🏥 SmartCita — Backend API

> **Sistema de gestión de citas médicas para consultorios privados (PYMES)**
> Equipo de desarrollo: 5 personas · Entrega: Primera semana de julio 2026

---

## 📋 Tabla de Contenidos

1. [Visión General y Stack Técnico](#1-visión-general-y-stack-técnico)
2. [Cómo Levantar el Proyecto Localmente](#2-cómo-levantar-el-proyecto-localmente)
3. [Nuestra Arquitectura (Explicación para Dummies)](#3-nuestra-arquitectura-explicación-para-dummies)
4. [Flujo de Trabajo en GitHub](#4-flujo-de-trabajo-en-github)
5. [⚠️ LA REGLA DE ORO DE LA BASE DE DATOS](#5-️-la-regla-de-oro-de-la-base-de-datos)
6. [Cómo Probar la API (Guía desde Cero)](#6-cómo-probar-la-api-guía-desde-cero)

---

## 1. Visión General y Stack Técnico

**SmartCita** es un MVP (Producto Mínimo Viable) para digitalizar la gestión de citas médicas en consultorios privados. El sistema permite registrar pacientes, administrar doctores y sus horarios disponibles, y programar o cancelar citas.

Este repositorio contiene **únicamente el Backend (API REST)**. El frontend en Angular vive en su propio repositorio.

### 🛠️ Stack Tecnológico

| Capa | Tecnología | Versión |
|---|---|---|
| **Frontend** | Angular + Bootstrap | Latest |
| **Backend API** | C# + ASP.NET Core | .NET 10 |
| **Base de Datos** | SQL Server | Express / Developer |
| **ORM** | Entity Framework Core | 10.0.8 |
| **CQRS / Mediador** | MediatR | 14.1.0 |
| **Validaciones** | FluentValidation | 12.1.1 |
| **Documentación API** | OpenAPI (Swagger) | Integrado en .NET 10 |

---

## 2. Cómo Levantar el Proyecto Localmente

Sigue estos pasos **en orden**. No te saltes ninguno.

---

### ✅ Prerrequisitos

Antes de empezar, verifica que tienes instalado todo esto en tu máquina:

- [ ] [.NET 10 SDK](https://dotnet.microsoft.com/download) — para compilar y ejecutar el proyecto
- [ ] [SQL Server Express o Developer](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) — la base de datos (ambas ediciones son **gratuitas**)
- [ ] [Visual Studio 2022](https://visualstudio.microsoft.com/es/) **o** [VS Code](https://code.visualstudio.com/) con la extensión C# Dev Kit
- [ ] [Git](https://git-scm.com/) — para clonar el repositorio

> 💡 **¿Cómo verifico que tengo .NET 10 instalado?** Abre una terminal y ejecuta `dotnet --version`. Deberías ver algo como `10.0.x`.

---

### Paso 1 — Clonar el Repositorio

Abre una terminal (CMD, PowerShell o la terminal de VS Code) y ejecuta:

```bash
git clone https://github.com/TU_ORGANIZACION/SmartCita.git
cd SmartCita
```

---

### Paso 2 — Restaurar los Paquetes NuGet

.NET restaura los paquetes automáticamente al compilar, pero puedes forzarlo manualmente:

```bash
dotnet restore
```

> 💡 **¿Qué hace esto?** Descarga todas las librerías externas (MediatR, FluentValidation, EF Core, etc.) que el proyecto necesita. Es equivalente a `npm install` en el mundo de Node.js.

---

### Paso 3 — Configurar la Cadena de Conexión a SQL Server

Abre el archivo `SmartCita.API/appsettings.json` y revisa que la cadena de conexión sea correcta para tu máquina:

```json
{
  "ConnectionStrings": {
    "cadenaConexion": "Server=localhost;Database=SmartCitaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**¿Qué significa cada parte?**

| Fragmento | Qué hace |
|---|---|
| `Server=localhost` | Busca SQL Server en tu misma máquina. Si usas una instancia con nombre, cámbialo a `Server=localhost\SQLEXPRESS` |
| `Database=SmartCitaDB` | El nombre de la base de datos. **No necesitas crearla manualmente**, EF Core la crea sola. |
| `Trusted_Connection=True` | Se autentica con tu usuario de Windows (sin necesidad de contraseña). |

> ⚠️ **Nunca subas contraseñas o datos sensibles al repositorio.** El archivo `appsettings.json` con datos de producción debe mantenerse **fuera del control de versiones**.

---

### Paso 4 — Crear la Base de Datos (Aplicar Migraciones)

Este comando le dice a EF Core que lea todas las migraciones del proyecto y cree/actualice la base de datos automáticamente.

**Opción A — Desde la terminal (recomendado):**

```bash
# Ejecuta desde la raíz de la solución (donde está el archivo .slnx)
dotnet ef database update --project SmartCita.Infrastructure --startup-project SmartCita.API
```

**Opción B — Desde la Consola del Administrador de Paquetes en Visual Studio:**

1. Ve a `Herramientas` → `Administrador de paquetes NuGet` → `Consola del Administrador de Paquetes`
2. En el desplegable **"Proyecto predeterminado"** (arriba a la derecha del panel), selecciona `SmartCita.Infrastructure`
3. Ejecuta:

```powershell
Update-Database
```

✅ Si todo salió bien, verás `Done.` en la consola. La base de datos `SmartCitaDB` con todas sus tablas (Pacientes, Doctores, Citas, Horarios, TiposCita) habrá sido creada en tu SQL Server.

---

### Paso 5 — Ejecutar la API

**Opción A — Desde la terminal:**

```bash
dotnet run --project SmartCita.API
```

**Opción B — Desde Visual Studio:**

1. En el Explorador de Soluciones, haz clic derecho sobre `SmartCita.API`
2. Selecciona **"Establecer como proyecto de inicio"**
3. Presiona `F5` o el botón ▶️ verde en la barra superior

---

### Paso 6 — Verificar que Todo Funciona

Una vez que la API esté corriendo, abre tu navegador en:

```
http://localhost:5029/openapi/v1.json
```

Deberías ver el documento JSON de la API. Para probar los endpoints, puedes usar el archivo `SmartCita.API/SmartCita.API.http` desde VS Code (con la extensión REST Client) o importarlo en Postman.

---

## 3. Nuestra Arquitectura (Explicación para Dummies)

### 🏗️ La Macro-Estructura: 3 Proyectos con Responsabilidades Claras

La solución está dividida en 3 proyectos físicos. Cada uno tiene **una única responsabilidad** y nunca deberían mezclarse:

```
SolutionSmartCita/
│
├── 📦 SmartCita.Domain
│   ├── Entities/          → Las "fichas" del negocio: Paciente.cs, Doctor.cs, Cita.cs...
│   └── Enums/             → Los "catálogos fijos": EstadoCita, EspecialidadMedica...
│   ❌ Sin dependencias externas. Sin NuGet. Sin referencias a otros proyectos.
│
├── 🗄️ SmartCita.Infrastructure
│   └── Persistence/
│       ├── AppDbContext.cs        → El "puente" entre C# y SQL Server
│       └── Migrations/            → El historial de cambios de la base de datos
│   ⚠️ Solo Luis (DBA) toca esta carpeta.
│
└── 🚀 SmartCita.API               → EL PROYECTO DONDE TODOS TRABAJAMOS
    ├── Common/
    │   ├── Behaviors/             → Magia de validación automática (no tocar)
    │   └── Exceptions/            → Manejo global de errores (no tocar)
    ├── Features/                  ← 👈 AQUÍ VIVE TODO TU TRABAJO
    │   ├── Pacientes/
    │   ├── Doctores/
    │   ├── Citas/
    │   └── Horarios/
    └── Program.cs                 → Configuración inicial de la app
```

---

### 🧩 La Micro-Estructura: Vertical Slice Architecture (VSA)

> **¿Por qué no usamos la arquitectura "tradicional" con Controladores y Servicios?**

En la arquitectura tradicional, cuando construyes la funcionalidad "Registrar un Paciente", tu código queda **esparcido** en al menos 3 o 4 archivos ubicados en carpetas distintas. Si dos compañeros trabajan en features diferentes pero "tocan" el mismo archivo `PacientesController.cs` o `PacienteService.cs`, Git entra en conflicto. Con 5 personas trabajando a la vez, esto se convierte en una pesadilla.

**La Analogía del Edificio:**

- 🏢 **Arquitectura Tradicional (horizontal):** El edificio tiene un piso solo para controladores, otro piso solo para servicios, otro piso solo para repositorios. Para atender a un cliente, tienes que subir y bajar pisos constantemente.
- 🏘️ **Nuestra Arquitectura (vertical):** Cada feature es un departamento completo con su propia cocina, sala y baño. Todo lo que necesita está en un solo lugar.

---

### ❌ Lo que NO hacemos (nunca, jamás):

```
/Controllers/
    PacientesController.cs    ← No
    DoctoresController.cs     ← No

/Services/
    PacienteService.cs        ← ❌ NUNCA CREES UNA CARPETA "Services"
    DoctorService.cs          ← ❌ ESTO ROMPE NUESTRA ARQUITECTURA

/Repositories/
    IPacienteRepository.cs    ← No
```

---

### ✅ Lo que SÍ hacemos:

Cada "acción" del sistema es **un único archivo** dentro de `/Features`:

```
/Features/
│
├── Pacientes/
│   ├── RegistrarPaciente.cs       ← Todo en uno ✅
│   ├── ListarPacientes.cs         ← Todo en uno ✅
│   └── ObtenerPacientePorId.cs    ← Todo en uno ✅
│
├── Doctores/
│   ├── RegistrarDoctor.cs         ← Todo en uno ✅
│   └── ListarDoctores.cs          ← Todo en uno ✅
│
├── Citas/
│   ├── CrearCita.cs               ← Todo en uno ✅
│   └── CancelarCita.cs            ← Todo en uno ✅
│
└── Horarios/
    ├── CrearHorario.cs            ← Todo en uno ✅
    └── ListarHorariosDisponibles.cs
```

---

### 📄 Anatomía de un Feature: Las 4 Piezas

Cada archivo de Feature tiene exactamente **4 piezas dentro de una clase contenedora**. Aquí el ejemplo real de `RegistrarPaciente.cs`:

```csharp
public class RegistrarPaciente  // Clase contenedora: solo organiza las 4 piezas
{
    // ──────────────────────────────────────────────────────────────────
    // 🧾 PIEZA 1: COMMAND — El "formulario" de datos de entrada
    //
    //    Define QUÉ datos necesita esta operación para ejecutarse.
    //    Es un "record" (inmutable), lo que lo hace seguro y predecible.
    //    El ": IRequest<Guid>" le dice a MediatR que esta operación
    //    debe devolver un Guid al terminar.
    // ──────────────────────────────────────────────────────────────────
    public record Command(
        string Nombres,
        string Apellidos,
        string Dni,
        DateOnly FechaNacimiento,
        string? Telefono,
        string? Email
    ) : IRequest<Guid>;


    // ──────────────────────────────────────────────────────────────────
    // ✅ PIEZA 2: VALIDATOR — Las reglas de validación
    //
    //    FluentValidation intercepta el Command ANTES de llegar al Handler.
    //    Si alguna regla falla, lanza automáticamente un error HTTP 400
    //    con los detalles del problema. El Handler nunca recibe datos sucios.
    // ──────────────────────────────────────────────────────────────────
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Dni).NotEmpty().Length(8).Matches("^[0-9]*$");
            RuleFor(x => x.Email).EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }


    // ──────────────────────────────────────────────────────────────────
    // ⚙️ PIEZA 3: HANDLER — La lógica de negocio
    //
    //    Aquí va TODO lo que hace la operación: consultar la BD,
    //    aplicar reglas de negocio, crear entidades y guardar cambios.
    //    Es el "corazón" del Feature.
    // ──────────────────────────────────────────────────────────────────
    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context) { _context = context; }

        public async Task<Guid> Handle(Command request, CancellationToken ct)
        {
            // Regla de negocio: el DNI debe ser único en el sistema
            bool dniExistente = await _context.Pacientes
                .AnyAsync(p => p.Dni == request.Dni, ct);
            if (dniExistente)
                throw new InvalidOperationException("Ya existe un paciente con ese DNI.");

            // Creamos la entidad usando el constructor del Domain (con sus validaciones)
            var paciente = new Paciente(request.Nombres, request.Apellidos,
                                        request.Dni, request.FechaNacimiento);
            paciente.Email = request.Email;
            paciente.Telefono = request.Telefono;

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync(ct);

            return paciente.Id;  // Devolvemos el Guid al cliente
        }
    }


    // ──────────────────────────────────────────────────────────────────
    // 🌐 PIEZA 4: MAP ENDPOINT — El punto de acceso HTTP
    //
    //    Define la ruta HTTP y qué respuesta devuelve.
    //    Este método se llama UNA SOLA VEZ en Program.cs para registrarlo.
    //    Usamos Minimal APIs (sin necesidad de un Controlador).
    // ──────────────────────────────────────────────────────────────────
    public static void MapEndPoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/pacientes", async (Command command, IMediator mediator) =>
        {
            var id = await mediator.Send(command);
            return Results.Created($"/api/pacientes/{id}", new { Id = id });
        })
        .WithTags("Pacientes")
        .WithName("RegistrarPaciente");
    }
}
```

**Y en `Program.cs`, solo agregas UNA línea para registrar el endpoint:**

```csharp
// Program.cs — al final, antes de app.Run()
SmartCita.API.Features.Pacientes.RegistrarPaciente.MapEndPoint(app);
SmartCita.API.Features.Doctores.RegistrarDoctor.MapEndPoint(app);   // Tu nueva feature
```

> **🔑 Regla clave al desarrollar:** Cuando te asignen una tarea, crea **un único archivo nuevo** dentro de la carpeta `/Features` correspondiente. El único archivo "compartido" que debes tocar es `Program.cs`, y solo para agregar una línea al final. Así nunca pisarás el trabajo de tus compañeros.

---

## 4. Flujo de Trabajo en GitHub

Para evitar conflictos de Git y mantener el código organizado, todos seguimos el mismo flujo.

### 🌿 Nomenclatura de Ramas

| Tipo de cambio | Formato | Ejemplo |
|---|---|---|
| Nueva funcionalidad | `feature/nombre-tarea` | `feature/registrar-doctor` |
| Corrección de bug | `fix/descripcion-del-bug` | `fix/validacion-dni-nulo` |
| Solo documentación | `docs/descripcion` | `docs/actualizar-readme` |

**Usa nombres cortos, en minúsculas y con guiones.** ✅ `feature/listar-horarios` · ❌ `feature/ListarHorarios_v2_FINAL`

---

### 📋 Paso a Paso para Trabajar una Tarea

```bash
# 1. SIEMPRE empieza desde main y asegúrate de tener la última versión
git checkout main
git pull origin main

# 2. Crea tu rama de trabajo a partir de main
git checkout -b feature/nombre-de-tu-tarea

# 3. Programa tu feature... cuando quieras guardar avances, haz commits
#    Usa mensajes descriptivos: QUÉ hiciste, no CÓMO lo hiciste
git add .
git commit -m "feat: agrega command y validator para registrar doctor"
git commit -m "feat: agrega handler con verificación de colegiatura duplicada"
git commit -m "feat: agrega endpoint POST /api/doctores"

# 4. Cuando termines, sube tu rama al repositorio remoto
git push origin feature/nombre-de-tu-tarea

# 5. Ve a GitHub y abre un Pull Request hacia 'main'
#    → Botón "Compare & Pull Request"
#    → Escribe una descripción breve de qué hiciste y por qué
#    → Asigna a Luis como revisor
```

---

### ✅ Reglas del Pull Request (PR)

1. **Nunca hagas push directo a `main`.** Siempre a través de un PR — sin excepciones.
2. El PR necesita al menos **1 aprobación** (de Luis u otro compañero) antes de hacer merge.
3. Antes de abrir el PR, verifica que tu código **compila sin errores**: `dotnet build`
4. Cada PR debe ser pequeño y enfocado en **una sola tarea**. Un PR con 15 archivos modificados no se aprueba.
5. Si `main` se actualizó mientras trabajabas en tu rama, **actualiza tu rama antes de pedir revisión**:

```bash
git checkout main
git pull origin main
git checkout feature/tu-rama
git merge main        # Resuelve conflictos aquí, en tu rama, no en main
```

---

## 5. ⚠️ LA REGLA DE ORO DE LA BASE DE DATOS

---

> # 🚨 ATENCIÓN EQUIPO — LEE ESTO COMPLETO 🚨

---

## ❌ NADIE, ABSOLUTAMENTE NADIE, EXCEPTO LUIS (EL DBA) ESTÁ AUTORIZADO PARA:

### **MODIFICAR EL ARCHIVO `AppDbContext.cs`**
### **EJECUTAR EL COMANDO `Add-Migration` (O `dotnet ef migrations add`)**
### **MODIFICAR O ELIMINAR ARCHIVOS DENTRO DE LA CARPETA `Migrations/`**

---

**¿Por qué existe esta regla?**

EF Core genera archivos de migración automáticamente basándose en el estado del `AppDbContext`. Si dos personas lo modifican simultáneamente y ambas ejecutan `Add-Migration`, se generan **dos migraciones en conflicto**. Fusionar esos archivos sin romper el historial de la base de datos es extremadamente complejo, y cualquier error puede corromper los datos del equipo entero.

Con 5 personas trabajando, centralizar este control en una sola persona **elimina el 100% de ese riesgo**.

---

### ¿Qué hago si necesito una tabla nueva o una columna nueva?

Sigue este proceso de **3 pasos**:

**Paso 1 — Crea el archivo de Entidad en `SmartCita.Domain`**

Escribe tu entidad siguiendo el mismo patrón de las existentes (constructor privado, constructor de negocio con validaciones, propiedades con setters controlados):

```csharp
// SmartCita.Domain/Entities/MiNuevaEntidad.cs
namespace SmartCita.Domain.Entities
{
    public class MiNuevaEntidad
    {
        private MiNuevaEntidad() { }  // ← Constructor vacío para EF Core

        public MiNuevaEntidad(string nombre)  // ← Constructor de negocio
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio");
            Nombre = nombre;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Nombre { get; private set; } = string.Empty;
        public bool EstaActivo { get; set; } = true;
    }
}
```

**Paso 2 — Avísale a Luis por el canal del equipo**

```
"Luis, necesito la entidad MiNuevaEntidad.cs en la base de datos.
 La encontrarás en SmartCita.Domain/Entities/MiNuevaEntidad.cs.
 Relaciones: [describe si tiene FK con alguna otra tabla].
 La necesito para el feature X que estoy desarrollando."
```

**Paso 3 — Espera a que Luis haga el push y luego actualiza tu BD local**

Una vez que Luis genere la migración y haga push, tú solo ejecutas:

```bash
# Este es el ÚNICO comando relacionado con la BD que todos pueden ejecutar libremente
dotnet ef database update --project SmartCita.Infrastructure --startup-project SmartCita.API
```

O desde Visual Studio (con el proyecto `SmartCita.Infrastructure` seleccionado):

```powershell
Update-Database
```

---

### Resumen rápido del proceso

```
TÚ: Creas la Entidad en Domain
         ↓
TÚ: Avisas a Luis con el contexto
         ↓
LUIS: Agrega al DbContext + Genera migración + Push
         ↓
TÚ: Ejecutas Update-Database en tu máquina local
         ↓
✅ Listo, puedes usar la nueva tabla en tu Feature
```

> 💡 **Esta regla no es burocracia.** Es lo que separa un proyecto funcional de un repositorio en llamas dos semanas antes de la entrega.

---

## 6. Cómo Probar la API (Guía desde Cero)

> **¿Qué significa "probar la API"?**
> Cuando programas un endpoint (ej. `POST /api/pacientes`), necesitas una forma de "llamarlo" para verificar que funciona correctamente antes de que el frontend lo consuma. Para eso existen las herramientas de prueba de APIs. Son como un "simulador de navegador" que te permite enviar cualquier tipo de petición HTTP y ver la respuesta exacta del servidor.

Tenemos **4 opciones** disponibles. Elige la que más te acomode:

| Herramienta | Ideal para | Requiere cuenta | Requiere instalación |
|---|---|---|---|
| **Swagger UI** | Prueba rápida sin instalar nada | No | No |
| **Archivo `.http`** | Pruebas desde VS Code / Visual Studio | No | Solo la extensión |
| **Bruno** | Pruebas serias, guardar colecciones | No | Sí (gratis) |
| **Postman** | El más conocido en la industria | Sí (gratis) | Sí |

---

### 🔵 Opción 1 — Swagger UI (La más rápida, sin instalar nada)

**¿Qué es?** Es una página web que genera automáticamente nuestra API como un formulario interactivo. Está integrada en el proyecto y funciona en el navegador.

**Paso 1 — Levantar la API** (si no la tienes corriendo ya):
```bash
dotnet run --project SmartCita.API
```

**Paso 2 — Abrir Swagger en el navegador:**
```
http://localhost:5029/openapi/v1.json
```

> 💡 `.NET 10` expone la especificación como JSON puro. Para verla con la interfaz visual (formularios interactivos), instala la extensión **"REST Client"** en VS Code y apunta al JSON, o usa el archivo `.http` descrito en la Opción 2. Alternativamente, puedes agregar Scalar o Swashbuckle al proyecto (pregúntale a Luis si lo necesitas).

---

### 🟢 Opción 2 — Archivo `.http` de VS Code / Visual Studio

**¿Qué es?** El proyecto ya incluye el archivo `SmartCita.API/SmartCita.API.http`. Es un archivo de texto simple donde escribes tus peticiones HTTP directamente. VS Code y Visual Studio las ejecutan con un clic.

#### En Visual Studio (sin extensiones):

Visual Studio 2022 ya incluye soporte nativo para archivos `.http`. Solo ábrelo y verás el botón ▶️ junto a cada request.

#### En VS Code:

**Paso 1 — Instalar la extensión "REST Client":**
1. Abre VS Code
2. Ve al panel de extensiones (`Ctrl + Shift + X`)
3. Busca `REST Client` (autor: Huachao Mao)
4. Haz clic en **Install**

**Paso 2 — Usar el archivo `.http` del proyecto:**

Abre `SmartCita.API/SmartCita.API.http`. Verás algo así:

```http
@SmartCita.API_HostAddress = http://localhost:5029

### Registrar un paciente nuevo
POST {{SmartCita.API_HostAddress}}/api/pacientes
Content-Type: application/json

{
  "nombres": "Carlos",
  "apellidos": "Mamani Quispe",
  "dni": "45678901",
  "fechaNacimiento": "1990-05-15",
  "telefono": "987654321",
  "email": "carlos.mamani@email.com",
  "alergias": null,
  "medicamentosActuales": null,
  "historialMedico": null
}
```

**Paso 3 — Ejecutar:**
Aparecerá el texto `Send Request` encima de cada `###`. Haz clic y la respuesta aparece en un panel lateral.

> 💡 **Tip:** Puedes agregar tus propios requests al final del archivo `.http` para probar tus features. Usa `###` para separar cada request.

---

### 🟠 Opción 3 — Bruno (Recomendado para el equipo)

**¿Qué es Bruno?** Es una aplicación de escritorio para probar APIs, similar a Postman, pero **completamente gratuita, sin cuenta y sin límites**. Guarda las colecciones de requests como archivos en tu carpeta del proyecto (se pueden versionar con Git).

**Paso 1 — Descargar e instalar Bruno:**

Ve a [https://www.usebruno.com](https://www.usebruno.com) → Download → Elige tu sistema operativo (Windows / Mac / Linux) → Instala normalmente.

**Paso 2 — Crear una colección para SmartCita:**

1. Abre Bruno
2. Clic en **"Create Collection"**
3. Nómbrala `SmartCita API`
4. Guárdala dentro de la carpeta del proyecto (ej. `/SmartCita/bruno-collection/`)

**Paso 3 — Crear tu primer request:**

1. Clic derecho sobre la colección → **"New Request"**
2. Nombre: `Registrar Paciente`
3. Método: `POST`
4. URL: `http://localhost:5029/api/pacientes`
5. Ve a la pestaña **"Body"** → selecciona **"JSON"**
6. Pega el siguiente cuerpo:

```json
{
  "nombres": "María",
  "apellidos": "López Torres",
  "dni": "12345678",
  "fechaNacimiento": "1995-08-20",
  "telefono": "999888777",
  "email": "maria.lopez@email.com",
  "alergias": "Penicilina",
  "medicamentosActuales": null,
  "historialMedico": null
}
```

7. Clic en **"Send"** (botón naranja o `Ctrl + Enter`)

**Paso 4 — Interpretar la respuesta:**

Bruno te mostrará la respuesta del servidor en el panel derecho:

```json
// ✅ Respuesta esperada si todo salió bien: HTTP 201 Created
{
  "id": "a3f7c2d1-08b4-4e9a-9f3a-123456789abc"
}

// ❌ Respuesta si el DNI ya existe: HTTP 400 Bad Request
{
  "error": "Ya existe un paciente registrado con el mismo DNI."
}

// ❌ Respuesta si mandas datos inválidos: HTTP 400 Bad Request
{
  "title": "Error de Validacion de Datos",
  "errors": {
    "Dni": ["El DNI debe contener exactamente 8 digitos"],
    "Email": ["'Email' is not a valid email address."]
  }
}
```

> 💡 **Tip para el equipo:** Agrega la carpeta `bruno-collection/` al repositorio para que todos compartan los mismos requests. En el `.gitignore` no está excluida, así que se versionará automáticamente.

---

### 🔴 Opción 4 — Postman

**¿Qué es Postman?** La herramienta de prueba de APIs más popular en la industria. Requiere crear una cuenta gratuita.

**Paso 1 — Descargar e instalar:**

Ve a [https://www.postman.com/downloads](https://www.postman.com/downloads) → Descarga la versión de escritorio para tu sistema operativo → Instala y crea una cuenta gratuita.

**Paso 2 — Crear una colección:**

1. En el panel izquierdo, clic en **"Collections"** → botón **"+"**
2. Nómbrala `SmartCita API`

**Paso 3 — Crear tu primer request:**

1. Clic en los tres puntos `···` junto a la colección → **"Add request"**
2. Configura así:

```
Método:  POST
URL:     http://localhost:5029/api/pacientes
```

3. Ve a la pestaña **"Body"** → selecciona **"raw"** → en el desplegable de la derecha elige **"JSON"**
4. Pega el cuerpo del request:

```json
{
  "nombres": "Juan",
  "apellidos": "Pérez García",
  "dni": "87654321",
  "fechaNacimiento": "1988-03-10",
  "telefono": "912345678",
  "email": "juan.perez@email.com",
  "alergias": null,
  "medicamentosActuales": "Metformina 500mg",
  "historialMedico": "Diabetes tipo 2 diagnosticada en 2015"
}
```

5. Clic en el botón azul **"Send"**

**Paso 4 — Configurar una variable de entorno (opcional pero recomendado):**

Para no escribir `http://localhost:5029` en cada request:

1. Clic en el ícono de ojo 👁️ (arriba a la derecha) → **"Add"**
2. Nombre del entorno: `SmartCita Local`
3. Agrega la variable:

| Variable | Initial Value | Current Value |
|---|---|---|
| `baseUrl` | `http://localhost:5029` | `http://localhost:5029` |

4. Ahora en tus requests usa `{{baseUrl}}/api/pacientes` en lugar de la URL completa.

---

### 📊 Tabla de Códigos de Respuesta HTTP que verás

Cuando hagas una prueba, el servidor siempre responde con un **código de estado**. Aquí los que usa SmartCita:

| Código | Nombre | ¿Cuándo lo ves? |
|---|---|---|
| ✅ `201 Created` | Creado | El recurso se creó correctamente (ej. paciente registrado) |
| ✅ `200 OK` | OK | La consulta devolvió datos correctamente |
| ❌ `400 Bad Request` | Error del cliente | Datos inválidos o regla de negocio violada (ej. DNI duplicado) |
| ❌ `404 Not Found` | No encontrado | El recurso que buscas no existe |
| 💥 `500 Internal Server Error` | Error del servidor | Algo explotó en el backend (revisa la consola de la API) |

---

### 🧪 Checklist de Pruebas Mínimas por Feature

Antes de abrir un Pull Request, verifica que tu feature pasa **al menos estos 3 casos**:

```
[ ] Caso feliz:     ¿Funciona con datos válidos?        → Esperas 200 o 201
[ ] Caso de error:  ¿Qué pasa con datos inválidos?      → Esperas 400 con mensaje claro
[ ] Caso duplicado: ¿Qué pasa si ya existe el registro? → Esperas 400 con mensaje claro
```

> **Ejemplo para `RegistrarPaciente`:**
> - ✅ Enviar un paciente con DNI `12345678` por primera vez → `201 Created`
> - ❌ Enviar el mismo DNI `12345678` por segunda vez → `400` con `"Ya existe un paciente con ese DNI"`
> - ❌ Enviar con DNI `123` (menos de 8 dígitos) → `400` con error de validación

---

*Desarrollado con ❤️ por el Equipo SmartCita — 2026*
