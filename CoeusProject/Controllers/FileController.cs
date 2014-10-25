using CoeusProject.Models;
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
            DoSaveFile(file, nmFile);
            return Content("");
        }

        public ActionResult SavePoster(HttpPostedFileBase poster, String nmFile)
        {
            DoSaveFile(poster, nmFile);
            return Content("");
        }

        private void DoSaveFile(HttpPostedFileBase file, String nmFile)
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
        }

        public String FormatImage(String nmFoto)
        {
            String physicalPath = Path.Combine(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "User_Data"), nmFoto);

            if (!System.IO.File.Exists(physicalPath))
            {
                return String.Empty;
            }

            Image resizedImage = ResizeImage(physicalPath, new Size(80, 80));
            System.IO.File.Delete(physicalPath);
            physicalPath = Path.ChangeExtension(physicalPath, "png");
            resizedImage.Save(physicalPath, ImageFormat.Png);

            Image thumb = ResizeImage(physicalPath, new Size(32, 32));
            String thumbPath = physicalPath.Replace(".png", "-thumb.png");

            if (System.IO.File.Exists(thumbPath))
            {
                System.IO.File.Delete(thumbPath);
            }

            thumb.Save(thumbPath, ImageFormat.Png);
            return Path.ChangeExtension(nmFoto, "png");
        }

        public String FormatPoster(String nmPoster)
        {
            String physicalPath = Path.Combine(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "User_Data"), nmPoster);

            if (!System.IO.File.Exists(physicalPath))
            {
                return String.Empty;
            }

            Image resizedImage = ResizeImage(physicalPath, new Size(400, 225));
            System.IO.File.Delete(physicalPath);
            physicalPath = Path.ChangeExtension(physicalPath, "png");
            resizedImage.Save(physicalPath, ImageFormat.Png);

            return Path.ChangeExtension(nmPoster, "png");
        }

        private Image ResizeImage(String path, Size size)
        {
            Stream imageStream = new FileStream(path, FileMode.Open);

            Image resizedImage = (Image)(new Bitmap(Image.FromStream(imageStream), size));
            imageStream.Close();
            imageStream.Dispose();

            return resizedImage;
        }

        public static void RemoveFile(String fileName)
        {
            String physicalPath = Path.Combine(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "User_Data"), fileName);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        public void CleanImages(CoeusProjectContext Context = null)
        {
            if (Context == null) Context = new CoeusProjectContext();

            Int32 idLastImage = Sequence.GetSequence("foto", Context);
            Int32 idLastUserVideoImage = 0;

            Usuario lastUser = Context.Usuarios.OrderByDescending(u => u.NmFoto).FirstOrDefault();
            if (lastUser == null) return;

            idLastUserVideoImage = Convert.ToInt32(lastUser.NmFoto.Replace(".png", "").Replace(".jpg", "").Replace(".bmp", ""));

            Video lastVideo = Context.Videos.OrderByDescending(v => v.TxUrlPoster).FirstOrDefault();
            if (lastVideo != null)
            {
                Int32 idPoster = Convert.ToInt32(lastVideo.TxUrlPoster.Replace(".png","").Replace(".jpg","").Replace(".bmp", ""));

                if (idPoster > idLastUserVideoImage) idLastUserVideoImage = idPoster;
            }

            for (Int32 i = idLastUserVideoImage; i < idLastImage; i++)
            {
                String LPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "User_Data");
                
                String nmImgFile = Path.Combine(LPath, (i.ToString() + ".png"));
                if (System.IO.File.Exists(nmImgFile))
                {
                    System.IO.File.Delete(nmImgFile);
                    continue;
                }

                nmImgFile = Path.Combine(LPath, (i.ToString() + ".jpg"));
                if (System.IO.File.Exists(nmImgFile))
                {
                    System.IO.File.Delete(nmImgFile);
                    continue;
                }

                nmImgFile = Path.Combine(LPath, (i.ToString() + ".bmp"));
                if (System.IO.File.Exists(nmImgFile))
                {
                    System.IO.File.Delete(nmImgFile);
                    continue;
                }
            }

            Sequence.ResetSequence("foto", (idLastUserVideoImage + 1), Context);
        }
    }
}