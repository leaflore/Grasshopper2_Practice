using System;
using Grasshopper2.Components;
using Grasshopper2.UI;
using GrasshopperIO;
using Rhino.Geometry;

using Rhino.PlugIns;
using Grasshopper2;
using Grasshopper2.UI.Icon;

namespace Grasshopper2_PlugIn
{
    [IoId("80dce277-379d-4041-bf94-8aa2a3a287eb")]
    public sealed class CurveBlend_Component : Component
    {
        public CurveBlend_Component() : base(new Nomen(
            "CurveBlend_Component",
            "Blend Two Curves",
            "MGH2",
            "Curve"))
        {
        }

        public CurveBlend_Component(IReader reader) : base(reader) { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void AddInputs(InputAdder inputs)
        {
            // Set ensures a default value
            inputs.AddPlane("Base Plane", "Bp", "Base plane for spiral").Set(Plane.WorldXY);
            inputs.AddNumber("Inner Radius", "R0", "Inner radius for spiral").Set(1.0);
            inputs.AddNumber("Outer Radius", "R1", "Outer radius for spiral").Set(10.0);
            inputs.AddInteger("Turn Count", "Tc", "Number of turns between radii").Set(10);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void AddOutputs(OutputAdder outputs)
        {
            outputs.AddCurve("Spiral Curve", "S", "Generated spiral curve");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void Process(IDataAccess access)
        {
            // First, we need to retrieve all data from the input parameters.
            // Unlike GH1, there is no need to check the output result, if data isn't fed in, the component won't run.
            access.GetItem(0, out Plane plane);
            access.GetItem(1, out double radius0);
            access.GetItem(2, out double radius1);
            access.GetItem(3, out int turns);

            // Second, we verify the data to check it against given conditions
            // If verification fails, the component will not run
            access.Verify(plane, (p) => p.IsValid, "Plane is Invalid", "Planes that are not valid cannot be used");

            // Third, we rectify data to force it to match given conditions, rectify will attempt to fix the data
            // If rectify fails, the component will not run
            access.RectifyPositive(ref radius0, "first radius");
            access.RectifyDomain(ref radius1, (radius0, double.MaxValue), "outer radius");
            access.RectifyPositive(ref turns, "Turns");

            // We're set to create the spiral now. To keep the size of the SolveInstance() method small, 
            // The actual functionality will be in a different method:
            Curve spiral = CreateSpiral(plane, radius0, radius1, turns);

            // Finally assign the spiral to the output parameter.
            access.SetItem(0, spiral);
        }

        private Curve CreateSpiral(Plane plane, double r0, double r1, int turns)
        {
            Line l0 = new Line(plane.Origin + r0 * plane.XAxis, plane.Origin + r1 * plane.XAxis);
            Line l1 = new Line(plane.Origin - r0 * plane.XAxis, plane.Origin - r1 * plane.XAxis);

            Point3d[] p0;
            Point3d[] p1;

            l0.ToNurbsCurve().DivideByCount(turns, true, out p0);
            l1.ToNurbsCurve().DivideByCount(turns, true, out p1);

            PolyCurve spiral = new PolyCurve();

            for (int i = 0; i < p0.Length - 1; i++)
            {
                Arc arc0 = new Arc(p0[i], plane.YAxis, p1[i + 1]);
                Arc arc1 = new Arc(p1[i + 1], -plane.YAxis, p0[i + 1]);

                spiral.Append(arc0);
                spiral.Append(arc1);
            }

            return spiral;
        }

        protected override IIcon IconInternal
        {
            get
            {
                return AbstractIcon.FromResource("Grasshopper2_PlugIn.Icons.Grasshopper2_FirstComponent.ghicon", GetType());
            }
        }
    }
}
