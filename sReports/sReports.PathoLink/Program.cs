using Newtonsoft.Json;
using sReports.PathoLink.Entities;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReports.PathoLink
{
    class Program
    {
        public static List<Field> formFields = new List<Field>();
        static void Main(string[] args)
        {
            /*  Entities.PathoLink pathoLink = JsonConvert.DeserializeObject<Entities.PathoLink>(File.ReadAllText(@"D:\sReports\sReports\sReports.PathoLink\bin\Debug\DataRequest_With_Data.json"));
              List<string> processed = new List<string>();
              processData(processed, pathoLink.Result);*/
        }

        /*static void processData(List<string> processed, List<PathoLinkField> fields)
      {

           foreach (PathoLinkField field in fields)
           {
               if (!processed.Contains(field.id))
               {
                   HandleField(processed, fields, field);
                   Console.WriteLine();
                   Console.WriteLine();
               }
           }
           string json = JsonConvert.SerializeObject(formFields);

           //write string to file
           File.WriteAllText(@"D:\fields.json", json);
       }

       static void HandleField(List<string> processed, List<PathoLinkField> fields, PathoLinkField field,string indent ="", string parent = "", FormField parentFormField = null)
       {
           if (processed.Contains(field.id)) return;


           var parentAndLevel = GetParentNameAndLevel(field.name);
           var siblings = GetSiblings(fields, parentAndLevel);
           bool isDistinctType = IsOnlyOneType(siblings);
           string mainType = GetMainType(siblings);

           if (string.IsNullOrWhiteSpace(parent))
           {
               Console.WriteLine($"-Same field types: {isDistinctType}, Main field type: {mainType}");
               Console.WriteLine($"-{parentAndLevel.Item1}");
           }

           if (mainType == Constants.Radio || mainType == Constants.Checkbox)
           {
               FormField formField = GetFormField(GetFieldName(parentAndLevel.Item1), GetInputType(field.type));
               formFields.Add(formField);

               for (int i = 0; i < siblings.Count(); i++)
               {
                   if (processed.Contains(siblings[i].id)) continue;

                   PathoLinkField current = siblings[i];
                   FormFieldValue value = GetFormFieldValue(current);
                   formField.Values.Add(value);
                   Console.WriteLine($"     {indent}-{GetFieldName(current.name)}");

                   if (i + 1 < siblings.Count())
                   {
                       var next = siblings[i + 1];
                       if (!next.type.Equals(mainType) && next.type == Constants.Input || next.type == Constants.Textarea)
                       {
                           FormField fieldDependable = GetFormField(GetFieldName(next.name), GetInputType(next.type), false);
                           InsertDependable(formField, fieldDependable.Id, value.Value);
                           formFields.Add(fieldDependable);

                           Console.WriteLine($"          {indent}-Descendant(diff type): {GetFieldName(next.name)};type: {next.type} ");
                           processed.Add(next.id);
                       }
                   }
                   //descendants
                   GetDescendants(fields, processed, current.name, parentAndLevel.Item2, indent, formField);
                   processed.Add(current.id);
               }
               InsertDependable(parentFormField, formField.Id, GetFieldName(parent));

           }
           else if (mainType == Constants.Input || mainType == Constants.Textarea)
           {
               HandleNonSelectableType(siblings, processed, parentFormField, parent, indent);
           }
       }

       static void HandleNonSelectableType(List<PathoLinkField> siblings, List<string> processed, FormField parentFormField, string parent, string indent)
       {
           foreach (PathoLinkField f in siblings.Where(x => !processed.Contains(x.id)))
           {
               FormField formField = new FormField()
               {
                   Label = GetFieldName(f.name),
                   Id = Guid.NewGuid().ToString(),
                   Dependables = new List<FormFieldDependable>(),
                   Values = new List<FormFieldValue>(),
                   Type = GetInputType(f.type),
                   IsVisible = parentFormField == null
               };
               InsertDependable(parentFormField, formField.Id, GetFieldName(parent));
               formFields.Add(formField);
               Console.WriteLine($"     {indent}-{GetFieldName(f.name)} - (parent){parent}");
               processed.Add(f.id);
           }
       }

       static void GetDescendants(List<PathoLinkField> fields, List<string> processed, string parent, int level, string indent,FormField parentFormField)
       {
           var descendants = fields.Where(x => !processed.Contains(x.id) && x.name.Contains(parent) && x.name.Split('[').Length == level + 1).ToList();
           foreach (PathoLinkField field in descendants)
           {
               HandleField(processed, fields, field, $"{indent}     ",parent, parentFormField);
           }
       }

       static string GetMainType(List<PathoLinkField> fields)
       {
           return fields.GroupBy(x => x.type)
               .Select(x => new { x.Key, count = x.Count() })
               .OrderByDescending(x => x.count)
               .FirstOrDefault()?.Key;
       }

       static bool IsOnlyOneType(List<PathoLinkField> fields)
       {
           return fields.Select(x => x.type).Distinct().Count() == 1;
       }

       static Tuple<string, int> GetParentNameAndLevel(string child)
       {
           string[] splitted = child.Split('[').ToArray();
           List<string> cloned = new List<string>(splitted.Clone() as string[]);
           cloned.RemoveAt(cloned.Count() - 1);
           return Tuple.Create(String.Join("[", cloned), splitted.Length);
       }

       static List<PathoLinkField> GetSiblings(List<PathoLinkField> fields, dynamic parentAndLevel)
       {
           return fields.Where(x => x.name.StartsWith(parentAndLevel.Item1) && x.name.Split('[').Length == parentAndLevel.Item2).ToList();
       }

       static string GetFieldName(string field)
       {
           string[] splitted = field.Split('[');
           return $"{splitted[splitted.Length - 1].Replace("]", "")}";
       }

       static FormField InstantiateFormField(string name)
       {
           return new FormField()
           {
               Label = name,
               Id = Guid.NewGuid().ToString(),
               Dependables = new List<FormFieldDependable>(),
               Values = new List<FormFieldValue>()
           };
       }

       static FormFieldValue GetFormFieldValue(PathoLinkField pathoLinkField)
       {
          return new FormFieldValue
           {
               Value = GetFieldName(pathoLinkField.name),
               Label = GetFieldName(pathoLinkField.name),
               ThesaurusId = pathoLinkField.id
           };
       }

       static string GetInputType(string type)
       {
           string result;
           switch (type)
           {
               case Constants.Input:
                   result = FieldTypes.Text;
                   break;
               case Constants.Checkbox:
                   result = FieldTypes.Checkbox;
                   break;
               case Constants.Radio:
                   result = FieldTypes.Radio;
                   break;
               case Constants.Textarea:
                   result = FieldTypes.LongText;
                   break;
               default:
                   result = FieldTypes.Text;
                   break;
           }

           return result;
       }

       static void InsertDependable(FormField parentFormField, string actionParams, string condition)
       {
           if (parentFormField != null)
           {
               FormFieldDependable dependable = new FormFieldDependable
               {
                   ActionParams = actionParams,
                   Condition = condition,
                   ActionType = FormFieldDependableType.Toggle
               };
               parentFormField.Dependables.Add(dependable);
           }
       }

       static FormField GetFormField(string label, string type, bool isVisible = true)
       {
           return new FormField()
           {
               Label = label,
               Id = Guid.NewGuid().ToString(),
               Dependables = new List<FormFieldDependable>(),
               Values = new List<FormFieldValue>(),
               Type = type,
               IsVisible = isVisible
           };*/
    }
}

