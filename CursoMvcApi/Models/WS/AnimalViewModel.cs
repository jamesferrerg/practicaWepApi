using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CursoMvcApi.Models.WS
{
    public class AnimalViewModel : Seguridad
    {
        public int idAnimal { get; set; }
        public string nombre { get; set; }
        public int patas { get; set; }
    }
}