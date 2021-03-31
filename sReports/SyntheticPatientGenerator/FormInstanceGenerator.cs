using MathNet.Numerics.Distributions;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyntheticPatientGenerator
{
    public static class FormInstanceGenerator
    {
        public static List<FormInstance> Generate(Form form, FormDistribution formDistribution, int numOfDocuments)
        {
            Dictionary<string, List<string>> generatedFields = new Dictionary<string, List<string>>();
            foreach(FormFieldDistribution field in formDistribution.Fields)
            {
                switch (field.Type)
                {
                    case FieldTypes.Radio:
                    case FieldTypes.Select:
                        generatedFields.Add(field.ThesaurusId, GenerateRadioExamples(field, numOfDocuments));
                        break;
                    case FieldTypes.Number:
                        generatedFields.Add(field.ThesaurusId, GenerateNumberExamples(field, numOfDocuments));
                        break;

                    case FieldTypes.Checkbox:
                        generatedFields.Add(field.ThesaurusId, GenerateCheckboxExamples(field, numOfDocuments));
                        break;
                }
            }

            return GenerateFormInstances(form, generatedFields, numOfDocuments);
        }

        private static List<FormInstance> GenerateFormInstances(Form form, Dictionary<string, List<string>> generatedFields, int numOfDocuments)
        {
            List<FormInstance> result = new List<FormInstance>();
            for(int i = 0; i < numOfDocuments; i++)
            {
                FormInstance formInstance = new FormInstance(form);
                foreach(string key in generatedFields.Keys)
                {
                    string currentValue = generatedFields[key][i];
                    FormField field = formInstance.GetAllFields().FirstOrDefault(x => x.ThesaurusId.Equals(key));
                    if(field != null)
                    {
                        field.Value = currentValue;
                    }
                }
                result.Add(formInstance);
            }
            return result;
        }

        private static List<string> GenerateRadioExamples(FormFieldDistribution field, int numberOfTrials)
        {
            List<string> result = new List<string>();
            Multinomial multinomial = new Multinomial(GetMultinominalWeights(field.Values), numberOfTrials);
            var samples = multinomial.Samples().Take(1);
            for (int i = 0; i < samples.ToArray()[1].Count(); i++)
            {
                string value = field.Values[i].Value;
                for(int j = 0; j < samples.ToArray()[1][i]; j++)
                {
                    result.Add(value);
                }
            }

            return result;
        }

        private static List<string> GenerateNumberExamples(FormFieldDistribution field, int numberOfTrials)
        {
            double mean = field.Mean ?? throw new ArgumentNullException("Mean must be defined");
            double standardDeviation = field.StandardDeviation ?? throw new ArgumentNullException("Standard deviation must be defined");

            var samples = new double[numberOfTrials];
            Normal.Samples(samples, mean, standardDeviation);            
            return samples.Select(x => x.ToString()).ToList();

        }

        private static List<string> GenerateCheckboxExamples(FormFieldDistribution field, int numberOfTrials)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            foreach (FormFieldValueDistribution value in field.Values)
            {
                if (value.SuccessProbability == null)
                    throw new ArgumentNullException("Value can not be null");

                var samples = new int[1];
                Binomial.Samples(samples, 0.9, numberOfTrials);
                for (int i = 0; i < numberOfTrials; i++)
                {
                    if (!values.ContainsKey(value.Value))
                    {
                        values[value.Value] = new List<string>();
                    }
                    if (values[value.Value].Count() < samples[1])
                    {
                        values[value.Value].Add(value.Value);
                    }
                    else
                    {
                        values[value.Value].Add(string.Empty);
                    }
                }
            }
            return GetCheckboxValuesJoined(values, numberOfTrials);
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
                result.Add(string.Join(" ", valueList));
            }

            return result;
        }

        private static double[] GetMultinominalWeights(List<FormFieldValueDistribution> values)
        {
            if (values == null) throw new ArgumentNullException("Values are not defined");
            if (values.Any(x => x.SuccessProbability == null)) throw new ArgumentNullException("Success probability is not defined");

            return values.Select(x => Convert.ToDouble(x.SuccessProbability)).ToArray();
        }

    }
}
