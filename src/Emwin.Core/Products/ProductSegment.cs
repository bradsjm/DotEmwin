﻿/*
 * Microsoft Public License (MS-PL)
 * Copyright (c) 2015 Jonathan Bradshaw <jonathan@nrgup.net>
 *     
 * This license governs use of the accompanying software. If you use the software, you
 * accept this license. If you do not accept the license, do not use the software.
 *     
 * 1. Definitions
 *     The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
 *     same meaning here as under U.S. copyright law.
 *     A "contribution" is the original software, or any additions or changes to the software.
 *     A "contributor" is any person that distributes its contribution under this license.
 *     "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 *     
 * 2. Grant of Rights
 *     (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 *     (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 *     
 * 3. Conditions and Limitations
 *     (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
 *     (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 *     (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 *     (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 *     (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Emwin.Core.DataObjects;
using Emwin.Core.Parsers;

namespace Emwin.Core.Products
{
    /// <summary>
    /// Class TextProduct. Represents a received text file.
    /// </summary>
    public class ProductSegment : TextProduct
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the geo codes.
        /// </summary>
        /// <value>The geo codes.</value>
        public IEnumerable<UniversalGeoCode> GeoCodes { get; set; }

        /// <summary>
        /// Gets or sets the polygons.
        /// </summary>
        /// <value>The polygons.</value>
        public GeoPoint[] Polygon { get; set; }

        /// <summary>
        /// Gets the primary vtec.
        /// </summary>
        /// <value>The primary vtec.</value>
        public PrimaryVtec PrimaryVtec { get; set; }

        /// <summary>
        /// Gets or sets the hydrologic vtec.
        /// </summary>
        /// <value>The hydrologic vtec.</value>
        public HydrologicVtec HydrologicVtec { get; set; }

        /// <summary>
        /// Gets the sequence number with the text product.
        /// </summary>
        /// <value>The sequence number.</value>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the tracking line.
        /// </summary>
        /// <value>The tracking line.</value>
        public TrackingLine TrackingLine { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the bulletin product.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="content">The content.</param>
        /// <param name="receivedAt">The received at.</param>
        /// <param name="header">The header.</param>
        /// <param name="seq">The seq.</param>
        /// <param name="source">The source.</param>
        /// <returns>TextProduct.</returns>
        public static ProductSegment Create(string filename, DateTimeOffset timeStamp, TextContent content, DateTimeOffset receivedAt, int seq, string source)
        {
            var product = new ProductSegment
            {
                Filename = filename,
                TimeStamp = timeStamp,
                Content = content,
                ReceivedAt = receivedAt,
                SequenceNumber = seq,
                Source = source
            };

            product.GeoCodes = product.GetGeoCodes();
            product.PrimaryVtec = product.GetPrimaryVtec().FirstOrDefault();
            product.Polygon = product.GetPolygons().FirstOrDefault();
            product.TrackingLine = product.GetTrackingLines().FirstOrDefault();
            product.HydrologicVtec = product.GetHydrologicVtec().FirstOrDefault();

            return product;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() =>
            $"[{nameof(ProductSegment)}] Filename={Filename} Date={TimeStamp:g} Sequence={SequenceNumber} {PrimaryVtec}";

        #endregion Public Methods

    }
}