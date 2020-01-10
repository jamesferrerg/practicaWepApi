using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CursoMvcApi.Models;

namespace CursoMvcApi.Controllers
{
    public class BaseController : ApiController
    {
        public string error = "";
        public bool Verify(string token)
        {
            using(BDApiWebEntities bd=new BDApiWebEntities())
            {
                if(bd.Usuarios.Where(rango=>rango.Token==token && rango.Habilitado == 1).Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
