CREATE TABLE messagestable(
	id INT PRIMARY KEY IDENTITY(1,1),
	content VARCHAR(100),
	userid INT NOT NULL,
	conversationid INT NOT NULL
);

INSERT INTO messagestable(content, userid, conversationid)
VALUES('Hello World 1', 1, 1);

DROP TABLE messagestable;