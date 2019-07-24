using System;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.MySql
{
    /// <summary>
    /// An implementation of the <see cref="IJournal"/> interface which tracks version numbers for a 
    /// MySql database using a table called SchemaVersions.
    /// </summary>
    public class MySqlTableJournal : TableJournal
    {
        /// <summary>
        /// Creates a new MySql table journal.
        /// </summary>
        /// <param name="connectionManager">The MySql connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="table">The name of the journal table.</param>
        public MySqlTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, new MySqlObjectParser(), schema, table)
        {
        }

        protected override string GetInsertJournalEntrySql(string scriptName, string scriptContents, string applied)
        {
            return $"INSERT INTO {FqSchemaTableName} (ScriptName, ScriptContents, Applied) VALUES ({@scriptName}, {scriptContents}, {@applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"SELECT ScriptName, ScriptContents, Applied FROM {FqSchemaTableName} ORDER BY ScriptName";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
$@"CREATE TABLE {FqSchemaTableName}
(
    `SchemaVersionId` INT NOT NULL AUTO_INCREMENT,
    `ScriptName` VARCHAR(255) NOT NULL,
    `ScriptContents` TEXT NOT NULL,
    `Applied` TIMESTAMP NOT NULL,
    PRIMARY KEY (`SchemaVersionId`)
);";
        }
    }
}
