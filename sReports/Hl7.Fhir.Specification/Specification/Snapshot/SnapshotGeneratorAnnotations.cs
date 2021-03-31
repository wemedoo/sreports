﻿/* 
 * Copyright (c) 2017, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/FirelyTeam/fhir-net-api/master/LICENSE
 */

using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hl7.Fhir.Specification.Snapshot
{
    /// <summary>Provides support for custom annotation types used by the <see cref="SnapshotGenerator"/>.</summary>
    public static class SnapshotGeneratorAnnotations
    {
        #region Annotation: Created By Snapshot Generator

        /// <summary>Annotation to mark a generated element, so we can prevent duplicate re-generation.</summary>
#if !NETSTANDARD1_1
        [Serializable]
#endif
        sealed class CreatedBySnapshotGeneratorAnnotation
        {
            public DateTimeOffset Created { get; }
            public CreatedBySnapshotGeneratorAnnotation() { Created = DateTimeOffset.UtcNow; }
        }

        /// <summary>Marks the specified element as generated by the <see cref="SnapshotGenerator"/>.</summary>
        internal static void SetCreatedBySnapshotGenerator(this Element elem) { elem?.AddAnnotation(new CreatedBySnapshotGeneratorAnnotation()); }

        /// <summary>Determines if the specified element was created by the <see cref="SnapshotGenerator"/>.</summary>
        /// <param name="elem">A FHIR <see cref="Element"/>.</param>
        /// <returns><c>true</c> if the element was created by the <see cref="SnapshotGenerator"/>, or <c>false</c> otherwise.</returns>
        public static bool IsCreatedBySnapshotGenerator(this Element elem) => elem != null && elem.HasAnnotation<CreatedBySnapshotGeneratorAnnotation>();

        #endregion

        #region Annotation: Differential Constraint

        /// <summary>
        /// Custom annotation for elements and properties in the <see cref="StructureDefinition.SnapshotComponent"/>
        /// that are constrained by the <see cref="StructureDefinition.DifferentialComponent"/>.
        /// </summary>
#if !NETSTANDARD1_1
        [Serializable]
#endif
        sealed class ConstrainedByDiffAnnotation
        {
            //
        }

        /// <summary>Annotate the specified snapshot element to indicate that it is constrained by the differential.</summary>
        internal static void SetConstrainedByDiffAnnotation(this Base element)
        {
            if (element == null) { throw Error.ArgumentNull(nameof(element)); }
            element.AddAnnotation(new ConstrainedByDiffAnnotation());
        }

        /// <summary>Remove any existing differential constraint annotation from the specified snapshot element.</summary>
        internal static void RemoveConstrainedByDiffAnnotation(this Base element)
        {
            if (element == null) { throw Error.ArgumentNull(nameof(element)); }
            element.RemoveAnnotations<ConstrainedByDiffAnnotation>();
        }

        /// <summary>Recursively remove any existing differential constraint annotations from the specified snapshot element and all it's children.</summary>
        internal static void RemoveAllConstrainedByDiffAnnotations(this Base element)
        {
            if (element == null) { throw Error.ArgumentNull(nameof(element)); }
            element.RemoveConstrainedByDiffAnnotation();
            foreach (var child in element.Children)
            {
                child.RemoveAllConstrainedByDiffAnnotations();
            }
        }

        /// <summary>Recursively remove any existing differential constraint annotations from the specified snapshot elements and all their children.</summary>
        internal static void RemoveAllConstrainedByDiffAnnotations<T>(this IEnumerable<T> elements) where T : Base
        {
            if (elements == null) { throw Error.ArgumentNull(nameof(elements)); }
            foreach (var elem in elements)
            {
                elem.RemoveAllConstrainedByDiffAnnotations();
            }
        }

        /// <summary>
        /// Determines if the specified element is annotated as being constrained by the differential.
        /// Note that this method is non-recursive; only the specified element itself is inspected, child element annotations are ignored.
        /// Use <seealso cref="HasDiffConstraintAnnotations"/> to perform a recursive check.
        /// </summary>
        public static bool IsConstrainedByDiff(this Base elem) => elem != null && elem.HasAnnotation<ConstrainedByDiffAnnotation>();

        /// <summary>Determines if the specified element or any of it's children is annotated as being constrained by the differential.</summary>
        public static bool HasDiffConstraintAnnotations(this Base elem)
            => elem != null && (
                elem.HasAnnotation<ConstrainedByDiffAnnotation>()
                || elem.Children.Any(e => e.HasDiffConstraintAnnotations())
            );

        #endregion


        #region Annotation: Snapshot ElementDefinition

        /// <summary>For annotating a differential element definition with a reference to the associated generated snapshot element definition.</summary>
#if !NETSTANDARD1_1
        [Serializable]
#endif
        sealed class SnapshotElementDefinitionAnnotation
        {
            /// <summary>
            /// Custom annotation type for <see cref="ElementDefinition"/> instances in the <see cref="StructureDefinition.Differential"/> component.
            /// Returns a reference to the associated <see cref="ElementDefinition"/> instance in the <see cref="StructureDefinition.Snapshot"/> component.
            /// </summary>
            public ElementDefinition SnapshotElement { get; }
            public SnapshotElementDefinitionAnnotation(ElementDefinition snapshotElement)
            {
                if (snapshotElement == null) { throw Error.ArgumentNull(nameof(snapshotElement)); }
                SnapshotElement = snapshotElement;
            }
        }

        /// <summary>
        /// Annotate the root <see cref="ElementDefinition"/> instance in the <see cref="StructureDefinition.Differential"/> component
        /// with a reference to the associated root <see cref="ElementDefinition"/> instance in the <see cref="StructureDefinition.Snapshot"/> component.
        /// </summary>
        internal static void SetSnapshotRootElementAnnotation(this StructureDefinition sd, ElementDefinition rootElemDef)
        {
            sd?.Differential?.Element[0]?.SetSnapshotElementAnnotation(rootElemDef);
        }

        /// <summary>
        /// Annotate the specified <see cref="ElementDefinition"/> instance in the <see cref="StructureDefinition.Differential"/> component
        /// with a reference to the associated <see cref="ElementDefinition"/> instance in the <see cref="StructureDefinition.Snapshot"/> component.
        /// </summary>
        /// <param name="diffElemDef"></param>
        /// <param name="snapElemDef"></param>
        internal static void SetSnapshotElementAnnotation(this ElementDefinition diffElemDef, ElementDefinition snapElemDef)
        {
            diffElemDef?.AddAnnotation(new SnapshotElementDefinitionAnnotation(snapElemDef));
        }

        /// <summary>
        /// Return the annotated reference to the associated root <see cref="ElementDefinition"/> instance
        /// in the <see cref="StructureDefinition.Snapshot"/> component, if it exists, or <c>null</c> otherwise.
        /// </summary>
        internal static ElementDefinition GetSnapshotRootElementAnnotation(this StructureDefinition sd) => sd?.Differential?.Element[0]?.GetSnapshotElementAnnotation();

        /// <summary>
        /// Return the annotated reference to the associated <see cref="ElementDefinition"/> instance
        /// in the <see cref="StructureDefinition.Snapshot"/> component, if it exists, or <c>null</c> otherwise.
        /// </summary>
        internal static ElementDefinition GetSnapshotElementAnnotation(this ElementDefinition ed) => ed?.Annotation<SnapshotElementDefinitionAnnotation>()?.SnapshotElement;

        /// <summary>Remove all <see cref="SnapshotElementDefinitionAnnotation"/> instances from the specified <see cref="ElementDefinition"/>.</summary>
        internal static void RemoveSnapshotElementAnnotations(this ElementDefinition ed) { ed?.RemoveAnnotations<SnapshotElementDefinitionAnnotation>(); }

        #endregion

    }
}
