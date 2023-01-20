using MathNet.Numerics.Distributions;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sReportsV2.Domain.FormValues;
using sReportsV2.Common.Helpers;
using System.Data;

namespace DocumentGenerator
{
    public static class FormInstanceGenerator
    {
        public static List<FormInstance> Generate(Form form, FormDistribution formDistribution, int numOfDocuments)
        {
            Dictionary<string, List<string>> generatedFields = SetNonDependableFields(formDistribution, numOfDocuments);
            List<FormInstance> generated = GenerateFormInstances(form, generatedFields, numOfDocuments);

            SetDependableFields(formDistribution, generated);
            RoundNumericFieldValues(form, generated);
            SetValuesForCalculativeField(form, generated);

            return generated;
        }

        public static List<FormInstance> GenerateDailyForms(List<FormInstance> whoDocuments, Form form, FormDistribution formDistribution)
        {
            List<int> daysDistribution = GenerateDailyFormNumExamples(whoDocuments.Count);
            int dailyNumOfDocuments = daysDistribution.Sum();
            int skip = 0;
            List<FormInstance> dailyDocumentsGenerated = Generate(form, formDistribution, dailyNumOfDocuments);
            for(int i = 0; i < daysDistribution.Count; i++)
            {
                int numOfDocumentsRequired = daysDistribution[i];
                List<FormInstance> dailyDocuments = dailyDocumentsGenerated.Skip(skip).Take(daysDistribution[i]).ToList();
                skip += daysDistribution[i];

                FormInstance whoDocument = whoDocuments[i];
                FieldValue patientIdField = whoDocument.Fields.FirstOrDefault(x => x.ThesaurusId == 15114);
                SetDailyFormsPatienId(dailyDocuments, patientIdField?.Value?[0]);
            }
            return dailyDocumentsGenerated;
        }

        private static void SetDailyFormsPatienId(List<FormInstance> dailyForms, string patientId)
        {
            if (!string.IsNullOrWhiteSpace(patientId))
            {
                foreach (FormInstance formInstance in dailyForms)
                {
                    formInstance.SetValueByThesaurusId(15114, patientId);
                }
            }

        }

        private  static Dictionary<string, List<string>> SetNonDependableFields(FormDistribution formDistribution, int numOfDocuments)
        {
            Dictionary<string, List<string>> generatedFields = new Dictionary<string, List<string>>();
            if (formDistribution.Fields != null) 
            {
                foreach (FormFieldDistribution field in formDistribution.Fields.Where(x => x.ValuesAll.Where(y => y.DependOn == null || y.DependOn.Count == 0).Count() > 0)) // generate non dependant
                {
                    switch (field.Type)
                    {
                        case FieldTypes.Radio:
                        case FieldTypes.Select:
                            generatedFields.Add(field.Id, GenerateRadioExamples(field.ValuesAll[0].Values, numOfDocuments));
                            break;
                        case FieldTypes.Number:
                            generatedFields.Add(field.Id, GenerateNumberExamples(field.ValuesAll[0].NormalDistributionParameters, numOfDocuments));
                            break;

                        case FieldTypes.Checkbox:
                            generatedFields.Add(field.Id, GenerateCheckboxExamples(field.ValuesAll[0].Values, numOfDocuments));
                            break;
                    }
                }

            }

            return generatedFields;
        }

        private static void SetDependableFields(FormDistribution formDistribution, List<FormInstance> generated)
        {
            if (formDistribution.Fields != null) 
            {
                foreach (FormFieldDistribution field in formDistribution.Fields.Where(x => x.ValuesAll.Where(y => y.DependOn != null && y.DependOn.Count > 0).Count() > 0)) // generate dependant
                {
                    switch (field.Type)
                    {
                        case FieldTypes.Radio:
                        case FieldTypes.Select:
                            GenerateDependantRadioExamples(field, generated);
                            break;
                        case FieldTypes.Number:
                            GenerateDependantNumberExamples(field, generated);
                            break;

                        case FieldTypes.Checkbox:
                            GenerateDependentCheckboxExamples(field, generated);
                            break;
                    }
                }

            }
        }

        private static List<FormInstance> GenerateFormInstances(Form form, Dictionary<string, List<string>> generatedFields, int numOfDocuments)
        {
            List<FormInstance> result = new List<FormInstance>();
            for (int i = 0; i < numOfDocuments; i++)
            {
                FormInstance formInstance = new FormInstance(form.Clone());
                formInstance.Fields = new List<FieldValue>();

                foreach (FieldSet fs in form.GetAllFieldSets())
                {
                    foreach(Field f in fs.Fields)
                    {
                        FieldValue fieldValue = new FieldValue()
                        {
                            Id = f.Id,
                            ThesaurusId = f.ThesaurusId,
                            InstanceId = $"{fs.Id}-0-{f.Id}-0",
                            Type = f.Type
                        };
                        formInstance.Fields.Add(fieldValue);
                    }
                }
                foreach (string key in generatedFields.Keys)
                {
                    string currentValue = generatedFields[key][i];
                    FieldValue field = formInstance.Fields.FirstOrDefault(x => x.Id.Equals(key));
                    if (field != null)
                    {                       
                        field.Value = new List<string>() { currentValue };
                    }
                }

                //the line of code is for covic who form, not will be used in other cases
                //TO DO implement new field type guid
                SetPatientIdentification(formInstance);
                result.Add(formInstance);
            }
            return result;
        }

        private static void SetPatientIdentification(FormInstance formInstance)
        {
            if (formInstance.ThesaurusId == 14573)
            {
                FieldValue patientIdField = formInstance.Fields.FirstOrDefault(x => x.ThesaurusId == 15114);
                patientIdField.Value = new List<string>() { Guid.NewGuid().ToString() };
            }
        }

        private static void GenerateDependantRadioExamples(FormFieldDistribution field, List<FormInstance> generated = null)
        {
            foreach (var fieldSingleValue in field.ValuesAll)
            {
                List<FormInstance> listToUpdateGenerate = GetDocumentsWithDependOnField(field, generated, fieldSingleValue);

                if (listToUpdateGenerate != null)
                {
                    List<string> examples = GenerateRadioExamples(fieldSingleValue.Values, listToUpdateGenerate.Count);
                    UpdateListWithDependables(listToUpdateGenerate, examples, field.Id, fieldSingleValue.DependOn);
                }
            }
        }

        private static List<string> GenerateRadioExamples(List<FormFieldValueDistribution> fieldValues, int numberOfTrials)
        {
            List<string> result = new List<string>();
            if (fieldValues.Sum(x => x.SuccessProbability) == 1)
            {
                Multinomial multinomial = new Multinomial(GetMultinominalWeights(fieldValues), numberOfTrials);
                var samples = multinomial.Samples().Take(1).ToList();
                for (int i = 0; i < samples[0].Count(); i++)
                {
                    string value = fieldValues[i].ThesaurusId.ToString();
                    if (samples[0][i] > 0)
                    {
                        for (int j = 0; j < samples[0][i]; j++)
                        {
                            result.Add(value);
                        }
                    }
                }
            }
            else
            {
                result = GenerateEmptyValues(numberOfTrials);
            }


            ValidateRadioExamples(result, fieldValues);
            return result;
        }

        private static void GenerateDependantNumberExamples(FormFieldDistribution field,  List<FormInstance> generated = null)
        {
            List<FormInstance> listToUpdateGenerated = null;
            foreach (var fieldSingleValue in field.ValuesAll)
            {
                listToUpdateGenerated = GetDocumentsWithDependOnField(field, generated, fieldSingleValue);
                
                if (listToUpdateGenerated != null)
                {
                    List<string> examples = GenerateNumberExamples(fieldSingleValue.NormalDistributionParameters, listToUpdateGenerated.Count);
                    UpdateListWithDependables(listToUpdateGenerated, examples, field.Id, fieldSingleValue.DependOn);
                }
            }
        }

        private static List<string> GenerateNumberExamples(FormFieldNormalDistributionParameters parameters, int numberOfTrials)
        {
            List<string> result = new List<string>();

            double mean = parameters.Mean;
            double standardDeviation = parameters.Deviation;
            
            var samples = new double[numberOfTrials];
            Normal.Samples(samples, mean, standardDeviation);
            result = samples.Select(x => x < 0 ? "0": ((float)x).ToString()).ToList();
            ValidateNumericExamples(result, parameters.Mean, parameters.Deviation);
            return result;

        }

        private static void GenerateDependentCheckboxExamples(FormFieldDistribution field, List<FormInstance> generated)
        {
            if (generated != null)
            {
                foreach (var fieldSingleValue in field.ValuesAll)
                {
                    List<FormInstance> forUpdateGenerated = GetDocumentsWithDependOnField(field, generated, fieldSingleValue);
                    List<string> examples = GenerateCheckboxExamples(fieldSingleValue.Values, forUpdateGenerated.Count);
                    if (forUpdateGenerated != null)
                    {
                        UpdateListWithDependables(forUpdateGenerated, examples, field.Id, fieldSingleValue.DependOn);
                    }
                }
            }
        }

        private static List<string> GenerateCheckboxExamples(List<FormFieldValueDistribution> fieldValues, int numberOfTrials)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            foreach (FormFieldValueDistribution value in fieldValues)
            {
                if (value.SuccessProbability == null)
                    throw new ArgumentNullException("Value can not be null");

                var samples = new int[1];
                Binomial.Samples(samples, value.SuccessProbability ?? 0, numberOfTrials);
                for (int i = 0; i < numberOfTrials; i++)
                {
                    if (!values.ContainsKey(value.Value))
                    {
                        values[value.Value] = new List<string>();
                    }
                    if (values[value.Value].Count() < samples[0])
                    {
                        values[value.Value].Add(value.Value);
                    }
                    else
                    {
                        values[value.Value].Add(string.Empty);
                    }
                }
            }
            List<string> result = GetCheckboxValuesJoined(values, numberOfTrials);
            ValidateCheckboxExamples(result, fieldValues);
            return result;
        }

        private static bool ValidateRadioExamples(List<string> examples, List<FormFieldValueDistribution> values)
        {
            double total = examples.Count;
            var percentages = examples.GroupBy(x => x).Select(x => new
            {
                Percentage = x.Count() / total,
                x,
                Count = x.Count()
            }).ToList();
            return false;
        }

        private static bool ValidateCheckboxExamples(List<string> examples, List<FormFieldValueDistribution> values)
        {
            double total = examples.Count;
            Dictionary<string, double> perventages = new Dictionary<string, double>();
            foreach (string value in values.Select(x => x.Value))
            {
                double numOfPositive = examples.Where(x => x.Contains(value)).Count();
                perventages.Add(value, numOfPositive / total);
            }
            return false;
        }

        private static bool ValidateNumericExamples(List<string> examples, double mean, double deviation)
        {
            double total = examples.Count;
            double total2deviations = examples.Select(x => Double.Parse(x)).Where(x => x > mean - (2 * deviation) && x < mean + 2 * deviation).Count();
            double percentage = total2deviations / total;
            return false;
        }

        private static List<FormInstance> GetDocumentsWithDependOnField(FormFieldDistribution targetField, List<FormInstance> instancesToFilter, FormFieldDistributionSingleParameter singleValue)
        {

            List<FormInstance> result = instancesToFilter;
            if (instancesToFilter != null && singleValue.DependOn != null)
            {
                foreach (SingleDependOnValue dependOnValue in singleValue.DependOn)
                {
                    result = FilterForDocumentsWithDependOnFields(targetField, result, dependOnValue);
                }
            }

            return result;
        }

        private static List<FormInstance> FilterForDocumentsWithDependOnFields(FormFieldDistribution targetField, List<FormInstance> instancesToFilter, SingleDependOnValue dependOnValue)
        {
            List<FormInstance> result = instancesToFilter;
            if (dependOnValue.Type == FieldTypes.Number)
            {
                result = FilterByNumericDependant(targetField, result, dependOnValue);
            }
            else
            {
                result = FilterBySelectableDependant(instancesToFilter, dependOnValue);
            }
            return result;
        }

        private static List<FormInstance> FilterByNumericDependant(FormFieldDistribution targetField, List<FormInstance> instancesToFilter, SingleDependOnValue dependOnValue)
        {
            List<FormInstance> result = instancesToFilter;
            RelatedVariable relatedVariable = targetField.GetRelatedVariableById(dependOnValue.Id);

            switch (dependOnValue.Value)
            {
                case "LTE":
                    result = result.Where(x => x.Fields.Any(y => y.Id.Equals(dependOnValue.Id) && float.Parse(y.Value?[0]) <= relatedVariable.LowerBoundary)).ToList();
                    break;
                case "BTW":
                    result = result.Where(x => x.Fields.Any(y => y.Id.Equals(dependOnValue.Id)
                    && float.Parse(y.Value?[0]) > relatedVariable.LowerBoundary
                    && float.Parse(y.Value?[0]) <= relatedVariable.UpperBoundary)).ToList();
                    break;
                case "GT":
                    result = result.Where(x => x.Fields.Any(y => y.Id.Equals(dependOnValue.Id) && float.Parse(y.Value?[0]) > relatedVariable.UpperBoundary)).ToList();
                    break;
            }

            return result;
        }

        private static List<FormInstance> FilterBySelectableDependant(List<FormInstance> instancesToFilter, SingleDependOnValue dependOnValue)
        {
            return instancesToFilter.Where(x => x.Fields.Any(y => y.Id.Equals(dependOnValue.Id) && y.Value != null && y.Value.Contains(dependOnValue.Value))).ToList();
        }

        private static void UpdateListWithDependables(List<FormInstance> listToUpdateGenerate, List<string> values, string fieldId, List<SingleDependOnValue> dependOn)
        {
            for (int i = 0; i < listToUpdateGenerate.Count; i++)
            {
                var withValues = listToUpdateGenerate[i].Fields.Where(y => dependOn.Select(x => x.Id).Contains(y.Id)).ToList();

                FieldValue field = listToUpdateGenerate[i].Fields.FirstOrDefault(x => x.Id.Equals(fieldId));

                field.Value = new List<string>() { values[i] };
            }
        }

        private static List<string> GetCheckboxValuesJoined(Dictionary<string, List<string>> values, int numberOfTrials)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < numberOfTrials; i++)
            {
                List<string> valueList = new List<string>();
                foreach (string key in values.Keys)
                {
                    valueList.Add(values[key][i]);
                }
                result.Add(string.Join(", ", valueList));
            }

            return result;
        }

        private static double[] GetMultinominalWeights(List<FormFieldValueDistribution> values)
        {
            if (values == null) throw new ArgumentNullException("Values are not defined");
            if (values.Any(x => x.SuccessProbability == null)) throw new ArgumentNullException("Success probability is not defined");

            return values.Select(x => Convert.ToDouble(x.SuccessProbability)).ToArray();
        }

        private static List<string> GenerateEmptyValues(int numOfRepeats)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < numOfRepeats; i++)
            {
                result.Add(string.Empty);
            }
            return result;
        }

        private static void RoundNumericFieldValues(Form form, List<FormInstance> formInstances)
        {
            foreach (FieldValue fieldValue in formInstances.SelectMany(x => x.Fields).Where(f => f.Type.Equals(FieldTypes.Number)))
            {
                FieldNumeric fieldDefinition = (FieldNumeric)form.GetFieldById(fieldValue.Id);
                double step = fieldDefinition.Step.HasValue ? fieldDefinition.Step.Value : 0.0001;
                int decimalsNumber = NumericHelper.GetDecimalsNumber(step);
                List<string> roundedValues = new List<string>();
                foreach (string value in fieldValue.Value)
                {
                    double numericValue;
                    if(Double.TryParse(value, out numericValue)) {
                        double numbericValueRounded = Math.Round(numericValue, decimalsNumber);
                        roundedValues.Add(numbericValueRounded.ToString());
                    }
                }
                fieldValue.Value = roundedValues;
            }

        }

        private static void SetValuesForCalculativeField(Form form, List<FormInstance> formInstances)
        {
            foreach (FormInstance formInstance in formInstances)
            {
                foreach (FieldValue calculativeField in formInstance.Fields.Where(f => f.Type.Equals(FieldTypes.Calculative)))
                {
                    try
                    {
                        string formula = CreateFormulaForCalculatedField(calculativeField.Id, formInstance, form);
                        object calculatedResult = new DataTable().Compute(formula, null);
                        double result = Math.Round(Convert.ToDouble(calculatedResult), 4);
                        calculativeField.Value = new List<string> { result.ToString() };
                    }
                    catch (Exception)
                    {
                        calculativeField.Value = new List<string> { "NaN" };
                    }
                }
            }
        }

        private static string CreateFormulaForCalculatedField(string fieldId, FormInstance formInstance, Form form)
        {
            FieldCalculative fieldDefinition = (FieldCalculative)form.GetFieldById(fieldId);
            string formula = fieldDefinition.Formula;
            foreach (KeyValuePair<string, string> entry in fieldDefinition.IdentifiersAndVariables)
            {
                string fieldValue = formInstance.GetFieldValueById(entry.Key);
                string variable = entry.Value;
                formula = formula.Replace(string.Concat("[", variable, "]"), fieldValue);
            }
            return formula;
        }




        /*HACKATON MODEL*/

        private static List<int> GenerateDailyFormNumExamples(int numOfExamples)
        {
            Multinomial multinomial = new Multinomial(GetDailyFormWeights(), numOfExamples);
            var samples = multinomial.Samples().Take(1).ToList();
            List<int> result = new List<int>();
            for (int i = 0; i < samples[0].Count(); i++)
            {
                if (samples[0][i] > 0)
                {
                    for (int j = 0; j < samples[0][i]; j++)
                    {
                        result.Add(i+1);
                    }
                }
            }

            return result;
        }


        //weight for each day in 14 days period
        private static double[] GetDailyFormWeights()
        {
            return new double[] { 0, 0.01,0.01,0.01, 0.02, 0.03, 0.03, 0.08, 0.11, 0.17, 0.20, 0.18, 0.10,0.05 };
        }
    }
}
