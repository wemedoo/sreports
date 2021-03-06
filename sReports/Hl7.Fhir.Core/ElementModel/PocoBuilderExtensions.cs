/* 
 * Copyright (c) 2018, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://github.com/FirelyTeam/fhir-net-api/blob/master/LICENSE
 */


using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hl7.Fhir.ElementModel
{
    public static class PocoBuilderExtensions
    {
        public static Base ToPoco(this ISourceNode source, Type pocoType = null, PocoBuilderSettings settings = null) =>
            new PocoBuilder(settings).BuildFrom(source, pocoType);

        public static T ToPoco<T>(this ISourceNode source, PocoBuilderSettings settings = null) where T : Base =>
               (T)source.ToPoco(typeof(T), settings);

        public static Base ToPoco(this ITypedElement element, PocoBuilderSettings settings = null) =>
            new PocoBuilder(settings).BuildFrom(element);

        public static T ToPoco<T>(this ITypedElement element, PocoBuilderSettings settings = null) where T : Base =>
               (T)element.ToPoco(settings);

#pragma warning disable 612, 618
        public static Base ToPoco(this IElementNavigator navigator, Type pocoType = null,
            PocoBuilderSettings settings = null) => navigator.ToSourceNode().ToPoco(pocoType, settings);

        public static T ToPoco<T>(this IElementNavigator navigator, PocoBuilderSettings settings = null) where T : Base =>
               (T)navigator.ToPoco(typeof(T), settings);
#pragma warning restore 612, 618


        public static Model.Quantity ParseQuantity(this ITypedElement instance)
        {
            var newQuantity = new Quantity
            {
                Value = instance.Children("value").SingleOrDefault()?.Value as decimal?,
                Unit = instance.Children("unit").GetString(),
                System = instance.Children("system").GetString(),
                Code = instance.Children("code").GetString()
            };

            var comp = instance.Children("comparator").GetString();
            if (comp != null)
                newQuantity.ComparatorElement = new Code<Quantity.QuantityComparator> { ObjectValue = comp };

            return newQuantity;
        }

        /// <summary>
        /// Parses a bindeable type (code, Coding, CodeableConcept, Quantity, string, uri) into a FHIR coded datatype.
        /// Extensions will be parsed from the 'value' of the (simple) extension.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>An Element of a coded type (code, Coding or CodeableConcept) dependin on the instance type,
        /// or null if no bindable instance data was found</returns>
        /// <remarks>The instance type is mapped to a codable type as follows:
        ///   'code' => code
        ///   'Coding' => Coding
        ///   'CodeableConcept' => CodeableConcept
        ///   'Quantity' => Coding
        ///   'Extension' => depends on value[x]
        ///   'string' => code
        ///   'uri' => code
        /// </remarks>
        public static Element ParseBindable(this ITypedElement instance)
        {
            var instanceType = ModelInfo.FhirTypeNameToFhirType(instance.InstanceType);

            switch (instanceType)
            {
                case FHIRAllTypes.Code:
                    return instance.ParsePrimitive<Code>();
                case FHIRAllTypes.Coding:
                    return instance.ParseCoding();
                case FHIRAllTypes.CodeableConcept:
                    return instance.ParseCodeableConcept();
                case FHIRAllTypes.Quantity:
                    return parseQuantity(instance);
                case FHIRAllTypes.String:
                    return new Code(instance.ParsePrimitive<FhirString>()?.Value);
                case FHIRAllTypes.Uri:
                    return new Code(instance.ParsePrimitive<FhirUri>()?.Value);
                case FHIRAllTypes.Extension:
                    return parseExtension(instance);
                default:
                    return null;
            }

            Coding parseQuantity(ITypedElement nav)
            {
                var newCoding = new Coding();
                var q = instance.ParseQuantity();
                newCoding.Code = q.Code;
                newCoding.System = q.System ?? "http://unitsofmeasure.org";
                return newCoding;
            }

            Element parseExtension(ITypedElement nav)
            {
                var valueChild = instance.Children("value").FirstOrDefault();
                return valueChild?.ParseBindable();
            }
        }

        public static T ParsePrimitive<T>(this ITypedElement instance) where T : Primitive, new()
                    => new T() { ObjectValue = instance.Value };


        public static Coding ParseCoding(this ITypedElement instance)
        {
            return new Coding()
            {
                System = instance.Children("system").GetString(),
                Version = instance.Children("version").GetString(),
                Code = instance.Children("code").GetString(),
                Display = instance.Children("display").GetString(),
                UserSelected = instance.Children("userSelected").SingleOrDefault()?.Value as bool?
            };
        }

        public static ResourceReference ParseResourceReference(this ITypedElement instance)
        {
            return new ResourceReference()
            {
                Reference = instance.Children("reference").GetString(),
                Display = instance.Children("display").GetString()
            };
        }

        public static CodeableConcept ParseCodeableConcept(this ITypedElement instance)
        {
            return new CodeableConcept()
            {
                Coding =
                    instance.Children("coding").Select(codingNav => codingNav.ParseCoding()).ToList(),
                Text = instance.Children("text").GetString()
            };
        }

        public static string GetString(this IEnumerable<ITypedElement> instance) => instance.SingleOrDefault()?.Value as string;

        [Obsolete("Use ParseCodeableConcept(this ITypedElement instance) instead")]
        public static CodeableConcept ParseCodeableConcept(this IElementNavigator instance)
            => ParseCodeableConcept(instance.ToTypedElement());


        [Obsolete("Use GetString(this IEnumerable<ITypedElement> instance) instead")]
        public static string GetString(this IEnumerable<IElementNavigator> instance)
            => instance.SingleOrDefault()?.Value as string;


        [Obsolete("Use ParseQuantity(this ITypedElement instance) instead")]
        public static Model.Quantity ParseQuantity(this IElementNavigator instance) => ParseQuantity(instance.ToTypedElement());

        [Obsolete("Use ParseBindable(this ITypedElement instance) instead")]
        public static Element ParseBindable(this IElementNavigator instance) => ParseBindable(instance.ToTypedElement());


        [Obsolete("Use ParsePrimitive<T>(this ITypedElement instance) instead")]
        public static T ParsePrimitive<T>(this IElementNavigator instance) where T : Primitive, new()
            => ParsePrimitive<T>(instance.ToTypedElement());

        [Obsolete("Use ParseCoding(this ITypedElement instance) instead")]
        public static Coding ParseCoding(this IElementNavigator instance)
            => ParseCoding(instance.ToTypedElement());

        [Obsolete("Use ParseResourceReference(this ITypedElement instance) instead")]
        public static ResourceReference ParseResourceReference(this IElementNavigator instance)
             => ParseResourceReference(instance.ToTypedElement());

    }
}
