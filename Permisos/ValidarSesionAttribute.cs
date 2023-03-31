using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoggingPractica.Permisos
{

    public class ValidarSesionAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session.GetString("usuario") == null)
            {
                filterContext.Result = new RedirectResult("~/acceso/loggin");
            }


            base.OnActionExecuting(filterContext);
        }
        
    }
    
}
