using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Colorspace;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using Newtonsoft.Json;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingDAtaFromForm
{
    class Program
    {
        public static Form formJson;
        public static PdfDocument pdfDocument;
        public static Document document;
        public static PdfAcroForm pdfAcroForm;

        static void Main(string[] args)
        {
            formJson = JsonConvert.DeserializeObject<Form>(File.ReadAllText(@"C:\sReports\sReports\sReports\Chapters\bin\Debug\presription.json"));
            pdfDocument = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfWriter(@"C:\sReports\sReports\sReports\Chapters\bin\Debug\" + DateTime.Now.Ticks + ".pdf"));
            document = new Document(pdfDocument);
            pdfAcroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);


            PdfButtonFormField group = PdfFormField.CreateRadioGroup(
     pdfDocument, "language", "");
            PdfFormField.CreateRadioButton(pdfDocument,
                new Rectangle(130, 760, 15, 15), group, "English");
            PdfFormField.CreateRadioButton(pdfDocument,
                new Rectangle(200, 760, 15, 15), group, "French");
            PdfFormField.CreateRadioButton(pdfDocument,
                new Rectangle(260, 760, 15, 15), group, "German");
            PdfFormField.CreateRadioButton(pdfDocument,
                new Rectangle(330, 760, 15, 15), group, "Russian");
            PdfFormField.CreateRadioButton(pdfDocument,
                new Rectangle(400, 760, 15, 15), group, "Spanish");
            pdfAcroForm.AddField(group);

            

            document.Close();
            pdfDocument.Close();
            Console.ReadLine();

        }
    }
}
