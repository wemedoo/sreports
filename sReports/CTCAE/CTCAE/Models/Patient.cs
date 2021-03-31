using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Models
{
    public class Patient
    {
        public string PatientId { get; set; }
        public string VisitNo { get; set; }
        public int SelectId { get; set; }
        public DateTime? Date { get; set; }
        public List<ReviewModel> ReviewModels { get; set; } = new List<ReviewModel>();
        public char FirstLetter { get; set; }
        public List<SelectItemModel> SelectItems { get; set; } = new List<SelectItemModel>();

        public void SetCheckedValues(int selectedValue, string indicator)
        {
            int count = 0;
            int iterator = 0;
            if (indicator != null)
            {
                for (int i = 0; i < this.SelectItems[selectedValue].DefaultList.Count; i++)
                {
                    if (this.ReviewModels.ElementAtOrDefault(iterator) == null)
                    {
                        break;
                    }
                    if (this.SelectItems[selectedValue].DefaultList[count].CTCAETerm != this.ReviewModels[iterator].CTCAETerms)
                    {
                        count++;
                    }
                    else
                    {
                        this.SelectItems[selectedValue].DefaultList[count].Grade = this.ReviewModels[iterator].Grades;
                        iterator++;
                        count++;
                    }
                }
            }
        }

        public void AddTerm(string[] chosen, List<CTCAEModel> models, int selectedValue)
        {
            if (chosen.Count() > 0)
            {
                for (int i = 0; i < chosen.Count(); i++)
                {
                    foreach (var model in models)
                    {
                        if (chosen[i] == model.CTCAETerm && !this.SelectItems[selectedValue].DefaultList.Any(s => s.CTCAETerm == chosen[i]))
                        {
                            this.SelectItems[selectedValue].DefaultList.Add(model);
                        }
                    }
                }
            }
        }

        public void RemoveTerm(string[] deleted, string selectId, int selectedValue)
        {
            if (deleted.Count() > 0)
            {
                for (int i = 0; i < deleted.Count(); i++)
                {
                    this.SelectItems[selectedValue].DefaultList.RemoveAll(s => s.CTCAETerm == deleted[i]);
                    if (selectId == null)
                        this.ReviewModels.RemoveAll(s => s.CTCAETerms == deleted[i]);
                }
            }
        }

        public void SetSelectedValue(int selectedValue, List<CTCAEModel> models, string indicator)
        {
            if (indicator == null)
            {
                SetDefaultList(models);
            }
            else
            {
                this.SelectItems[selectedValue].Id = selectedValue;
                this.SelectId = selectedValue;
            }
        }

        private void SetDefaultList(List<CTCAEModel> models)
        {
            this.SelectItems.Clear();

            SelectItemModel selectItemModel = new SelectItemModel();
            selectItemModel.Id = 0;
            selectItemModel.Label = "Head and Neck Cancer Patients";

            foreach (var model in models)
            {
                if (model.CTCAETerm == "Dermatitis radiation" || model.CTCAETerm == "Dry mouth" || model.CTCAETerm == "Dysphagia")
                {
                    selectItemModel.DefaultList.Add(model);
                }
            }
            this.SelectItems.Add(selectItemModel);

            SelectItemModel selectItemModel2 = new SelectItemModel();
            selectItemModel2.Id = 1;
            selectItemModel2.Label = "Pancreas";

            foreach (var model in models)
            {
                if (model.CTCAETerm == "Pancreas infection" || model.CTCAETerm == "Pancreatic fistula")
                {
                    selectItemModel2.DefaultList.Add(model);
                }
            }
            this.SelectItems.Add(selectItemModel2);
        }

        public void SetCTCAETerms(string[] chosen, string[] deleted, string selectId, List<CTCAEModel> models, int selectedValue)
        {
            if (selectId != null)
            {
                SetDefaultList(models);
                this.ReviewModels.Clear();
            }
            else
            {
                this.AddTerm(chosen, models, selectedValue);
                this.RemoveTerm(deleted, selectId, selectedValue);
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

    }
}
