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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.CodeAssist;
using QuickSharp.CodeAssist.Sql;
using QuickSharp.SqlManager;

namespace QuickSharp.CodeAssist.SQLite
{
    public class SQLiteCodeAssistProvider : 
        DbSqlCodeAssistProviderBase, ISqlCodeAssistProvider
    {
        #region ISqlCodeAssistProvider

        public LookupList GetLookupList(ScintillaEditForm document)
        {
            return GetList(document);
        }

        public void ColorizeDocument(ScintillaEditForm document)
        {
            Colorize(document);
        }

        public void LoadDatabaseMetadata()
        {
            LoadDatabaseObjects();
        }

        #endregion

        #region DatabaseObjects

        protected override DatabaseObjectsBase GetDatabaseObjects(
            SqlConnection connection)
        {
            return new DatabaseObjects(connection)
                as DatabaseObjectsBase;
        }

        #endregion

        #region Code Assist Driver

        private LookupList GetList(ScintillaEditForm document)
        {
            if (activeDatabase == null) return null;

            Colorize(document);

            string source = document.GetContent() as string;
            if (source == null) return null;

            /*
             * Mark the current positon and cleanup.
             */

            source = source.Insert(document.Editor.CurrentPos, "¬¬¬");
            source = Cleanup(source);

            int pos = source.IndexOf("¬¬¬");
            if (pos == -1) return null;

            string src1 = source.Substring(0, pos);
            string src2 = source.Substring(pos + 3);

            /*
             * Get the current statement; before and after the cursor.
             */

            string[] split1 = src1.Split(';');
            src1 = split1[split1.Length - 1];

            string[] split2 = src2.Split(';');
            src2 = split2[0];

            string firstToken = GetFirstToken(src1);
            if (!IsSQLStatement(firstToken)) return null;

            string secondToken = GetSecondToken(src1);
            string activeKeyword = GetActiveKeyword(src1);

            /*
             * Although the 'main' database is a schema it is the only one that can appear
             * in the lookup so we might as well not display it.
             * The design of the SQL Code Assist system means that it won't work with
             * attached databases. If we attach a database we need to refresh the connection
             * to get the new schema but this causes the attached database to be detached
             * again so we can never see attached databases in the code assist lookups.
             */

            switch (firstToken)
            {
                case "select":
                    switch (activeKeyword)
                    {
                        case "select":
                            return GetEntityList(src1, src2, false, true, true, true);
                        case "from":
                        case "join":
                            return GetEntityList(src1, src2, false, true, false, true);
                        case "where":
                        case "on":
                        case "order_by":
                        case "group_by":
                        case "having":
                            return GetEntityList(src1, src2, false, true, true, true);
                        default:
                            return null;
                    }
                case "insert":
                    switch (activeKeyword)
                    {
                        case "into":
                            return GetEntityList(src1, src2, false, true, false, false);
                        default:
                            return null;
                    }
                case "update":
                    switch (activeKeyword)
                    {
                        case "update":
                            return GetEntityList(src1, src2, false, true, false, false);
                        case "set":
                            return GetEntityList(src1, src2, false, false, true, false);
                        case "where":
                            return GetEntityList(src1, src2, false, false, true, false);
                        default:
                            return null;
                    }
                case "delete":
                    switch (activeKeyword)
                    {
                        case "from":
                            return GetEntityList(src1, src2, false, true, false, false);
                        case "where":
                            return GetEntityList(src1, src2, false, false, true, false);
                        default:
                            return null;
                    }
                case "create":
                    switch (activeKeyword)
                    {
                        case "table":
                        case "view":
                            return GetEntityList(src1, src2, false, false, false, false);
                    }
                    if (secondToken == "view")
                    {
                        switch (activeKeyword)
                        {
                            case "select":
                                return GetEntityList(src1, src2, false, true, true, true);
                            case "from":
                            case "join":
                                return GetEntityList(src1, src2, false, true, false, true);
                            case "where":
                            case "on":
                            case "order_by":
                            case "group_by":
                            case "having":
                                return GetEntityList(src1, src2, false, true, true, true);
                            default:
                                return null;
                        }
                    }
                    return null;
                case "alter":
                case "drop":
                    switch (activeKeyword)
                    {
                        case "table":
                        case "view":
                            return GetEntityList(src1, src2, false, true, false, false);
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        #endregion

        #region Helpers

        private string RemoveComments(string text)
        {
            StringBuilder sb = new StringBuilder();
            int max = text.Length - 1;
            int i = 0;

            /*
             * Substitute spaces in entity names and
             * remove the quotes.
             */

            bool changingSpaces = false;
            bool skipChar = false;
            bool inQuotes = false;

            while (i < text.Length)
            {
                skipChar = false;

                if (text[i] == '-' && i < max && text[i + 1] == '-')
                {
                    // Goto line end
                    while (i <= max && text[i] != '\n') i++;
                }
                else if (text[i] == '/' && i < max && text[i + 1] == '*')
                {
                    // Goto next '*/'
                    while (i < max)
                    {
                        if (text[i] == '*' && text[i + 1] == '/') break;
                        i++;
                    }
                    i += 2;
                }
                else if (text[i] == '"' || text[i] == '\'' || text[i] == '`')
                {
                    inQuotes = !inQuotes;
                    changingSpaces = inQuotes;
                    skipChar = true;
                }

                if (i <= max && !skipChar)
                {
                    if (changingSpaces && text[i] == ' ')
                        sb.Append('~');
                    else if (inQuotes && text[i] == ';')
                        sb.Append(' ');
                    else
                        sb.Append(text[i]);
                }

                i++;
            }

            return sb.ToString();
        }

        private string Cleanup(string text)
        {
            text = text.Replace("\r\n", "\n");
            text = RemoveComments(text);
            text = text.Replace('\n', ' ');
            text = text.Replace('\t', ' ');
            return RemoveMultiSpaces(text);
        }

        protected override string GetEscapedName(string name)
        {
            return GetGenericEscapedName(name, "`", "`");
        }

        protected override string GetEscapedFullName(string name)
        {
            return GetGenericEscapedFullName(name, "`", "`");
        }

        private void SplitFullTableName(string text,
            ref string schema, ref string table)
        {
            string[] split = text.Split('.');

            if (split.Length == 1)
            {
                if (activeDatabase.HaveDatabase)
                    schema = activeDatabase.Database;

                table = split[0];
            }
            else
            {
                schema = split[0];
                table = split[1];
            }
        }

        #endregion

        #region Entity List

        private LookupList GetEntityList(
            string src1, string src2,
            bool showSchema, bool showTables, bool showColumns,
            bool isSelect)
        {
            List<string> selectedTables =
                GetSelectedTables(src1 + src2, isSelect);

            string lookAhead = String.Empty;

            if (!src1.EndsWith(" "))
            {
                string[] split = src1.Split(stmtDelimiters);
                lookAhead = split[split.Length - 1];
            }

            string[] tokens = lookAhead.Split('.');

            if (tokens.Length == 1)
            {
                return GetFirstLevelEntities(
                    selectedTables,
                    showSchema, showTables, showColumns,
                    tokens[0]);
            }
            else if (tokens.Length == 2 && showTables)
            {
                /*
                 * Is it an alias?
                 */

                string tokenNC = tokens[0].ToLower();

                if (tableAliases.ContainsKey(tokenNC))
                {
                    string target = tableAliases[tokenNC].TableName;

                    string schemaName = null;
                    string tableName = null;

                    SplitFullTableName(target, ref schemaName, ref tableName);

                    return GetThirdLevelEntities(
                        schemaName, tableName, tokens[1]);
                }

                if (activeDatabase.Schemata.ContainsKey(tokens[0].ToLower()))
                    return GetSecondLevelEntities(tokens[0], tokens[1]);

                else if (showColumns && activeDatabase.HaveDatabase)
                    return GetThirdLevelEntities(
                        activeDatabase.Database, tokens[0], tokens[1]);
            }
            else if (tokens.Length == 3 && showTables && showColumns)
            {
                return GetThirdLevelEntities(
                    tokens[0], tokens[1], tokens[2]);
            }

            return null;
        }

        private LookupList GetFirstLevelEntities(
            List<string> selectedTables,
            bool showSchema,
            bool showTables,
            bool showColumns,
            string lookAhead)
        {
            List<LookupListItem> list = new List<LookupListItem>();

            /*
             * Get the columns from the selected tables.
             */

            if (showColumns)
            {
                foreach (string selectedTable in selectedTables)
                {
                    string schemaName = null;
                    string tableName = null;

                    SplitFullTableName(selectedTable,
                        ref schemaName, ref tableName);

                    if (schemaName == null || tableName == null)
                        continue;

                    string schemaNameNC = schemaName.ToLower();
                    string tableNameNC = tableName.ToLower();

                    if (activeDatabase.Schemata.ContainsKey(schemaNameNC) &&
                        activeDatabase.Schemata[schemaNameNC].
                            Tables.ContainsKey(tableNameNC))
                    {
                        Table table = activeDatabase.Schemata[schemaNameNC].
                            Tables[tableNameNC];

                        list.AddRange(BuildTableColumnList(table));
                    }
                }
            }

            /*
             * Add the schema (except for the default).
             */

            if (showSchema)
                list.AddRange(BuildSchemaList());

            /*
             * Add the tables from the default schema.
             */

            if (showTables &&
                activeDatabase.HaveDatabase &&
                activeDatabase.Schemata.ContainsKey(
                    activeDatabase.Database))
            {
                Schema defaultSchema =
                    activeDatabase.Schemata[activeDatabase.Database];

                list.AddRange(BuildSchemaTableList(defaultSchema));
            }

            /*
             * Add the table aliases if in a column clause.
             */

            if (showColumns)  
                list.AddRange(BuildTableAliasList());

            return new LookupList(lookAhead.Replace('~', ' '), list);
        }

        private LookupList GetSecondLevelEntities(
            string schemaName,
            string lookAhead)
        {
            if (schemaName == null) return null;

            Schema schema = null;
            string schemaNameNC = schemaName.ToLower();

            if (activeDatabase.Schemata.ContainsKey(schemaNameNC))
                schema = activeDatabase.Schemata[schemaNameNC];

            if (schema == null) return null;

            /*
             * Get the tables belonging to the selected schema.
             */

            List<LookupListItem> list = new List<LookupListItem>();
            list.AddRange(BuildSchemaTableList(schema));

            return new LookupList(lookAhead.Replace('~', ' '), list);
        }

        private LookupList GetThirdLevelEntities(
            string schemaName,
            string tableName,
            string lookAhead)
        {
            if (schemaName == null || tableName == null)
                return null;

            Table table = null;
            string schemaNameNC = schemaName.ToLower();
            string tableNameNC = tableName.ToLower();

            if (activeDatabase.Schemata.ContainsKey(schemaNameNC) &&
                activeDatabase.Schemata[schemaNameNC].
                    Tables.ContainsKey(tableNameNC))
                table = activeDatabase.Schemata[schemaNameNC].
                    Tables[tableNameNC];

            if (table == null) return null;

            /*
             * Get the columns belonging to the selected table.
             */

            List<LookupListItem> list = new List<LookupListItem>();
            list.AddRange(BuildTableColumnList(table));

            return new LookupList(lookAhead.Replace('~', ' '), list);
        }

        #endregion
    }
}
