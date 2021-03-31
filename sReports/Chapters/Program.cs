using AutoMapper;
using Chapters.MapperProfiles;
using Hl7.Fhir.Model;
using Newtonsoft.Json;
using RestSharp;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.PatientEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using UMLSClient.Client;
using UMLSClient.UMLSClasses;
namespace Chapters
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string pdfDocumentPath = @"C:\sReports\sReports\sReports\Chapters\bin\Debug\" + DateTime.Now.Ticks + ".pdf";

            Form form = JsonConvert.DeserializeObject<Form>(System.IO.File.ReadAllText(@"C:\Users\Sotex\Desktop\formJson.js"));
            PdfGenerator pdfGenerator = new PdfGenerator(form, @"C:\sReports\sReports\sReports\sReportsV2\App_Data");
            var bytes = pdfGenerator.Generate();
            System.IO.File.WriteAllBytes(pdfDocumentPath, bytes);
            //PdfFormParser parser = new PdfFormParser(jsonWithPatient, @"C:\sReports\sReports\sReports\Chapters\bin\Debug\");
            //parser.ReadFieldsFromPdf();

        }      
    }
}

