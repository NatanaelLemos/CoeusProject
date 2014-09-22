using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoeusProject.Controllers
{
    public class FileController : Controller
    {
        public ActionResult SaveFile(HttpPostedFileBase file, String nmFile)
        {
            if (file != null)
            {
                String extension = Path.GetExtension(file.FileName);

                String physicalPath = Path.Combine(Server.MapPath("~/User_Data"), (nmFile + extension));

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }

                file.SaveAs(physicalPath);
            }
            return Content("");
        }

        public String FormatImage(String nmFoto)
        {
            String physicalPath = Path.Combine(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "User_Data"), nmFoto);

            if(!System.IO.File.Exists(physicalPath))
            {
                return String.Empty;
            }
            Stream imageStream = new FileStream(physicalPath, FileMode.Open);

            Image resizedImage = (Image)(new Bitmap(Image.FromStream(imageStream), new Size(80, 80)));
            imageStream.Close();
            imageStream.Dispose();

            System.IO.File.Delete(physicalPath);

            physicalPath = Path.ChangeExtension(physicalPath, "png");
            resizedImage.Save(physicalPath, ImageFormat.Png);

            return Path.ChangeExtension(nmFoto, "png");
        }
    }
}