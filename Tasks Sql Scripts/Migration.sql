-- Execute ONLY after users migration!

USE TasksUniversity;

INSERT TasksUniversity.Task(Id, Topic, Content, StartPeriod, EndPeriod, Author, Genre_Id, Status_id, Priority_Id)
SELECT
	z.id AS Id,
    'Zmigrowane ze starej bazy danych' AS Topic,
    z.opis AS Content,
    cast(concat(cast(z.data_zlecenia as char),' ', cast(z.godzina_zlecenia as char)) as datetime) as StartPeriod,
 	CASE WHEN z.data_wykonania != '' THEN cast(concat(cast(z.data_wykonania as char),' ', cast(z.godzina_wykonania as char)) as datetime) 
		 ELSE NULL END as EndPeriod,    
    z.autor AS Author,
    CASE WHEN z.zlecenie_rodzaj_id = 3 THEN 1
		 WHEN z.zlecenie_rodzaj_id = 4 THEN 2
         WHEN z.zlecenie_rodzaj_id = 2 THEN 3
         WHEN z.zlecenie_rodzaj_id = 1 THEN 4 END AS Genre_Id,
    CASE WHEN z.zlecenie_status_id = 4 THEN 1
		 WHEN z.zlecenie_status_id = 1 THEN 2
         WHEN z.zlecenie_status_id = 3 THEN 3 END AS Status_Id,   
    z.zlecenie_priorytet_id AS Priority_Id    
FROM Zlecenia.Zlecenie z;

INSERT TasksUniversity.TaskComment(Content, `Date`, User_Id, Task_Id)
SELECT
	z.uwagi AS Content,
    now() AS `Date`,
    u.Id AS User_Id,
    z.Id AS Task_Id
FROM Zlecenia.Zlecenie z
JOIN TasksUniversity.User u ON u.Username = z.autor;

DELETE tc FROM TasksUniversity.TaskComment tc WHERE length(tc.Content) = 0;

INSERT TasksUniversity.TasksToUsers(User_Id, Task_Id)
SELECT 
	u.Id,
	t.Id
FROM Zlecenia.Uzytkownik_has_zlecenie uz
JOIN TasksUniversity.Task t ON t.id = uz.zlecenie_id
JOIN TasksUniversity.User u ON u.Username = uz.uzytkownik_login;

