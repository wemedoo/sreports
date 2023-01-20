﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldCheckboxDataOut : FieldSelectableDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldCheckbox.cshtml";
        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableCheckBoxField.cshtml";

        public override string GetValue()
        {
            return HasValue() ? base.GetValue() : null;
        }
    }
}