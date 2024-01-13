CREATE TABLE conversations(
	id INT PRIMARY KEY IDENTITY(1,1),
	conversationname VARCHAR(100)
);

INSERT INTO conversations(conversationname)
VALUES('TestChat'),('TestChat2'),('TestChat3');

DROP TABLE conversations;