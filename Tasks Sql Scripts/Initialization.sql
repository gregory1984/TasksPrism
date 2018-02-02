CREATE SCHEMA `TasksUniversity`;
CREATE USER 'TasksUniUser' IDENTIFIED BY 'TasksUniPassword';
GRANT ALL PRIVILEGES ON TasksUniversity.* TO 'TasksUniUser';