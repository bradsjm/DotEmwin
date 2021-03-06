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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Emwin.Core.DataObjects;
using Emwin.Core.Products;

namespace Emwin.Core.Parsers
{
    /// <summary>
    /// Text Product Parsers.
    /// </summary>
    public static class TextProductParser
    {

        #region Private Fields

        /// <summary>
        /// The bullets in text product regex
        /// </summary>
        private static readonly Regex BulletsRegex = new Regex(@"\r\n\s*\*\s*(?<bullet>[^\*]+)", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// The Header Block
        /// </summary>
        private static readonly Regex HeaderBlockRegex = new Regex(
            @"^(?<dtl>[A-Z]{4}[0-9]{2})\s(?<station>[A-Z]{4})\s(?<time>[0-9]{6})(\s(?<indicator>[A-Z]{3}))?(\r\n(?<afos>[A-Z0-9\s]{4,6}))?", 
            RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// The product segment split regex
        /// </summary>
        private static readonly Regex SegmentSplitRegex = new Regex(@"\r\n\$\$\s*\r\n", RegexOptions.Singleline | RegexOptions.Compiled);

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Gets the bullet content in the text.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> GetBullets(this TextProduct product)
        {
            return BulletsRegex
                .Matches(product.Content.RawBody)
                .Cast<Match>()
                .Select(x => x.Groups["bullet"].Value);
        } 

        /// <summary>
        /// Gets the segments in the text product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>IEnumerable&lt;TextProductSegment&gt;.</returns>
        public static IEnumerable<TextProductSegment> GetSegments(this TextProduct product)
        {
            var matches = SegmentSplitRegex.Split(product.Content.RawBody);
            var seq = 1;
            foreach (var segment in matches)
            {
                var newFilename = string.Concat(
                    Path.GetFileNameWithoutExtension(product.Filename),
                    '-', seq.ToString("00"),
                    Path.GetExtension(product.Filename));

                yield return TextProductSegment.Create(
                    newFilename,
                    product.TimeStamp,
                    new TextContent { RawHeader = product.Content.RawHeader, RawBody = segment.Trim() },
                    product.ReceivedAt,
                    seq++,
                    product.Source);
            }
        }

        /// <summary>
        /// Gets the wmo header.
        /// </summary>
        /// <param name="textProduct">The text product.</param>
        /// <returns>WmoHeader.</returns>
        public static WmoHeader GetWmoHeader(this TextProduct textProduct)
        {
            var match = HeaderBlockRegex.Match(textProduct.Content.RawHeader);
            if (!match.Success) return new WmoHeader();

            return new WmoHeader
            {
                DataType = match.Groups["dtl"].Value.Substring(0, 2),
                Distribution = match.Groups["dtl"].Value.Substring(2),
                WmoId = match.Groups["station"].Value,
                IssuedAt = TimeParser.ParseDayHourMinute(textProduct.TimeStamp, match.Groups["time"].Value),
                Designator = match.Groups["indicator"].Success ? match.Groups["indicator"].Value : string.Empty,
                ProductCategory = match.Groups["afos"].Success ? match.Groups["afos"].Value.Substring(0,3) : string.Empty,
                LocationIdentifier = match.Groups["afos"].Success ? match.Groups["afos"].Value.Substring(3).TrimEnd() : string.Empty
            };
        }

        #endregion Public Methods

    }
}
