using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CursoMvcApi.Models.WS;
using CursoMvcApi.Models;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Text;

namespace CursoMvcApi.Controllers
{
    public class AnimalController : BaseController
    {
        [HttpPost]
        public Reply Get([FromBody] Seguridad model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!Verify(model.token))
            {
                oR.message = "No autorizado";
                return oR;
            }
            try
            {
                //List<AnimalCLS> listaAnimal=null;
                using(BDApiWebEntities bd=new BDApiWebEntities())
                {
                    List<AnimalCLS> listaAnimal = List(bd);

                    oR.result = 1;
                    oR.data = listaAnimal;
                }
            }
            catch (Exception ex)
            {
                oR.message = "Error en el servidor";
            }

            return oR;
        }

        [HttpPost]
        public Reply Add([FromBody] AnimalViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!Verify(model.token))
            {
                oR.message = "No autorizado";
                return oR;
            }
            //Validaciones
            if (!Validate(model))
            {
                oR.message = error;
                return oR;
            }

            try
            {
                using (BDApiWebEntities bd = new BDApiWebEntities())
                {
                    Animales oAnimal = new Animales();
                    oAnimal.Nombre = model.nombre;
                    oAnimal.Patas = model.patas;
                    oAnimal.Habilitado = 1;

                    bd.Animales.Add(oAnimal);
                    bd.SaveChanges();

                    List<AnimalCLS> listaAnimal = List(bd);

                    oR.result = 1;
                    oR.data = listaAnimal;
                }
            }
            catch (Exception ex)
            {

                oR.message = "Error con el servidor";
            }


            return oR;
        }

        [HttpPost]
        public Reply Edit([FromBody] AnimalViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!Verify(model.token))
            {
                oR.message = "No autorizado";
                return oR;
            }
            //Validaciones
            if (!Validate(model))
            {
                oR.message = error;
                return oR;
            }

            try
            {
                using (BDApiWebEntities bd = new BDApiWebEntities())
                {
                    Animales oAnimal = bd.Animales.Find(model.idAnimal);
                    oAnimal.Nombre = model.nombre;
                    oAnimal.Patas = model.patas;

                    bd.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                    bd.SaveChanges();

                    List<AnimalCLS> listaAnimal = List(bd);

                    oR.result = 1;
                    oR.data = listaAnimal;
                }
            }
            catch (Exception ex)
            {

                oR.message = "Error con el servidor";
            }


            return oR;
        }

        [HttpDelete]
        public Reply Delete([FromBody] AnimalViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!Verify(model.token))
            {
                oR.message = "No autorizado";
                return oR;
            }

            try
            {
                using (BDApiWebEntities bd = new BDApiWebEntities())
                {
                    Animales oAnimal = bd.Animales.Find(model.idAnimal);
                    oAnimal.Habilitado = 2;
                    oAnimal.Nombre = model.nombre;
                    oAnimal.Patas = model.patas;

                    bd.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                    bd.SaveChanges();

                    List<AnimalCLS> listaAnimal = List(bd);

                    oR.result = 1;
                    oR.data = listaAnimal;
                }
            }
            catch (Exception ex)
            {

                oR.message = "Error con el servidor";
            }


            return oR;
        }

        [HttpPost]
        public async Task<Reply> Photo([FromUri] AnimalPicture model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            if (!Verify(model.token))
            {
                oR.message = "No autorizado";
                return oR;
            }

            //validacion multipart
            if (!Request.Content.IsMimeMultipartContent())
            {
                oR.message = "No hay imagen";
                return oR;
            }

            await Request.Content.ReadAsMultipartAsync(provider);

            FileInfo fileInfoPicture = null;
            foreach(MultipartFileData fileData in provider.FileData)
            {
                if (fileData.Headers.ContentDisposition.Name.Replace("\\","").Replace("\"","").Equals("Picture"))
                {
                    fileInfoPicture = new FileInfo(fileData.LocalFileName);
                }
            }

            if (fileInfoPicture != null)
            {
                using(FileStream fs=fileInfoPicture.Open(FileMode.Open, FileAccess.Read))
                {
                    byte[] b = new byte[fileInfoPicture.Length];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    while (fs.Read(b, 0, b.Length) > 0) ;

                    try
                    {
                        using(BDApiWebEntities bd=new BDApiWebEntities())
                        {
                            var oAnimal = bd.Animales.Find(model.idPicture);
                            oAnimal.Picture = b;
                            bd.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                            bd.SaveChanges();
                            oR.result = 1;
                        }
                    }
                    catch (Exception ex)
                    {

                        oR.message = "Error al agregar la imagen";
                    }
                }
            }

            return oR;
        }


        #region HELPERS

        private bool Validate(AnimalViewModel model)
        {
            if (model.nombre == "")
            {
                error = "El nombre es obligatorio";
                return false;
            }
            return true;
        }

        private List<AnimalCLS> List(BDApiWebEntities bd)
        {
            List<AnimalCLS> listaAnimal= (from rango in bd.Animales
                                          where rango.Habilitado == 1
                                          select new AnimalCLS
                                          {
                                              nombre = rango.Nombre,
                                              patas = (int)rango.Patas
                                          }).ToList();
            return listaAnimal;
        }

        #endregion
    }
}
