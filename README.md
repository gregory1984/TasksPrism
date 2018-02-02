# Tasks - Prism

Installation instructions:
1. Install MySQL Server with MySQL Workbench;
2. Open MySQL Workbench and fire: `Tasks Sql Scripts/Initialization.sql`. If You want to change database user password then go ahead, but don't forget to change it inside `Tasks Prism.exe.config`. Beware: this file will be encrypted after the first application run so it's very good idea to backup it and restore for changing due to connection failure;
3. **OPTIONAL:** If You want to migrate users from `Zlecenia` database then just select all rows from table `uzytkownik` and save the result as `CSV` file into `Migration/users.csv`. If the `Migration` folder doesn't exists then just create it inside the root application location;
4. Run `Tasks Prism.exe`. All `Tasks - Prism` tables will be created automatically via NHibarnate ORM. If point no. 3 has been performed then application will migrate all `Zlecenia` users into the new infrastructure, with passwords same as usernames, so don't forget to change those. It will also create `administrator` user with password rule as above;
5. **OPTIONAL:** If You want to migrate all tasks from `Zlecenia` database then just execute: `Tasks Sql Scripts/Migration.sql`. *This point is optional but if You decide to perform it You have to perform point no. 3 first!*

...and Voilà, that's all!

