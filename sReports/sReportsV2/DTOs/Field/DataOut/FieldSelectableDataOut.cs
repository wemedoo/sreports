﻿using sReportsV2.Domain.Extensions;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldSelectableDataOut : FieldDataOut
    {
        public List<FormFieldValueDataOut> Values { get; set; } = new List<FormFieldValueDataOut>();
        public List<FormFieldDependableDataOut> Dependables { get; set; } = new List<FormFieldDependableDataOut>();

        public void GetDependablesData(List<FieldDataOut> fields, List<FormFieldDependableDataOut> dependables)
        {
            dependables = Ensure.IsNotNull(dependables, nameof(dependables));

            foreach (FormFieldDependableDataOut formFieldDependable in dependables)
            {
                FieldDataOut dependableField = fields.FirstOrDefault(x => x.Id.Equals(formFieldDependable.ActionParams));
                if (dependableField != null)
                {
                    if(dependableField is FieldSelectableDataOut)
                    {
                        LoadMoreDependables(fields, dependableField as FieldSelectableDataOut, formFieldDependable);
                    }
                }
            }
        }
        
        protected void LoadMoreDependables(List<FieldDataOut> fields, FieldSelectableDataOut field, FormFieldDependableDataOut formFieldDependable)
        {

            field = Ensure.IsNotNull(field, nameof(field));
            if (field.Dependables != null && field.Dependables.Any())
            {
                GetDependablesData(fields, field.Dependables);
                formFieldDependable.Dependables.AddRange(field.Dependables);
            }
        }
    }
}