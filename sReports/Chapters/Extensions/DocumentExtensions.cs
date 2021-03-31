using Chapters.Helpers;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Layout;
using iText.Layout.Element;
using sReportsV2.Domain.Entities.OrganizationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapters
{
    public static class DocumentExtensions
    {
        private const int TextMaxLength = 90;
        private const int Step = 20;
        private const int PageHeight = 833;
        private const int PageWidth = 425;
        public static void AddParagraph(this Document doc, string basePath, string text, int padding, int fontSize, int pageCounter, ref int offset, int additionalPadding, int additionalOffset = 0, bool isBlack = false, bool isPageDescription = false)
        {
            if (text != null)
            {
                List<string> paragraphValues = text.GetRows(TextMaxLength);

                foreach (string value in paragraphValues)
                {
                    offset++;
                    Paragraph paragraph = new Paragraph(value);
                    paragraph.SetFixedPosition(padding, PageHeight - Step * offset + additionalOffset + additionalPadding, PageWidth);
                    paragraph.SetPageNumber(pageCounter);
                    paragraph.SetFontSize(fontSize - 1);

                    if (isBlack)
                    {
                        paragraph.SetFontColor(new DeviceRgb(0, 0, 0));
                    }
                    else
                    {
                        paragraph.SetFontColor(new DeviceRgb(61, 69, 69));
                    }

                    if (isPageDescription)
                    {
                        paragraph.SetFontColor(new DeviceRgb(104, 115, 165));
                        paragraph.SetItalic();
                    }

                    if (fontSize > 9 && !isPageDescription)
                    {
                        paragraph.SetBold();
                        //paragraph.SetStrokeWidth(0);
                    }

                    paragraph.SetFont(PdfFontFactory.CreateFont(basePath + @"\AppResource\ROBOTO-REGULAR.TTF",PdfEncodings.IDENTITY_H, true));
                    doc.Add(paragraph);

                }
            }
        }

        public static void AddParagraphs(this Document doc, List<Paragraph> paragraphs)
        {
            foreach (Paragraph paragraph in paragraphs)
            {
                doc.Add(paragraph);
            }
        }

        public static void AddPageImage(this Document document, string imagePath, int pageNum, float defaultPageWidth, int bottom, ref int additionalPadding)
        {
            Image img = new Image(GetDataByImageExtension(imagePath));
            float imgHeight = img.GetImageHeight();
            float imgWidth = img.GetImageWidth();
            if (bottom - imgHeight > 70 && imgWidth < defaultPageWidth)
            {
                img.SetFixedPosition(pageNum, (defaultPageWidth + 94 - imgWidth) / 2, bottom - imgHeight);
                additionalPadding -= (int)img.GetImageHeight();
                document.Add(img);
            }
            else 
            {
                if (bottom - imgHeight < 70) 
                {
                    //scale by height  
                    float corectorOfImageSizeByHeight = (float)(bottom - 70) / (float)imgHeight;
                    imgHeight = imgHeight * corectorOfImageSizeByHeight;
                    imgWidth = imgWidth * corectorOfImageSizeByHeight;
                }

                if (imgWidth > defaultPageWidth) 
                {
                    //scale by width
                    float corectorOfImageSizeByWidth = (float)(defaultPageWidth) / (float)imgWidth;
                    imgHeight = imgHeight * corectorOfImageSizeByWidth;
                    imgWidth = imgWidth * corectorOfImageSizeByWidth;
                }

                img.SetFixedPosition(pageNum, (defaultPageWidth + 94 - imgWidth) / 2 , bottom - imgHeight);
                img.SetWidth(imgWidth);
                img.SetHeight(imgHeight);

                additionalPadding -= (int)(imgHeight + 5);
                
                document.Add(img);
            }
        }
        public static void AddImage(this Document document, string imagePath, int pageNum, int left, int bottom, int height, int width)
        {
            Image img = new Image(GetDataByImageExtension(imagePath));
            img.SetHeight(height);
            img.SetWidth(width);
            img.SetFixedPosition(pageNum, left, bottom);
            document.Add(img);
        }

        public static ImageData GetDataByImageExtension(string imagePath)
        {
            ImageData data = null;
            switch (System.IO.Path.GetExtension(imagePath).ToUpperInvariant())
            {
                case PdfGeneratorType.Jpg:
                    {
                        data = ImageDataFactory.CreateJpeg(new System.Uri(imagePath));
                    }
                    break;
                case PdfGeneratorType.Png:
                    {
                        data = ImageDataFactory.CreatePng(new System.Uri(imagePath));
                    }
                    break;
            }
            return data;
        }

        public static void AddParagraph(this Document document, ParagraphParameters paragraphHelper, string basePath, bool isBold = true)
        {
            Paragraph pa = new Paragraph(paragraphHelper.Text);
            if (isBold)
            {
                pa.SetFontSize(paragraphHelper.FontSize).SetBold();
            }
            else
            {
                pa.SetFontSize(paragraphHelper.FontSize);
            }
            pa.SetFixedPosition(paragraphHelper.Left, paragraphHelper.Bottom, paragraphHelper.Width);
            pa.SetPageNumber(paragraphHelper.PageCounter);
            pa.SetFont(PdfFontFactory.CreateFont($@"{basePath}\AppResource\roboto-regular.ttf", PdfEncodings.IDENTITY_H, true));
            pa.SetFontColor(paragraphHelper.Color);
            document.Add(pa);
        }


    }
}
