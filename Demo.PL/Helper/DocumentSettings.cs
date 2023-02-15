using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Demo.PL.Helper
{
    public class DocumentSettings
    {
        public static string UploadFile(IFormFile file,string folderName)
        {
            //1-get located folder path
            var folderpath=Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file",folderName);
            //2- get file name and make it unique
            var fileName = $"{Guid.NewGuid()}{Path.GetFileName(file.FileName)}";
            //3-get file path
            var filePath=Path.Combine(folderpath,fileName);
            //4- save file as streams
            using var fileStream=new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);
            return fileName;
        }
    }
}
