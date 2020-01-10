using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CursoMvcApi.Models.WS;
using CursoMvcApi.Models;

namespace CursoMvcApi.Controllers
{
    public class AccessController : ApiController
    {
        [HttpGet]
        public Reply Hi()
        {
            Reply oR = new Reply();
            oR.result = 1;
            oR.message = "What's up bro";

            return oR;
        }
        
        [HttpPost]
        public Reply Login([FromBody] UsuarioCLS model)
        {
            Reply oR = new Reply();
            oR.result = 0;
            try
            {
                using (BDApiWebEntities bd=new BDApiWebEntities())
                {
                    var lista = bd.Usuarios.Where(rango => rango.Email == model.email  && rango.Password == model.password && rango.Habilitado == 1);
                    if (lista.Count() > 0)
                    {
                        oR.result = 1;
                        oR.data = Guid.NewGuid().ToString();

                        Usuarios oUsuario = lista.First();
                        oUsuario.Token = (string)oR.data;
                        bd.Entry(oUsuario).State = System.Data.Entity.EntityState.Modified;
                        bd.SaveChanges();
                    }
                    else
                    {
                        oR.message = "Datos incorrectos";
                    }
                }

            }
            catch (Exception ex)
            {
                oR.message = "Ocurrio un error, estamos corrigiendo";
            }
            return oR;
        }
    }
}
