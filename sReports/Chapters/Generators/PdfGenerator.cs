using Chapters.Helpers;
using Hl7.Fhir.Model;
using iText.Forms;
using iText.Forms.Fields;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Enums;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Color = iText.Kernel.Colors.Color;
using Image = iText.Layout.Element.Image;
using Math = System.Math;
using Rectangle = iText.Kernel.Geom.Rectangle;

namespace Chapters
{
    public class PdfGenerator
    {
        private int counter = 0; //4
        private int pageCounter = 1;
        private int aboutRectanglesCounter = 5;
        private int additionalPadding = 0;
        private FormChapter currentChapter;
        private const int PageFontSize = 12;
        private const int FieldFontSize = 10;
        private const int CheckAndRadioFontSize = 8;
        private const int footerX = 22;
        private const int footerY = 60;
        private const int footerWidth = 557;
        private const int footerHeight = 1;
        private const int PagePaddingX = 47;
        private const int FormFieldPadding = 87;
        private const int RectanglePadding = 81;
        private const int RectangleWidth = 557;
        private const int PageHeight = 833;
        private const int Step = 20;
        private int numberOfFields = 0;
        private int chapterCount = 0;
        private Form formJson;
        private PdfDocument pdfDocument;
        private Document document;
        private PdfAcroForm pdfAcroForm;
        private PdfWriter pdfWritter;
        private MemoryStream stream;
        private List<RectangleParameters> rectangles = new List<RectangleParameters>();
        private Dictionary<string,int> dicPagePosition = new Dictionary<string, int>();
        private readonly string basePath; 
        public sReportsV2.Domain.Entities.OrganizationEntities.Organization Organization;
        public User User;
        public string Definition;
        public string Language;
        public string PostingDateTranslation;
        public string VersionTranslation;
        public string LanguageTranslation;
        private string Html = string.Empty;
        private int startPosition;
        private int endPosition;
        private int pageTemp = 1;
        private bool isPageChanged = false;
        private int fieldSetPosition = 0;

        private FileStream fileStream;

        public PdfGenerator(Form formChapter, string path)
        {
            basePath = path;
            formJson = formChapter;
        }

        public void InitializeDocument()
        {
            stream = new MemoryStream();
            pdfWritter = new PdfWriter(stream);
            pdfDocument = new PdfDocument(pdfWritter);
            document = new Document(pdfDocument);
            pdfAcroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
            pdfAcroForm.SetGenerateAppearance(true);
        }

        public byte[] Generate()
        {
            InitializeDocument();

            pdfDocument.AddNewPage();
            AddAboutElements();
            AddInfo();
            AddChapters();
            AddAdditionalElements();

            SetNotes();
            MergeHtmlWithPdf();

            AddAllRectangles();
            AddHeader();
            AddFooter();
            AddPageLines();

            pdfDocument.Close();

            return GetPdfBytes();
        }

        public byte[] GetPdfBytes()
        {
            byte[] pdfBytes = null;

            if (stream != null)
            {
                document.Flush();
                pdfBytes = stream.ToArray();
            }

            return pdfBytes;
        }

        public void AddAdditionalElements() 
        {
            pageCounter++;
            // notes, date and formState
            AddLabelFieldPair("Notes :","note");
            AddLabelFieldPair("Date (YYYY-MM-DD) :", "date");
        }

        public void AddLabelFieldPair(string Label, string keyName) 
        {
            CheckPagePosition();
            document.AddParagraph(basePath, Label, FormFieldPadding, FieldFontSize, pageCounter, ref counter, additionalPadding, 22);
            CheckPagePosition();
            AddField(keyName, "");
            additionalPadding -= 7;
        }

       

        public string GetStyledHtmlString() 
        {

            string styledHtml = "<style> " +
                    "h2{font-size : 18px;}" +
                    "h3{font-size : 16px; margin-left : 47px; margin-right : 47px; }" +
                    "p {font-size : 12px; margin-left : 87px; margin-right : 87px; }" +
                    "u {font-size : 12px; margin-left : 87px; margin-right : 87px; }" +
                    "img {margin-left : 87px; margin-right : 87px; max-width : 80%; max-height : 750px; }" +
                " </style>" +
                "<div style=\"margin-bottom : 70px; color:#3d4545; overflow: hidden;\" > " + Html.ReplaceAllBr() + "</div>";

            return styledHtml;
        }

        public ConverterProperties GetConverterProperties()
        {
            ConverterProperties properties = new ConverterProperties();
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddFont($@"{basePath}\AppResource\roboto-regular.ttf", PdfEncodings.IDENTITY_H);
            properties.SetFontProvider(fontProvider);

            return properties;
        }

        public void MergeHtmlWithPdf() 
        {
            if (!string.IsNullOrWhiteSpace(Html)) 
            {
                string pathToTempPdfFile = basePath + "tempPdf.pdf";

                fileStream = new FileStream(pathToTempPdfFile, FileMode.Append);
                HtmlConverter.ConvertToPdf(GetStyledHtmlString(), fileStream, GetConverterProperties());

                fileStream = new FileStream(pathToTempPdfFile, FileMode.Open);
                PdfDocument htmlPdf = new PdfDocument(new PdfReader(fileStream));
                htmlPdf.CopyPagesTo(1, htmlPdf.GetNumberOfPages(), pdfDocument);

                pageCounter += htmlPdf.GetNumberOfPages();
                fileStream.Dispose();
                File.Delete(pathToTempPdfFile);
            }
        }

        public void AddNote(Help note)
        {
            Html += note.Content;
        }

        public void SetNotes() 
        {
            foreach (FieldSet fieldSet in formJson.GetAllFieldSets()) 
            {
                if (fieldSet.Help != null) 
                {
                    AddNote(fieldSet.Help);
                }

                foreach (Field field in fieldSet.Fields) 
                {
                    if (field.Help != null)
                    {
                        AddNote(field.Help);
                    }
                }
            }   
        }

        public void AddPageLines()
        {
            int position = 0;
            int addPadding = 0;

            foreach (var kvp in dicPagePosition)
            {
                int.TryParse(kvp.Key.Split('-')[0], out position);
                int.TryParse(kvp.Key.Split('*')[1], out addPadding);

                AddLine(kvp.Value, 21, 843 -  position * Step + addPadding, 12, 3.3);
            }
        }

        public void AddIconRectangle(RectangleParameters rectangle)
        {
            int bottom = PageHeight - Step * rectangle.Position + rectangle.AdditionalPadding;
            int additionalPadd = 0;
            if (rectangle.NumOfRows % 2 == 0 && rectangle.IsPencil)
            {
                additionalPadd = 11;
                document.AddImage($@"{basePath}\AppResource\fieldSetBackground.jpg", rectangle.PageCounter, 45, bottom, 25, 25);
            }
            string imagePath = rectangle.IsPencil ? $@"{basePath}\AppResource\icon1.jpg" : $@"{basePath}\AppResource\fieldSetBackground.jpg";
            document.AddImage(imagePath, rectangle.PageCounter, 45, bottom + additionalPadd, 25, 25);
        }

        public void AddAllRectangles()
        {
            foreach (var rectangle in rectangles)
            {
                switch (rectangle.Type)
                {
                    case "fieldSets":
                        {
                            counter += rectangle.NumOfRows - 1;
                            AddRectangle(rectangle.Position, rectangle.PageCounter, rectangle.AdditionalPadding);
                            AddTextIntoRectangle(rectangle, "black", true, 74 ,rectangle.AdditionalPadding, false, false, true);
                            AddIconRectangle(rectangle);
                        }
                        break;
                    case "chapters":
                        {
                            
                            AddRectangleForChapter(rectangle.Position, rectangle.PageCounter, rectangle.AdditionalPadding);
                            AddTextIntoRectangle(rectangle, "white", true, 34, rectangle.AdditionalPadding);

                        }
                        break;
                    case "about":
                        {
                            AddRectangleForAbout(rectangle.Position, rectangle.PageCounter);
                        }
                        break;
                }
            }

            SetAboutTextIntoRectangle();
            

        }

        public void SetAboutTextIntoRectangle()
        {
            foreach (var rectangle in rectangles.Where(x => x.Type.Equals("about")))
            {
                AddTextIntoRectangle(rectangle, "black", true, 37, 0, rectangle.Text.Equals("sReports"), true);
            }
        }

        public void AddFooterContent(int pagePosition)
        {
            AddLine(pagePosition, footerX, footerY, footerWidth, footerHeight);

            Paragraph footerParagraph = new Paragraph("Powered by niAnalytics GmbH");
            footerParagraph.SetFixedPosition(pagePosition, 32, 25, 300);
            footerParagraph.SetFontSize(10);
            footerParagraph.SetFontColor(new DeviceRgb(61, 69, 69));
            footerParagraph.SetFont(PdfFontFactory.CreateFont($@"{basePath}\AppResource\roboto-regular.ttf", PdfEncodings.IDENTITY_H, true));

            document.Add(footerParagraph);

            AddFooterIcons(pagePosition);
        }

        public void AddFooter()
        {
            for (int i = 1; i < pageCounter + 1 ; i++)
            {
                AddFooterContent(i);                
            }
        }

        public void AddFooterIcons(int pageNum)
        {
            string imagePath = $@"{basePath}\AppResource\footerIcons.png";
            ImageData data = ImageDataFactory.CreatePng(new System.Uri(imagePath));
            Image img = new Image(data);
            img.SetHeight(33);
            img.SetWidth(33);
            img.SetFixedPosition(pageNum, 542,18);
            document.Add(img);
        }

        public void AddHeader()
        {
            /*Paragraph paraHeader = new Paragraph();
            paraHeader.Add(Organization != null && Organization.Alias != null ? new Text(Organization.Alias).SetBold() : new Text("Inselspital").SetBold());
            paraHeader.Add(Organization != null && Organization.Name != null ? new Text(","+ Organization.Name) : new Text(", University Hospital Bern"));
            paraHeader.SetFixedPosition(40, 797, 200);
            paraHeader.SetFontSize(10);
            paraHeader.SetFontColor(new DeviceRgb(74, 74, 74));
            paraHeader.SetFont(PdfFontFactory.CreateFont($@"{basePath}\AppResource\roboto-regular.ttf", PdfEncodings.IDENTITY_H, true));
            document.Add(paraHeader);

            string text = Organization != null && Organization.Address != null && !string.IsNullOrEmpty(Organization.Address.Street) ? Organization.Address.Street : "Freiburgstrasse";
            ParagraphParameters paraHelper = new ParagraphParameters(text, 10, 40, 785, 200, 1, new DeviceRgb(74, 74, 74));
            document.AddParagraph(paraHelper, basePath);

            string textPara = Organization != null && Organization.Address != null ? Organization.Address.ToString() : "CH-2010 Bern";
            ParagraphParameters paraHelper2 = new ParagraphParameters(textPara, 10, 40, 773, 200, 1, new DeviceRgb(74, 74, 74));
            document.AddParagraph(paraHelper2, basePath);

            string textParagrah = Organization != null && Organization.Telecom != null && Organization.Telecom.Count() > 0 ? Organization.Telecom[0].Value.ToString() : "T +41 31 632 21 11";
            ParagraphParameters paraHelper3 = new ParagraphParameters(textParagrah, 10, 40, 761, 200, 1, new DeviceRgb(74, 74, 74));
            document.AddParagraph(paraHelper3, basePath);

            /*string imagePath = Organization == null || Organization.LogoUrl == null ? $@"{basePath}\AppResource\insel.png" : Organization.LogoUrl.ToString();
            document.AddImage(imagePath,1, 458, 773,40,115);*/

            //AddLine(1, 21, 759, 0.7, 53);*/
        }

        public void AddTextIntoRectangle(RectangleParameters rectangle,  string colorOfText, bool isBolded, int padding, int addPadding, bool isHeaderTitle = false, bool isAboutRectangles = false, bool isFieldSetTitle = false)
        {
            PdfCanvas over = new PdfCanvas(pdfDocument, rectangle.PageCounter);
            Paragraph p = GetParagraphForRectangles(rectangle.Text, isHeaderTitle, colorOfText, isBolded, isAboutRectangles);

            int paddingCenterText = isFieldSetTitle ? 6 : 4;

            if (isAboutRectangles)
            {
                new Canvas(over, pdfDocument, pdfDocument.GetDefaultPageSize()).ShowTextAligned(p, padding, PageHeight - 3 - Step * (rectangle.Position-1) - aboutRectanglesCounter * 7, 1, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                aboutRectanglesCounter--;
            }
            else
            {
                new Canvas(over, pdfDocument, pdfDocument.GetDefaultPageSize()).ShowTextAligned(p, padding + 10, PageHeight + paddingCenterText - Step * rectangle.Position + addPadding, 1, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
            }

            over.SaveState();
            over.RestoreState();
        }

        public Paragraph GetParagraph(string text, List<string> values, bool isAbout, bool isHeaderTitle)
        {
            Paragraph p = new Paragraph();
            if (text != null)
            {
                if (values.Count > 1)
                {
                    p.SetFontSize(10);
                    if (isAbout)
                    {
                        p.Add(new Text(values[0] + ": "));
                        p.Add(new Text(values[1]).SetBold());
                    }
                    else
                    {
                        for (int i= 0; i < values.Count() - 1; i++)
                        {
                            p.Add(new Text(values[i] + ": ").SetBold());
                        }

                        p.Add(new Text(values[values.Count()-1]).SetBold());
                    }
                }
                else
                {
                    p = isHeaderTitle ? new Paragraph(text).SetFontSize(16) : new Paragraph(text).SetFontSize(10);
                }
            }
            return p;
        }

        public Paragraph GetParagraphForRectangles(string text, bool isHeaderTitle, string colorOfText, bool isBolded, bool isAbout)
        {
            List<string> values = text.Split(':').ToList();
            Paragraph p = GetParagraph(text, values, isAbout, isHeaderTitle);
            p.SetColor(colorOfText, isHeaderTitle);
            p.SetFont(PdfFontFactory.CreateFont($@"{basePath}\AppResource\roboto-regular.ttf", PdfEncodings.IDENTITY_H, true));

            if (isBolded && (values.Count < 2 || string.IsNullOrEmpty(values[1])))
            {
                p.SetBold();
            }
            
            return p;
        }

        public Color GetOrganizationColor()
        {
            Color colorHelper = Organization != null && !string.IsNullOrEmpty(Organization.PrimaryColor) ? new DeviceRgb(System.Drawing.ColorTranslator.FromHtml(Organization.PrimaryColor)) : new DeviceRgb(236, 236, 236);
            float sum = colorHelper.GetColorValue().Sum();
            Color color = Organization == null || string.IsNullOrEmpty(Organization.PrimaryColor) || sum < 1.5f ? new DeviceRgb(236, 236, 236) : new DeviceRgb(System.Drawing.ColorTranslator.FromHtml(Organization.PrimaryColor));

            return color;
        }

        public void AddRectangle(int position, int pageNum, int addPadding)
        {
            PdfCanvas canvas = new PdfCanvas(pdfDocument, pageNum);
            canvas.Rectangle(RectanglePadding - 35, PageHeight - Step * position + addPadding, RectangleWidth - 25, 25);
            canvas.SetColor(GetOrganizationColor(), true);
            canvas.Fill();
        }

        public void AddLine(int pageNum, int x, int y,double width, double height)
        {
            PdfCanvas canvas = new PdfCanvas(pdfDocument, pageNum);
            Color color = new DeviceRgb(230, 230, 230);
            canvas.Rectangle(x, y, width, height );
            canvas.SetColor(color, true);
            canvas.Fill();
        }

        public void AddRectangleForChapter(int position, int pageNum, int addPadding)
        {
            PdfCanvas canvas = new PdfCanvas(pdfDocument, pageNum);
            Color color = new DeviceRgb(0, 150, 112);
            canvas.Rectangle(RectanglePadding - 60, PageHeight - Step * position + addPadding, RectangleWidth, 24);
            canvas.SetColor(color, true);
            canvas.Fill();
        }

        public void AddRectangleForAbout(int position, int pageNum)
        {
            PdfCanvas canvas = new PdfCanvas(pdfDocument, pageNum);
            Color color = new DeviceRgb(236, 236, 236);
            canvas.Rectangle(RectanglePadding - 60, PageHeight - Step * position - 3 , RectangleWidth, 20);
            canvas.SetColor(color, true);
            canvas.Fill();
        }

        public void AddInfo()
        {
            Dictionary<string, string> dicInfo = new Dictionary<string, string>();
            
            dicInfo.Add("formId", formJson.Id.ToString());
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetMoreInfo(dicInfo);
        }

        public void AddMultiRowRectangles(string text, string type, int partSize)
        {
            List<string> rows = text.GetRows(partSize);

            foreach (var row in rows)
            {
                counter++;
                rectangles.Add(new RectangleParameters(row, counter, pageCounter, type, additionalPadding));

            }
        }

        public void AddChapters()
        {
            chapterCount = formJson.Chapters.Count;
            int count = 0;
            foreach (FormChapter fc in formJson.Chapters)
            {
                count++;
                currentChapter = fc;
                additionalPadding += 6;
                if (fc.Title != null)
                {
                    counter++;
                    AddMultiRowRectangles(fc.Title, "chapters", 87);
                }

                //Chapter description same rectangl as title
                //if (fc.Description != null)
                //{
                //    additionalPadding += 10;
                //    counter++;
                //    AddMultiRowRectangles(fc.Description, "chapters", 87);
                //}

                counter++;
                AddPages(fc);
                CheckIsLastChapter(count);
                numberOfFields = 0;
                counter = 3;
            }
        }

        public void CheckIsLastChapter(int count)
        {

            if (count != chapterCount)
            {
                pdfDocument.AddNewPage();
                pageCounter++;
                additionalPadding = 50;
                
            }
        }

        public void AddPages(FormChapter chapter)
        {
            additionalPadding += 4;
            int i = 0;
            foreach (FormPage fp in chapter.Pages)
            {
                i++;
                if (fp.IsVisible)
                {
                    document.AddParagraph(basePath, fp.Title.ToUpper(), PagePaddingX, PageFontSize, pageCounter, ref counter, additionalPadding, 0, true);
                    dicPagePosition.Add(counter + "-" + pageCounter + "*" + additionalPadding, pageCounter);
                    document.AddParagraph(basePath, fp.Description, PagePaddingX, 10, pageCounter, ref counter, additionalPadding + 6, 0, true, true);
                }

                //add form page if exist
                if (fp.ImageMap != null && fp.ImageMap.Url != null)
                {
                    document.AddPageImage(fp.ImageMap.Url.ToString(), pageCounter, pdfDocument.GetDefaultPageSize().GetWidth() - 94, PageHeight - Step * counter + additionalPadding, ref additionalPadding);
                    additionalPadding -=5 ;
                }

                counter++;


                AddListOfFieldSets(fp.ListOfFieldSets);
                //AddFieldSets(fp.FieldSets);
                if (i != chapter.Pages.Count())
                {
                    pdfDocument.AddNewPage();
                    pageCounter++;
                }
                
                additionalPadding = 0;
                counter = 2;
            }
        }

        public void AddListOfFieldSets(List<List<FieldSet>> listOfFieldSets) 
        {
            CreateRepetitionsOfFieldSets(listOfFieldSets);

            foreach (List<FieldSet> list in listOfFieldSets) 
            {
                AddFieldSets(list);
            }
        }

        public void CreateRepetitionsOfFieldSets(List<List<FieldSet>> listOfFieldSets) 
        {
            foreach (List<FieldSet> list in listOfFieldSets)
            {
                if (list[0].IsRepetitive)
                {
                    int numberOfRepetitions = list[0].NumberOfRepetitionsForPdf > 0 ? list[0].NumberOfRepetitionsForPdf : 3;

                    for (int i = 0; i < numberOfRepetitions - 1; i++)
                    {
                        list.Add(list[0].Clone());
                    }
                }
            }
        }

        public void AddFieldSetRectangles(List<string> rows)
        {
            int i = 0;
            additionalPadding += 10;

            foreach (var row in rows)
            {
                CheckPagePosition();
                rectangles.Add(new RectangleParameters(row, counter, pageCounter, "fieldSets", i.Equals(rows.Count() / 2), rows.Count(), additionalPadding));
                counter++;
                i++;
            }
        }

        public void AddFieldSets(List<FieldSet> fieldSets)
        {
            fieldSetPosition = 0;
            foreach (FieldSet fieldSet in fieldSets)
            {

                if (fieldSet.Label != null)
                {
                    counter++;
                    AddFieldSetRectangles(fieldSet.Label.GetRows(100));
                    counter++;
                }
                additionalPadding += 7;
                AddFormField(fieldSet);
                fieldSetPosition++;

            }
            fieldSetPosition = 0;
        }

        public  List<string> SplitInParts(string s, int partLength)
        {
            List<string> listOfValues = new List<string>();
           
            for (var i = 0; i < s.Length; i += partLength)
                listOfValues.Add(s.Substring(i, Math.Min(partLength, s.Length - i)));

            return listOfValues;
        }

        public void AddFormField(FieldSet fieldSet)
        {
            int i = 0;
            CheckPagePosition();

            foreach (Field formField in fieldSet.Fields.Where(x => !x.IsHiddenOnPdf))
            {
                AddFormFieldElements(formField, CheckDependables(formField, fieldSet), fieldSet.Id);
                if (++i == fieldSet.Fields.Count())
                {
                    additionalPadding += 7;
                }
            }
            counter++;
        }

        public string CheckDependables(Field formField,FieldSet fieldSet)
        {
            string result = string.Empty;
            foreach (FieldSelectable fField in fieldSet.Fields.OfType<FieldSelectable>())
            {
                FormFieldDependable dependable = fField.Dependables.FirstOrDefault(x => x.ActionParams.Equals(formField.Id));
                if (dependable != null)
                {
                    result = dependable.Condition;
                    break;
                }
            }
            return result;
        }

        public void CheckPagePosition(int valuesCount = 1)
        {
            if (PageHeight - (Step * counter) + additionalPadding  < 20 * valuesCount + 80)
            {
                int numOfFields = currentChapter.GetNumberOfFieldsForChapter();

                if ((numberOfFields != numOfFields) || (numberOfFields == numOfFields && chapterCount == formJson.Chapters.Count))
                {
                    pdfDocument.AddNewPage();
                    pageCounter++;
                    counter = 2;
                    additionalPadding = 0;
                }
            }
        }

        public void CheckPagePositionWithoutAddingPage(int valuesCount = 1)
        {
            if (PageHeight - (Step * counter) + additionalPadding < 20 * valuesCount + 80)
            {
                int numOfFields = currentChapter.GetNumberOfFieldsForChapter();

                if ((numberOfFields != numOfFields) || (numberOfFields == numOfFields && chapterCount == formJson.Chapters.Count))
                {
                    pageCounter++;
                    counter = 2;
                    additionalPadding = 0;
                }
            }
        }

        public void AddAboutElements()
        {
            rectangles.Add(new RectangleParameters("sReports", ++counter, pageCounter, "about"));
            rectangles.Add(new RectangleParameters(formJson.EntryDatetime != null ?$"{PostingDateTranslation}: {formJson.EntryDatetime.ToShortDateString().ToString()}" : "", ++counter, pageCounter, "about"));
            rectangles.Add(new RectangleParameters(formJson.Version != null ? $"{VersionTranslation}: {formJson.Version.Major}.{formJson.Version.Minor}" : "", ++counter, pageCounter, "about"));
            rectangles.Add(new RectangleParameters(this.Language != null ?  $"{LanguageTranslation} : {this.Language}" : "", ++counter, pageCounter, "about"));

           /* if (!string.IsNullOrEmpty(Definition))
            {
                AddMultiRowRectangles(Definition, "about", 130);
            }*/
        }

        public void AddField(string keyName, string value)
        {
            string fieldValue = value ?? string.Empty;
            PdfTextFormField field = PdfTextFormField.CreateText(pdfDocument, new Rectangle(FormFieldPadding, PageHeight - Step * (counter++) + additionalPadding, 322, 15), keyName, fieldValue);
            field.SetPage(pageCounter);
            field.SetFontSize(9);
            pdfAcroForm.AddField(field);
        }

        public void AddRadioField(FieldRadio formField,string fieldSetId, int fieldSetPosition)
        {
            PdfButtonFormField group = PdfFormField.CreateRadioGroup(pdfDocument, $"{fieldSetId}-{formField.Id}-{counter}-{pageCounter}-{fieldSetPosition}", " ");
            int radioCounter = 0;

            foreach (var radio in formField.Values)
            {
                CheckPagePosition(radio.Value.Length < 100 ? 1 : radio.Value.Length / 100 + 1);
                AddRadioInput(group, radio.ThesaurusId);

                document.AddParagraphs(AddRadioParagraph(radio, ++radioCounter));

                pdfAcroForm.AddField(group);
                counter++;
            }
        }

        public void AddRadioInput(PdfButtonFormField group,string thesaurus)
        {
            PdfFormField button = PdfFormField.CreateRadioButton(pdfDocument, new Rectangle(FormFieldPadding, PageHeight - Step * counter + additionalPadding, 15, 15), group, thesaurus).SetFont(PdfFontFactory.CreateFont($@"{basePath}\AppResource\roboto-regular.ttf", PdfEncodings.IDENTITY_H, true));
            group.SetRadio(true);
        }

        public List<Paragraph> AddRadioParagraph(FormFieldValue radio, int radioCounter)
        {
            int position = counter;
            var rows = radio.Label.GetRows(108);
            counter += rows.Count() - 1;
            return GetParagraphsForRadio(rows, position);
        }

        public List<Paragraph> GetParagraphsForRadio(List<string> rows, int position)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();

            int counterOfRows = 0;

            foreach (var row in rows)
            {
                Paragraph radioPara = AddRadioParagraphMultiLine(row, counter.Equals(2) ? 2 : position, counterOfRows);

                counterOfRows++;
                paragraphs.Add(radioPara);
            }

            return paragraphs;
        }

        public Paragraph AddRadioParagraphMultiLine(string row, int position, int counterOfRows)
        {
            Paragraph radioPara = new Paragraph(row);

            radioPara.SetFontSize(CheckAndRadioFontSize);
            radioPara.SetFixedPosition(FormFieldPadding + 20, PageHeight - 0 - Step * position - counterOfRows * 20 + additionalPadding, 400);
            radioPara.SetPageNumber(pageCounter);
            radioPara.SetFont(PdfFontFactory.CreateFont($@"{basePath}\AppResource\roboto-regular.ttf",PdfEncodings.IDENTITY_H, true));
            radioPara.SetFontColor(new DeviceRgb(61, 69, 69));

            return radioPara;
        }

        public PdfFormField CreateCheckBoxInput(bool checkedField, int position,int checkBoxCounter, FieldCheckbox formField,string thesaurus, int corector = 0)
        {
            string checkedValue = checkedField ? "Yes" : "Off";
            PdfButtonFormField checkField = PdfFormField.CreateCheckBox(pdfDocument, new Rectangle(FormFieldPadding + 250 * position, PageHeight - Step * (counter - corector) + Step * position + 0 * (checkBoxCounter - 1)  + additionalPadding, 15, 15), $"Field-{formField.Id}-{counter}-{position}-{thesaurus}", checkedValue, PdfFormField.TYPE_CHECK);
            checkField.SetPage(pageCounter);

            return checkField;
        }

        public void CreateCheckBoxLabel(string fieldValue,int position, int checkBoxCounter, int corector = 0)
        {
            CheckPagePosition();
            ParagraphParameters paraHelper = new ParagraphParameters(fieldValue, CheckAndRadioFontSize, FormFieldPadding + 20 + 250 * position, PageHeight - (Step) * (counter - corector) + Step * position + 0 * (checkBoxCounter - 1) + additionalPadding, 200, pageCounter, new DeviceRgb(61, 69, 69));
            document.AddParagraph(paraHelper, basePath, false);
        }

        public PdfFormField CreateCheckBoxInput(FieldCheckbox formField, int position, string thesaurus,string fieldSetId, int fieldSetPosition )
        {
            PdfButtonFormField checkField = PdfFormField.CreateCheckBox(pdfDocument, new Rectangle(FormFieldPadding + 250 * position, PageHeight - Step * counter + additionalPadding, 15, 15), $"{fieldSetId}-{formField.Id}-{counter}-{position}-{thesaurus}-{pageCounter}-{fieldSetPosition}", "Off", PdfFormField.TYPE_CHECK);
            return checkField;
        }

        public void AddCheckBoxField(FieldCheckbox formField,string fieldSetId, int fieldSetPosition)
        {
            int checkBoxCounter = 0;
            List<string> checkedValues = formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0].Split(',').ToList() : new List<string>();
            startPosition = counter;
            endPosition = counter;
            bool isLast = false;
            int i = 0;

            foreach (var fieldValue in formField.Values.OrderByDescending(x => x.Label.Length).ToList())
            {
                i++;
                int position = checkBoxCounter % 2;
                CheckPagePosition();

                pdfAcroForm.AddField(CreateCheckBoxInput(formField, position,fieldValue.ThesaurusId, fieldSetId, fieldSetPosition), pdfDocument.GetPage(pageCounter));

                if (i == formField.Values.Count()) 
                {
                    isLast = true;
                }

                AddCheckBoxPara(fieldValue.Label, position, isLast);
                checkBoxCounter++;

            }
            counter++;
            CheckPagePosition();
        }

        public void AddCheckBoxPara(string value, int position, bool isLast) 
        {
            int startPage = pageCounter;
            startPosition = counter;
            int startAdditionalPadding = additionalPadding;

            List<string> rows = value.GetRows(55);

            foreach (string row in rows) 
            {
                Paragraph paragraph = new Paragraph(row);
                paragraph.SetFixedPosition(FormFieldPadding + 20 + 250 * position, PageHeight - Step * counter + additionalPadding, 240);
                paragraph.SetPageNumber(pageCounter);
                paragraph.SetFontSize(CheckAndRadioFontSize);
                paragraph.SetFont(PdfFontFactory.CreateFont(basePath + @"\AppResource\ROBOTO-REGULAR.TTF", PdfEncodings.IDENTITY_H, true));

                document.Add(paragraph);
                counter++;
                if (position == 0)
                {
                    CheckPagePosition();
                }
                else 
                {
                    CheckPagePositionWithoutAddingPage();
                }
               
            }

            if (position == 0 && !isLast)
            {
                endPosition = counter;

                if (pageCounter != startPage)
                {
                    counter = startPosition;
                    pageTemp = pageCounter;
                    pageCounter--;
                    additionalPadding = startAdditionalPadding;
                    isPageChanged = true;
                }
                else 
                {
                    counter -= rows.Count();
                }
            }
            
            if(position == 1)
            {
                if (isPageChanged)
                {
                    isPageChanged = false;
                    if (startPage == pageCounter)
                    {
                        pageCounter++;
                        additionalPadding = 0;
                    }
                }

                counter = endPosition;
            }

        }

        public void AddSelectField(FieldSelect formField, string fieldSetId, int fieldSetPosition)
        {
            List<string> availableOptions = formField.Values.Select(x => x.Label).ToList();
            PdfChoiceFormField choiceField = PdfFormField.CreateComboBox(pdfDocument, new Rectangle(FormFieldPadding, PageHeight - Step * counter + additionalPadding, 322, 15), $"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.GetSelectedValue(), availableOptions.ToArray());
            choiceField.SetPage(pageCounter);
            choiceField.SetFontSize(9);
            
            pdfAcroForm.AddField(choiceField);

            counter++;
        }

        public void AddFieldLabel(Field formField, string condition)
        {
            string value = string.Empty;

            if (formField.Label != null)
            {
                if (formField.Label.Equals(condition) || formField.Label.Equals(condition + " (specify)") || formField.Label.Equals(condition + " (specify):"))
                {
                    value = !string.IsNullOrEmpty(condition) ? $"{formField.Label}" : formField.Label ?? "";
                }
                else
                {
                    value = !string.IsNullOrEmpty(condition) ? $"{formField.Label}({condition}(specify))" : formField.Label ?? "";
                }

                value = formField.Type.Equals(PdfGeneratorType.Date) || formField.Type.Equals(PdfGeneratorType.DateTime) ? value + "(ex: YYYY-MM-DD)" : value;
            }

            value += formField is FieldString && ((FieldString)formField).IsRepetitive ? " (Repetititve)" : "";
            value += formField is FieldCalculative ? " (Calculative)" : "";


            if (!string.IsNullOrWhiteSpace(value))
            {
                CheckPagePosition();
                document.AddParagraph(basePath, value, FormFieldPadding, FieldFontSize, pageCounter, ref counter, additionalPadding, 22);
            }
            else
            {
                counter++;
            }
        }

        public void AddFormFieldElements(Field formField,string condition, string fieldSetId)
        {
            AddFieldLabel(formField, condition);

            if (formField is FieldString && ((FieldString)formField).IsRepetitive) 
            {
                int numOfRepetitons = ((FieldString)formField).NumberOfRepetitions != null && ((FieldString)formField).NumberOfRepetitions > 0 ? ((FieldString)formField).NumberOfRepetitions : 3;

                for (int i = 0; i < numOfRepetitons; i++) 
                {
                    AddFieldInput(formField, fieldSetId);
                }
            }
            else 
            {
                AddFieldInput(formField, fieldSetId);
            }

        }

        public void AddFieldInput(Field formField, string fieldSetId) 
        {
            switch (formField.Type)
            {
                case PdfGeneratorType.Text:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.File:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.LongText:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.Radio:
                    {
                        additionalPadding += 7;
                        //CheckPagePosition(formField.Values.Count != 0 ? formField.Values.Count + 1 : 2);
                        numberOfFields++;
                        AddRadioField((FieldRadio)formField, fieldSetId, fieldSetPosition);
                        additionalPadding += 2;

                        counter++;
                    }
                    break;
                case PdfGeneratorType.Number:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-number-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding += 4;
                        counter++;
                    }
                    break;
                case PdfGeneratorType.Digits:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.Url:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.Date:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.DateTime:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.Email:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(2);
                        numberOfFields++;
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
                case PdfGeneratorType.Checkbox:
                    {
                        additionalPadding += 7;
                        //CheckPagePosition(formField.Values.Count != 0 ? formField.Values.Count + 1 : 2);
                        numberOfFields++;
                        AddCheckBoxField((FieldCheckbox)formField, fieldSetId, fieldSetPosition);
                        additionalPadding += 22;

                        counter++;
                    }
                    break;
                case PdfGeneratorType.Select:
                    {
                        additionalPadding += 7;
                        CheckPagePosition();
                        numberOfFields++;
                        AddSelectField((FieldSelect)formField, fieldSetId,fieldSetPosition);
                        additionalPadding -= 13;
                    }
                    break;
                default:
                    {
                        additionalPadding += 7;
                        CheckPagePosition(formField is FieldSelectable && ((FieldSelectable)formField).Values.Count != 0 ? ((FieldSelectable)formField).Values.Count + 1 : 2);
                        AddField($"{fieldSetId}-{formField.Id}-{counter}-{fieldSetPosition}", formField.Value != null && formField.Value.Count > 0 ? formField.Value?[0] : string.Empty);
                        additionalPadding -= 13;
                    }
                    break;
            }
        }
    }
}
