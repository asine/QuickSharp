/*
 * QuickSharp Copyright (C) 2008-2011 Steve Walker.
 *
 * This file is part of QuickSharp.
 *
 * QuickSharp is free software: you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * QuickSharp is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with QuickSharp. If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.Editor
{
    /// <summary>
    /// Provides access to the public members of the search form.
    /// </summary>
    public class SearchFormProxy : BaseFormProxy
    {
        #region Singleton

        private static SearchFormProxy _singleton;

        /// <summary>
        /// Get a reference to the SearchFormProxy singleton.
        /// </summary>
        /// <returns>A reference to SearchFormProxy.</returns>
        public static SearchFormProxy GetInstance()
        {
            if (_singleton == null)
                _singleton = new SearchFormProxy();

            return _singleton;
        }

        #endregion

        private SearchFormProxy()
        {
        }
    }
}
