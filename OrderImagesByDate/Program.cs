using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace OrderImagesByDate
{
    class Program
    {
        private static string imagesDirectory;

        static void Main(string[] args)
        {
            imagesDirectory = args[0];

            string imagesExtension = "*.*";
            if (args.Length > 1)
                imagesExtension = args[1];

            string[] imagesPaths = Directory.GetFiles(imagesDirectory, imagesExtension);

            Console.WriteLine(imagesPaths.Length + " images à déplacer");

            PropertyItem dateProperty;
            foreach (string currentImagePath in imagesPaths)
            {
                using (Image currentImage = Image.FromFile(currentImagePath))
                {
                    try
                    {
                        dateProperty = currentImage.GetPropertyItem(36867);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Date inexistante pour l'image " + currentImagePath);
                        continue;
                    }
                }

                //La date est stockée sous forme d'une chaine d'octets terminée par le char('/0'); seule la partie date sans le caractère de terminaison nous intéresse
                string EXIFpictureDate = Encoding.ASCII.GetString(dateProperty.Value, 0, 19);

                string folder = TryCreateFolder(EXIFpictureDate);
                MoveImageToFolder(currentImagePath, folder);
            }
        }

        private static string TryCreateFolder(string pictureDate)
        {
            //Le format EXIF est exotique, pas de DateTime.Parse simple, donc Split pour aller vite
            //aaaa:mm:dd hh:mm:ss
            string[] splittedDate = pictureDate.Split(':', ' ');

            //Dans notre cas, seuls l'année et le mois nous intéressent
            string dateDirectory = $"{imagesDirectory}\\{splittedDate[0]}-{splittedDate[1]}";
            Directory.CreateDirectory(dateDirectory);

            return dateDirectory;
        }

        private static void MoveImageToFolder(string currentImagePath, string destinationFolder)
        {
            string imageName = new FileInfo(currentImagePath).Name;

            //Il faut supprimer les images déjà copiées, le File.Move ne perrmet pas de les écraser
            File.Delete($"{destinationFolder}\\{imageName}");
            File.Move(currentImagePath, $"{destinationFolder}\\{imageName}");
        }
    }
}
