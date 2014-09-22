using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace CoeusProject.Facade
{
    public class AccountFacade
    {
        public static void Login(Usuario usuario, Boolean flContinuarConectado)
        {
            FormsAuthentication.SetAuthCookie(usuario.IdUsuario.ToString(), flContinuarConectado);
        }

        public static void Logoff()
        {
            FormsAuthentication.SignOut();
        }

        public static Usuario GetLoggedInUser()
        {
            String idUsuario = HttpContext.Current.User.Identity.Name;

            if (!String.IsNullOrEmpty(idUsuario) && Convert.ToInt32(idUsuario) > 0)
            {
                Int32 idUsuarioInteger = Convert.ToInt32(idUsuario);
                CoeusProjectContext context = new CoeusProjectContext();
                Usuario usuario =context.Usuarios.Where(u => u.IdUsuario == idUsuarioInteger).FirstOrDefault();

                if (usuario == null)
                {
                    AccountFacade.Logoff();
                    return null;
                }
                return usuario.Decrypt();
            }

            return null;
        }
    }
}