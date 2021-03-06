﻿using System;
using System.Collections.Generic;
using Multiconsult_V001.Classes;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;

namespace Multiconsult_V001.Components
{
    public class MR_Column_Brep : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MR_Column_Brep class.
        /// </summary>
        public MR_Column_Brep()
          : base("ColumnFromBrep", "COB",
              "Coulmn from brep",
              "Multiconsult", "Revit")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "BC", "Brep which have to be change to column", GH_ParamAccess.item);
            pManager.AddTextParameter("RevitMaterials", "RM", "Revit element materials", GH_ParamAccess.item, "Revit Material : Betong - B35");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("MultiColumn", "MC", "Mulitconsult column object", GH_ParamAccess.item);
            pManager.AddTextParameter("Informations", "I", "Informations about transition", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //inputs
            Brep brep = new Brep();
            string type = "";

            DA.GetData(0, ref brep);
            DA.GetData(1, ref type);

            //parameters
            List<string> infos = new List<string>();

            //create Multiconsut column
            infos.Add(" Create Mulitconsult column class ");
            var cpts = Methods.Geometry.findBottomAndTopCenterPoints(brep);
            //create Multiconsult columne object
            Column c = new Column(new Line(cpts[0],cpts[1]) );
            
            //assign section to column
            c.section = Methods.Geometry.findColumnSection(brep);
            c.name = "column FromBrep";
            c.pt_end = cpts[1];
            c.pt_st = cpts[0];
            
            //get materials from revit string
            string[] RevitMats = type.Split(':');
            Material m = new Material();
            m.RevitMaterialName = RevitMats[1];
            string[] matName = type.Split('-');
            m.name = matName[1].Trim();

            //assign material to column
            c.material = m;

            //validate the information about analysis
            infos.Add("Revit Material= " + m.RevitMaterialName);
            infos.Add("Section = " + c.section.name);

            //outputs
            DA.SetData(0, c);
            DA.SetDataList(1, infos);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a30ab3bd-46b1-4cc6-843e-87ce79ab2519"); }
        }
    }
}