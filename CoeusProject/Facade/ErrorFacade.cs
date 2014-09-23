using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoeusProject.Facade
{
    public class ErrorFacade
    {
        public static string GetErrorMessage(ModelStateDictionary ModelState)
        {
            foreach (var value in ModelState.Values)
            {
                if (value.Errors.Count > 0)
                {
                    return value.Errors[0].ErrorMessage;
                }
            }
            return "Erro não identificado";
        }

        public static string GetErrorMessage(Exception exception)
        {
            String error = String.Empty;

            if (exception is DbEntityValidationException)
            {
                DbEntityValidationException entityException = exception as DbEntityValidationException;
                error = entityException.EntityValidationErrors.FirstOrDefault()
                            .ValidationErrors.FirstOrDefault().ErrorMessage;
            }
            else
            {
                error = GetInnerException(exception);
            }

            return error;
        }

        private static String GetInnerException(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }

            return GetInnerException(ex.InnerException);
        }
    }
}