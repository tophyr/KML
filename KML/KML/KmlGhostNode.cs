﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KML
{
    /// <summary>
    /// A KmlGhostNode ist a node that is just for visual or structural capsulation.
    /// It is not written to data file, just unpacked flat. So the content will be written
    /// as if it was content of KmlGhostNode.Parent.
    /// </summary>
    public class KmlGhostNode : KmlNode
    {
        /// <summary>
        /// Creates a KmlGhostNode with a given Name as a child of given parent node.
        /// </summary>
        /// <param name="tag">Tag of the ghost node, will be visible but not written</param>
        public KmlGhostNode(string tag)
            : base(tag)
        {
        }
    }
}
