using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCita.Domain.Enums
{
    public enum EstadoCita
    {
        Programada = 1,
        Completada = 2,
        CanceladaPorPaciente = 3,
        CanceladaPorMedico = 4,
        NoAsistida = 5
    }
    
}
