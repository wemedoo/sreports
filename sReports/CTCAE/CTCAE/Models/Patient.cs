using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Models
{
    public class Patient
    {
        public string PatientId { get; set; }
        public string FormInstanceId { get; set; }
        public string OrganizationRef { get; set; }
        public string VisitNo { get; set; }
        public string SelectId { get; set; }
        public DateTime? Date { get; set; }
        public List<ReviewModel> ReviewModels { get; set; } = new List<ReviewModel>();
        public char FirstLetter { get; set; }
        public List<SelectItemModel> SelectItems { get; set; } = new List<SelectItemModel>();
        public List<SelectItemModel> Templates { get; set; } = new List<SelectItemModel>();
        public string Title { get; set; }
        public void SetCheckedValues(string selectedValue, string indicator)
        {
            int count = 0;
            int iterator = 0;
            if (indicator != null)
            {
                for (int i = 0; i < this.SelectItems.Find(x => x.Id == selectedValue).DefaultList.Count; i++)
                {
                    if (this.ReviewModels.ElementAtOrDefault(iterator) == null)
                        break;
                    if (this.SelectItems.Find(x => x.Id == selectedValue).DefaultList[count].CTCAETerm != this.ReviewModels[iterator].CTCAETerms)
                        count++;
                    else
                    {
                        this.SelectItems.Find(x => x.Id == selectedValue).DefaultList[count].Grade = this.ReviewModels[iterator].Grades;
                        iterator++;
                        count++;
                    }
                }
            }
        }

        public void AddTerm(string[] chosen, List<CTCAEModel> models, string selectedValue, string indicator)
        {
            if (chosen.Count() > 0)
            {
                for (int i = 0; i < chosen.Count(); i++)
                {
                    foreach (var model in models)
                    {
                        if (chosen[i] == model.CTCAETerm && !this.SelectItems.Find(x => x.Id == selectedValue).DefaultList.Any(s => s.CTCAETerm == chosen[i]))
                        {
                            this.SelectItems.Find(x => x.Id == selectedValue).DefaultList.Add(model);
                            if (indicator == "admin")
                                this.Templates.Find(x => x.Id == selectedValue).DefaultList.Add(model);
                        }
                    }
                }
            }
        }

        public void RemoveTerm(string[] deleted, string selectId, string selectedValue, string indicator)
        {
            if (deleted.Count() > 0)
            {
                this.SelectItems.Find(x => x.Id == selectedValue).DefaultList.RemoveAll(s => deleted.Contains(s.CTCAETerm));
                if (indicator == "admin")
                    this.Templates.Find(x => x.Id == selectedValue).DefaultList.RemoveAll(s => deleted.Contains(s.CTCAETerm));

                if (selectId == null)
                    this.ReviewModels.RemoveAll(s => deleted.Contains(s.CTCAETerms));
            }
        }

        public void SetSelectedValue(string selectedValue, List<SelectItemModel> templateList)
        {
            if (selectedValue == null)
                SetDefaultList(templateList);
            else
            {
                this.SelectItems.Find(x => x.Id == selectedValue).Id = selectedValue;
                this.SelectId = selectedValue;
            }
        }

        private void SetDefaultList(List<SelectItemModel> templateList)
        {
            foreach (SelectItemModel item in templateList)
            {
                if (!this.SelectItems.Any(x => x.Label == item.Label))
                    this.SelectItems.Add(item);
            }
        }

        public void SetCTCAETerms(string[] chosen, string[] deleted, string selectId, List<CTCAEModel> models, string selectedValue, List<SelectItemModel> templateList)
        {
            if (selectId != null)
            {
                SetDefaultList(templateList);
                this.ReviewModels.Clear();
            }
            else
            {
                this.AddTerm(chosen, models, selectedValue, "");
                this.RemoveTerm(deleted, selectId, selectedValue, "");
            }
        }

        public void SetReviewModel(string[] grades, string[] description, string[] terms, string[] codes)
        {
            for (int i = 0; i < grades.Count(); i++)
            {
                ReviewModel reviewModel = new ReviewModel();
                reviewModel.CTCAETerms = terms[i];
                reviewModel.Grades = grades[i];
                reviewModel.MedDRACode = codes[i];
                reviewModel.GradeDescription = description[i];
                this.ReviewModels.Add(reviewModel);
            }
        }

        public void SetCheckedModel(string[] selectedItem, string[] terms, string[] grades)
        {
            if (selectedItem.Length != 0 && selectedItem[0] != null)
            {
                this.ReviewModels.Clear();
                for (int i = 0; i < selectedItem.Count(); i++)
                {
                    ReviewModel reviewModel = new ReviewModel();
                    reviewModel.CTCAETerms = terms[i];
                    reviewModel.Grades = grades[i];
                    reviewModel.GradeDescription = selectedItem[i];
                    this.ReviewModels.Add(reviewModel);
                }
            }
        }

        public void SetTemplateList(string[] chosen, string[] deleted, List<CTCAEModel> models, string title) 
        {
            this.SelectItems.Find(x => x.Id == this.SelectId).Label = title;
            this.Templates.Find(x => x.Id == this.SelectId).Label = title;
            foreach (var model in models)
            {
                for (int i = 0; i < chosen.Length; i++)
                {
                    if (model.CTCAETerm == chosen[i] && !this.SelectItems.Find(x => x.Id == this.SelectId).DefaultList.Any(x => x.CTCAETerm == chosen[i]))
                    {
                        this.SelectItems.Find(x => x.Id == this.SelectId).DefaultList.Add(model);
                        this.Templates.Find(x => x.Id == this.SelectId).DefaultList.Add(model);
                    }
                }
            }
            for (int i = 0; i < deleted.Length; i++)
            {
                this.SelectItems.Find(x => x.Id == this.SelectId).DefaultList.RemoveAll(s => s.CTCAETerm == deleted[i]);
                this.Templates.Find(x => x.Id == this.SelectId).DefaultList.RemoveAll(s => s.CTCAETerm == deleted[i]);
            }
        }

        public void SetTemplatePatientInfo()
        {
            SelectItemModel selectItem = new SelectItemModel();
            selectItem.Id = Guid.NewGuid().ToString();
            this.SelectItems.Add(selectItem);
            this.Templates.Add(selectItem);
            this.SelectId = selectItem.Id;
            this.FirstLetter = 'A';
        }

        public void EditInformation(List<SelectItemModel> templateList, string templateId)
        {
            foreach (SelectItemModel item in templateList)
            {
                if (item.Id == templateId)
                {
                    this.Title = item.Label;
                    this.SelectId = templateId;
                    this.FirstLetter = 'A';
                }
            }
        }

    }
}
