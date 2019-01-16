	SQL scripts in this folder are examples of what may need to be distributed
alongside with your application when you are delivering an update to your users.
If in new version of your application you modified your model classes which in
turn changed the underlying database schema, then your established users will
have to migrate their existing databases to match the new schema. This is done
semi-automatically by this NuGet package. There are two steps which developer
is responsible for (see below) and the rest is done by this package.

When application starts, this package detects if there is a version mismatch
between existing database and latest schema version as hardcoded in
SampleDataModel.DataModel.CurrentSchemaVersion. If existing database version is
lower, then this package starts looking for *.sql scripts in the "migrations"
subfolder alongside the application. Script file names start with a number
followed by a meaningful description. Each script migrates database one version
up and the number prefix of its file name is the new target version. Scripts are
run by this package one at a time upgrading database step by step until its
version reaches the hardcoded version.

Here is an example scenario.

Suppose you distributed first version of your application with schema version
hardcoded to 0, i.e. SampleDataModel.DataModel.CurrentSchemaVersion = 0. It only
contained one table "Clients".

After first run of your application Entity Framework created a new database on
your users' machines with the schema matching your current data model, i.e. only
"Clients" table.

In the next version of your application you add another table to your data model,
"Products", and also set SampleDataModel.DataModel.CurrentSchemaVersion=1. You
distribute this new version with a simple SQL script which adds "Products" table
and sets database version to 1.

Script relative path and file name:
./migrations/1.AddProductsTable.sql

Script body:
PRAGMA user_version=1;
CREATE TABLE[Products]([ProductId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                       [Name] nvarchar NOT NULL, [Price] INTEGER NOT NULL);

For new users who do not have any database yet, Entity Framework will simply
create a new database according to the latest schema and version set to 1.
For users with existing databases the application when run for the first time
after the upgrade will execute the script and upgrade the existing database
to the latest schema.

In the next version of your application you added a "Description" column to the
"Products" table by adding a "Description" property to your Product model class.
You also set SampleDataModel.DataModel.CurrentSchemaVersion=2. And now
distribute this new version with two SQL scripts:

./migrations/1.AddProductsTable.sql
./migrations/2.AddDescriptionColumnToProductsTable.sql

First one is unchanged and the second one is to add the new column:

PRAGMA user_version=2;
ALTER TABLE[Products] ADD COLUMN[Description] TEXT NULL;

For new users, as always, only Entity Framework is at play, it creates new
database according to the latest schema and version set to 2.
Users running your previous release will see their databases upgraded from
version 1 to version 2 because first run of this new release will run script
2.AddDescriptionColumnToProductsTable.sql for them.
Users who skipped previous upgrade and upgraded from first release to this
latest one will see their databases upgraded from version 0 to version 2
because first run of this new release will run both SQL scripts in proper order
to migrate existing database from version 0 to 1 first and then from 1 to 2.

As a developer you have two responsibilities when releasing a new version with
updated schema:
1. Increment SampleDataModel.DataModel.CurrentSchemaVersion
2. Provide SQL script to migrate from previous schema to the latest one. Name
   of this script file must start with the new version number and script itself
   must write same version to the database with "PRAGMA user_version" command.

You can set up Visual Studio to copy your script files to the output folder so
that they automatically end up where they should be. In order to do that, after
adding your script files to your project right click on each of them in the
Solution Explorer, select Properties and in the Properties window set "Build
Action" to "Content" and "Copy to Output Directory" to "Copy if newer".
