namespace DynamicsCRM.Api.Services.Helpers
{
   using System;
   using System.Text;

   internal class Helper
   {
      /// <summary> Displays exception information to the console. </summary>
      /// <param name="ex">The exception to output</param>
      internal static string GetExceptionDetail(Exception ex)
      {
         StringBuilder exceptionMessage = new StringBuilder();

         exceptionMessage.Append(ex.Message);
         while (ex.InnerException != null)
         {
            exceptionMessage.Append(string.Format("\t* {0}", ex.InnerException?.Message));
            ex = ex.InnerException;
         }

         exceptionMessage.Append(ex);
         return exceptionMessage.ToString();
      }
   }
}
