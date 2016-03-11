﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KML
{
    /// <summary>
    /// KmlPart represents a KmlNode with the "PART" tag.
    /// </summary>
    class KmlPart : KmlNode
    {
        /// <summary>
        /// Possible origins where this node is found.
        /// Regular PART nodes are children of a "VESSEL" node.
        /// </summary>
        public enum PartOrigin { Vessel, Other };

        /// <summary>
        /// Get the origin of this node.
        /// <see cref="KML.KmlPart.PartOrigin"/>
        /// </summary>
        public PartOrigin Origin { get; private set; }

        /// <summary>
        /// Get a list of all KmlResource children.
        /// </summary>
        public List<KmlResource> Resources { get; private set; }

        /// <summary>
        /// Get a set of all types (names) of resources in this part.
        /// </summary>
        public SortedSet<string> ResourceTypes { get; private set; }

        /// <summary>
        /// Get the uid this part is identified by.
        /// </summary>
        public string Uid { get; private set; }

        /// <summary>
        /// Get x, y, z coordinates of this part relative to the vessel
        /// </summary>
        public Point3D Position { get; private set; }

        /// <summary>
        /// Get the parent part index from vessel structure.
        /// This data is read from KML attributes within this part.
        /// </summary>
        public int ParentPartIndex { get; private set; }

        /// <summary>
        /// Get the parent part from vessel structure.
        /// This data is identified after vessel reading is completed.
        /// </summary>
        public KmlPart ParentPart { get; private set; }

        // TODO KmlPart: Not make lists public, better have a Add(method)

        /// <summary>
        /// Get a list of part indexes this part is node attached to.
        /// This data is read from KML attributes within this part.
        /// </summary>
        public List<int> AttachedToNodeIndices { get; private set; }

        /// <summary>
        /// Get a part index this part is surface attached to.
        /// This data is read from KML attributes within this part.
        /// </summary>
        public int AttachedToSurfaceIndex { get; private set; }

        /// <summary>
        /// Get a list of KmlParts this part is node attached to in top direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading this part alone.
        /// </summary>
        public List<KmlPart> AttachedToPartsTop { get; private set; }

        /// <summary>
        /// Get a list of KmlParts this part is node attached to in bottom direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading this part alone.
        /// </summary>
        public List<KmlPart> AttachedToPartsBottom { get; private set; }

        /// <summary>
        /// Get a list of KmlParts this part is node attached to in front direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading this part alone.
        /// </summary>
        public List<KmlPart> AttachedToPartsFront { get; private set; }

        /// <summary>
        /// Get a list of KmlParts this part is node attached to in back direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading this part alone.
        /// </summary>
        public List<KmlPart> AttachedToPartsBack { get; private set; }

        /// <summary>
        /// Get a list of KmlParts this part is node attached to in left direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading this part alone.
        /// </summary>
        public List<KmlPart> AttachedToPartsLeft { get; private set; }

        /// <summary>
        /// Get a list of KmlParts this part is node attached to in right direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading this part alone.
        /// </summary>
        public List<KmlPart> AttachedToPartsRight { get; private set; }

        /// <summary>
        /// Get the KmlPart this node is surface atteached to. In a correct persistence file this should only be one.
        /// The part will be assigned by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the AttachedToSurfaceIndex that is filled on reading this part alone.
        /// </summary>
        public KmlPart AttachedToPartSurface { get; private set; }

        /// <summary>
        /// Get a list of KmlParts node attached to this part in top direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading the other parts.
        /// </summary>
        public List<KmlPart> AttachedPartsTop { get; private set; }

        /// <summary>
        /// Get a list of KmlParts node attached to this part in bottom direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading the other parts.
        /// </summary>
        public List<KmlPart> AttachedPartsBottom { get; private set; }

        /// <summary>
        /// Get a list of KmlParts node attached to this part in front direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading the other parts.
        /// </summary>
        public List<KmlPart> AttachedPartsFront { get; private set; }

        /// <summary>
        /// Get a list of KmlParts node attached to this part in back direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading the other parts.
        /// </summary>
        public List<KmlPart> AttachedPartsBack { get; private set; }

        /// <summary>
        /// Get a list of KmlParts node attached to this part in left direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading the other parts.
        /// </summary>
        public List<KmlPart> AttachedPartsLeft { get; private set; }

        /// <summary>
        /// Get a list of KmlParts node attached to this part in right direction.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading the other parts.
        /// </summary>
        public List<KmlPart> AttachedPartsRight { get; private set; }

        /// <summary>
        /// Get a list of KmlParts surface attached to this part.
        /// The list will be filled by static BuildAttachmentStructure() method after a complete vessel is read,
        /// based on the indices and index lists that are filled on reading the other parts.
        /// </summary>
        public List<KmlPart> AttachedPartsSurface { get; private set; }

        /// <summary>
        /// Get info whether this part has resources or not.
        /// </summary>
        public bool HasResources
        {
            get
            {
                return Resources.Count > 0;
            }
        }

        /// <summary>
        /// Get the worst ratio of all resources in this part.
        /// If there is no resource it returns -1.0;
        /// </summary>
        public double WorstResourceRatio 
        { 
            get
            {
                if (HasResources)
                {
                    double worstRatio = 1.0;
                    foreach (KmlResource res in Resources)
                    {
                        if (res.AmountRatio < worstRatio)
                        {
                            worstRatio = res.AmountRatio;
                        }
                    }
                    return worstRatio;
                }
                else
                {
                    return -1.0;
                }
            }
        }

        /// <summary>
        /// This bool value has no meaning within the part itself,
        /// but could be used from other methods to traverse over all parts
        /// and mark the ones it has already been to.
        /// </summary>
        public bool Visited { get; set; }

        private string CraftName { get; set; }

        /// <summary>
        /// Creates a KmlPart as a copy of a given KmlNode.
        /// </summary>
        /// <param name="node">The KmlNode to copy</param>
        public KmlPart(KmlNode node)
            : base(node.Line, node.Parent)
        {
            if (node.Parent != null && node.Parent.Tag.ToLower() == "vessel")
            {
                Origin = PartOrigin.Vessel;
            }
            else
            {
                Origin = PartOrigin.Other;
            }

            Resources = new List<KmlResource>();
            ResourceTypes = new SortedSet<string>();
            Uid = "";
            Position = new Point3D(0.0, 0.0, 0.0);

            ParentPartIndex = -1;
            ParentPart = null;

            AttachedToNodeIndices = new List<int>();
            AttachedToSurfaceIndex = -1;

            AttachedToPartsTop = new List<KmlPart>();
            AttachedToPartsBottom = new List<KmlPart>();
            AttachedToPartsFront = new List<KmlPart>();
            AttachedToPartsBack = new List<KmlPart>();
            AttachedToPartsLeft = new List<KmlPart>();
            AttachedToPartsRight = new List<KmlPart>();
            AttachedToPartSurface = null;
            
            AttachedPartsTop = new List<KmlPart>();
            AttachedPartsBottom = new List<KmlPart>();
            AttachedPartsFront = new List<KmlPart>();
            AttachedPartsBack = new List<KmlPart>();
            AttachedPartsLeft = new List<KmlPart>();
            AttachedPartsRight = new List<KmlPart>();
            AttachedPartsSurface = new List<KmlPart>();

            Visited = false;

            AddRange(node.AllItems);
        }

        /// <summary>
        /// Adds a child KmlItem to this nodes lists of children, depending of its
        /// derived class KmlNode, KmlAttrib or further derived from these.
        /// When an KmlAttrib "Name" is found, its value 
        /// will be used for the corresponding property of this node.
        /// </summary>
        /// <param name="item">The KmlItem to add</param>
        public override void Add(KmlItem item)
        {
            KmlItem newItem = item;
            if (item is KmlAttrib)
            {
                KmlAttrib attrib = (KmlAttrib)item;
                if (attrib.Name.ToLower() == "part")
                {
                    CraftName = attrib.Value;
                    attrib.AttribValueChanged += PartName_Changed;
                    attrib.CanBeDeleted = false;
                }
                else if (attrib.Name.ToLower() == "uid")
                {
                    Uid = attrib.Value;
                    attrib.AttribValueChanged += Uid_Changed;
                    attrib.CanBeDeleted = false;
                }
                else if (attrib.Name.ToLower() == "parent")
                {
                    int p = ParentPartIndex;
                    if (int.TryParse(attrib.Value, out p))
                    {
                        ParentPartIndex = p;
                    }
                    else
                    {
                        Syntax.Warning(this, "Unreadable parent part: " + attrib.ToString());
                    }
                    attrib.AttribValueChanged += ParentPart_Changed;
                    attrib.CanBeDeleted = false;
                }
                else if (attrib.Name.ToLower() == "attn")
                {
                    // Value looks like "top, 12", "bottom, -1", "left, 1", "top2, 3", etc.
                    string[] items = attrib.Value.Split(new char[] {','});
                    int index = -1;
                    if (items.Count() == 2 && int.TryParse(items[1], out index))
                    {
                        if (index >= 0)
                        {
                            AttachedToNodeIndices.Add(index);
                            attrib.CanBeDeleted = false;
                        }
                    }
                    else
                    {
                        Syntax.Warning(this, "Bad formatted part node attachment: " + attrib.ToString());
                    }
                    attrib.AttribValueChanged += AttachmentNode_Changed;
                }
                else if (attrib.Name.ToLower() == "srfn")
                {
                    // Value looks like "srfAttach, 12"
                    string[] items = attrib.Value.Split(new char[] { ',' });
                    int index = -1;
                    if (items.Count() == 2 && int.TryParse(items[1], out index))
                    {
                        if (index >= 0)
                        {
                            if (AttachedToSurfaceIndex < 0)
                            {
                                AttachedToSurfaceIndex = index;
                                attrib.CanBeDeleted = false;
                            }
                            else
                            {
                                Syntax.Warning(this, "More than one surface attachment is not allowed, already attached to [" + AttachedToSurfaceIndex + "], could not attach to [" + index + "]");
                            }
                        }
                    }
                    else
                    {
                        Syntax.Warning(this, "Bad formatted part surface attachment: " + attrib.ToString());
                    }
                    attrib.AttribValueChanged += AttachmentSurface_Changed;
                }
                else if (attrib.Name.ToLower() == "position")
                {
                    // Value looks like "0.1,0,-0.3E-07"
                    string[] items = attrib.Value.Split(new char[] { ',' });
                    double x = 0;
                    double y = 0;
                    double z = 0;
                    if (items.Count() == 3 &&
                        double.TryParse(items[0], NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out x) &&
                        double.TryParse(items[1], NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out y) &&
                        double.TryParse(items[2], NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out z))
                    {
                        Position = new Point3D(x, y, z);
                        attrib.CanBeDeleted = false;
                    }
                    else
                    {
                        Syntax.Warning(this, "Bad formatted part position: " + attrib.ToString());
                    }
                }
            }
            else if (item is KmlResource)
            {
                KmlResource res = (KmlResource)item;
                Resources.Add(res);
                ResourceTypes.Add(res.Name);

                // Get notified when Resources change
                res.MaxAmount.AttribValueChanged += Resources_Changed;
                res.MaxAmount.CanBeDeleted = false;
                res.Amount.AttribValueChanged += Resources_Changed;
                res.Amount.CanBeDeleted = false;
            }
            /*if (Item is KmlNode)
            {
                KmlNode node = (KmlNode)Item;
                if (node.Tag.ToLower() == "resource")
                {
                    newItem = new KmlResource(node);
                }
            }*/
            base.Add(newItem);
        }

        /// <summary>
        /// After a part is completely loaded the Intentify() method is called.
        /// Then can be determined, if this part represents a docking port part or not,
        /// checking all child nodes and their attributes.
        /// If it doesen't need to be replaced, null is returned.
        /// <see cref="KML.KmlItem.Identify()"/>
        /// </summary>
        /// <returns>A KmlPortDock if this KmlPart needs to be replaced by or null otherwise</returns>
        protected override KmlItem Identify()
        {
            if (KmlPartDock.PartIsDock(this))
            {
                return new KmlPartDock(this);
            }
            else
            {
                return base.Identify();
            }
        }

        /// <summary>
        /// Refill all resources of this part.
        /// </summary>
        public void Refill()
        {
            foreach(KmlResource res in Resources)
            {
                res.Refill();
            }
        }

        /// <summary>
        /// Refill all resources of a certain type of this part.
        /// </summary>
        /// <param name="type">The type (name) of the resource to refill (ElectricCharge, LiquidFuel, Oxidizer, etc.)</param>
        public void Refill(string type)
        {
            foreach (KmlResource res in Resources)
            {
                if (res.Name.ToLower() == type.ToLower())
                {
                    res.Refill();
                }
            }
        }

        /// <summary>
        /// Parts of a vessel are first read one after the other, so the first part can only store indices of later parts it is connected to.
        /// After a complete vessel is read also all parts are read and the indices can be translated in references to now existing KmlParts.
        /// This is done by this method. Also reverse information (what parts are connected to this one) is then stored in a part.
        /// </summary>
        /// <param name="parts">The list of KmlParts, a KmlVessel will have one</param>
        /// <returns>A list of root parts (not pointing to another parent part). Could be more than one, if some connections are broken.</returns>
        public static List<KmlPart> BuildAttachmentStructure(List<KmlPart> parts)
        {
            List<KmlPart> roots = new List<KmlPart>();
            for (int i = 0; i < parts.Count; i++ )
            {
                // Check parent connection
                KmlPart part = parts[i];
                if(part.ParentPartIndex == i)
                {
                    // Parent part is itself, so ParentPart property stays null
                    roots.Add(part);
                }
                else if (part.ParentPartIndex < 0 || part.ParentPartIndex >= parts.Count)
                {
                    Syntax.Warning(part, "Part's parent part index [" + part.ParentPartIndex + "] does not point to a valid part.");
                    roots.Add(part);
                }
                else 
                {
                    part.ParentPart = parts[part.ParentPartIndex];
                    if (!part.AttachedToNodeIndices.Contains(part.ParentPartIndex) && part.AttachedToSurfaceIndex != part.ParentPartIndex)
                    {
                        // Part could be docked to parent
                        if ((part is KmlPartDock) && (part.ParentPart is KmlPartDock))
                        {
                            KmlPartDock docker = (KmlPartDock)part;
                            KmlPartDock dockee = (KmlPartDock)part.ParentPart;

                            // In case of "Docked (same vessel)" there has to be another docker-dockee connection and that would have connection via parent part
                            if (docker.DockState.ToLower() == "docked (docker)")
                            {
                                if (dockee.DockState.ToLower() != "docked (dockee)")
                                {
                                    Syntax.Warning(dockee, "Dock part is parent of other docker part. Docking state should be 'Docked (dockee)' but is '" + dockee.DockState + "', other dock: " + docker);
                                    docker.NeedsRepair = true;
                                    dockee.NeedsRepair = true;
                                }
                                else
                                {
                                    KmlPartDock.BuildDockStructure(dockee, docker);
                                    KmlPartDock.BuildDockStructure(docker, dockee);
                                }
                            }
                            else if (docker.DockState.ToLower() == "docked (dockee)")
                            {
                                if (dockee.DockState.ToLower() != "docked (docker)")
                                {
                                    Syntax.Warning(dockee, "Dock part is parent of other dockee part. Docking state should be 'Docked (docker)' but is '" + dockee.DockState + "', other dock: " + docker);
                                    docker.NeedsRepair = true;
                                    dockee.NeedsRepair = true;
                                }
                                else
                                {
                                    KmlPartDock.BuildDockStructure(dockee, docker);
                                    KmlPartDock.BuildDockStructure(docker, dockee);
                                }
                            }
                            else
                            {
                                if (dockee.DockState.ToLower() == "docked (dockee)")
                                {
                                    Syntax.Warning(docker, "Dock part is docked to parent dockee part. Docking state should be 'Docked (docker)' but is '" + docker.DockState + "', parent dock: " + dockee);
                                }
                                else if (dockee.DockState.ToLower() == "docked (docker)")
                                {
                                    Syntax.Warning(docker, "Dock part is docked to parent docker part. Docking state should be 'Docked (dockee)' but is '" + docker.DockState + "', parent dock: " + dockee);
                                }
                                else
                                {
                                    Syntax.Warning(docker, "Dock part is docked to parent dock part. Docking state should be 'Docked (docker)' or 'Docked (dockee)' but is '" + docker.DockState + "', parent dock: " + dockee);
                                    Syntax.Warning(dockee, "Dock part is parent of other dock part. Docking state should be 'Docked (dockee)' or 'Docked (docker)' but is '" + dockee.DockState + "', other dock: " + docker);
                                }
                                docker.NeedsRepair = true;
                                dockee.NeedsRepair = true;
                            }
                        }
                        // Part could be grappled by parent
                        else if ((part.ParentPart is KmlPartDock) && (part.ParentPart as KmlPartDock).DockType == KmlPartDock.DockTypes.Grapple)
                        {
                            KmlPartDock grapple = (KmlPartDock)part.ParentPart;

                            if (grapple.DockUid != part.Uid)
                            {
                                Syntax.Warning(part, "Part not attached or grappled by parent grappling part: " + grapple);
                                Syntax.Warning(grapple, "Grappling part is parent of other part, but is not grappled to it: " + part);
                                grapple.NeedsRepair = true;
                            }
                            else if (grapple.DockState.ToLower() != "grappled")
                            {
                                Syntax.Warning(part, "Part grappled by parent part. Docking state should be 'Grappled' but is '" + grapple.DockState + "', parent grapple: " + grapple);
                                Syntax.Warning(grapple, "Grappling part is parent of grappled part. Docking state should be 'Grappled' but is '" + grapple.DockState + "', grappled part: " + part);
                                grapple.NeedsRepair = true;
                            }
                            else
                            {
                                // It's docked but grappling needs a node attachment
                                KmlPartDock.BuildDockStructure(grapple, part);
                                Syntax.Warning(part, "Part is docked but not attached to parent grappling part: " + grapple);
                                Syntax.Warning(grapple, "Grappling part is parent and docked but not attached to grappled part: " + part);
                                grapple.NeedsRepair = true;
                            }
                        }

                        // TODO KmlPart.BuildAttachmentStructure(): How do KAS links work?

                        // Usually you can only attach a new part by a node to the surface of parent
                        // and not attach a part by surface to parents node. But if you have vessels docked
                        // this situation may happen and this leads to this additional check
                        else if (part.ParentPart.AttachedToSurfaceIndex != i)
                        {
                            Syntax.Warning(part, "Part not attached to parent part: " + part.ParentPart);
                        }
                    }
                }

                // Check attachments
                foreach (int p in part.AttachedToNodeIndices)
                {
                    if(p >= 0 && p < parts.Count)
                    {
                        KmlPart other = parts[p];
                        // Sort attached part in the corresponding list, identified by position not by node name
                        double diffX = part.Position.X - other.Position.X;
                        double diffY = part.Position.Y - other.Position.Y;
                        double diffZ = part.Position.Z - other.Position.Z;
                        if (Math.Abs(diffX) > Math.Abs(diffY) && Math.Abs(diffX) > Math.Abs(diffZ))
                        {
                            if (diffX > 0)
                            {
                                other.AttachedPartsRight.Add(part);
                                part.AttachedToPartsLeft.Add(other);
                            }
                            else
                            {
                                other.AttachedPartsLeft.Add(part);
                                part.AttachedToPartsRight.Add(other);
                            }
                        }
                        else if (Math.Abs(diffZ) > Math.Abs(diffX) && Math.Abs(diffZ) > Math.Abs(diffY))
                        {
                            if (diffZ > 0)
                            {
                                other.AttachedPartsFront.Add(part);
                                part.AttachedToPartsBack.Add(other);
                            }
                            else
                            {
                                other.AttachedPartsBack.Add(part);
                                part.AttachedToPartsFront.Add(other);
                            }
                        }
                        else
                        {
                            if (diffY > 0)
                            {
                                other.AttachedPartsTop.Add(part);
                                part.AttachedToPartsBottom.Add(other);
                            }
                            else
                            {
                                other.AttachedPartsBottom.Add(part);
                                part.AttachedToPartsTop.Add(other);
                            }
                        }
                        if(!other.AttachedToNodeIndices.Contains(parts.IndexOf(part)))
                        {
                            if ((other is KmlPartDock) && (other as KmlPartDock).DockType == KmlPartDock.DockTypes.Grapple)
                            {
                                KmlPartDock grapple = (KmlPartDock)other;
                                if (grapple.DockUid != part.Uid)
                                {
                                    Syntax.Warning(part, "Part node attachment not responded from other grappling part: " + grapple);
                                    grapple.NeedsRepair = true;
                                }
                                else if (grapple.DockState.ToLower() != "grappled")
                                {
                                    Syntax.Warning(part, "Part grappled by other grappling part. Docking state should be 'Grappled' but is '" + grapple.DockState + ", other grapple: " + grapple);
                                    grapple.NeedsRepair = true;
                                }
                                else
                                {
                                    KmlPartDock.BuildDockStructure(grapple, part);
                                }
                            }
                            else if ((part is KmlPartDock) && (part as KmlPartDock).DockType == KmlPartDock.DockTypes.Grapple)
                            {
                                KmlPartDock grapple = (KmlPartDock)part;
                                if (grapple.DockUid != other.Uid)
                                {
                                    Syntax.Warning(grapple, "Grappling part node attachment not responded from other grappled part: " + other);
                                    grapple.NeedsRepair = true;
                                }
                                else if (grapple.DockState.ToLower() != "grappled")
                                {
                                    Syntax.Warning(grapple, "Grappling part grappled attached part. Docking state should be 'Grappled' but is '" + grapple.DockState + ", attached part: " + other);
                                    grapple.NeedsRepair = true;
                                }
                                else
                                {
                                    KmlPartDock.BuildDockStructure(grapple, other);
                                }
                            }
                            else
                            {
                                Syntax.Warning(part, "Part node attachment not responded from other part: " + other.ToString());
                            }
                        }
                    }
                    else
                    {
                        Syntax.Warning(part, "Part supposed to be node attached to part index [" + p + "], which does not point to a valid part");
                    }
                }
                if(part.AttachedToSurfaceIndex >= 0 && part.AttachedToSurfaceIndex < parts.Count)
                {
                    parts[part.AttachedToSurfaceIndex].AttachedPartsSurface.Add(part);
                }
                else if (part.AttachedToSurfaceIndex != -1)
                {
                    Syntax.Warning(part, "Part supposed to be surface attached to part index [" + part.AttachedToSurfaceIndex + "], which does not point to a valid part");
                }

                // Check docking (with parent involved is already checked above, here needs to e checked a 'Docked (same vessel)'
                // Need to check one side only, other part will be touched here in another iteration of the loop
                if (part is KmlPartDock)
                {
                    KmlPartDock dock = (KmlPartDock)part;
                    KmlPart other = null;
                    foreach(KmlPart p in parts)
                    {
                        if (p.Uid == dock.DockUid)
                        {
                            other = p;
                            break;
                        }
                    }
                    if (other == null)
                    {
                        // This happens a lot, parts show UId 0 or some other UId they have been attached to before
                        if (dock.DockState.ToLower() == "docked (docker)" || dock.DockState.ToLower() == "docked (dockee)" ||
                            dock.DockState.ToLower() == "docked (same vessel)" || dock.DockState.ToLower() == "grappled")
                        {
                            Syntax.Warning(dock, "Dock part supposed to be attached to (UId " + dock.DockUid + "), which does not point to a valid part");
                            dock.NeedsRepair = true;
                        }
                        // If it still says 'Disengage' something went wron on undocking. We can repair if undocked part is now another vessel
                        // so other will be null because not found in this vessel
                        // Also could be 'Disarmed', 'Disabled' etc., so it's not checked to be 'Ready'.
                        else if (dock.DockState.ToLower() == "disengage")
                        {
                            Syntax.Warning(dock, "Dock part state should be 'Ready' but is '" + dock.DockState + "'");
                            dock.NeedsRepair = true;
                        }
                    }
                    // Docking with parent already checked
                    else if (other != dock.ParentPart && other.ParentPartIndex != i)
                    {
                        if (dock.DockState.ToLower() == "docked (docker)" || dock.DockState.ToLower() == "docked (dockee)" ||
                            dock.DockState.ToLower() == "docked (same vessel)" || dock.DockState.ToLower() == "grappled")
                        {
                            KmlPartDock.BuildDockStructure(dock, other);
                            if (other is KmlPartDock)
                            {
                                KmlPartDock otherDock = (KmlPartDock)other;
                                if (otherDock.DockUid != dock.Uid)
                                {
                                    Syntax.Warning(dock, "Dock part docked to other dock part, but docking not responded from other side. Other dock: " + otherDock);
                                    dock.NeedsRepair = true;
                                    otherDock.NeedsRepair = true;
                                }
                                else if (otherDock.DockState.ToLower() == "docked (dockee)")
                                {
                                    if (dock.DockState.ToLower() != "docked (same vessel)")
                                    {
                                        Syntax.Warning(dock, "Dock part is docked to dockee part. Docking state should be 'Docked (same vessel)' but is '" + dock.DockState + "', dockee part: " + otherDock);
                                        dock.NeedsRepair = true;
                                        otherDock.NeedsRepair = true;
                                    }
                                }
                                else if (otherDock.DockState.ToLower() == "docked (same vessel)")
                                {
                                    if (dock.DockState.ToLower() != "docked (dockee)")
                                    {
                                        Syntax.Warning(dock, "Dock part is docked to same vessel docking part. Docking state should be 'Docked (dockee)' but is '" + dock.DockState + "', same vessel docking part: " + otherDock);
                                        dock.NeedsRepair = true;
                                        otherDock.NeedsRepair = true;
                                    }
                                }
                                else
                                {
                                    Syntax.Warning(dock, "Dock part is docked to other dock part. Docking state should be 'Docked (same vessel)' or 'Docked (dockee)' but is '" + dock.DockState + "', other dock: " + otherDock);
                                    dock.NeedsRepair = true;
                                    otherDock.NeedsRepair = true;
                                }
                            }
                            else if (dock.DockType != KmlPartDock.DockTypes.Grapple)
                            {
                                Syntax.Warning(dock, "Dock part is no grappling device, so it should be only docked to other dock parts, but is docked to: " + other);
                                dock.NeedsRepair = true;
                            }
                        }
                    }
                }
            }
            return roots;
        }

        /// <summary>
        /// Generates a nice informative string to be used in display for this part.
        /// It will contain the tag, the index in parent's part-list, a root marker and the name.
        /// </summary>
        /// <returns>A string to display this node</returns>
        public override string ToString()
        {
            string s = "";
            if (Parent is KmlVessel)
            {
                KmlVessel p = (KmlVessel)Parent;
                s += " [" + p.Parts.IndexOf(this).ToString();
                if (p.RootPart == this)
                {
                    s += ", root";
                }
                s += "]";
            }
            if (Name.Length > 0)
            {
                s += " (" + Name + ")";
            } 
            else if (CraftName.Length > 0)
            {
                // In *.craft files there is no Name but a PartName
                s += " (" + CraftName + ")";
            }
            return Tag + s;
        }

        private void PartName_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            CraftName = GetAttribWhereValueChanged(sender).Value;
            InvokeToStringChanged();
        }

        private void Resources_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            // Ok it's not the string..
            // but the ProgressBar for resource display also updates by this event
            InvokeToStringChanged();
        }

        private void ParentPart_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            int p = 0;
            if (int.TryParse(GetAttribWhereValueChanged(sender).Value, out p))
            {
                ParentPartIndex = p;
                // TODO KmlPart.ParentPart_Changed(): Rebuild the whole part structure
                System.Windows.MessageBox.Show("You need to save and reload to see the changed parent part in the structure");
            }
        }

        private void Uid_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            Uid = GetAttribWhereValueChanged(sender).Value;
        }

        private void AttachmentSurface_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO KmlPart.AttachmentSurface_Changed(): Reassign AttachedToSurfaceIndex
            // TODO KmlPart.AttachmentSurface_Changed(): Rebuild the whole part structure
            System.Windows.MessageBox.Show("You need to save and reload to see the changed surface attachment in the structure");
        }

        private void AttachmentNode_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO KmlPart.AttachmentNode_Changed(): Delete old index from appropiate list, add new one
            // TODO KmlPart.AttachmentNode_Changed(): Rebuild the whole part structure
            System.Windows.MessageBox.Show("You need to save and reload to see the changed node attachment in the structure");
        }
    }
}
