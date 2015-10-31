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
using System.Text.RegularExpressions;
using Emwin.Core.Contracts;
using System.Linq;
using System.Text;
using Emwin.Core.Products;

namespace Emwin.Core.Parsers
{
    public static class BulletinParser
    {

        #region Private Fields

        private static readonly Regex BulletinRegex = new Regex(@"[\r\n]+(?<bulletin>[A-Z]{2}[CZ][0-9AL]{3}([A-Z0-9\r\n>-]*?)[0-9]{6}-[\r\n]+.+?[\r\n]+\$\$)", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        private static readonly Regex HeaderRegex = new Regex(@"^(?<header>.+?)[A-Z]{2}[CZ][0-9AL]{3}([A-Z0-9\r\n>-]*?)[0-9]{6}-[\r\n]+.+?[\r\n]+\$\$", RegexOptions.Singleline | RegexOptions.ExplicitCapture);

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Parses the product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>IEnumerable&lt;ITextProduct&gt;.</returns>
        public static IEnumerable<IBulletinProduct> ParseProduct(ITextProduct product)
        {
            var headerMatch = HeaderRegex.Match(product.Content);
            if (!headerMatch.Success) yield break;

            var header = headerMatch.Groups["header"].Value;
            var matches = BulletinRegex.Matches(product.Content);
            var seq = 1;
            foreach (var bulletin in matches.Cast<Match>().Select(match => match.Groups["bulletin"].Value))
            {
                var newFilename = string.Concat(
                    Path.GetFileNameWithoutExtension(product.Filename), 
                    '.', seq.ToString("00"),
                    Path.GetExtension(product.Filename));

                yield return ProductFactory.CreateBulletinProduct(
                    newFilename, 
                    product.TimeStamp, 
                    Encoding.ASCII.GetBytes(header + bulletin), 
                    product.ReceivedAt,
                    seq++);
            }
        }

        #endregion Public Methods

    }
}